# Hospital Management System

A web-based hospital management system designed for day hospitals to manage patient admissions, electronic prescriptions, medication dispensing, surgery bookings, and clinical workflows.

Built with **ASP.NET Core MVC**, **Entity Framework Core**, **SQL Server**, and **ASP.NET Core Identity**, the system provides role-based functionality for administrators, nurses, surgeons, and pharmacists.

## Features

### Admin

* Manage hospital and medical staff records
* Manage wards and theatres
* Manage diagnoses and medications
* Manage medication interactions and active ingredients
* Maintain system lookup data

### Nurse

* Register and manage patient information
* Record patient vitals
* Capture allergies and medical history
* Administer prescribed medication
* Record patient discharge information

### Surgeon

* Book surgeries by selecting the patient, theatre, date, session, and surgeon
* Create electronic prescriptions
* Check prescriptions for potential contraindications
* Receive alerts for allergies, medication interactions, and relevant medical conditions
* Provide a justification when overriding a medication warning
* View prescription status and pharmacist feedback

### Pharmacist

* Review and dispense prescriptions
* Prioritise urgent prescription requests
* Manage medication stock levels
* Reject prescriptions when required
* Provide feedback to surgeons

## Medication Safety

One of the key features of the system is its medication safety workflow.

When a surgeon creates a prescription, the application checks relevant patient information and medication data for potential risks, including:

* Patient allergies
* Medication interactions
* Medical-condition-related contraindications

When a potential risk is detected, the surgeon is alerted before continuing. If the prescription is still required, the system requires a justification for overriding the warning.

## Technology Stack

| Technology              | Purpose                            |
| ----------------------- | ---------------------------------- |
| C#                      | Application development            |
| ASP.NET Core 8 MVC      | Web application framework          |
| Entity Framework Core 8 | ORM and database access            |
| SQL Server              | Relational database                |
| ASP.NET Core Identity   | Authentication and user management |
| Razor Views             | Server-side UI                     |
| MailKit                 | Email functionality                |
| Bootstrap / CSS         | User interface styling             |
| Git & GitHub            | Version control                    |

## Architecture

The application follows the **Model-View-Controller (MVC)** architecture:

```text
HospitalSystem/
├── Areas/
│   └── Identity/          # Authentication and account pages
├── Controllers/           # Application controllers
├── Data/                  # Entity Framework database context
├── Migrations/            # Entity Framework database migrations
├── Models/                # Domain and view models
├── Views/                 # Razor UI views
├── wwwroot/               # Static files and frontend assets
├── Program.cs             # Application configuration and startup
├── DEMO.csproj            # Project dependencies and configuration
└── appsettings.json       # Application configuration
```

## Database

The project uses **SQL Server** with **Entity Framework Core**. Database changes are managed through Entity Framework migrations.

The database contains data relating to areas such as:

* Patients
* Staff and user accounts
* Admissions
* Surgeries
* Prescriptions
* Medications
* Active ingredients
* Allergies
* Diagnoses
* Medication interactions
* Medication stock
* Dispensed prescriptions
* Patient vitals

## Getting Started

### Prerequisites

Before running the application, install:

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server or SQL Server Express
* Visual Studio 2022 or another compatible .NET development environment
* Git

### 1. Clone the Repository

```bash
git clone https://github.com/Naldo1818/HospitalSystem.git
cd HospitalSystem
```

### 2. Configure SQL Server

Update the `DefaultConnection` value in `appsettings.json` so it points to your local SQL Server instance.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=HospitalDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
  }
}
```

**Do not commit passwords, API keys, or other sensitive credentials to the repository.**

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Apply Database Migrations

```bash
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

Alternatively, open `DEMO.sln` in Visual Studio and run the project using the built-in development server.

## Authentication

The application uses **ASP.NET Core Identity** for authentication and account management.

Different hospital roles are supported so that users can access functionality appropriate to their responsibilities.

## My Contribution

This was a group software development project. My primary responsibility was implementing the **surgeon functionality**.

I worked on:

* Surgery booking functionality
* Prescription creation and management
* Medication contraindication checks
* Allergy and medication interaction validation
* Medication safety alerts
* Prescription override and justification logic
* Prescription status and pharmacist feedback workflows
* Database interactions and validation for surgeon-related features

The project gave me practical experience working with a team on a large MVC application while dealing with relational data, authentication, validation, business rules, and role-specific workflows.

## Learning Outcomes

This project strengthened my experience with:

* ASP.NET Core MVC development
* C# and object-oriented programming
* Entity Framework Core
* SQL Server database design and migrations
* Authentication and role-based application workflows
* CRUD operations
* Server-side validation
* Business-rule implementation
* Relational database queries
* Team-based software development
* Debugging and maintaining a multi-feature application

## Project Status

This project was developed as an academic/group software development project and is maintained as part of my software development portfolio.



* GitHub: [Naldo1818](https://github.com/Naldo1818)
* Portfolio: [Ronaldoprofile](https://naldo1818.github.io/Ronaldoprofile/)
