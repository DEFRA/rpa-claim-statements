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
    [Category("CurrencyService")]
    public class CurrencyServiceTests
    {
        [Test]
        public void Test_Check_Tolerance_Positive_Within_Tolerance()
        {
            CurrencyService service = new CurrencyService();

            var result = service.CheckTolerance(10.55M, 10.50M, 0.05M);

            Assert.IsTrue(result);
        }

        [Test]
        public void Test_Check_Tolerance_Negative_Within_Tolerance()
        {
            CurrencyService service = new CurrencyService();

            var result = service.CheckTolerance(10.55M, 10.60M, 0.05M);

            Assert.IsTrue(result);
        }

        [Test]
        public void Test_Check_Tolerance_Positive_Outside_Tolerance()
        {
            CurrencyService service = new CurrencyService();

            var result = service.CheckTolerance(10.55M, 10.49M, 0.05M);

            Assert.IsFalse(result);
        }

        [Test]
        public void Test_Check_Tolerance_Negative_Outside_Tolerance()
        {
            CurrencyService service = new CurrencyService();

            var result = service.CheckTolerance(10.55M, 10.61M, 0.05M);

            Assert.IsFalse(result);
        }

        [Test]
        public void Test_Covert_Converts_By_Rate()
        {
            CurrencyService service = new CurrencyService();

            var result = service.Convert(1.56M, 1.85834M);

            Assert.AreEqual(2.9M, result);
        }        
    }
}
