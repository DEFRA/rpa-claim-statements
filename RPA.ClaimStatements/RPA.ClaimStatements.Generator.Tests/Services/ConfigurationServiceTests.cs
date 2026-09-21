using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Tests.Services
{
    [TestFixture]
    [Category("Configuration Service")]
    public class ConfigurationServiceTests
    {
        Mock<DbSet<Configuration>> mockConfigurationSet;
        Mock<ClaimStatementsContext> mockContext;
        ConfigurationService service;

        [SetUp]
        public void Setup()
        {
            var data = new List<Configuration>
            {
                new Configuration { Setting = "Configuration1", Value = "Active" },
                new Configuration { Setting = "Configuration2", Value = "Inactive" }
            };

            mockConfigurationSet = new Mock<DbSet<Configuration>>().SetupData(data);

            mockContext = new Mock<ClaimStatementsContext>();
            mockContext.Setup(x => x.Configurations).Returns(mockConfigurationSet.Object);

            service = new ConfigurationService(mockContext.Object);
        }

        [Test]                     
        public void Test_GetValue_returns_value()
        {
            var result = service.GetValue("Configuration1");

            Assert.AreEqual("Active", result);
        }

        [Test]
        public void Test_IsActive_returns_True_When_Active()
        {
            var result = service.IsActive("Configuration1");

            Assert.IsTrue(result);
        }

        [Test]
        public void Test_IsActive_returns_False_When_Inactive()
        {
            var result = service.IsActive("Configuration2");

            Assert.IsFalse(result);
        }

        [Test]
        public void Test_IsActive_returns_False_When_No_Match()
        {  
            var result = service.IsActive("Unmatched");

            Assert.IsFalse(result);
        }        
    }
}
