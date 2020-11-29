# LAPS - The Loan Applicatoin Processing System

This applicatoin is developed on the .NET technology stack (Razor Pages, EF Core, MS SQL Server)

Software requirements:
.NET Core 3.1

## How to Setup:
Open cmd from the solution folder and run the command: **dotnet build**  
Now go to the project folder which will be Laps.Web  
Open cmd from the project folder and run the following commands:  
To create user-secrets to setup data: **dotnet user-secrets set "SeedUserPW" "put_your_pswd_here"**  
To trust a certificate run the given command and then click yes to the prompt : **dotnet dev-certs https --trust**  
Then to start the application run the command: **dotnet run**  

You should be able to access the application through: **https://localhost:5001**  

### Now you can login to the application as a:  

Customer with following credentials:  
Email: customer@laps.com  
Password: put_your_pswd_here

Processing team member with following credentials:  
Email: processor@laps.com  
Password: put_your_pswd_here

Manager with following credentials:  
Email: manager@laps.com  
Password: put_your_pswd_here
