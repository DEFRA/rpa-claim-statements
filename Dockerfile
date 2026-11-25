# syntax=docker/dockerfile:1
# Builds the Web and Generator images per the Defra .NET container standards
# (github.com/DEFRA/defra-docker-dotnetcore). The last stage is the Web image, so a plain
# `docker build` builds Web; build the Generator with `--target generator`.
ARG PARENT_VERSION=2.1.9-dotnet10.0

FROM scratch AS renderer-source
ADD --checksum=sha256:dd3e50a83d8e21a80c1271fdd16cb4c234d96e03906971aea41e3f1b82c49b74 \
    https://github.com/Kozea/WeasyPrint/archive/refs/tags/v70.0.tar.gz \
    /weasyprint-70.0.tar.gz

# Development stage: SDK parent image used to restore and publish both applications.
FROM defradigital/dotnetcore-development:${PARENT_VERSION} AS development
ARG PARENT_VERSION
LABEL uk.gov.defra.parent-image=defradigital/dotnetcore-development:${PARENT_VERSION}
USER root
WORKDIR /src
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 TMPDIR=/build-scratch
COPY RPA.ClaimStatements/RPA.ClaimStatements.Web/RPA.ClaimStatements.Web.csproj RPA.ClaimStatements/RPA.ClaimStatements.Web/
COPY RPA.ClaimStatements/RPA.ClaimStatements.Generator/RPA.ClaimStatements.Generator.csproj RPA.ClaimStatements/RPA.ClaimStatements.Generator/
# NUGET_SOURCE is an optional non-secret package source (for example an internal mirror); the default is nuget.org.
ARG NUGET_SOURCE
RUN mkdir -p /build-scratch && \
    if [ -n "$NUGET_SOURCE" ]; then \
        dotnet restore RPA.ClaimStatements/RPA.ClaimStatements.Web/RPA.ClaimStatements.Web.csproj \
          --source "$NUGET_SOURCE" -r linux-musl-x64 -p:SelfContained=false -p:UseAppHost=false; \
    else \
        dotnet restore RPA.ClaimStatements/RPA.ClaimStatements.Web/RPA.ClaimStatements.Web.csproj \
          -r linux-musl-x64 -p:SelfContained=false -p:UseAppHost=false; \
    fi
COPY . .
RUN dotnet publish RPA.ClaimStatements/RPA.ClaimStatements.Web/RPA.ClaimStatements.Web.csproj \
      -c Release --no-restore -o /publish -r linux-musl-x64 --self-contained false \
      -p:UseAppHost=false

FROM development AS generator-publish
RUN dotnet publish RPA.ClaimStatements/RPA.ClaimStatements.Generator/RPA.ClaimStatements.Generator.csproj \
      -c Release --no-restore -o /generator -r linux-musl-x64 --self-contained false \
      -p:UseAppHost=false

# Shared runtime layer for both images: WeasyPrint, native dependencies and working folders.
FROM defradigital/dotnetcore:${PARENT_VERSION} AS base
ARG PARENT_VERSION
LABEL uk.gov.defra.parent-image=defradigital/dotnetcore:${PARENT_VERSION}
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    CLAIMSTATEMENTS_ROOT=/var/lib/claimstatements/work \
    HOME=/var/lib/claimstatements/home \
    TMPDIR=/var/lib/claimstatements/scratch \
    PATH=/opt/weasyprint/bin:$PATH
WORKDIR /app
COPY container/weasyprint-requirements.txt /tmp/weasyprint-requirements.txt
USER root
RUN --mount=from=renderer-source,target=/renderer-source \
    apk add --no-cache python3 pango harfbuzz-subset py3-cffi py3-cssselect2 py3-fonttools py3-brotli py3-zopfli \
      py3-pillow py3-pydyf py3-pyphen py3-tinyhtml5 py3-tinycss2 \
      font-liberation fontconfig libstdc++ icu-libs icu-data-full && \
    apk add --no-cache --virtual .weasy-build py3-pip py3-flit-core && \
    python3 -m venv --system-site-packages --without-pip /opt/weasyprint && \
    python3 -m pip --python /opt/weasyprint install --no-index --find-links=/renderer-source --no-cache-dir --no-deps --no-build-isolation \
      --require-hashes -r /tmp/weasyprint-requirements.txt && \
    /opt/weasyprint/bin/python -m pip check && \
    apk del .weasy-build && \
    mkdir -p /var/lib/claimstatements/work/XML /var/lib/claimstatements/work/DOCX \
      /var/lib/claimstatements/work/HTML /var/lib/claimstatements/work/PDF \
      /var/lib/claimstatements/work/Uploads /var/lib/claimstatements/work/Templates \
      /var/lib/claimstatements/home /var/lib/claimstatements/scratch && \
    chown -R 1000:1000 /var/lib/claimstatements
USER 1000:1000

# Run-to-completion job image: only the console application, no Web files or listener.
FROM base AS generator
ARG BUILD_VERSION=local
ARG VCS_REF=unknown
ARG BUILD_DATE=unknown
LABEL org.opencontainers.image.title="rpa-claim-statements-generator" \
      org.opencontainers.image.version="$BUILD_VERSION" \
      org.opencontainers.image.revision="$VCS_REF" \
      org.opencontainers.image.created="$BUILD_DATE"
COPY --from=generator-publish /generator/ ./
ENTRYPOINT ["dotnet", "RPA.ClaimStatements.Generator.dll"]

# Production stage: the Web application (last stage, so it is the default build target).
FROM base AS production
ARG BUILD_VERSION=local
ARG VCS_REF=unknown
ARG BUILD_DATE=unknown
LABEL org.opencontainers.image.title="rpa-claim-statements-web" \
      org.opencontainers.image.version="$BUILD_VERSION" \
      org.opencontainers.image.revision="$VCS_REF" \
      org.opencontainers.image.created="$BUILD_DATE"
COPY --from=development /publish/ ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "RPA.ClaimStatements.Web.dll"]
