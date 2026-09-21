const fs = require('fs');
const path = require('path');
const { exit } = require("process");
const pdf = require('pdf-parse');
const stringHash = require("string-hash");
 
async function main() {
        try {
                const path1 = 'C:/Projects/RPA.ClaimStatements/RPA.ClaimStatements/RPA.ClaimStatements.Generator/PDF/';
                const path2 = 'C:/Projects/RPA.ClaimStatements/RPA.ClaimStatements/RPA.ClaimStatements.Generator/PDF/';
                const files1 = fs.readdirSync(path1);

                files1.forEach(statement => {
                        var statementFileNamePrefix = statement.substring(0, "RPA_OUT_BPS2021_ClmStmt_1100016414_106753051_".length);
                   
                        getFileContentsAsHash(statement, path1).then(function(hashFile1) { 
                                // Find the corresponding file in the other directory.
                                let files2 = fs.readdirSync(path2)
                                let duplicateStatements = files2.filter(file => file.startsWith(statementFileNamePrefix));
                                
                                if (duplicateStatements.length > 1) {
                                        console.log(`${statementFileNamePrefix} found more than once`);
                                        exit();
                                } else {
                                        getFileContentsAsHash(duplicateStatements[0], path2).then(function(hashFile2) { 
                                                if (hashFile1 != hashFile2) {
                                                        console.log(`PDF content different for: ${statementFileNamePrefix}`); 
                                                }       
                                        });
                                }
                        });
                        

                        
                });
        } catch (error) {
                console.log(error);
        }
};

async function getFileContentsAsHash(file, dir) {
        let dataBuffer = fs.readFileSync(path.join(dir, file));
        let hash = 0;

        await pdf(dataBuffer).then(function(data) {
                // number of pages
                //console.log(data.numpages);
                // number of rendered pages
                //console.log(data.numrender);
                // PDF info
                //console.log(data.info);
                // PDF metadata
                //console.log(data.metadata); 
                // PDF.js version
                // check https://mozilla.github.io/pdf.js/getting_started/
                //console.log(data.version);
                // PDF text
                //console.log(data.text); 
                hash = stringHash(data.text);
            });

        return hash;
}

main();