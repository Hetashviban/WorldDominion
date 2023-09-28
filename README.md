# ASP.NET MVC Project Setup in VSCode

Follow this guide to create a new ASP.NET MVC project using VSCode. We'll also use the ASPNET CodeGenerator package and configure the Entity Framework to use MySQL.

## Prerequisites

- [VSCode](https://code.visualstudio.com/)
- [.NET SDK](https://dotnet.microsoft.com/download)
- MySQL Server installed and running

## Steps

### 1. Initialize a New Project

Open your terminal and create a new directory for your project. Navigate into it and run the following command to create a new ASP.NET MVC project:

```bash
dotnet new mvc -o NameOfMyProject
dotnet dev-certs https --trust
```

### 2. Add MySQL Entity Framework Package

Add the NuGet package for Entity Framework Core MySQL provider.

```bash
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SQLite
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package MySql.EntityFrameworkCore --version 7.0.2
```

### 3. Add ASPNET CodeGenerator

Install the ASPNET CodeGenerator package:

```bash
dotnet tool uninstall --global dotnet-aspnet-codegenerator
dotnet tool install --global dotnet-aspnet-codegenerator

```

### 4. Create Models

Create your model classes in the `Models` folder, using the code generator:
```bash
dotnet aspnet-codegenerator model -n Department -o Models
```

### 5. Add DbContext

Create a new folder called **Data**. In the folder create a new file called **ApplicationDbContext.cs**. Add the following to the file:
```csharp
using Microsoft.EntityFrameworkCore;

namespace WorldDominion.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        // Change to be your model(s) and table(s)
        public DbSet<ModelName> TableName { get; set; }
    }
}
```

In the Program.cs file, add the following line, after the **"// Add services to the container."** comment:
```csharp
// Add MySQL
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseMySQL(connectionString));
```

### 6. Scaffold Controllers and Views

Using the terminal, run the following command to scaffold controllers and views:

```bash
dotnet aspnet-codegenerator controller -name [ControllerName] -m [ModelName] -dc [DbContextName] --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
```

Replace `[ControllerName]`, `[ModelName]`, and `[DbContextName]` with appropriate names.

### 7. Run Migrations

Initialize a new migration:

```bash
dotnet ef migrations add InitialMigration
```

Apply the migration to the database:

```bash
dotnet ef database update
```

### 8. Run the Project

Run your project to make sure everything works:

```bash
dotnet run
```