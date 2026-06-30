# Bincom Car Dealer

A car dealership web app built for the Bincom Academy C# Beginner Class final test. Visitors browse available cars and submit inquiries; an authenticated admin manages inventory and reviews incoming inquiries. Built with ASP.NET Core MVC, Entity Framework Core, and SQL Server, deployed to Azure with CI/CD through Azure DevOps Pipelines.

## Live Links

- **Live site:** `https://bincomdrive.azurewebsites.net/`
- **Swagger API docs:** `https://bincomdrive.azurewebsites.net/swagger`
- **Source repo:** `https://github.com/EmmyAkine/BincomCarDealer.git`

## Features

### Public

- Browse all available cars with year, mileage, and price shown up front
- View full car details, including description and photo
- Submit an inquiry on any car (name, email, message)

### Admin

- Cookie-based login, credentials stored in configuration (not in source control)
- Dashboard with quick links to all admin actions
- Upload new cars, including a photo
- Edit existing car listings, including replacing the photo
- Delete cars (with inline confirm step, no JavaScript)
- View all customer inquiries, with a direct link back to the car in question

### API

- Public, unauthenticated REST endpoints for cars and inquiries
- Interactive documentation via Swagger UI

### Other

- Dark / light theme toggle, persisted via cookie, no flash of the wrong theme on load
- Privacy policy page
- Custom error page

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET) |
| Database | SQL Server (Azure SQL Database in production, LocalDB locally) |
| ORM | Entity Framework Core |
| Auth | ASP.NET Core Cookie Authentication |
| API docs | Swashbuckle / Swagger |
| Hosting | Azure App Service |
| CI/CD | Azure DevOps Pipelines (YAML) |

## Project Structure

```
Controllers/
  CarsController.cs      Public car browsing + inquiry submission
  AdminController.cs     Login, dashboard, upload, edit, delete, inquiries
  Api/
    CarApiController.cs  Public REST API (no auth)

Models/
  CarItem.cs              Car listing
  CarInquiry.cs           Customer inquiry

DTO/
  CreateCarUploadDto.cs
  CreateInquiryFormDto.cs
  CreateLoginRequestDto.cs
  EditCarDto.cs

Data/
  AppDbContext.cs

Views/
  Cars/                   Index, Details, Privacy
  Admin/                  Login, Dashboard, Upload, ManageCars, EditCarDetails, Inquiries
  Shared/                 _Layout, Error

wwwroot/
  css/site.css            All styling, theme variables, no Bootstrap dependency
```

## Local Setup

1. Clone the repo and open the solution in Visual Studio.
2. Restore NuGet packages (should happen automatically on build):
   ```
   Microsoft.EntityFrameworkCore.SqlServer
   Microsoft.EntityFrameworkCore.Tools
   Microsoft.EntityFrameworkCore.Design
   Swashbuckle.AspNetCore
   ```
3. Set your local connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CarDealerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```
4. Set admin credentials in `appsettings.json` (or User Secrets locally):
   ```json
   "AdminSettings": {
     "Username": "admin",
     "Password": "your-password-here"
   }
   ```
5. Run migrations:
   ```
   Add-Migration InitialCreate
   Update-Database
   ```
6. Run the app. It lands on `/Cars` by default.

## Deployment

The app deploys via an Azure DevOps Pipeline (`azure-pipelines.yml`) on every push to `main`:

```
dotnet build → dotnet publish → archive as zip → deploy to Azure App Service
```

The database connection string and admin credentials are set as **Connection strings** / **App settings** directly in the Azure App Service configuration, never committed to source control.

Uploaded car images are written to persistent storage (`/home/data/uploads` on Azure App Service) rather than `wwwroot`, since `wwwroot` is not guaranteed to survive app restarts or redeployments on App Service's filesystem.

## Known Limitations / Roadmap

These are acknowledged gaps, not required by the test brief, but worth noting as deliberate scope decisions rather than oversights:

- **No image compression.** Uploaded photos are stored as-is. A future improvement would convert uploads to a compressed format (e.g. WebP/AVIF) on the server before saving, reducing file size and page load time — especially relevant once the inventory grows.
- **No pagination.** The inventory page currently loads every car in the database at once. At small scale this is fine; at hundreds or thousands of listings, this should be replaced with paged results (e.g. 20 per page with a "Load more" or numbered pager) so the homepage stays fast regardless of inventory size.
- **No search/filter.** There's currently no way to filter by make, price range, or body style. A search bar with basic filtering would be the natural next feature once pagination is in place, since search and pagination are usually implemented together (filtered results still need to be paged).
- **Single hardcoded admin.** There's one admin account, no registration, no roles. This is intentional for the scope of this project — a single-admin dealership doesn't need a full user management system — but wouldn't scale to multiple staff accounts without revisiting the auth design.

## Author

Built by Emmanuel Oyebamiji as part of the Bincom Academy C# Beginner Class, Contact Session 7 final test.