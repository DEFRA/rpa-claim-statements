using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPA.ClaimStatements.Generator.Models.Entities.SITI
{        
    [Table("Claims", Schema="SITI")]
    public class Claim
    {
        public Guid ClaimID { get; set; }

        public Guid SUMID { get; set; }

        public Guid SUM2ID { get; set; }

        public Guid BPSID { get; set; }

        public Guid BPSPENID { get; set; }

        public Guid GRID { get; set; }

        public Guid GRPENID { get; set; }

        public Guid? YFID { get; set; }
        
        public Guid CLDID { get; set; }

        public Guid? PRID { get; set; }

        public bool IsValid
        {
            get
            {
                if(SUM != null && SUM2 != null && BPS != null && BPSPEN != null && GR != null && GRPEN != null
                    && CLD != null && PR != null)
                {
                    return true;
                }

                return false;
            }
        }
        
        public virtual SUM SUM { get; set; }
        
        public virtual SUM2 SUM2 { get; set; }
        
        public virtual BPS BPS { get; set; }
        
        public virtual BPSPEN BPSPEN { get; set; }
        
        public virtual GR GR { get; set; }
        
        public virtual GRPEN GRPEN { get; set; }
        
        public virtual YF YF { get; set; }
        
        public virtual CLD CLD { get; set; }

        public virtual PR PR { get; set; }

        public Claim()
        {
            ClaimID = Guid.NewGuid();
        }

        public Claim(Guid claimID)
        {
            ClaimID = claimID;
        }
    }
}