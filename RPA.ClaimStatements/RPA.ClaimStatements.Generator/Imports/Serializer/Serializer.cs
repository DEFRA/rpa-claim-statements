using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Imports.Serializer
{
    public class Serializer<T> : ISerializer<T> where T : class
    {
        protected string delimiter = ",";

        public Serializer()
        {
        }

        public Serializer(string delimiter)
        {
            this.delimiter = delimiter;
        }

        public virtual void Serialize(List<T> obj, string filename, string header = null, CsvClassMap<T> map = null)
        {
            using (TextWriter tw = File.CreateText(filename))
            {
                var csv = new CsvWriter(tw);
                csv.Configuration.Delimiter = delimiter;

                if (map != null)
                {
                    csv.Configuration.RegisterClassMap(map);
                }

                if (!string.IsNullOrEmpty(header))
                {
                    csv.WriteField(header);
                    csv.NextRecord();
                }

                csv.WriteRecords(obj);
            }
        }

        public virtual List<T> DeSerialize(string filename, CsvClassMap<T> map = null, bool hasHeader = true)
        {
            List<T> obj;

            using (TextReader tr = File.OpenText(filename))
            {
                var csv = new CsvReader(tr);
                csv.Configuration.WillThrowOnMissingField = false;
                csv.Configuration.Delimiter = delimiter;
                csv.Configuration.TrimFields = true;
                csv.Configuration.HasHeaderRecord = hasHeader;

                if (map != null)
                {
                    csv.Configuration.RegisterClassMap(map);
                }

                obj = csv.GetRecords<T>().ToList();
            }

            return obj;
        }
    }
}
