using NUnit.Framework;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Tests.Services
{
    [TestFixture]
    [Category("Conversion Service")]
    public class ConversionServiceTests
    {
        [Test]
        public void Test_TransformInvoice_Transforms_DAX_To_SITI()
        {
            ConversionService service = new ConversionService();

            var result = service.TransformInvoice("S1234567C1234567V001", "SITI");

            Assert.AreEqual("SITI1234567", result);
        }

        [Test]
        public void Test_TransformInvoice_Transforms_DAX_To_XB()
        {
            ConversionService service = new ConversionService();

            var result = service.TransformInvoice("S1234567C1234567V001", "XB");

            Assert.AreEqual("PFSY1234567", result);
        }
    }
}
