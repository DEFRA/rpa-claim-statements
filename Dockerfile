# syntax=docker/dockerfile:1
# Builds rpa-claim-statements (Web) per Defra .NET container standards, mirroring rpa-cph-non-subsidy.
ARG PARENT_VERSION=2.1.9-dotnet10.0

# Development stage: SDK-based parent image used to restore, test and publish the app.
FROM defradigital/dotnetcore-development:$PARENT_VERSION AS development
ENV TZ="Europe/London"
ARG PARENT_VERSION

LABEL uk.gov.defra.parent-image=defradigital/dotnetcore-development:${PARENT_VERSION}

RUN mkdir -p ./RPA.ClaimStatements.Generator/ ./RPA.ClaimStatements.Web/ ./RPA.ClaimStatements.Web.Tests/

# Restore dependencies first so the layer is cached (Web.Tests references Web and Generator).
# NUGET_SOURCE is an optional non-secret package source (e.g. an internal feed mirror).
ARG NUGET_SOURCE
COPY --chown=dotnet:dotnet RPA.ClaimStatements/RPA.ClaimStatements.Generator/*.csproj ./RPA.ClaimStatements.Generator/
COPY --chown=dotnet:dotnet RPA.ClaimStatements/RPA.ClaimStatements.Web/*.csproj ./RPA.ClaimStatements.Web/
COPY --chown=dotnet:dotnet RPA.ClaimStatements/RPA.ClaimStatements.Web.Tests/*.csproj ./RPA.ClaimStatements.Web.Tests/
RUN if [ -n "$NUGET_SOURCE" ]; then \
        dotnet restore ./RPA.ClaimStatements.Web.Tests/RPA.ClaimStatements.Web.Tests.csproj --source "$NUGET_SOURCE"; \
    else \
        dotnet restore ./RPA.ClaimStatements.Web.Tests/RPA.ClaimStatements.Web.Tests.csproj; \
    fi

# Copy in app and test code.
COPY --chown=dotnet:dotnet RPA.ClaimStatements/RPA.ClaimStatements.Generator/ ./RPA.ClaimStatements.Generator/
COPY --chown=dotnet:dotnet RPA.ClaimStatements/RPA.ClaimStatements.Web/ ./RPA.ClaimStatements.Web/
COPY --chown=dotnet:dotnet RPA.ClaimStatements/RPA.ClaimStatements.Web.Tests/ ./RPA.ClaimStatements.Web.Tests/

# Build the app.
RUN dotnet publish ./RPA.ClaimStatements.Web/ -c Release -o /home/dotnet/out

ARG PORT=8080
ENV PORT=${PORT}
EXPOSE ${PORT}

# Root-owned so the dotnet user can't modify its own startup script.
USER root
COPY --chown=root:root dev-entrypoint.sh ./
RUN chmod +x ./dev-entrypoint.sh
USER dotnet
CMD ["./dev-entrypoint.sh"]

# Production stage
FROM defradigital/dotnetcore:$PARENT_VERSION AS production
ARG PARENT_VERSION
ARG BUILD_VERSION=local
ARG VCS_REF=unknown
ARG BUILD_DATE=unknown

LABEL uk.gov.defra.parent-image=defradigital/dotnetcore:${PARENT_VERSION} \
      org.opencontainers.image.title="rpa-claim-statements" \
      org.opencontainers.image.version="$BUILD_VERSION" \
      org.opencontainers.image.revision="$VCS_REF" \
      org.opencontainers.image.created="$BUILD_DATE"

ARG PORT=8080
# ICU is installed below so en-GB culture formatting works; invariant mode is switched off.
ENV ASPNETCORE_URLS=http://*:${PORT} \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE ${PORT}

USER root
RUN apk add --no-cache icu-libs icu-data-full

COPY --from=development --chown=root:root /home/dotnet/out/ ./

RUN chmod -R a-w /home/dotnet
USER dotnet

CMD ["dotnet", "RPA.ClaimStatements.Web.dll"]
