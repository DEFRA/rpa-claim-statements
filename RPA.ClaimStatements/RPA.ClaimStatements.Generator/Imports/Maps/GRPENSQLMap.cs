using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Imports.Maps
{
    public static class GRPENSQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("GRPENID,GRPENID");
            mappings.Add("ClaimID,ClaimID");
            mappings.Add("CropDiversificationNumber,CropDiversificationNumber");
            mappings.Add("CropDiversificationRate,CropDiversificationRate");
            mappings.Add("CropDiversificationReduction,CropDiversificationReduction");
            mappings.Add("PermanentGrasslandNumber,PermanentGrasslandNumber");
            mappings.Add("PermanentGrasslandRate,PermanentGrasslandRate");
            mappings.Add("PermanentGrasslandReduction,PermanentGrasslandReduction");
            mappings.Add("EFANumber,EFANumber");
            mappings.Add("EFARate,EFARate");
            mappings.Add("EFAReduction,EFAReduction");
            mappings.Add("LateApplicationPercent,LateApplicationPercent");
            mappings.Add("LateApplicationReduction,LateApplicationReduction");
            mappings.Add("LateEvidencePercent,LateEvidencePercent");
            mappings.Add("LateEvidenceReduction,LateEvidenceReduction");
            mappings.Add("NonDeclarationPercent,NonDeclarationPercent");
            mappings.Add("NonDeclarationReduction,NonDeclarationReduction");
            mappings.Add("FDMReduction,FDMReduction");
            mappings.Add("TotalGreening,TotalGreening");
            mappings.Add("AdministrativeArea,AdministrativeArea");
            mappings.Add("AdministrativeReduction,AdministrativeReduction");

            return mappings;
        }
    }
}
