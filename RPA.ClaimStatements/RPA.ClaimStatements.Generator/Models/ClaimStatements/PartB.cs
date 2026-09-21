using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class PartB
    {
        public BPSPayment BPSPayment { get; set; }

        public GreeningPayment GreeningPayment { get; set; }

        public YoungFarmerPayment YoungFarmerPayment { get; set; }

        public ProgressiveReductionsReduction ProgressiveReductions { get; set; }

        public PartB() { }

        public PartB(BPSPayment bpspayment, GreeningPayment greeningPayment)
        {
            BPSPayment = bpspayment;
            GreeningPayment = greeningPayment;
        }

        public PartB(BPSPayment bpspayment, GreeningPayment greeningPayment, YoungFarmerPayment youngFarmerPayment):this(bpspayment, greeningPayment)
        {
            YoungFarmerPayment = youngFarmerPayment;
        }

        public PartB(BPSPayment bpspayment, GreeningPayment greeningPayment, ProgressiveReductionsReduction progressiveReductions) : this(bpspayment, greeningPayment)
        {
            ProgressiveReductions = progressiveReductions;
        }

        public PartB(BPSPayment bpspayment, GreeningPayment greeningPayment, ProgressiveReductionsReduction progressiveReductions, YoungFarmerPayment youngFarmerPayment) : this(bpspayment, greeningPayment)
        {
            YoungFarmerPayment = youngFarmerPayment;
            ProgressiveReductions = progressiveReductions;
        }
    }       
}

    
