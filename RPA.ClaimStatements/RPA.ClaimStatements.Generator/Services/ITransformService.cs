using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface ITransformService
    {
        void Transform(Request request, ClaimStatement statement, string statementType);

        string ConvertToXML(ClaimStatement statement, string statementType);

        XmlDocument TransformXML(string xml, string xslt);

        string CopyTemplate(Guid claimId, string template);

        void CreateDOCX(string filePath, XmlDocument xml, int sbi, string statementType);

        string ConvertToPDF(string filePath, long frn, int sbi, int schemeYear);

        void Publish(string filePath);
    }
}
