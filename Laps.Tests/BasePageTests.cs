using Laps.Web.Data;
using Laps.Web.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Laps.Tests
{
    public class BasePageTests
    {
        public BasePageTests()
        {
            //Each test can run in parallel, so each test needs its own database
            ContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            AppDbContext = new ApplicationDbContext(ContextOptions);
            Seed();
        }

        public DbContextOptions<ApplicationDbContext> ContextOptions { get; }
        public ApplicationDbContext AppDbContext { get; }

        private void Seed()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var loanApplication1 = new LoanApplication
                {
                    OwnerId = "test.user",
                    Id = 1,
                    ExternalId = "test.id",
                    Purpose = "Test",
                    Age = 25,
                    EmploymentType = EmployementType.Salaried,
                    IncomePerMonth = 2500,
                    Address = "Test",
                    City = "Test",
                    State = "Test",
                    Zip = "Test",
                    MobileNumber = 9848022338,
                    LoanAmount = 5000,
                    Status = LoanStatus.UnderReview,
                    ApplicationSubmittedOn = DateTime.Now,
                    StatusLastUpdatedOn = DateTime.Now
                };

                var loanApplication2 = new LoanApplication
                {
                    OwnerId = "test.user2",
                    Id = 2,
                    ExternalId = "test.id2",
                    Purpose = "Test2",
                    Age = 25,
                    EmploymentType = EmployementType.Salaried,
                    IncomePerMonth = 2500,
                    Address = "Test2",
                    City = "Test2",
                    State = "Test2",
                    Zip = "Test2",
                    MobileNumber = 9848022338,
                    LoanAmount = 5000,
                    Status = LoanStatus.Submitted,
                    ApplicationSubmittedOn = DateTime.Now,
                    StatusLastUpdatedOn = DateTime.Now
                };

                var loanApplication3 = new LoanApplication
                {
                    OwnerId = "test.user3",
                    Id = 3,
                    ExternalId = "test.id3",
                    Purpose = "Test3",
                    Age = 25,
                    EmploymentType = EmployementType.Salaried,
                    IncomePerMonth = 2500,
                    Address = "Test3",
                    City = "Test3",
                    State = "Test3",
                    Zip = "Test3",
                    MobileNumber = 9848022338,
                    LoanAmount = 5000,
                    Status = LoanStatus.AwaitingManagersApproval,
                    ApplicationSubmittedOn = DateTime.Now,
                    StatusLastUpdatedOn = DateTime.Now
                };

                context.AddRange(loanApplication1, loanApplication2, loanApplication3);

                context.SaveChanges();
            }
        }
    }
}
