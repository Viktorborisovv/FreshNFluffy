# FreshNFluffy 🍰
FreshNFluffy is a demo ASP.NET Core MVC web application for a bakery.  
Users can browse products, create order requests, add items, and track order status in a clean “glass UI” design.

---

## ✨ Features
- **Products**
  - Create, edit, delete, view details
  - Product image support (with placeholder when missing)
  - Filtering by **category**, **nutrition type**, and **search**
- **Order Requests**
  - Create an order request (customer + pickup date/time + notes)
  - Add/remove items and update quantities
  - Order status workflow: **Pending → Confirmed → Ready → Completed** (+ Cancelled)
  - Manage Orders page with filtering + search
- **Authentication**
  - ASP.NET Core **Identity** enabled (Register / Login / Logout)

---

## 🛠 Tech Stack
| Technology            | Version | Purpose                    |
|-----------------------|---------|----------------------------|
| ASP.NET Core MVC      | 8.0     | Web framework              |
| Entity Framework Core | 8.0     | ORM / Database access      |
| SQL Server (LocalDB)  | -       | Database                   |
| ASP.NET Core Identity | 8.0     | Authentication             |
| Bootstrap             | 5       | Frontend styling           |
| Razor Views           | -       | Server-side rendering      |

---


## 🚀 Getting Started (Local Setup)

### 1) Prerequisites
- **.NET SDK** (recommended: .NET 8)
- **SQL Server** (Docker / local instance)
- (Optional) **Visual Studio 2022** / VS Code

### 2) Clone the repository
```bash
git clone https://github.com/Viktorborisovv/FreshNFluffy.git
cd FreshNFluffy

Restore dependencies:
dotnet restore

3) 🗄️ Database Setup (Recommended: Docker SQL Server):
Start SQL Server container:
cd BakaryProject2026   # repo root where docker-compose.yml is
docker compose up -d
The repository includes `appsettings.json` with a placeholder connection string.

Create **locally** an `appsettings.Development.json` (this file is ignored by git) and put your real SQL Server credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FreshNFluffyDb;User Id=sa;Password=YourPasswordHere;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True"
  }
}

4) Run migrations using Development environment:
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project ./FreshNFluffy/FreshNFluffy.csproj --startup-project ./FreshNFluffy/FreshNFluffy.csproj

5) Run the app:
dotnet run --project ./FreshNFluffy/FreshNFluffy.csproj

Then open the shown URL (example):
https://localhost:7249

💻 Usage

1. Register and log in.
2. Go to Products and add products (images optional).
3. Go to Order Request to create a new order.
4. Add items and set pickup date/time.
5. Open Orders to manage status and view details.

📁 Project Structure
FreshNFluffy/

├── Controllers -> MVC Controllers
├── Data -> DbContext and EF Core migrations
├── Models -> Domain models
├── Services -> Business logic / service layer
├── ViewModels -> ViewModels used by views
├── Views -> Razor Views (.cshtml)
├── wwwroot -> Static files (CSS, JS, images)
├── appsettings.json -> App configuration
└── Program.cs -> App entry point / middleware

Recommended:
Use images in JPG/PNG
Wide images work best (e.g. 1200x800), but any size is acceptable.

🔐 Authentication (Identity)
The project uses ASP.NET Core Identity for registration and login.
Note: Role management (Admin/User) is not required for this demo version (can be extended later).

📄 Privacy
FreshNFluffy is a demo project. It stores only what is needed for order processing:

Customer name & phone
Pickup date/time
Order items and quantities
Optional notes

✅ Project Status
This project is ready for submission as a complete demo MVC application:

Clean UI
Working CRUD
Working order workflow
Database + migrations
Identity login/register

📄 License
This project is licensed under the MIT License. See LICENSE for details.

📬 Contact
Viktor Borisov – vsborisov7@gmail.com
Project Link: https://github.com/Viktorborisovv/FreshNFluffy
