using System.Xml.Serialization;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class Rate
    {        
        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public decimal Value { get; set; }

        public Rate() { }
        
        public Rate(string description, decimal value)
        {
            Description = description;
            Value = value;
        }
    }
}