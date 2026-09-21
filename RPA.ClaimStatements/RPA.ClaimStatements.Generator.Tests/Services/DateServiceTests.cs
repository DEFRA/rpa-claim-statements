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
    [Category("Date Service")]
    public class DateServiceTests
    {
        [Test]
        public void Test_CurrentDateTimeString_Returns_String()
        {
            DateService service = new DateService();

            var result = service.CurrentDateTimeString();

            Assert.IsTrue(result.Contains("_"));
        }
    }
}
