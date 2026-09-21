using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.Entities.XB;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Generation;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace RPA.ClaimStatements.Generator.Components.FuelTank
{
    public class FuelTank : IFuelTank
    {
        IClaimStatementsContext db;
        IConfigurationService configurationService;
        IErrorService errorService;
        IConversionService conversionService;
        ICurrencyService currencyService;
        IMonitorService monitorService;

        public FuelTank(IClaimStatementsContext context, IConfigurationService configurationService, IErrorService errorService,
            IConversionService conversionService, ICurrencyService currencyService, IMonitorService monitorService)
        {
            this.db = context;
            this.configurationService = configurationService;
            this.errorService = errorService;
            this.conversionService = conversionService;
            this.currencyService = currencyService;
            this.monitorService = monitorService;
        }

        public Request[] Fill(string statementType, int maximumBatchSize, bool dbFunctions = true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Filling Fuel Tank with - {0}", statementType);
            Console.ResetColor();

            Monitor loadMonitor = new Monitor(monitorService.GetMonitorProcessId("Load Claim Statement Data"), string.Format("Load Claim Statement Data - {0}", statementType));
            monitorService.CreateMonitor(loadMonitor);

            List<DAXOutstanding> daxOutstanding = GetTriggers(statementType);

            List<DAXOutstanding> daxOutstandingFinal = daxOutstanding.GroupBy(x => new { x.FRN, x.MarketingYear }).Select(x => x.First()).ToList();

            maximumBatchSize = RefineMaximumBatchSize(maximumBatchSize, daxOutstandingFinal.Count);

            daxOutstandingFinal = OrderDAX(daxOutstandingFinal, maximumBatchSize);

            var requests = BuildRequests(daxOutstandingFinal, maximumBatchSize, statementType, dbFunctions);

            monitorService.EndMonitor(loadMonitor);

            return requests;
        }

        private List<DAXOutstanding> GetTriggers(string statementType)
        {
            int[] activeSchemeYears = db.SchemeYears.AsNoTracking().Where(x => x.Active).Select(x => x.SchemeYearNumber).ToArray();

            List<AP> apOutstanding = db.AP.AsNoTracking().Where(x => x.LogID == null && x.Active && (activeSchemeYears.Any((p => p == x.MarketingYear)))).ToList();
            List<AR> arOutstanding = db.AR.AsNoTracking().Where(x => x.LogID == null && (activeSchemeYears.Any((p => p == x.H_MarketingYear)))).ToList();
            List<Error> errorsOutstanding = db.Errors.AsNoTracking().Where(x => x.Resolved == false && (activeSchemeYears.Any((p => p == x.SchemeYear)))).ToList();
            List<Suppression> suppressionsOutstanding = db.Suppressions.AsNoTracking().Where(x => x.SuppressionEnd == null && (activeSchemeYears.Any((p => p == x.SchemeYear)))).ToList();
            List<XB> xbList = db.XB.AsNoTracking().Where(x => x.Active && (activeSchemeYears.Any((p => p == x.SchemeYear)))).ToList();

            List<Skip> skipList = BuildSkips(errorsOutstanding, suppressionsOutstanding, xbList, statementType);

            List<DAXOutstanding> daxOutstanding = BuildDAXOutstanding(apOutstanding, arOutstanding, skipList, xbList, statementType);

            return daxOutstanding;
        }

        public List<Skip> BuildSkips(List<Error> errorsOutstanding, List<Suppression> suppressionsOutstanding, List<XB> xbList, string statementType)
        {
            List<Skip> skipList = new List<Skip>();

            for (int i = 0; i < errorsOutstanding.Count; i++)
            {
                Skip skip = new Skip(errorsOutstanding[i].FRN, errorsOutstanding[i].SchemeYear.Value);
                skipList.Add(skip);
            }

            for (int i = 0; i < suppressionsOutstanding.Count; i++)
            {
                Skip skip = new Skip(suppressionsOutstanding[i].FRN, suppressionsOutstanding[i].SchemeYear);
                skipList.Add(skip);
            }

            if (statementType == "BPS")
            {
                for (int i = 0; i < xbList.Count; i++)
                {
                    Skip skip = new Skip(xbList[i].FRN, xbList[i].SchemeYear);
                    skipList.Add(skip);
                }
            }

            return skipList;
        }

        private List<DAXOutstanding> BuildDAXOutstanding(List<AP> apOutstanding, List<AR> arOutstanding, List<Skip> skipList, List<XB> xbList, string statementType)
        {
            List<DAXOutstanding> daxOutstanding = new List<DAXOutstanding>();

            for (int i = 0; i < apOutstanding.Count; i++)
            {
                if (!skipList.Exists(x => x.FRN == apOutstanding[i].Supplier && x.SchemeYear == apOutstanding[i].MarketingYear))
                {
                    switch (statementType)
                    {
                        case "BPS":
                            DAXOutstanding dax = new DAXOutstanding(apOutstanding[i].Invoice, apOutstanding[i].Supplier, apOutstanding[i].MarketingYear, apOutstanding[i].LastSettlementDate);
                            daxOutstanding.Add(dax);
                            break;
                        case "XB":
                            if (xbList.Exists(x => x.FRN == apOutstanding[i].Supplier && x.SchemeYear == apOutstanding[i].MarketingYear))
                            {
                                DAXOutstanding dax2 = new DAXOutstanding(apOutstanding[i].Invoice, apOutstanding[i].Supplier, apOutstanding[i].MarketingYear, apOutstanding[i].LastSettlementDate);
                                daxOutstanding.Add(dax2);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            for (int i = 0; i < arOutstanding.Count; i++)
            {
                if (!skipList.Exists(x => x.FRN == arOutstanding[i].H_InvoiceAccount && x.SchemeYear == arOutstanding[i].H_MarketingYear))
                {
                    switch (statementType)
                    {
                        case "BPS":
                            DAXOutstanding dax = new DAXOutstanding(arOutstanding[i].InvoiceNumber, arOutstanding[i].H_InvoiceAccount, arOutstanding[i].H_MarketingYear, arOutstanding[i].H_InvoiceDate);
                            daxOutstanding.Add(dax);
                            break;
                        case "XB":
                            if (xbList.Exists(x => x.FRN == arOutstanding[i].H_InvoiceAccount && x.SchemeYear == arOutstanding[i].H_MarketingYear))
                            {
                                DAXOutstanding dax2 = new DAXOutstanding(arOutstanding[i].InvoiceNumber, arOutstanding[i].H_InvoiceAccount, arOutstanding[i].H_MarketingYear, arOutstanding[i].H_InvoiceDate);
                                daxOutstanding.Add(dax2);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (daxOutstanding.Count > 1)
            {
                daxOutstanding = daxOutstanding.OrderByDescending(x => x.Invoice).ToList();
            }

            return daxOutstanding;
        }

        public int RefineMaximumBatchSize(int maximumBatchSize, int daxOutstandingTotal)
        {
            int maximum = 0;

            int.TryParse(configurationService.GetValue("Maximum Batch Size"), out maximum);

            if (maximum < maximumBatchSize)
            {
                maximumBatchSize = maximum;
            }

            if (daxOutstandingTotal <= maximumBatchSize)
            {
                maximumBatchSize = daxOutstandingTotal;
            }

            return maximumBatchSize;
        }

        public List<DAXOutstanding> OrderDAX(List<DAXOutstanding> daxOutstandingFinal, int maximumBatchSize)
        {
            if (daxOutstandingFinal.Count > maximumBatchSize)
            {
                return daxOutstandingFinal.OrderBy(x => x.SettlementDate).ToList();
            }

            return daxOutstandingFinal;
        }

        private Request[] BuildRequests(List<DAXOutstanding> daxOutstandingFinal, int maximumBatchSize, string statementType, bool dbFunctions = true)
        {
            Request[] requests = new Request[maximumBatchSize];

            int count = 0;

            for (int i = 0; i < daxOutstandingFinal.Count; i++)
            {
                if (count == maximumBatchSize)
                {
                    break;
                }

                Claim claim = GetClaim(daxOutstandingFinal[i].Invoice, daxOutstandingFinal[i].FRN, daxOutstandingFinal[i].MarketingYear);

                if (claim != null)
                {
                    Request request;
                    int marketingYear = daxOutstandingFinal[i].MarketingYear;
                    List<Rate> rates = GetRates(daxOutstandingFinal[i].MarketingYear);
                    decimal conversionRate = rates.Where(x => x.Description == "Euro Exchange").Select(x => x.Value).FirstOrDefault();
                    List<Invoice> invoices = GetInvoices(daxOutstandingFinal[i].FRN, daxOutstandingFinal[i].MarketingYear, conversionRate, statementType, dbFunctions);

                    switch (statementType)
                    {
                        case "BPS":
                            request = new Request(claim, daxOutstandingFinal[i].SettlementDate, invoices, rates);
                            requests[count] = request;
                            count++;
                            break;
                        case "XB":
                            XBData xbData = GetXBData(daxOutstandingFinal[i].Invoice, daxOutstandingFinal[i].FRN, daxOutstandingFinal[i].MarketingYear, daxOutstandingFinal[i].SettlementDate, dbFunctions);

                            if (xbData != null)
                            {
                                request = new Request(claim, xbData, daxOutstandingFinal[i].SettlementDate, invoices, rates);
                                requests[count] = request;
                                count++;
                            }
                            else
                            {
                                errorService.Log(daxOutstandingFinal[i].FRN, daxOutstandingFinal[i].MarketingYear, string.Format("No Cross Border data for invoice {0}", conversionService.TransformInvoice(daxOutstandingFinal[i].Invoice, "SITI")));
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    errorService.Log(daxOutstandingFinal[i].FRN, daxOutstandingFinal[i].MarketingYear, string.Format("No SITI data for invoice {0}", conversionService.TransformInvoice(daxOutstandingFinal[i].Invoice, "SITI")));
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Fuel tank filled with {0} statements", requests.Length);
            Console.ResetColor();

            return requests;
        }

        public Claim GetClaim(string invoice, long frn, int schemeYear)
        {
            string invoiceTransformed = conversionService.TransformInvoice(invoice, "SITI");

            Guid claimId = db.SUM.AsNoTracking().Where(x => x.InvoiceNumber == invoiceTransformed && x.FRN == frn && x.SchemeYear == schemeYear).Select(x => x.ClaimID).FirstOrDefault();

            Claim claim = db.Claims.AsNoTracking().Where(x => x.ClaimID == claimId)
                .Include(x => x.SUM)
                .Include(x => x.SUM2)
                .Include(x => x.BPS)
                .Include(x => x.BPSPEN)
                .Include(x => x.GR)
                .Include(x => x.GRPEN)
                .Include(x => x.YF)
                .Include(x => x.CLD)
                .Include(x => x.PR)
                .FirstOrDefault();

            return claim;
        }

        public XBData GetXBData(string invoice, long frn, int schemeYear, DateTime settlementDate, bool dbFunctions = true)
        {
            string invoiceTransformed = conversionService.TransformInvoice(invoice, "XB");

            XBData xbData;

            if (dbFunctions)
            {
                xbData = db.XBData.AsNoTracking().Where(x => x.InvoiceNumber == invoiceTransformed && x.FRN == frn && x.SchemeYear == schemeYear && DbFunctions.TruncateTime(x.CalculationDate) <= settlementDate).OrderByDescending(x => x.CalculationDate).FirstOrDefault();
            }
            else
            {
                xbData = db.XBData.AsNoTracking().Where(x => x.InvoiceNumber == invoiceTransformed && x.FRN == frn && x.SchemeYear == schemeYear && x.CalculationDate.Date <= settlementDate).OrderByDescending(x => x.CalculationDate).FirstOrDefault();
            }

            return xbData;
        }

        public List<Invoice> GetInvoices(long frn, int schemeYear, decimal conversionRate, string statementType, bool dbFunctions = true)
        {
            List<Invoice> invoices = new List<Invoice>();

            List<AP> apList = db.AP.AsNoTracking().Where(x => x.Supplier == frn && x.MarketingYear == schemeYear && x.Active).ToList();
            List<ARGroup> arList = GetAR(frn, schemeYear);

            for (int i = 0; i < apList.Count; i++)
            {
                Invoice invoice = new Invoice(apList[i].LastSettlementDate.ToShortDateString(), apList[i].LastSettlementDate, apList[i].Invoice, "Payment", apList[i].TransactionInvoiceValue, apList[i].TransactionCurrency);
                invoice.ClaimValueEuro = GetClaimValue(apList[i].Invoice, frn, schemeYear, apList[i].LastSettlementDate, statementType, dbFunctions);
                invoice.ClaimValueSterling = currencyService.Convert(invoice.ClaimValueEuro, conversionRate);

                invoices.Add(invoice);
            }

            for (int i = 0; i < arList.Count; i++)
            {
                Invoice invoice = new Invoice(arList[i].Date.ToShortDateString(), arList[i].Date, arList[i].InvoiceNumber, "Recovery", arList[i].TransactionValue, arList[i].Currency);
                invoice.ClaimValueEuro = GetClaimValue(arList[i].InvoiceNumber, frn, schemeYear, arList[i].Date, statementType, dbFunctions);
                invoice.ClaimValueSterling = currencyService.Convert(invoice.ClaimValueEuro, conversionRate);

                invoices.Add(invoice);
            }

            return invoices;
        }

        public decimal GetClaimValue(string invoice, long frn, int schemeYear, DateTime invoiceDate, string statementType, bool dbFunctions = true)
        {
            string invoiceTransformed;

            invoiceDate = invoiceDate.Date;

            decimal value = 0;

            switch (statementType)
            {
                case "BPS":
                    invoiceTransformed = conversionService.TransformInvoice(invoice, "SITI");

                    Guid claimId = db.SUM.AsNoTracking().Where(x => x.InvoiceNumber == invoiceTransformed && x.FRN == frn && x.SchemeYear == schemeYear).Select(x => x.ClaimID).FirstOrDefault();

                    value = db.SUM2.AsNoTracking().Where(x => x.ClaimID == claimId).Select(x => x.TotalClaimEuro).FirstOrDefault();
                    break;
                case "XB":
                    XBData xbData;

                    invoiceTransformed = conversionService.TransformInvoice(invoice, "XB");

                    if (dbFunctions)
                    {
                        xbData = db.XBData.AsNoTracking().Where(x => x.InvoiceNumber == invoiceTransformed && x.FRN == frn && x.SchemeYear == schemeYear && DbFunctions.TruncateTime(x.CalculationDate) <= invoiceDate).OrderByDescending(x => x.CalculationDate).FirstOrDefault();
                    }
                    else
                    {
                        xbData = db.XBData.AsNoTracking().Where(x => x.InvoiceNumber == invoiceTransformed && x.FRN == frn && x.SchemeYear == schemeYear && x.CalculationDate.Date <= invoiceDate).OrderByDescending(x => x.CalculationDate).FirstOrDefault();
                    }

                    if (xbData != null)
                    {
                        value = xbData.EnglandTotalEuro + xbData.ScotlandTotalEuro + xbData.WalesTotalEuro + xbData.NITotalEuro;
                    }
                    break;
                default:
                    break;
            }

            return value;
        }

        public List<ARGroup> GetAR(long frn, int schemeYear)
        {
            var arList = db.AR.AsNoTracking().Where(x => x.H_InvoiceAccount == frn && x.H_MarketingYear == schemeYear).ToList();

            List<ARGroup> arGroups = new List<ARGroup>();

            List<string> invoiceNumbers = new List<string>();

            foreach (var ar in arList)
            {
                if (!invoiceNumbers.Exists(x => x == ar.InvoiceNumber))
                {
                    invoiceNumbers.Add(ar.InvoiceNumber);
                }
            }

            foreach (var invoiceNumber in invoiceNumbers)
            {
                ARGroup group = new ARGroup();

                group.InvoiceNumber = invoiceNumber;
                group.Date = arList.Where(x => x.InvoiceNumber == invoiceNumber).Max(x => x.H_InvoiceDate);
                group.TransactionValue = arList.Where(x => x.InvoiceNumber == invoiceNumber).Sum(x => x.L_LineAmount);
                group.Currency = arList.Where(x => x.InvoiceNumber == invoiceNumber).Select(x => x.H_CurrencyCode).FirstOrDefault();

                arGroups.Add(group);
            }

            return arGroups;
        }

        private List<Rate> GetRates(int schemeYear)
        {
            List<Rate> rates = new List<Rate>();

            var conversionRates = db.ConversionRates.AsNoTracking().Where(x => x.SchemeYear == schemeYear);

            foreach (var cRate in conversionRates)
            {
                rates.Add(new Rate(cRate.Description, cRate.Rate));
            }

            return rates;
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
