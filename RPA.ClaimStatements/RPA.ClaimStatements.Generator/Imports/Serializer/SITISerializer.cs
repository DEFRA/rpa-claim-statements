using RPA.ClaimStatements.Data.Entities.SITI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace RPA.ClaimStatements.Generator.Imports.Serializer
{
    public class SITISerializer : Serializer<Claim>
    {
        public override List<Claim> DeSerialize(string filename, CsvClassMap<Claim> map = null, bool hasHeader = false)
        {
            List<Claim> claims = new List<Claim>();

            using (TextReader tr = File.OpenText(filename))
            {
                var csv = new CsvReader(tr);
                csv.Configuration.WillThrowOnMissingField = false;
                csv.Configuration.Delimiter = "|";
                csv.Configuration.TrimFields = true;
                csv.Configuration.HasHeaderRecord = hasHeader;

                if (map != null)
                {
                    csv.Configuration.RegisterClassMap(map);
                }

                string currentLine = null;
                string lastLine = null;
                Claim claim = null;

                while (csv.Read())
                {
                    currentLine = csv.GetField<string>(0);

                    if (currentLine == "PTD")
                    {
                        if (lastLine != "PTD" && lastLine != null)
                        {
                            claim.CLD = new CLD
                            {
                                ClaimID = claim.ClaimID,
                                NonSDAAreaOnApplication = !string.IsNullOrEmpty(csv.GetField<string>(1)) ? csv.GetField<decimal>(1) : 0,
                                NonSDAAreaEligible = !string.IsNullOrEmpty(csv.GetField<string>(2)) ? csv.GetField<decimal>(2) : 0,
                                NonSDAEntitlements = !string.IsNullOrEmpty(csv.GetField<string>(3)) ? csv.GetField<decimal>(3) : 0,
                                SDAAreaOnApplication = !string.IsNullOrEmpty(csv.GetField<string>(4)) ? csv.GetField<decimal>(4) : 0,
                                SDAAreaEligible = !string.IsNullOrEmpty(csv.GetField<string>(5)) ? csv.GetField<decimal>(5) : 0,
                                SDAEntitlements = !string.IsNullOrEmpty(csv.GetField<string>(6)) ? csv.GetField<decimal>(6) : 0,
                                MoorlandAreaOnApplication = !string.IsNullOrEmpty(csv.GetField<string>(7)) ? csv.GetField<decimal>(7) : 0,
                                MoorlandAreaEligible = !string.IsNullOrEmpty(csv.GetField<string>(8)) ? csv.GetField<decimal>(8) : 0,
                                MoorlandEntitlements = !string.IsNullOrEmpty(csv.GetField<string>(9)) ? csv.GetField<decimal>(9) : 0,
                            };

                            if (claim.IsValid)
                            {
                                claim.SUMID = claim.SUM.SUMID;
                                claim.SUM2ID = claim.SUM2.SUM2ID;
                                claim.BPSID = claim.BPS.BPSID;
                                claim.BPSPENID = claim.BPSPEN.BPSPENID;
                                claim.GRID = claim.GR.GRID;
                                claim.GRPENID = claim.GRPEN.GRPENID;
                                claim.YFID = claim.YF.YFID;
                                claim.CLDID = claim.CLD.CLDID;

                                claims.Add(claim);
                            }

                            lastLine = "PTD";
                        }
                    }
                    else
                    {
                        switch (currentLine)
                        {
                            case "PTA1":

                                claim = new Claim();

                                claim.SUM = new SUM
                                {
                                    ClaimID = claim.ClaimID,
                                    SchemeYear = csv.GetField<int>(1),
                                    ApplicationID = csv.GetField<string>(2),
                                    CalculationDate = DateTime.ParseExact(csv.GetField<string>(3), "dd-MMM-yy HH.mm.ss", CultureInfo.InvariantCulture),
                                    InvoiceNumber = csv.GetField<string>(4),
                                    CalculationRefNumber = csv.GetField<string>(5),
                                    BusinessName = csv.GetField<string>(6),
                                    SBI = csv.GetField<int>(7),
                                    FRN = csv.GetField<long>(8),
                                    AddressLine1 = !string.IsNullOrEmpty(csv.GetField<string>(9)) ? csv.GetField<string>(9) : null,
                                    AddressLine2 = !string.IsNullOrEmpty(csv.GetField<string>(10)) ? csv.GetField<string>(10) : null,
                                    AddressLine3 = !string.IsNullOrEmpty(csv.GetField<string>(11)) ? csv.GetField<string>(11) : null,
                                    PostCode = !string.IsNullOrEmpty(csv.GetField<string>(12)) ? csv.GetField<string>(12) : null
                                };

                                lastLine = "PTA1";

                                break;
                            case "PTA2":

                                if (lastLine == "PTA1")
                                {
                                    claim.SUM2 = new SUM2
                                    {
                                        ClaimID = claim.ClaimID,
                                        BPSValue = !string.IsNullOrEmpty(csv.GetField<string>(1)) ? csv.GetField<decimal>(1) : 0,
                                        GreeningValue = !string.IsNullOrEmpty(csv.GetField<string>(2)) ? csv.GetField<decimal>(2) : 0,
                                        YoungFarmerValue = !string.IsNullOrEmpty(csv.GetField<string>(3)) ? csv.GetField<decimal>(3) : 0,
                                        SubTotal = !string.IsNullOrEmpty(csv.GetField<string>(4)) ? csv.GetField<decimal>(4) : 0,
                                        CrossCompliancePercent = !string.IsNullOrEmpty(csv.GetField<string>(5)) ? csv.GetField<decimal>(5) : 0,
                                        CrossComplianceReduction = !string.IsNullOrEmpty(csv.GetField<string>(6)) ? csv.GetField<decimal>(6) : 0,
                                        TotalClaimEuro = !string.IsNullOrEmpty(csv.GetField<string>(7)) ? csv.GetField<decimal>(7) : 0,
                                    };

                                    lastLine = "PTA2";
                                }

                                break;
                            case "PTBE":

                                if (lastLine == "PTA2")
                                {
                                    claim.BPS = new BPS
                                    {
                                        ClaimID = claim.ClaimID,
                                        NonSDANumber = !string.IsNullOrEmpty(csv.GetField<string>(1)) ? csv.GetField<decimal>(1) : 0,
                                        NonSDATotal = !string.IsNullOrEmpty(csv.GetField<string>(2)) ? csv.GetField<decimal>(2) : 0,
                                        SDANumber = !string.IsNullOrEmpty(csv.GetField<string>(3)) ? csv.GetField<decimal>(3) : 0,
                                        SDATotal = !string.IsNullOrEmpty(csv.GetField<string>(4)) ? csv.GetField<decimal>(4) : 0,
                                        MoorlandNumber = !string.IsNullOrEmpty(csv.GetField<string>(5)) ? csv.GetField<decimal>(5) : 0,
                                        MoorlandSDATotal = !string.IsNullOrEmpty(csv.GetField<string>(6)) ? csv.GetField<decimal>(6) : 0,
                                        AVGEntitlementValue = !string.IsNullOrEmpty(csv.GetField<string>(7)) ? csv.GetField<decimal>(7) : 0,
                                    };

                                    lastLine = "PTBE";
                                }

                                break;
                            case "PTBPEN":

                                if (lastLine == "PTBE")
                                {
                                    claim.BPSPEN = new BPSPEN
                                    {
                                        ClaimID = claim.ClaimID,
                                        OverDeclarationHectares = !string.IsNullOrEmpty(csv.GetField<string>(1)) ? csv.GetField<decimal>(1) : 0,
                                        OverDeclarationReduction = !string.IsNullOrEmpty(csv.GetField<string>(2)) ? csv.GetField<decimal>(2) : 0,
                                        LateClaimSubmissionPercent = !string.IsNullOrEmpty(csv.GetField<string>(3)) ? csv.GetField<decimal>(3) : 0,
                                        LateClaimSubmissionReduction = !string.IsNullOrEmpty(csv.GetField<string>(4)) ? csv.GetField<decimal>(4) : 0,
                                        LateEntitlementsApplicationPercent = !string.IsNullOrEmpty(csv.GetField<string>(5)) ? csv.GetField<decimal>(5) : 0,
                                        LateEntitlementsApplicationReduction = !string.IsNullOrEmpty(csv.GetField<string>(6)) ? csv.GetField<decimal>(6) : 0,
                                        LateEvidencePercent = !string.IsNullOrEmpty(csv.GetField<string>(7)) ? csv.GetField<decimal>(7) : 0,
                                        LateEvidenceReduction = !string.IsNullOrEmpty(csv.GetField<string>(8)) ? csv.GetField<decimal>(8) : 0,
                                        LateEntitlementsAmendmentPercent = !string.IsNullOrEmpty(csv.GetField<string>(9)) ? csv.GetField<decimal>(9) : 0,
                                        LateEntitlementsAmendmentReduction = !string.IsNullOrEmpty(csv.GetField<string>(10)) ? csv.GetField<decimal>(10) : 0,
                                        FDMPercent = !string.IsNullOrEmpty(csv.GetField<string>(11)) ? csv.GetField<decimal>(11) : 0,
                                        FDMReduction = !string.IsNullOrEmpty(csv.GetField<string>(12)) ? csv.GetField<decimal>(12) : 0,
                                        ReductionOfPaymentsOver150kPercent = !string.IsNullOrEmpty(csv.GetField<string>(13)) ? csv.GetField<decimal>(13) : 0,
                                        ReductionOfPaymentsOver150kReduction = !string.IsNullOrEmpty(csv.GetField<string>(14)) ? csv.GetField<decimal>(14) : 0,
                                        TotalBPS = !string.IsNullOrEmpty(csv.GetField<string>(15)) ? csv.GetField<decimal>(15) : 0,
                                        OverDeclarationPercent = !string.IsNullOrEmpty(csv.GetField<string>(16)) ? csv.GetField<decimal>(16) : 0,
                                        NonDeclarationPercent = !string.IsNullOrEmpty(csv.GetField<string>(17)) ? csv.GetField<decimal>(17) : 0,
                                        NonDeclarationReduction = !string.IsNullOrEmpty(csv.GetField<string>(18)) ? csv.GetField<decimal>(18) : 0,
                                        AdditionalOverDeclarationReduction = !string.IsNullOrEmpty(csv.GetField<string>(19)) ? csv.GetField<decimal>(19) : 0,
                                        PreviousYearOverDeclaration = !string.IsNullOrEmpty(csv.GetField<string>(20)) ? csv.GetField<bool>(20) : false
                                    };

                                    lastLine = "PTBPEN";
                                }

                                break;
                            case "PTBG":

                                if (lastLine == "PTBPEN")
                                {
                                    claim.GR = new GR
                                    {
                                        ClaimID = claim.ClaimID,
                                        NonSDANumber = !string.IsNullOrEmpty(csv.GetField<string>(1)) ? csv.GetField<decimal>(1) : 0,
                                        NonSDATotal = !string.IsNullOrEmpty(csv.GetField<string>(2)) ? csv.GetField<decimal>(2) : 0,
                                        SDANumber = !string.IsNullOrEmpty(csv.GetField<string>(3)) ? csv.GetField<decimal>(3) : 0,
                                        SDATotal = !string.IsNullOrEmpty(csv.GetField<string>(4)) ? csv.GetField<decimal>(4) : 0,
                                        MoorlandNumber = !string.IsNullOrEmpty(csv.GetField<string>(5)) ? csv.GetField<decimal>(5) : 0,
                                        MoorlandTotal = !string.IsNullOrEmpty(csv.GetField<string>(6)) ? csv.GetField<decimal>(6) : 0,
                                        AvgGreeningRate = !string.IsNullOrEmpty(csv.GetField<string>(7)) ? csv.GetField<decimal>(7) : 0
                                    };

                                    lastLine = "PTBG";
                                }

                                break;
                            case "PTBGPEN":

                                if (lastLine == "PTBG")
                                {
                                    claim.GRPEN = new GRPEN
                                    {
                                        ClaimID = claim.ClaimID,
                                        CropDiversificationNumber = !string.IsNullOrEmpty(csv.GetField<string>(1)) ? csv.GetField<decimal>(1) : 0,
                                        CropDiversificationRate = !string.IsNullOrEmpty(csv.GetField<string>(2)) ? csv.GetField<decimal>(2) : 0,
                                        CropDiversificationReduction = !string.IsNullOrEmpty(csv.GetField<string>(3)) ? csv.GetField<decimal>(3) : 0,
                                        PermanentGrasslandNumber = !string.IsNullOrEmpty(csv.GetField<string>(4)) ? csv.GetField<decimal>(4) : 0,
                                        PermanentGrasslandRate = !string.IsNullOrEmpty(csv.GetField<string>(5)) ? csv.GetField<decimal>(5) : 0,
                                        PermanentGrasslandReduction = !string.IsNullOrEmpty(csv.GetField<string>(6)) ? csv.GetField<decimal>(6) : 0,
                                        EFANumber = !string.IsNullOrEmpty(csv.GetField<string>(7)) ? csv.GetField<decimal>(7) : 0,
                                        EFARate = !string.IsNullOrEmpty(csv.GetField<string>(8)) ? csv.GetField<decimal>(8) : 0,
                                        EFAReduction = !string.IsNullOrEmpty(csv.GetField<string>(9)) ? csv.GetField<decimal>(9) : 0,
                                        LateApplicationPercent = !string.IsNullOrEmpty(csv.GetField<string>(10)) ? csv.GetField<decimal>(10) : 0,
                                        LateApplicationReduction = !string.IsNullOrEmpty(csv.GetField<string>(11)) ? csv.GetField<decimal>(11) : 0,
                                        LateEvidencePercent = !string.IsNullOrEmpty(csv.GetField<string>(12)) ? csv.GetField<decimal>(12) : 0,
                                        LateEvidenceReduction = !string.IsNullOrEmpty(csv.GetField<string>(13)) ? csv.GetField<decimal>(13) : 0,
                                        FDMReduction = !string.IsNullOrEmpty(csv.GetField<string>(14)) ? csv.GetField<decimal>(14) : 0,
                                        TotalGreening = !string.IsNullOrEmpty(csv.GetField<string>(15)) ? csv.GetField<decimal>(15) : 0,
                                        NonDeclarationPercent = !string.IsNullOrEmpty(csv.GetField<string>(16)) ? csv.GetField<decimal>(16) : 0,
                                        NonDeclarationReduction = !string.IsNullOrEmpty(csv.GetField<string>(17)) ? csv.GetField<decimal>(17) : 0,
                                        AdministrativeArea = !string.IsNullOrEmpty(csv.GetField<string>(18)) ? csv.GetField<decimal>(18) : 0,
                                        AdministrativeReduction = !string.IsNullOrEmpty(csv.GetField<string>(19)) ? csv.GetField<decimal>(19) : 0
                                    };

                                    lastLine = "PTBGPEN";
                                }

                                break;
                            case "PTBYF":

                                if (lastLine == "PTBGPEN")
                                {
                                    claim.YF = new YF
                                    {
                                        ClaimID = claim.ClaimID,
                                        EntitlementsUsedToClaim = !string.IsNullOrEmpty(csv.GetField<string>(1)) ? csv.GetField<decimal>(1) : 0,
                                        AvgEntitlementValue = !string.IsNullOrEmpty(csv.GetField<string>(2)) ? csv.GetField<decimal>(2) : 0,
                                        OverDeclarationArea = !string.IsNullOrEmpty(csv.GetField<string>(3)) ? csv.GetField<decimal>(3) : 0,
                                        OverDeclarationReduction = !string.IsNullOrEmpty(csv.GetField<string>(4)) ? csv.GetField<decimal>(4) : 0,
                                        LateClaimSubmissionPercent = !string.IsNullOrEmpty(csv.GetField<string>(5)) ? csv.GetField<decimal>(5) : 0,
                                        LateClaimSubmissionReduction = !string.IsNullOrEmpty(csv.GetField<string>(6)) ? csv.GetField<decimal>(6) : 0,
                                        LateEvidencePercent = !string.IsNullOrEmpty(csv.GetField<string>(7)) ? csv.GetField<decimal>(7) : 0,
                                        LateEvidenceReduction = !string.IsNullOrEmpty(csv.GetField<string>(8)) ? csv.GetField<decimal>(8) : 0,                                        
                                        FDMReduction = !string.IsNullOrEmpty(csv.GetField<string>(9)) ? csv.GetField<decimal>(9) : 0,
                                        TotalYF = !string.IsNullOrEmpty(csv.GetField<string>(10)) ? csv.GetField<decimal>(10) : 0,
                                        OverDeclarationPercent = !string.IsNullOrEmpty(csv.GetField<string>(11)) ? csv.GetField<decimal>(11) : 0,
                                        NonSDANumber = !string.IsNullOrEmpty(csv.GetField<string>(12)) ? csv.GetField<decimal>(12) : 0,
                                        SDANumber = !string.IsNullOrEmpty(csv.GetField<string>(13)) ? csv.GetField<decimal>(13) : 0,
                                        MoorlandNumber = !string.IsNullOrEmpty(csv.GetField<string>(14)) ? csv.GetField<decimal>(14) : 0,
                                        TotalEntitlementValue = !string.IsNullOrEmpty(csv.GetField<string>(15)) ? csv.GetField<decimal>(15) : 0,
                                        NonDeclarationPercent = !string.IsNullOrEmpty(csv.GetField<string>(16)) ? csv.GetField<decimal>(16) : 0,
                                        NonDeclarationReduction = !string.IsNullOrEmpty(csv.GetField<string>(17)) ? csv.GetField<decimal>(17) : 0,
                                        AdditionalOverDeclarationReduction = !string.IsNullOrEmpty(csv.GetField<string>(18)) ? csv.GetField<decimal>(18) : 0,
                                        PreviousYearOverDeclaration = !string.IsNullOrEmpty(csv.GetField<string>(19)) ? csv.GetField<bool>(19) : false
                                    };

                                    lastLine = "PTBYF";
                                }

                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return claims;
        }
    }
}
