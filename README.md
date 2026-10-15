# Introduction 
The Claim Statement Generator (CSG) generates a claim statement for every BPS claim upon every change in settlement position.

See the project wiki for more information: https://dev.azure.com/defradev/Defra%20DDTS%20Development%20Team/_wiki/wikis/Defra%20DDTS%20Development%20Team%20Wiki/48/RPA.Claim-Statement-Generator

# Getting Started
The CSG is a .NET 10 solution (`RPA.ClaimStatements/RPA.ClaimStatements.sln`) of four projects plus a utility:
 - RPA.ClaimStatements.Generator
    - A .NET console application that processes DAX and SITI files, stores the data in a SQL Server database and outputs Claim Statements as PDFs.
 - RPA.ClaimStatements.Web
    - An ASP.NET Core MVC application (Windows authentication) that lets users check for statement generation failures, manage suppressions and configuration, and trigger generation. It runs the Generator in-process.
 - RPA.ClaimStatements.Generator.Tests and RPA.ClaimStatements.Web.Tests
    - NUnit unit tests.
 - PdfCompare
    - A standalone Node.js utility to compare PDF content between two builds (see Testing).

## Prerequisites
 - Windows. The Generator uses `System.Drawing`, `WinSCP.exe` and `wkhtmltopdf.exe`, so `net10.0` is Windows-only in practice.
 - The .NET 10 SDK (10.0.x) to build; the .NET 10 runtime (ASP.NET Core runtime for the web application) to run.
 - SQL Server (Express, LocalDB or full) for the `ClaimStatementsContext` database and, for the web application, the `Security` database.
 - The Word templates `RPA.ClaimStatements.Generator/Templates/ClaimTemplate.docx` and `ClaimTemplateXB.docx`. They are not in source control (`*.docx` is git-ignored); copy them in before building, otherwise the build fails with MSB3030 for those files.
 - To host in IIS: the .NET 10 Hosting Bundle (Windows Server Hosting). Not needed for local development.

## Configuration
Configuration is in `App.config` of each project, with one transform per build configuration (`App.Debug.config`, `App.Release.config`, `App.SIT.config`, `App.UAT.config`, `App.Production.config`; applied by SlowCheetah at build time). The result is `<AssemblyName>.dll.config` next to the binaries:
 - `ClaimStatementsContext` connection string (both projects).
 - `IDTSecurityConnection` connection string and the `IDTSecurityApplicationName` app setting (Web): the `Security` database that holds users, roles and role inheritance.
 - `system.net/mailSettings/smtp` for the tracking report e-mail (pickup directory by default, network host in SIT/UAT/Production).

Everything else is in the database: `CS.Configuration` (feature switches such as `Generation`, `Validation`, `Archive`, `SITI Load`, `XB Load`, `AP Load`, `AR Load`, `Maximum Batch Size`, `Email Account`), `CS.Folders` (inbound, archive and PDF outbound folders, SITI FTP host), `CS.Transformations`, `CS.ConversionRates` and `CS.SchemeYears`. The `SITI Inbound` folder path must end with a backslash (WinSCP requires it).

## Build, test and publish
```
cd RPA.ClaimStatements
dotnet restore RPA.ClaimStatements.sln
dotnet build RPA.ClaimStatements.sln -c Release      # Debug, Release, SIT, UAT and Production are solution configurations
dotnet test RPA.ClaimStatements.sln -c Release
dotnet publish RPA.ClaimStatements.Generator -c Release -o <folder>
dotnet publish RPA.ClaimStatements.Web -c Release -o <folder>
```
Both projects copy what they need next to the binaries: `WinSCP.exe`, the pinned `wkhtmltopdf` folder (version 0.12.4, so PDFs match earlier releases), `Templates` and `XSLT` (the Web project links them from the Generator project).

## Running the Generator
The Generator resolves its working folders (`Templates`, `XSLT`, `XML`, `DOCX`, `HTML`, `PDF`) two levels above its binaries, as before. Deploy it as `<root>\bin\<Configuration>\...` with `<root>\Templates` and `<root>\XSLT` beside `bin`. With the `BPS` or `XB` argument the working folders must already exist; `ALL` creates them.

```
RPA.ClaimStatements.Generator.exe ALL 10     # imports, then BPS and XB generation, at most 10 statements per type
RPA.ClaimStatements.Generator.exe BPS 5      # imports, then BPS generation only (XB likewise)
RPA.ClaimStatements.Generator.exe I          # imports only
RPA.ClaimStatements.Generator.exe            # prompts for the statement type and batch size
```
Each run: imports SITI (FTP via WinSCP), XB, DAX AP and DAX AR files, generates statements (only when `Generation` is `Active`; `Validation` reconciles Part A and Part C), publishes the PDFs and sends the tracking report. Only one run of each kind can be active at a time (named mutexes).

## Running the web application
Locally (Kestrel, Windows authentication through Negotiate):
```
cd RPA.ClaimStatements/RPA.ClaimStatements.Web
dotnet run
```
Access is granted by roles read from the `Security` database for the signed-in `DOMAIN\user` (application name from `IDTSecurityApplicationName`, role inheritance resolved by the database). The roles are named `Claim Statement Generator: <View Claim Statements | Trigger Generation | Manage Exceptions | Manage Suppressions | Manual Upload | Download Data | Force Generation>`. A signed-in user without the needed role receives HTTP 403.

In IIS (in-process hosting; this setup has not been verified on IIS, see Known limitations):
 - Install the .NET 10 Hosting Bundle, create an application whose path is the web publish folder, and use an application pool with "No Managed Code".
 - Enable Windows Authentication and disable Anonymous Authentication for the site (the published `web.config` does not set them).
 - The application pool identity needs access to the `ClaimStatementsContext` and `Security` databases (integrated security), write access to the application folder (`Uploads`, `DOCX`, `HTML`, `PDF`, `XML`), the PDF outbound and archive folders from `CS.Folders`, and the mail pickup folder.
 - Use an application pool identity with a user profile (or configure ASP.NET Core Data Protection key storage) so that antiforgery and TempData keys survive restarts.

# Testing
 Both test projects run with `dotnet test`. It is also possible to validate the content of PDFs. This can be helpful when a change has been made to the CSG that doesn't involve PDF content changes.

 To check PDF content between two versions of the CSG do the following:

 1. Set up the data. Run the following SQL (in UAT):

    create table InvoiceTestData
    (
        InvoiceNumber nvarchar(max),
        apid uniqueidentifier
    )

    delete InvoiceTestData
    insert into InvoiceTestData
    select top <the number of statements required>  a.Invoice, a.APID
    from dax.AP a
        inner join siti.sum s on s.FRN = a.Supplier and s.SchemeYear = a.MarketingYear 
            and CONCAT('SITI', substring(a.Invoice, 2, 7)) = s.InvoiceNumber
    where MarketingYear = '2017' and LastSettlementDate > '2017-12-19' --and a.LogID is not null
    order by apid asc

    delete cs.errors

    update a
    set LogID = null, Active = 1
    from dax.ap a
        inner join InvoiceTestData t on t.apid = a.APID and t.invoicenumber = a.Invoice 

2. Clear the PDF output folder. The location of the PDF output folder can be found here: RPA.ClaimStatements.CS.Folders 
3. Run the CSG. 
4. Check the PDFs have been created.
5. Move the PDFs to new folder
6. Install the new version of the CSG
7. Repeat steps 1-4
8. Go to the folder ./PdfCompre
9. Open index.js and update the variables path1 and path2 with the location of the above folders
10. At a command prompt in the ./PdfCompare folder run `npm install` once, then run the following: node .
11. If there are no differences the command will complete with no output. 

# Known limitations and accepted risks
Recorded during the .NET 10 upgrade (details and evidence in `.github/modernize/runtime-framework-compatibility/plan.md`):
 - IIS-hosted behaviour (in-process hosting, IIS Windows authentication, application pool identity, data protection keys) has not been verified; the web application has been verified on Kestrel only.
 - The Generation Log page's manual-upload form posts to `Home/Upload`, which does not exist in the source; it is shown only when the `Manual Upload` setting is `Active` and the user has the `Manual Upload` role, and submitting it returns 404.
 - Front-end libraries are old and have known medium-severity advisories: jQuery 1.10.2 (end of life) and Bootstrap 3.0.0. `wkhtmltopdf` (0.12.4) is affected by CVE-2022-35583 and has no patched release. These await an owner decision.
 - Behaviour differences from the .NET Framework version: users who are signed in but lack a role now get HTTP 403 (previously 401); a missing antiforgery token gives 400 (previously 500); generation error messages that wrap several errors now include the inner message.
