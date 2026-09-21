using System.Collections.Generic;
using CsvHelper.Configuration;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.Serializer
{
    public interface ISerializer<T> where T : class
    {
        List<T> DeSerialize(string filename, CsvClassMap<T> map = null, bool hasHeader = true);
        void Serialize(List<T> obj, string filename, string header = null, CsvClassMap<T> map = null);
    }
}