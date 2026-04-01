# FreshNFluffy 🍰

FreshNFluffy is a demo **ASP.NET Core MVC** web application for a bakery.
It allows users to browse products, create orders, and manage them through a structured workflow, all wrapped in a modern **glass UI design**.

---

## 🚀 Features

### 🛍️ Product Management

* Create, edit, delete, and view products
* Image support with fallback placeholder
* Advanced filtering:

  * Category
  * Nutrition types (Flags enum)
  * Search

---

### 📦 Order Management

* Create order requests
* Add / remove items
* Update quantities
* Order workflow:
  **Pending → Confirmed → Ready → Completed (+ Cancelled)**
* Business rules validation
* Automatic total price calculation

---

### 🛠️ Admin Area

* Dedicated Admin panel (Area-based architecture)
* Manage all orders in one place
* Filtering and searching capabilities

---

### ⭐ Reviews System

* Add reviews to products
* View reviews per product
* Sorted by latest

---

### 🔐 Authentication & Authorization

* ASP.NET Core Identity
* Register / Login / Logout
* Role-based access control (**Admin / User**)

---

## 🧠 Architecture

The project follows a **clean layered architecture**:

**Controllers → Services → Repositories → Data**

### Key Design Principles:

* ✔️ Repository Pattern
* ✔️ Service Layer (business logic separation)
* ✔️ Dependency Injection
* ✔️ ViewModels (no direct entity exposure)
* ✔️ Enum-driven workflows
* ✔️ LINQ query composition

---

## 🧪 Testing

Unit tests are implemented using:

* ✔️ xUnit
* ✔️ EF Core InMemory Provider

### Coverage:

* ✅ 65%+ Line Coverage

### Tested Components:

* ProductService
* OrderService (complex workflow logic)
* ReviewService

### Focus:

* Business logic validation
* Edge cases
* Guard clauses

---

## 🛠️ Tech Stack

| Technology              | Version | Purpose                        |
| ----------------------- | ------- | ------------------------------ |
| ASP.NET Core MVC        | 8.0     | Web framework                  |
| Entity Framework Core   | 8.0     | ORM / Database access          |
| SQL Server (Docker)     | -       | Database                       |
| Docker & Docker Compose | -       | Containerization               |
| ASP.NET Core Identity   | 8.0     | Authentication & Authorization |
| Bootstrap               | 5       | Frontend styling               |
| Razor Views             | -       | Server-side rendering          |
| xUnit                   | -       | Unit testing                   |

---

## ⚙️ Getting Started

### 1️⃣ Prerequisites

* .NET 8 SDK
* Docker (for SQL Server)
* Visual Studio 2022 / VS Code (optional)

---

### 2️⃣ Clone the repository

```bash
git clone https://github.com/Viktorborisovv/FreshNFluffy.git
cd FreshNFluffy
```

---

### 3️⃣ Restore dependencies

```bash
dotnet restore
```

---

### 4️⃣ Database Setup (Docker SQL Server)

```bash
docker compose up -d
```

Create `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FreshNFluffyDb;User Id=sa;Password=YourPasswordHere;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True"
  }
}
```

---

### 5️⃣ Apply migrations

```bash
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project ./FreshNFluffy/FreshNFluffy.csproj --startup-project ./FreshNFluffy/FreshNFluffy.csproj
```

---

### 6️⃣ Run the application

```bash
dotnet run --project ./FreshNFluffy/FreshNFluffy.csproj
```

Open:

```
https://localhost:7249
```

---

## 💻 Usage

1. Register and log in
2. Browse or create products
3. Create an order request
4. Add items and set pickup time
5. Manage order status

---

## 📁 Project Structure

```
FreshNFluffy/
├── Areas/Admin
├── Controllers
├── Data
│   ├── Models
│   ├── Repository
│   ├── Migrations
│   └── Seeding
├── Services
├── ViewModels
├── Views
├── wwwroot
└── Program.cs
```

---

## 🔐 Security

* Authentication via ASP.NET Core Identity
* Role-based authorization (Admin / User)
* Protection against unauthorized actions

---

## 📄 Privacy

This is a demo project. It stores only:

* Customer name & phone
* Pickup date/time
* Order items
* Optional notes

---

## ✅ Project Status

✔️ Fully functional MVC application
✔️ Clean architecture
✔️ Role-based security
✔️ Database & migrations
✔️ Unit testing coverage

---

## 📄 License

MIT License

---

## 📬 Contact

**Viktor Borisov**
📧 [vsborisov7@gmail.com](mailto:vsborisov7@gmail.com)

🔗 GitHub:
https://github.com/Viktorborisovv/FreshNFluffy
