using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components.Turbine;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Exceptions;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Tests.Components.Turbine
{
    [TestFixture]
    [Category("Validation Service")]
    public class ValidationServiceTests
    {
        [Test]
        public void Test_ValidateCurrency_When_Valid()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                Currency = "Euros"
            };

            Assert.DoesNotThrow(() => service.ValidateCurrency(statement));
        }

        [Test]
        public void Test_ValidateCurrency_When_Invalid()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement();
            
            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateCurrency(statement));
        }

        [Test]
        public void Test_ValidateCurrency_When_Valid_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                Currency = "Euros"
            };

            Assert.DoesNotThrow(() => service.ValidateCurrency(statement));
        }

        [Test]
        public void Test_ValidateCurrency_When_Invalid_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB();

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateCurrency(statement));
        }

        [Test]
        public void Test_ValidateInvoices_When_Valid()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice()
                    }
                }
            };

            Assert.DoesNotThrow(() => service.ValidateInvoices(statement));
        }

        [Test]
        public void Test_ValidateInvoices_When_Invalid()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                PartC = new PartC
                {
                    Invoices = new List<Invoice>()
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateInvoices(statement));
        }

        [Test]
        public void Test_ValidateInvoices_When_Valid_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice()
                    }
                }
            };

            Assert.DoesNotThrow(() => service.ValidateInvoices(statement));
        }

        [Test]
        public void Test_ValidateInvoices_When_Invalid_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                PartC = new PartC
                {
                    Invoices = new List<Invoice>()
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateInvoices(statement));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Valid()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                PartA = new PartA
                {
                    TotalEuro = 100
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        }
                    }
                }
            };

            Assert.DoesNotThrow(() => service.ValidateClaimHistory(statement, "BPS"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Valid_Multiple_Invoices()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                PartA = new PartA
                {
                    TotalEuro = 150
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        },
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,2),
                            Reference = "Invoice2",
                            ClaimValueEuro = 150
                        }
                    }
                }
            };

            Assert.DoesNotThrow(() => service.ValidateClaimHistory(statement, "BPS"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Valid_Multiple_Invoices_Same_Day()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                PartA = new PartA
                {
                    TotalEuro = 150
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        },
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice2",
                            ClaimValueEuro = 150
                        }
                    }
                }
            };

            Assert.DoesNotThrow(() => service.ValidateClaimHistory(statement, "BPS"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Invalid()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                PartA = new PartA
                {
                    TotalEuro = 150
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        }
                    }
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateClaimHistory(statement, "BPS"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Invalid_Multiple_Invoices()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                PartA = new PartA
                {
                    TotalEuro = 100
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        },
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,2),
                            Reference = "Invoice2",
                            ClaimValueEuro = 150
                        }
                    }
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateClaimHistory(statement, "BPS"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Invalid_Multiple_Invoices_Same_Day()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                PartA = new PartA
                {
                    TotalEuro = 100
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        },
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice2",
                            ClaimValueEuro = 150
                        }
                    }
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateClaimHistory(statement, "BPS"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Valid_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                PartAXB = new PartAXB
                {
                    TotalTotalEuro = 100
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        }
                    }
                }
            };

            Assert.DoesNotThrow(() => service.ValidateClaimHistory(statement, "XB"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Valid_Multiple_Invoices_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                PartAXB = new PartAXB
                {
                    TotalTotalEuro = 150
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        },
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,2),
                            Reference = "Invoice2",
                            ClaimValueEuro = 150
                        }
                    }
                }
            };

            Assert.DoesNotThrow(() => service.ValidateClaimHistory(statement, "XB"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Valid_Multiple_Invoices_Same_Day_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                PartAXB = new PartAXB
                {
                    TotalTotalEuro = 150
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        },
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice2",
                            ClaimValueEuro = 150
                        }
                    }
                }
            };

            Assert.DoesNotThrow(() => service.ValidateClaimHistory(statement, "XB"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Invalid_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                PartAXB = new PartAXB
                {
                    TotalTotalEuro = 150
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        }
                    }
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateClaimHistory(statement, "XB"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Invalid_Multiple_Invoices_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                PartAXB = new PartAXB
                {
                    TotalTotalEuro = 100
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        },
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,2),
                            Reference = "Invoice2",
                            ClaimValueEuro = 150
                        }
                    }
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateClaimHistory(statement, "XB"));
        }

        [Test]
        public void Test_ValidateClaimHistory_When_Invalid_Multiple_Invoices_Same_Day_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                PartAXB = new PartAXB
                {
                    TotalTotalEuro = 100
                },
                PartC = new PartC
                {
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice1",
                            ClaimValueEuro = 100
                        },
                        new Invoice
                        {
                            DateDate = new DateTime(2016,1,1),
                            Reference = "Invoice2",
                            ClaimValueEuro = 150
                        }
                    }
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateClaimHistory(statement, "XB"));
        }

        [Test]
        public void Test_ValidateReconciliation_When_Valid_Euros()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                Currency = "Euros",
                PartA = new PartA
                {
                    TotalEuro = 100
                },
                PartC = new PartC
                {
                    TotalPayments = 100
                }
            };

            Assert.DoesNotThrow(() => service.ValidateReconciliation(statement, "BPS"));
        }

        [Test]
        public void Test_ValidateReconciliation_When_Invalid_Euros()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                Currency = "Euros",
                PartA = new PartA
                {
                    TotalEuro = 150
                },
                PartC = new PartC
                {
                    TotalPayments = 100
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateReconciliation(statement, "BPS"));
        }

        [Test]
        public void Test_ValidateReconciliation_When_Valid_Sterling()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                Currency = "Sterling",
                PartA = new PartA
                {
                    TotalSterling = 100
                },
                PartC = new PartC
                {
                    TotalPayments = 100
                }
            };

            Assert.DoesNotThrow(() => service.ValidateReconciliation(statement, "BPS"));
        }

        [Test]
        public void Test_ValidateReconciliation_When_Invalid_Sterling()
        {
            ValidationService service = new ValidationService();

            ClaimStatement statement = new ClaimStatement
            {
                Currency = "Sterling",
                PartA = new PartA
                {
                    TotalSterling = 150
                },
                PartC = new PartC
                {
                    TotalPayments = 100
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateReconciliation(statement, "BPS"));
        }

        [Test]
        public void Test_ValidateReconciliation_When_Valid_Euros_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                Currency = "Euros",
                PartAXB = new PartAXB
                {
                    TotalTotalEuro = 100
                },
                PartC = new PartC
                {
                    TotalPayments = 100
                }
            };

            Assert.DoesNotThrow(() => service.ValidateReconciliation(statement, "XB"));
        }

        [Test]
        public void Test_ValidateReconciliation_When_Invalid_Euros_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                Currency = "Euros",
                PartAXB = new PartAXB
                {
                    TotalTotalEuro = 150
                },
                PartC = new PartC
                {
                    TotalPayments = 100
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateReconciliation(statement, "XB"));
        }

        [Test]
        public void Test_ValidateReconciliation_When_Valid_Sterling_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                Currency = "Sterling",
                PartAXB = new PartAXB
                {
                    TotalTotalSterling = 100
                },
                PartC = new PartC
                {
                    TotalPayments = 100
                }
            };

            Assert.DoesNotThrow(() => service.ValidateReconciliation(statement, "XB"));
        }

        [Test]
        public void Test_ValidateReconciliation_When_Invalid_Sterling_XB()
        {
            ValidationService service = new ValidationService();

            ClaimStatementXB statement = new ClaimStatementXB
            {
                Currency = "Sterling",
                PartAXB = new PartAXB
                {
                    TotalTotalSterling = 150
                },
                PartC = new PartC
                {
                    TotalPayments = 100
                }
            };

            Assert.Throws<ClaimStatementValidationException>(() => service.ValidateReconciliation(statement, "XB"));
        }
    }
}
