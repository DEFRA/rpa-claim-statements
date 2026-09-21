using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.SITI
{
    [Table("BPSPEN", Schema = "SITI")]
    public class BPSPEN
    {
        public Guid BPSPENID { get; set; }

        public Guid ClaimID { get; set; }

        public decimal OverDeclarationHectares { get; set; }

        public decimal OverDeclarationReduction { get; set; }

        public decimal LateClaimSubmissionPercent { get; set; }

        public decimal LateClaimSubmissionReduction { get; set; }

        public decimal LateEntitlementsApplicationPercent { get; set; }

        public decimal LateEntitlementsApplicationReduction { get; set; }

        public decimal LateEvidencePercent { get; set; }

        public decimal LateEvidenceReduction { get; set; }

        public decimal LateEntitlementsAmendmentPercent { get; set; }

        public decimal LateEntitlementsAmendmentReduction { get; set; }

        public decimal NonDeclarationPercent { get; set; }

        public decimal NonDeclarationReduction { get; set; }

        public decimal FDMPercent { get; set; }

        public decimal FDMReduction { get; set; }

        public decimal ReductionOfPaymentsOver150kPercent { get; set; }

        public decimal ReductionOfPaymentsOver150kReduction { get; set; }

        public decimal TotalBPS { get; set; }

        public decimal OverDeclarationPercent { get; set; }

        public decimal AdditionalOverDeclarationReduction { get; set; }

        public bool PreviousYearOverDeclaration { get; set; }

        public decimal LateChangePenaltyReduction { get; set; }

        public BPSPEN()
        {
            BPSPENID = Guid.NewGuid();
        }
    }
}