# Gearix – Vehicle Inventory Management System (Backend)

<div align="center">
  <img width="500" height="136" alt="gear" src="https://github.com/user-attachments/assets/dad1b396-8495-4883-8b7a-9f52055da518" />
</div>

**Gearix Backend** is a RESTful Web API built with **ASP.NET Core** (.NET 8) powering the Gearix Vehicle Inventory Management System. It handles authentication, role-based access control, inventory management, invoicing, vendor management, reporting, and automated notifications for three user roles: **Admin**, **Staff**, and **Customer**.

---

## Features

### Admin Endpoints
1. **Financial Reports** – Daily, monthly, and yearly revenue/expense/profit summaries.
2. **Staff Management** – Register staff, assign roles, deactivate accounts.
3. **Parts Management** – Full CRUD for vehicle parts with category and stock info.
4. **Purchase Invoices** – Create vendor purchase invoices to restock inventory.
5. **Vendor Management** – Full CRUD for vendors (name, email, phone, address).

### Staff Endpoints
6. **Customer Registration** – Register customers with vehicle details.
7. **Sales Invoices** – Create sales invoices for parts and services; trigger email delivery.
8. **Customer Details & History** – Retrieve customer profiles, purchase history, and vehicles.
9. **Customer Reports** – Regular customers, high spenders, and pending credit reports.
10. **Advanced Customer Search** – Search by vehicle number, phone, customer ID, or name.

### Customer Endpoints
11. **Self-Registration & Profile** – Register, log in, update profile and vehicle info.
12. **Bookings & Service Requests** – Book service appointments, request unavailable parts.
13. **Purchase/Service History** – Retrieve past invoices and service records.

### System-Wide
14. **JWT Authentication** – Secure token-based auth with role claims.
15. **Automated Notifications** – Low-stock alerts for Admin (<10 units); automated email reminders for customers with overdue credits (>1 month).
16. **Loyalty Discount Logic** – Automatic 10% discount applied server-side on purchases exceeding Rs. 5,000.

---

## Tech Stack

| Technology | Purpose |
|---|---|
| **ASP.NET Core 8 Web API** | Core framework |
| **C# 12** | Primary language |
| **Entity Framework Core 8** | ORM / database access |
| **SQL Server** | Primary database |
| **JWT Bearer Auth** | Authentication & authorization |
| **AutoMapper** | DTO ↔ entity mapping |
| **FluentValidation** | Request model validation |
| **Serilog** | Structured logging |
| **Hangfire** | Background jobs (scheduled notifications) |
| **MailKit / MimeKit** | Email delivery (invoices, reminders) |
| **Swagger / Scalar** | API documentation |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8) or later
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (2019+ or LocalDB for development)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) / [JetBrains Rider](https://www.jetbrains.com/rider/) (latest)
- [Git](https://git-scm.com/)
- SMTP credentials (for email features)

---

## Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/gearix-backend.git
cd gearix-backend
```

### 2. Configure the Database

Open `appsettings.json` (or `appsettings.Development.json`) and set your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GearixDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For **LocalDB** (development only):

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GearixDB;Trusted_Connection=True;"
```

### 3. Configure App Settings

Update `appsettings.json` with your environment values:

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-at-least-32-characters",
    "Issuer": "GearixAPI",
    "Audience": "GearixClient",
    "ExpiresInMinutes": 60
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@gearix.com",
    "SenderName": "Gearix System",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },
  "AppSettings": {
    "LowStockThreshold": 10,
    "LoyaltyDiscountThreshold": 5000,
    "LoyaltyDiscountPercentage": 10
  }
}
```

> **Important:** Never commit real secrets to source control. Use **User Secrets** or environment variables in production.

### 4. Apply Database Migrations

```bash
dotnet ef database update
```

Or from the Package Manager Console in Visual Studio:

```powershell
Update-Database
```

### 5. Seed Initial Data (Optional)

The project includes a seeder for default admin credentials and sample data:

```bash
dotnet run --seed
```

Default admin credentials after seeding:

| Field | Value |
|---|---|
| Email | admin@gearix.com |
| Password | Admin@1234 |

### 6. Run the API

```bash
dotnet run
```

Or press **F5** in Visual Studio / Rider.

The API will be available at:

```
https://localhost:7001
http://localhost:5000
```

Swagger UI: `https://localhost:7001/swagger`

---

## Project Structure

```
Gearix.API/
├── Controllers/              # API controllers (Auth, Admin, Staff, Customer)
├── Data/
│   ├── AppDbContext.cs       # EF Core DbContext
│   └── Migrations/           # EF Core migration files
├── DTOs/                     # Data Transfer Objects (Request/Response)
├── Entities/                 # Domain models / EF Core entities
├── Enums/                    # Enumerations (Role, InvoiceStatus, etc.)
├── Helpers/                  # Utility classes (JWT, Email, PDF)
├── Interfaces/               # Service and repository interfaces
├── Mapping/                  # AutoMapper profiles
├── Middleware/               # Custom middleware (error handling, logging)
├── Services/                 # Business logic services
│   ├── AuthService.cs
│   ├── PartsService.cs
│   ├── InvoiceService.cs
│   ├── ReportService.cs
│   ├── EmailService.cs
│   └── NotificationService.cs
├── Validators/               # FluentValidation validators
├── Jobs/                     # Hangfire background jobs
│   ├── LowStockNotificationJob.cs
│   └── CreditReminderJob.cs
├── appsettings.json
├── appsettings.Development.json
├── Program.cs                # App entry point & DI configuration
└── Gearix.API.csproj
```

---

## API Endpoints

### Authentication

| Method | Endpoint | Description | Access |
|---|---|---|---|
| POST | `/api/auth/register` | Register a new customer | Public |
| POST | `/api/auth/login` | Login and receive JWT | Public |
| POST | `/api/auth/refresh` | Refresh JWT token | Authenticated |
| POST | `/api/auth/logout` | Invalidate token | Authenticated |

### Admin – Parts

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/admin/parts` | Get all parts (with filters) |
| GET | `/api/admin/parts/{id}` | Get part by ID |
| POST | `/api/admin/parts` | Add new part |
| PUT | `/api/admin/parts/{id}` | Update part |
| DELETE | `/api/admin/parts/{id}` | Delete part |

### Admin – Vendors

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/admin/vendors` | List all vendors |
| POST | `/api/admin/vendors` | Create vendor |
| PUT | `/api/admin/vendors/{id}` | Update vendor |
| DELETE | `/api/admin/vendors/{id}` | Delete vendor |

### Admin – Staff

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/admin/staff` | List all staff |
| POST | `/api/admin/staff/register` | Register staff member |
| PUT | `/api/admin/staff/{id}` | Update staff |
| DELETE | `/api/admin/staff/{id}` | Deactivate staff |

### Admin – Purchase Invoices

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/admin/purchase-invoices` | List purchase invoices |
| POST | `/api/admin/purchase-invoices` | Create purchase invoice (updates stock) |
| GET | `/api/admin/purchase-invoices/{id}` | Get invoice details |

### Admin – Reports

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/admin/reports/financial?type=daily&date=2025-01-01` | Daily financial report |
| GET | `/api/admin/reports/financial?type=monthly&month=2025-01` | Monthly report |
| GET | `/api/admin/reports/financial?type=yearly&year=2025` | Yearly report |

### Staff – Sales

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/staff/sales/invoices` | Create sales invoice |
| GET | `/api/staff/sales/invoices/{id}` | Get invoice |
| POST | `/api/staff/sales/invoices/{id}/email` | Email invoice to customer |

### Staff – Customers

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/staff/customers/register` | Register customer + vehicle |
| GET | `/api/staff/customers/{id}` | Get customer profile |
| GET | `/api/staff/customers/{id}/history` | Purchase & service history |
| GET | `/api/staff/customers/search?q=...` | Advanced search |

### Staff – Reports

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/staff/reports/regular-customers` | Regular customers |
| GET | `/api/staff/reports/high-spenders` | High spender customers |
| GET | `/api/staff/reports/pending-credits` | Customers with overdue credits |

### Customer – Self-Service

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/customer/profile` | Get own profile |
| PUT | `/api/customer/profile` | Update profile |
| GET | `/api/customer/history` | Purchase and service history |
| POST | `/api/customer/bookings` | Book a service appointment |
| GET | `/api/customer/bookings` | List own bookings |
| POST | `/api/customer/part-requests` | Request an unavailable part |

---

## Authentication & Authorization

The API uses **JWT Bearer tokens**. Each token contains a `role` claim (`Admin`, `Staff`, or `Customer`) used to enforce access control via `[Authorize(Roles = "...")]` attributes.

**Token flow:**

```
POST /api/auth/login → { token, refreshToken, expiresAt, role }
```

Include the token in the `Authorization` header for all protected requests:

```
Authorization: Bearer <your-token-here>
```

---

## Background Jobs (Hangfire)

Two recurring jobs run automatically:

| Job | Schedule | Description |
|---|---|---|
| `LowStockNotificationJob` | Every hour | Notifies Admin when any part stock drops below threshold (default: 10 units) |
| `CreditReminderJob` | Daily at 8:00 AM | Sends email reminders to customers with unpaid credits older than 30 days |

Hangfire dashboard (Admin only): `https://localhost:7001/hangfire`

---

## Environment Variables

For production deployments, override `appsettings.json` with environment variables:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=prod-server;...
JwtSettings__SecretKey=your-production-secret
EmailSettings__Password=your-smtp-password
```

---

## Running Tests

```bash
dotnet test
```

Or in Visual Studio: **Test → Run All Tests**

Test projects:

```
Gearix.Tests/
├── Unit/        # Service and validator unit tests
└── Integration/ # Controller integration tests (WebApplicationFactory)
```

---

## Database Migrations

**Add a new migration:**

```bash
dotnet ef migrations add <MigrationName> --project Gearix.API
```

**Apply migrations:**

```bash
dotnet ef database update
```

**Rollback:**

```bash
dotnet ef database update <PreviousMigrationName>
```

---

## Build for Production

```bash
dotnet publish -c Release -o ./publish
```

For Docker:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Gearix.API.dll"]
```

---

## Available Scripts / Commands

| Command | Description |
|---|---|
| `dotnet run` | Start development server |
| `dotnet watch run` | Start with hot reload |
| `dotnet build` | Build the project |
| `dotnet publish -c Release` | Publish production build |
| `dotnet test` | Run all tests |
| `dotnet ef migrations add <Name>` | Create a new migration |
| `dotnet ef database update` | Apply pending migrations |
| `dotnet ef database drop` | Drop the database (dev only) |

---

## Key NuGet Packages

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `8.x` | JWT authentication |
| `Microsoft.EntityFrameworkCore.SqlServer` | `8.x` | SQL Server provider |
| `Microsoft.EntityFrameworkCore.Tools` | `8.x` | EF Core CLI tools |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | `12.x` | DTO mapping |
| `FluentValidation.AspNetCore` | `11.x` | Request validation |
| `Serilog.AspNetCore` | `8.x` | Structured logging |
| `Hangfire.AspNetCore` | `1.8.x` | Background jobs |
| `Hangfire.SqlServer` | `1.8.x` | Hangfire SQL storage |
| `MailKit` | `4.x` | SMTP email sending |
| `Swashbuckle.AspNetCore` | `6.x` | Swagger / OpenAPI |
| `BCrypt.Net-Next` | `4.x` | Password hashing |

---

## Role-Based Access Summary

| Feature | Admin | Staff | Customer |
|---|---|---|---|
| Financial Reports | ✅ | ❌ | ❌ |
| Parts CRUD | ✅ | ❌ | ❌ |
| Vendor CRUD | ✅ | ❌ | ❌ |
| Purchase Invoices | ✅ | ❌ | ❌ |
| Staff Management | ✅ | ❌ | ❌ |
| Sales Invoices | ❌ | ✅ | ❌ |
| Customer Registration | ❌ | ✅ | ✅ (self) |
| Customer Search | ❌ | ✅ | ❌ |
| Customer Reports | ❌ | ✅ | ❌ |
| Own Profile | ❌ | ❌ | ✅ |
| Bookings | ❌ | ❌ | ✅ |
| Purchase History | ❌ | ✅ | ✅ (own) |
| Hangfire Dashboard | ✅ | ❌ | ❌ |

---

## Contributing

1. Fork the repository
2. Create a feature branch:

```bash
git checkout -b feature/amazing-feature
```

3. Commit your changes:

```bash
git commit -m "Add amazing feature"
```

4. Push to the branch:

```bash
git push origin feature/amazing-feature
```

5. Open a Pull Request

---

## License

This project is for educational/personal use.
All rights reserved.
