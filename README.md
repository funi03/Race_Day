# 🏁 RaceDay Event Management System — PROG6212 Part 3

**A Full-Stack Event Management Web Application**

RaceDay is a full-stack web application developed as part of the PROG6212 Programming 2B portfolio. The project consists of a RESTful ASP.NET Core Web API (Part 2) and an ASP.NET Core MVC Web Application (Part 3), which communicates with the API to provide a complete event management solution.

The system allows organisers to manage events and participant enrolments, while participants can browse available events and register for events.

The application incorporates role-based authentication, Azure Blob Storage integration with local storage fallback, Docker containerisation and automated CI/CD using GitHub Actions.

---

## 👤 Author Information

| Detail | Information |
|---|---|
| **Name** | Funanani Nelitshindwe |
| **Student Number** | 10480209 |
| **Module** | PROG6212 — Programming 2B |
| **Project** | RaceDay Event Management System — Part 3 |

---

## 🛠️ Technologies Used

| Technology | Purpose |
|---|---|
| ASP.NET Core 8.0 | Web API and MVC development |
| C# | Backend programming language |
| SQL Server | Database management |
| Entity Framework Core | Database operations and ORM |
| Cookie Authentication | User authentication |
| Azure Blob Storage | Event image storage |
| Bootstrap 5 | Responsive user interface |
| Razor Views | Dynamic web pages |
| HTML5 & CSS3 | Frontend development |
| Docker | Application containerisation |
| GitHub Actions | Continuous Integration (CI/CD) |
| Swagger UI | API testing and documentation |

---

## ✨ Main Features

### 🔐 Role-Based Authentication
- Secure user login and logout.
- Cookie-based authentication.
- Separate access permissions for Organisers and Participants.

### 📅 Event Management
- Organisers can create and manage events.
- Organisers can upload event banner images.
- Participants can browse available events.
- Participants can view event information.

### 📝 Participant Enrolments
- Participants can enrol in available events.
- Participants can view their personal enrolments.
- Organisers can view participant enrolments.

### ☁️ Azure Blob Storage Integration
- Event banner images are uploaded through the Web API.
- Azure Blob Storage is used for cloud-based image storage.
- Local storage fallback is available when Azure configuration is not provided.

### 🐳 Docker Containerisation
- The MVC application can run inside a Docker container.
- Simplifies application setup and deployment.
- Provides a consistent execution environment.

### ⚙️ GitHub Actions CI/CD
- Automated dependency restoration.
- Automated API project build.
- Automated MVC project build.
- Workflow triggered by pushes to the `Part-3` branch.

---

## 💻 Setup and Installation

Follow the instructions below to run the RaceDay application locally.

### Prerequisites

Ensure that the following software is installed:

- Visual Studio 2022
- .NET 8.0 SDK
- SQL Server (LocalDB or SQL Server Express)
- Docker Desktop (for containerisation)

### Step 1: Clone the Repository

Clone the repository from GitHub and open the solution in Visual Studio.

```bash
git clone YOUR_GITHUB_REPOSITORY_LINK
```

Open the solution file:

```text
Race_Day.sln
```

### Step 2: Configure the Database

1. Open Visual Studio.
2. Navigate to **Tools → NuGet Package Manager → Package Manager Console**.
3. Set the Default Project to `Race_Day` (API project).
4. Execute the following command:

```powershell
Update-Database
```

This command applies the Entity Framework Core migrations and creates or updates the database.

### Step 3: Run the Application

1. Right-click the solution in Solution Explorer.
2. Select **Configure Startup Projects**.
3. Choose **Multiple startup projects**.
4. Set the following projects to Start:

| Project | Action |
|---|---|
| Race_Day (API) | Start |
| RaceDay.Web (MVC) | Start |
| Test Project | None |

5. Click Apply and OK.
6. Press **F5** to run the application.

The API will open with Swagger UI, while the MVC application will launch in your browser.

---

## 👥 User Roles

RaceDay supports two main user roles:

### 1. Organiser
Organisers are responsible for managing events and monitoring participant activity.

**Permissions:**
- Log in to the system.
- Create and manage events.
- Upload event banner images.
- View participant enrolments.

### 2. Participant
Participants can explore events and manage their registrations.

**Permissions:**
- Log in to the system.
- Browse available events.
- View event details.
- Enrol in events.
- View personal enrolments.

---

## ☁️ Azure Blob Storage Configuration

The MVC application does not communicate directly with Azure Blob Storage. Instead, all image uploads are sent to the Web API through the `BlobController`, which handles the storage operations.

To enable Azure Blob Storage:

1. Open the API project's `appsettings.json` file.
2. Add your Azure Storage connection string:

```json
{
  "AzureBlob": {
    "ConnectionString": "YOUR_AZURE_STORAGE_CONNECTION_STRING"
  }
}
```

**Important:** Replace the placeholder with your actual Azure Storage connection string. Do not commit real credentials or connection strings to GitHub. Use secure configuration such as User Secrets or environment variables.

**Local Storage Fallback**

If an Azure connection string is not configured, the API falls back to local image storage in:

```text
wwwroot/images
```

This allows the application to remain functional during local development and demonstrations.

---

## 🐳 Docker Configuration

The MVC application supports Docker containerisation.

Ensure Docker Desktop is running before executing the following commands from the repository's root directory.

### Step 1: Build the Docker Image

```bash
docker build -t raceday-web -f RaceDay.Web/Dockerfile .
```

### Step 2: Run the Docker Container

```bash
docker run -d -p 8080:8080 --name raceday-container raceday-web
```

### Step 3: Access the Application

Open your browser and navigate to:

```text
http://localhost:8080
```

The RaceDay MVC application should now be accessible through the Docker container.

---

## ⚙️ Continuous Integration (CI/CD)

The project uses **GitHub Actions** to automate the build and validation process.

The workflow is triggered whenever changes are pushed to the `Part-3` branch.

<img width="1360" height="602" alt="Screenshot 2026-11-09 164051" src="https://github.com/user-attachments/assets/970185a1-fce3-429e-ad74-7842d2ed2f48" />



### Pipeline Activities

1. Sets up the .NET 8.0 SDK.
2. Restores project dependencies.
3. Builds the ASP.NET Core Web API project.
4. Builds the ASP.NET Core MVC Web Application project.

This automated process helps identify build errors and ensures that both projects compile successfully.

---

## 🎥 Video Demonstration

The video demonstration provides an overview of the RaceDay Event Management System and its implementation.

The walkthrough covers:

- MVC architecture and application functionality.
- User authentication and role-based access.
- Event creation and management.
- Participant enrolments.
- Azure Blob Storage integration.
- Docker containerisation.
- GitHub Actions CI/CD pipeline.

▶️ **[Watch the RaceDay Part 3 Video Demonstration](YOUR_YOUTUBE_LINK_HERE)**

---

## 📌 Project Information

**Module:** PROG6212 — Programming 2B

**Portfolio:** Part 3

**Project:** RaceDay Event Management System

**Framework:** ASP.NET Core 8.0

**Architecture:** RESTful Web API + ASP.NET Core MVC

---

## 🙌 Acknowledgement

This project demonstrates the development of a full-stack event management application using modern software development technologies and practices.

It brings together backend API development, MVC architecture, database integration, authentication, cloud storage, containerisation and automated build processes.

**Thank you for exploring the RaceDay Event Management System!** 🏁

```

