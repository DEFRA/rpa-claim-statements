// =============================================================================================
// Replaces the EF6-only `Mock<DbSet<T>>.SetupData(data)` extension of EntityFrameworkTesting.Moq with an EF Core
// compatible equivalent built on MockQueryable.Moq's `BuildMockDbSet<T>(IEnumerable<T>)`.
//
// This is a LINQ-to-Objects test double, NOT proof of SQL translation. It only wires up the IQueryable<T> surface
// (Provider/Expression/ElementType/GetEnumerator) that production `.Where()`, `.Select()`, `.OrderBy()`,
// `.FirstOrDefault()`, `.Any()`, `.Count()`, `.Sum()`, `.Max()` and `.AsNoTracking()` calls rely on. Virtual
// non-query members such as `.Add()` and `.Remove()` stay ordinary loosely-mocked Moq members on the original mock
// (for example `mockSet.Verify(x => x.Add(It.IsAny<T>()), Times.Once)` still works).
//
// The call-site signature matches the old extension (`new Mock<DbSet<T>>().SetupData(data)`).
// =============================================================================================
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;

namespace RPA.ClaimStatements.Generator.Tests.TestSupport
{
    public static class MockDbSetExtensions
    {
        public static Mock<DbSet<T>> SetupData<T>(this Mock<DbSet<T>> mockSet, IEnumerable<T> data) where T : class
        {
            Mock<DbSet<T>> builtMock = data.BuildMockDbSet();
            IQueryable<T> queryable = builtMock.Object;

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return mockSet;
        }
    }
}