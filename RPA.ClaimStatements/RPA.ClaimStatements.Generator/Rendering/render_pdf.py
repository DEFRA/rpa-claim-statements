import io
from contextlib import closing
import logging
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

from PIL import Image, ImageOps
import tinycss2
from tinycss2.color4 import parse_color
import tinyhtml5
from weasyprint import CSS, HTML
from weasyprint.urls import URLFetcher, URLFetcherResponse


COLOR_PROPERTIES = {
    "background", "background-image", "border", "border-top", "border-right",
    "border-bottom", "border-left", "outline", "text-decoration", "column-rule",
    "box-shadow", "text-shadow", "fill", "stroke",
}
# Word emits CSS system colours that WeasyPrint rejects, silently dropping the whole declaration
# (for example every table border). They resolve to black on print media.
SYSTEM_COLORS = {"windowtext", "canvastext", "buttontext"}
SCALE = 1.18
MINIMUM_SCALE = 0.5
UNITS_PX = {"px": 1.0, "in": 96.0, "pt": 96 / 72, "pc": 16.0, "cm": 96 / 2.54, "mm": 96 / 25.4}
BLANK_PARAGRAPH_BLOCKERS = {"br", "img", "table", "hr"}


class ResourceError(ValueError):
    pass


def gray_tokens(tokens):
    result = []
    for token in tokens:
        if token.type == "ident" and token.lower_value in SYSTEM_COLORS:
            result.extend(tinycss2.parse_component_value_list("rgb(0,0,0)"))
            continue
        color = parse_color(token) if token.type in ("ident", "hash", "function") else None
        if color is not None and not isinstance(color, str):
            red, green, blue = color.to("srgb").coordinates
            gray = round(255 * (0.299 * (red or 0) + 0.587 * (green or 0) + 0.114 * (blue or 0)))
            gray = max(0, min(255, gray))
            result.extend(tinycss2.parse_component_value_list(
                f"rgba({gray},{gray},{gray},{color.alpha})"))
        else:
            if token.type == "function" and token.lower_name not in ("url", "local"):
                token.arguments = gray_tokens(token.arguments)
            result.append(token)
    return result


def without_symbol_font(tokens):
    # The Symbol font is not installed; Word bullets use it, so fall back to a normal font.
    if any(token.type in ("ident", "string") and token.value.lower() == "symbol" for token in tokens):
        return tinycss2.parse_component_value_list("Arial, sans-serif")
    return tokens


def gray_css(rules):
    for rule in rules:
        if rule.type == "declaration":
            name = rule.lower_name
            if name == "color" or name.endswith("-color") or name in COLOR_PROPERTIES or name.startswith("--"):
                rule.value = gray_tokens(rule.value)
            elif name == "font-family":
                rule.value = without_symbol_font(rule.value)
        elif rule.type in ("qualified-rule", "at-rule") and rule.content is not None:
            rule.content = tinycss2.parse_component_value_list(
                gray_css(tinycss2.parse_blocks_contents(rule.content)))
        elif rule.type == "error":
            raise ValueError("Invalid statement CSS")
    return tinycss2.serialize(rules)


def length_px(token):
    if token.type == "dimension" and token.lower_unit in UNITS_PX:
        return token.value * UNITS_PX[token.lower_unit]
    if token.type == "number" and token.value == 0:
        return 0.0
    return None


def left_margin_px(declarations):
    if "margin-left" in declarations:
        values = declarations["margin-left"]
        return length_px(values[0]) if len(values) == 1 else None
    values = declarations.get("margin")
    if not values:
        return None
    lengths = [length_px(value) for value in values]
    if None in lengths or len(lengths) > 4:
        return None
    return lengths[3] if len(lengths) == 4 else lengths[1] if len(lengths) > 1 else lengths[0]


def span_declarations(root):
    rules = {}
    for element in root.iter():
        if element.tag.rsplit("}", 1)[-1] != "style" or not element.text:
            continue
        for rule in tinycss2.parse_stylesheet(element.text, skip_comments=True, skip_whitespace=True):
            if rule.type != "qualified-rule":
                continue
            match = re.fullmatch(r"span\.([\w-]+)", tinycss2.serialize(rule.prelude).strip())
            if match is None:
                continue
            declarations = rules.setdefault(match.group(1), {})
            for item in tinycss2.parse_blocks_contents(rule.content, skip_comments=True, skip_whitespace=True):
                if item.type == "declaration":
                    declarations[item.lower_name] = [token for token in item.value if token.type != "whitespace"]
    return rules


def add_style(element, declaration):
    existing = element.get("style", "").strip().rstrip(";")
    element.set("style", f"{existing}; {declaration}" if existing else declaration)


def merge_tab_stops(root):
    # Clippit emulates a Word tab as an inline-block label with a min-width, followed by a
    # nbsp span with a left margin. A label wider than the min-width therefore pushes its value
    # right of the intended stop. Combining both into one min-width puts every value on the stop.
    rules = span_declarations(root)

    def declared(element):
        merged = {}
        for name in element.get("class", "").split():
            merged.update(rules.get(name, {}))
        return merged

    for parent in root.iter():
        children = list(parent)
        for label, tab in zip(children, children[1:]):
            if label.tag != "span" or tab.tag != "span" or (tab.text or "").strip():
                continue
            label_style = declared(label)
            minimum = label_style.get("min-width", [])
            if [token.value for token in label_style.get("display", []) if token.type == "ident"] != ["inline-block"]:
                continue
            if len(minimum) != 1 or length_px(minimum[0]) is None:
                continue
            inner = list(tab)
            if len(inner) != 1 or inner[0].tag != "span" or inner[0].text != "\u00a0" or len(inner[0]) > 0:
                continue
            margin = left_margin_px(declared(inner[0]))
            if margin is None or margin <= 0:
                continue
            add_style(label, f"min-width: {length_px(minimum[0]) + margin:.3f}px")
            add_style(inner[0], "margin-left: 0")


def remove_trailing_blank_paragraphs(root):
    # Empty paragraphs at the very end add no content but can spill onto an extra blank page.
    container = next((element for element in root.iter() if element.tag.rsplit("}", 1)[-1] == "body"), None)
    while container is not None and len(container) and container[-1].tag == "div":
        container = container[-1]
    while container is not None and len(container):
        last = container[-1]
        blank = (last.tag == "p"
                 and not any(element.tag in BLANK_PARAGRAPH_BLOCKERS for element in last.iter())
                 and not "".join(last.itertext()).replace("\u00a0", " ").strip())
        if not blank:
            break
        container.remove(last)


def prepare_html(source):
    root = tinyhtml5.parse(source, namespace_html_elements=False)
    for element in root.iter():
        tag = element.tag.rsplit("}", 1)[-1]
        if tag == "img" and not element.get("src"):
            raise ResourceError("Image has no source")
        if tag == "style" and element.text:
            element.text = gray_css(tinycss2.parse_stylesheet(element.text))
        if "style" in element.attrib:
            element.set("style", gray_css(tinycss2.parse_blocks_contents(element.get("style"))))
        for attribute in ("color", "bgcolor", "bordercolor", "fill", "stroke", "stop-color"):
            if attribute in element.attrib:
                element.set(attribute, tinycss2.serialize(
                    gray_tokens(tinycss2.parse_component_value_list(element.get(attribute)))))
        if tag not in ("style", "script"):
            # Word bullets are Symbol-font private-use characters; use the standard bullet.
            if element.text:
                element.text = element.text.replace("\uf0b7", "\u2022")
            if element.tail:
                element.tail = element.tail.replace("\uf0b7", "\u2022")
    merge_tab_stops(root)
    remove_trailing_blank_paragraphs(root)
    return ET.tostring(root, encoding="unicode", method="html")


class Diagnostics(logging.Handler):
    def __init__(self):
        super().__init__()
        self.errors = 0
        self.warnings = 0

    def emit(self, record):
        if record.levelno >= logging.ERROR:
            self.errors += 1
        elif record.levelno >= logging.WARNING:
            self.warnings += 1


def render(source, destination):
    blocked = []

    class EmbeddedFetcher(URLFetcher):
        def fetch(self, url, headers=None):
            if not url.lower().startswith("data:"):
                blocked.append(True)
                raise ResourceError("Only embedded resources are allowed")
            with closing(super().fetch(url, headers)) as response:
                data = response.read()
                mime_type = response.content_type.lower()
                encoding = response.charset
            if mime_type.startswith("image/"):
                if mime_type not in ("image/png", "image/jpeg", "image/gif", "image/tiff"):
                    blocked.append(True)
                    raise ResourceError("Unsupported embedded image format")
                # Restrict the decoder by byte format too, not just the untrusted MIME label.
                with Image.open(io.BytesIO(data), formats=("PNG", "JPEG", "GIF", "TIFF")) as image:
                    image = ImageOps.exif_transpose(image).convert("RGBA").convert("LA")
                    output = io.BytesIO()
                    image.save(output, format="PNG")
                data, mime_type = output.getvalue(), "image/png"
            elif mime_type == "text/css":
                rules, _ = tinycss2.parse_stylesheet_bytes(data, protocol_encoding=encoding)
                data = gray_css(rules).encode("utf-8")
                mime_type = "text/css; charset=utf-8"
            else:
                blocked.append(True)
                raise ResourceError("Unsupported embedded resource type")
            return URLFetcherResponse(url, body=data, headers={"Content-Type": mime_type})

    diagnostics = Diagnostics()
    logger = logging.getLogger("weasyprint")
    logger.handlers = [diagnostics]
    logger.propagate = False

    def page_css(scale):
        # WeasyPrint zoom scales ALL units, so compensate the physical page size and margins.
        return CSS(string=(
            f"@page {{ size: {210 / scale}mm {297 / scale}mm !important;"
            f" margin: {10 / scale}mm {25 / scale}mm {10 / scale}mm {12.7 / scale}mm !important; }}"))

    def statement(prepared):
        return HTML(string=prepared, base_url="https://statement-resources.invalid/",
                    media_type="screen", url_fetcher=EmbeddedFetcher(allowed_protocols=["data"]))

    prepared = prepare_html(source)
    scale = fit_scale(statement(prepared).render(stylesheets=[page_css(SCALE)]))
    diagnostics.warnings = 0
    pdf = statement(prepared).write_pdf(stylesheets=[page_css(scale)], zoom=scale)
    if blocked or diagnostics.errors:
        raise ResourceError("A statement resource was blocked or could not be loaded")
    if diagnostics.warnings:
        print(f"PDF compatibility warnings: {diagnostics.warnings}", file=sys.stderr)
    Path(destination).write_bytes(pdf)


def fit_scale(document):
    # Like browsers and the original wkhtmltopdf smart shrinking, reduce the scale just enough
    # that the widest content fits the printable width instead of being clipped at the margin.
    widest = 0.0
    printable = None
    for page in document.pages:
        page_box = page._page_box
        printable = page_box.width
        for box in page_box.descendants():
            if box is not page_box and hasattr(box, "position_x") and hasattr(box, "margin_width"):
                widest = max(widest, box.position_x + box.margin_width() - page_box.content_box_x())
    if printable is None or widest <= printable + 0.5:
        return SCALE
    scale = SCALE * printable / widest
    if scale < MINIMUM_SCALE:
        raise ValueError("Statement content is too wide to fit the page")
    return scale


if __name__ == "__main__":
    try:
        render(Path(sys.argv[1]).read_text(encoding="utf-8-sig"), sys.argv[2])
    except ResourceError:
        print("Statement resource or image failure", file=sys.stderr)
        sys.exit(3)
