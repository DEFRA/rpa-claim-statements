using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.FDMR
{
    [Table("Extract", Schema = "FDMR")]
    public class Extract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int scheme_year { get; set; }
        [MaxLength(255)]
        public string business_name { get; set; }
        public int sbi { get; set; }
        public Int64 frn { get; set; }
        public decimal gross_after_net4 { get; set; }
        public decimal cross_compliance_percent { get; set; }
        [Required]
        [MaxLength(16)]
        public string country { get; set; }
        [Required]
        [MaxLength(8)]
        public string claim_number { get; set; }
        [Required]
        [MaxLength(50)]
        public string last_invoice_number { get; set; }
        [MaxLength(3)]
        public string last_invoice_currency { get; set; }
        public DateTime? Sent_to_FDMR { get; set; }
    }
}