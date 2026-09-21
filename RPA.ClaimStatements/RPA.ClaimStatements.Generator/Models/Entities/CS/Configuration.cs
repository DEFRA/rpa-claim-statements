using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("Configuration", Schema="CS")]
    public class Configuration
    {
        public Guid ConfigurationID { get; set; }

        public string Setting { get; set; }

        public string Value { get; set; }

        public Configuration()
        {
            ConfigurationID = Guid.NewGuid();
        }

        public Configuration(string setting, string value):this()
        {
            Setting = setting;
            Value = value;
        }
    }
}