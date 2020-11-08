using Laps.Web.Authorization;
using Laps.Web.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Laps.Web.Data
{
    public static class SeedData
    {
        #region snippet_Initialize
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>

                // The manager user can do anything
                var managerID = await EnsureUser(serviceProvider, testUserPw, "manager@laps.com");
                await EnsureRole(serviceProvider, managerID, Constants.LoanManagersRole);

                // allowed user can create and edit contacts that they create
                var processorId = await EnsureUser(serviceProvider, testUserPw, "processor@laps.com");
                await EnsureRole(serviceProvider, processorId, Constants.LoanProcessorsRole);

                //Customer
                await EnsureUser(serviceProvider, testUserPw, "customer@laps.com");

                SeedDB(context, processorId);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if(user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }
            
            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
        #endregion

        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.LoanApplications.Any())
            {
                return;   // DB has been seeded
            }

            context.LoanApplications.AddRange(
                new LoanApplication
                {
                    ExternalId = Guid.NewGuid().ToString(),
                    Purpose = "Home Rennovation",
                    Age = 25,
                    EmploymentType = EmployementType.Salaried,
                    IncomePerMonth = 3000,
                    Address = "1234 Main St",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    MobileNumber = 9848022338,
                    LoanAmount = 6500,
                    Status = LoanStatus.Approved,
                    OwnerId = adminID,
                    ApplicationSubmittedOn = DateTime.Now.AddDays(-7),
                    StatusLastUpdatedOn = DateTime.Now.AddDays(-3)
                },
                new LoanApplication
                {
                    ExternalId = Guid.NewGuid().ToString(),
                    Purpose = "Christmas Holidays",
                    Age = 32,
                    EmploymentType = EmployementType.Salaried,
                    IncomePerMonth = 5000,
                    Address = "5678 1st Ave W",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    MobileNumber = 9848022338,
                    LoanAmount = 5000,
                    Status = LoanStatus.Approved,
                    OwnerId = adminID,
                    ApplicationSubmittedOn = DateTime.Now.AddDays(-5),
                    StatusLastUpdatedOn = DateTime.Now
                },
             new LoanApplication
             {
                 ExternalId = Guid.NewGuid().ToString(),
                 Purpose = "Family Vacation",
                 Age = 40,
                 EmploymentType = EmployementType.SelfEmployed,
                 IncomePerMonth = 5000,
                 Address = "9012 State st",
                 City = "Redmond",
                 State = "WA",
                 Zip = "10999",
                 MobileNumber = 9848022338,
                 LoanAmount = 7000,
                 Status = LoanStatus.Approved,
                 OwnerId = adminID,
                 ApplicationSubmittedOn = DateTime.Now.AddDays(-74),
                 StatusLastUpdatedOn = DateTime.Now.AddDays(-58)
             },
             new LoanApplication
             {
                 ExternalId = Guid.NewGuid().ToString(),
                 Purpose = "Business Purpose",
                 Age = 45,
                 EmploymentType = EmployementType.BusinessOwner,
                 IncomePerMonth = 8000,
                 Address = "3456 Maple St",
                 City = "Redmond",
                 State = "WA",
                 Zip = "10999",
                 MobileNumber = 9848022338,
                 LoanAmount = 15000,
                 Status = LoanStatus.Submitted,
                 OwnerId = adminID,
                 ApplicationSubmittedOn = DateTime.Now.AddDays(-4),
                 StatusLastUpdatedOn = DateTime.Now.AddDays(-4),
             },
             new LoanApplication
             {
                 ExternalId = Guid.NewGuid().ToString(),
                 Purpose = "Personal Reason",
                 Age = 22,
                 EmploymentType = EmployementType.Salaried,
                 IncomePerMonth = 1000,
                 Address = "7890 2nd Ave E",
                 City = "Redmond",
                 State = "WA",
                 Zip = "10999",
                 MobileNumber = 9848022338,
                 LoanAmount = 15000,
                 Status = LoanStatus.Rejected,
                 OwnerId = adminID,
                 ApplicationSubmittedOn = DateTime.Now.AddDays(-57),
                 StatusLastUpdatedOn = DateTime.Now.AddDays(-33),
             }
             );
            context.SaveChanges();
        }
    }
}
