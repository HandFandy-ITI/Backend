<p align="center">
  <img src="[https://your-logo-link.png](https://res.cloudinary.com/dy35wrc6z/image/upload/v1752076118/logo_nkqsnx.png)" alt="OstaFandy Logo" width="200"/>
</p>

# 🛠️ OstaFandy Backend

This is the backend REST API for **OstaFandy**, a home-services booking platform.  
It powers the system for customers to book services, and handles handyman onboarding, booking management, notifications, admin tools, wallet management, and intelligent customer support.

---

## 🚀 Key Features

- 🔐 **JWT-based authentication & role-based authorization**
- 👨‍🔧 **Customer & Handyman registration & login**
- 👨‍💼 **Handyman onboarding** with admin-controlled approval & interview
- 📝 **Service catalog** with categories, multi-service booking in the same category & optional inspection service
- 📆 **Admin-managed handyman availability & vacations** (fixed shift: e.g., 10 AM–6 PM)
- 🤖 **Automatic handyman assignment** based on location & availability
- ✅ **Bookings are auto-confirmed** — no need for handyman to accept
- 🪪 **Fixed salary system** for handymen (no commissions)
- 🔔 **Real-time notifications** (SignalR)
- 💬 **Real-time chat** with handyman (SignalR)
- 📷 **Image upload** via Cloudinary
- 💳 **Stripe integration** for secure post-service payments
- 🧹 **Admin dashboard** for managing users, services, bookings, and handymen
- 🤖 **Chatbot** to help customers choose the best service for their problem


## 📂 Project Structure
```
OstaFandy/
│
├── DAL/ # Data Access Layer
│ ├── Entities/ # Database models (generated from the database)
│ │ ├── AppDbContext.cs
│ │ ├── User.cs
│ │ ├── Handyman.cs
│ │ ├── Customer.cs
│ │ ├── Service.cs
│ │ ├── Booking.cs
│ │ └── ...
│ │
│ ├── Repos/ # Data access logic
│ │ ├── IRepos/ # Repository interfaces
│ │ │ ├── IUnitOfWork.cs
│ │ │ └── ...
│ │ ├── UnitOfWork.cs # Unit of Work implementation
│ │ └── ...
│
├── PL/ # Presentation Layer (Web API)
│ ├── Controllers/ # API endpoints
│ │ ├── AuthController.cs
│ │ ├── BookingController.cs
│ │ ├── HandymanController.cs
│ │ ├── AdminController.cs
│ │ ├── ServiceController.cs
│ │ └── ...
│ │
│ ├── DTOs/ # Data Transfer Objects (request/response models)
│ │ ├── Auth/
│ │ ├── Booking/
│ │ ├── Handyman/
│ │ ├── Service/
│ │ └── ...
│ │
│ ├── BL/ # Business Logic Layer
│ │ ├── IBL/ # Business interfaces
│ │ └── Implementations/ # Business logic implementations
│ │
│ ├── General/ # Constants & Enums
│ │
│ ├── Hub/ # SignalR hubs (e.g., for real-time chat)
│ │
│ ├── Mapper/ # DTO <-> Entity mapping classes
│ │
│ ├── Utils/ # Helper functions & utilities
│ │
│ ├── Program.cs # Application startup & dependency injection
│ ├── appsettings.json # Application configuration
│ ├── appsettings.Development.json
│ ├── PL.csproj # Presentation Layer project file
└── 
```

## 🛠️ Tech Stack

- **Language:** C#
- **Framework:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Authentication:** JWT
- **Real-time:** SignalR
- **Cloud Storage:** Cloudinary
- **Payments:** Stripe

## ⚙️ Getting Started

Follow these steps to set up and run the OstaFandy backend locally.

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  

 ### Configuration

Update the `appsettings.json` file with your environment-specific settings.  
Below is an example configuration structure (sensitive values should be replaced):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;User ID=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=False;MultipleActiveResultSets=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY"
  },
  "CloudinarySettings": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "YOUR_API_SECRET"
  },
  "Stripe": {
    "SecretKey": "YOUR_STRIPE_SECRET_KEY",
    "PublishableKey": "YOUR_STRIPE_PUBLISHABLE_KEY"
  },
  "OpenRouter": {
    "ApiKey": "YOUR_OPENROUTER_API_KEY"
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSSL": true,
    "UserName": "YOUR_EMAIL_USERNAME",
    "Password": "YOUR_EMAIL_PASSWORD",
    "From": "YOUR_FROM_EMAIL"
  }
}
```
## 🗂️ Database Schema

Below is the database schema diagram illustrating the main entities and their relationships:

![OstaFandy Database Schema]([./docs/schema.png](https://res.cloudinary.com/dy35wrc6z/image/upload/v1752076110/ERD_xkpruu.png))
