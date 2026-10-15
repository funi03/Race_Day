# RaceDay Event Management System — PROG6212 Part 2

A RESTful API for managing race events, categories, participant enrolments, and race results. Built with **ASP.NET Core Web API (.NET 8), Entity Framework Core, and SQL Server**.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [User Roles](#user-roles)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Running the Application](#running-the-application)
- [CI/CD](#cicd)
- [Video Walkthrough](#video-walkthrough)
- [Author](#author)

## Overview

RaceDay is an event management system API that supports event creation, category management, participant enrolments, and race result tracking.

The API uses cookie-based authentication and role-based authorisation to distinguish between Organisers and Participants. Swagger UI provides interactive API documentation and testing.

## Features

- User registration, login, and logout.
- Secure password hashing using BCrypt.
- Cookie-based authentication.
- Role-based access control.
- Event and category management.
- Participant enrolments and cancellations.
- Race result recording and viewing.
- SQL Server database integration using Entity Framework Core Code-First.
- Interactive API documentation through Swagger UI.
- Automated builds using GitHub Actions.

## Technologies Used

| Technology | Purpose |
|---|---|
| C# | Programming language |
| ASP.NET Core Web API (.NET 8) | API development |
| Entity Framework Core 8 | Database access and ORM |
| SQL Server | Database management |
| BCrypt.Net-Next | Password hashing |
| Swagger / Swashbuckle | API documentation and testing |
| GitHub Actions | Continuous integration |

## Prerequisites

Before running the project, ensure you have installed:

- Visual Studio 2022
- .NET 8 SDK
- SQL Server Express or SQL Server LocalDB
- SQL Server Management Studio (SSMS), optional

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/funi03/Race_Day.git
```

### 2. Open the Solution

Open `Race_Day.sln` in Visual Studio 2022.

### 3. Configure the Database Connection

Open `appsettings.json` and check the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\race_day;Database=RaceDayDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Update the server name if your SQL Server instance uses a different configuration.

### 4. Apply Database Migrations

In Visual Studio, open **Tools → NuGet Package Manager → Package Manager Console**.

Set the Default Project to `Race_Day`, then run:

```powershell
Update-Database
```

Ensure that the configured SQL Server instance is available before applying migrations.

## Running the Application

1. Open the solution in Visual Studio.
2. Set `Race_Day` as the startup project.
3. Run the application using **F5** or the green Start button.
4. Open Swagger UI using the HTTPS URL displayed by the application, followed by `/swagger`.

Example:

```text
https://localhost:<port>/swagger
```

Replace `<port>` with the port configured for your application.

## User Roles

### Organiser

Organisers can:

- Register and log in.
- Create, update, and delete their own events.
- Manage categories for their events.
- View enrolments for their events.
- Record and update race results.

### Participant

Participants can:

- Register and log in.
- Browse public events and categories.
- Enrol in event categories.
- Cancel their own enrolments.
- View their own race results.

**Access control:** Protected endpoints require authentication and enforce the relevant role and ownership permissions.

## API Endpoints

The following table summarises the main API routes.

### Authentication — `/api/auth`

| Method | Endpoint | Access | Description |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Register a user |
| POST | `/api/auth/login` | Public | Log in |
| POST | `/api/auth/logout` | Authenticated | Log out |

### Users — `/api/users`

| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/users/me` | Authenticated | View own profile |
| PUT | `/api/users/me` | Authenticated | Update own profile |

### Events — `/api/events`

| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/events` | Public | List all events |
| GET | `/api/events/{id}` | Public | View an event |
| POST | `/api/events` | Organiser | Create an event |
| PUT | `/api/events/{id}` | Organiser | Update own event |
| DELETE | `/api/events/{id}` | Organiser | Delete own event |

### Categories — `/api/events/{eventId}/categories` and `/api/categories`

| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/events/{eventId}/categories` | Public | List event categories |
| POST | `/api/events/{eventId}/categories` | Organiser | Create a category |
| PUT | `/api/categories/{id}` | Organiser | Update own category |
| DELETE | `/api/categories/{id}` | Organiser | Delete own category |

### Enrolments — `/api/enrolments`

| Method | Endpoint | Access | Description |
|---|---|---|---|
| POST | `/api/enrolments` | Participant | Enrol in a category |
| GET | `/api/enrolments/me` | Participant | View own enrolments |
| GET | `/api/events/{eventId}/enrolments` | Organiser | View event enrolments |
| PUT | `/api/enrolments/{id}/cancel` | Participant | Cancel own enrolment |

### Results — `/api/results`

| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/events/{eventId}/results` | Public | View event results |
| POST | `/api/results` | Organiser | Record a result |
| PUT | `/api/results/{id}` | Organiser | Update a result |
| GET | `/api/participants/me/results` | Participant | View own results |

> **Note:** Endpoint access and behaviour should match the implemented controllers and authorisation rules.

## Project Structure

```text
Race_Day/
├── Controllers/       # API controllers and endpoints
├── Data/              # Database context
├── Migrations/        # Entity Framework migrations
├── Models/            # Entities and DTOs
├── Properties/        # Launch settings
├── Program.cs         # Application configuration
├── appsettings.json   # Application settings
└── Race_Day.csproj    # Project configuration
```

## CI/CD

The project uses **GitHub Actions** to build the API automatically when changes are pushed to, or pull requests target, the configured branch.

The workflow helps identify build errors and maintain code quality.

## Video Walkthrough

**RaceDay API — Part 2 Walkthrough**

The walkthrough demonstrates:

- Project structure and database configuration.
- API endpoints using Swagger UI.
- Authentication and role-based access.
- Event, category, enrolment, and result operations.
- The GitHub Actions build workflow.

*Add your unlisted YouTube video URL here.*

---

## Submission Structure

This project is submitted as **two separate folders**:

### 1. `Race_Day/` — The API Project

The main ASP.NET Core Web API project. To run the API:

1. Open `Race_Day.sln` in Visual Studio 2022.
2. Run `Update-Database` in the Package Manager Console (Default project: `Race_Day`).
3. Press **F5** — Swagger opens at `/swagger`.

### 2. `TestProjectRD/` — The Test Project

The xUnit test project. To run the tests:

1. Open `TestProjectRD.csproj` in Visual Studio 2022.
2. Ensure the project reference to `..\Race_Day\Race_Day.csproj` resolves — the API project must be at the path `..\Race_Day\` relative to this project.
3. Press **Test → Run All Tests**.

### Relationship between the two folders

`TestProjectRD` references `Race_Day` via a project reference. When both folders are placed as siblings — `Race_Day/` and `TestProjectRD/` — the reference resolves correctly and the tests run against the API project.


## Author

**Funanani Nelitshindwe**

- **Student Number:** 10480209
- **Module:** PROG6212 — Programming 2B
- **Institution:** Rosebank International

---

*RaceDay Event Management System — PROG6212 Part 2*
