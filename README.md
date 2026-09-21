# Introduction 
The Claim Statement Generator (CSG) generates a claim statement for every BPS claim upon every change in settlement position.

See the project wiki for more information: https://dev.azure.com/defradev/Defra%20DDTS%20Development%20Team/_wiki/wikis/Defra%20DDTS%20Development%20Team%20Wiki/48/RPA.Claim-Statement-Generator

# Getting Started
The CSG consists of two projects:  
 - RPA.ClaimStatements.Generator
    - This is a .net console application that processes DAX and SITI files, stores the data in a SQL database and outputs Claims Statement as PDfs.
 - RPA.ClaimStatements.Web
    - This is a .net web MVC application and allows users to check for any statement generation failures.

# Testing
 Both projects have unit tests but it is also possible to validate the content of PDFs. This can be helpful when a change has been made to the CSG that doesn't involve PDF content changes.

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
10. At a command prompt in the ./PdfCompare folder run the following: node .
11. If there are no differences the command will complete with no output. 
