using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.SITI
{
    [Table("GRPEN", Schema = "SITI")]
    public class GRPEN
    {
        public Guid GRPENID { get; set; }

        public Guid ClaimID { get; set; }

        public decimal CropDiversificationNumber { get; set; }

        public decimal CropDiversificationNumberAdditional { get; set; }

        public decimal CropDiversificationRate { get; set; }

        public decimal CropDiversificationReduction { get; set; } 

        public decimal PermanentGrasslandNumber { get; set; }

        public decimal PermanentGrasslandRate { get; set; }

        public decimal PermanentGrasslandReduction { get; set; }

        public decimal EFANumber { get; set; }

        public decimal EFANumberAdditional { get; set; }

        public decimal EFARate { get; set; }

        public decimal EFAReduction { get; set; }

        public decimal LateApplicationPercent { get; set; }

        public decimal LateApplicationReduction { get; set; }

        public decimal LateEvidencePercent { get; set; }

        public decimal LateEvidenceReduction { get; set; }

        public decimal NonDeclarationPercent { get; set; }

        public decimal NonDeclarationReduction { get; set; }

        public decimal FDMReduction { get; set; }

        public decimal TotalGreening { get; set; }

        public decimal AdministrativeArea { get; set; }

        public decimal AdministrativeReduction { get; set; }

        public decimal LateChangePenaltyReduction { get; set; }

        public GRPEN()
        {
            GRPENID = Guid.NewGuid();
        }
    }
}