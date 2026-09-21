using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.SITI
{
    [Table("YF", Schema="SITI")]
    public class YF
    {
        public Guid YFID { get; set; }

        public Guid ClaimID { get; set; }

        public decimal NonSDANumber { get; set; }

        public decimal SDANumber { get; set; }

        public decimal MoorlandNumber { get; set; }

        public decimal TotalEntitlementValue { get; set; }

        public decimal EntitlementsUsedToClaim { get; set; }

        public decimal AvgEntitlementValue { get; set; }

        public decimal OverDeclarationArea { get; set; }

        public decimal OverDeclarationReduction { get; set; }

        public decimal LateClaimSubmissionPercent { get; set; }

        public decimal LateClaimSubmissionReduction { get; set; }

        public decimal LateEvidencePercent { get; set; }

        public decimal LateEvidenceReduction { get; set; }

        public decimal NonDeclarationPercent { get; set; }

        public decimal NonDeclarationReduction { get; set; }

        public decimal FDMPercent { get; set; }

        public decimal FDMReduction { get; set; }

        public decimal TotalYF { get; set; }

        public decimal OverDeclarationPercent { get; set; }

        public decimal AdditionalOverDeclarationReduction { get; set; }

        public bool PreviousYearOverDeclaration { get; set; }

        public decimal LateChangePenaltyReduction { get; set; }

        public YF()
        {
            YFID = Guid.NewGuid();
        }
    }
}