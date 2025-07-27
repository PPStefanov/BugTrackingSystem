# 🐛 Bug Tracking System Pro

A modern, professional web application built with ASP.NET Core 8.0 that provides comprehensive bug tracking and project management capabilities with a beautiful, responsive user interface.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blue)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-green)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple)
![License](https://img.shields.io/badge/License-MIT-yellow)

---

## ✨ Key Features

### 🎨 **Modern UI/UX Design**
- **Collapsible Sidebar Navigation** - Smooth, animated sidebar with Texcycle-inspired color scheme
- **Professional Dashboard** - Interactive statistics cards with real-time data visualization
- **Responsive Design** - Mobile-first approach supporting all device sizes
- **Dark Theme Elements** - Modern gradient backgrounds and professional styling
- **Smooth Animations** - Cubic-bezier transitions and hover effects throughout

### 🔐 **Authentication & Authorization**
- **ASP.NET Identity Integration** - Secure user registration and authentication
- **Role-Based Access Control** - Administrator and User roles with different permissions
- **User Management** - Complete user administration interface
- **Session Management** - Secure login/logout with state persistence

### 🐞 **Bug Management**
- **Comprehensive Bug Reporting** - Detailed bug submission with priority levels
- **Status Tracking** - Real-time bug status updates (Open, In Progress, Testing, Resolved)
- **Priority Management** - Critical, High, Medium, Low priority classification
- **Application Assignment** - Link bugs to specific applications/projects
- **Comment System** - Collaborative commenting on bug reports

### 📊 **Dashboard & Analytics**
- **Interactive Statistics** - Visual representation of bug metrics
- **Status Distribution** - Progress bars showing bug status breakdown
- **Priority Analysis** - Priority-based bug distribution charts
- **Recent Activity Feed** - Real-time updates on system activity
- **Quick Actions** - One-click access to common tasks

### 🛠️ **Advanced Features**
- **Custom Error Pages** - Professional 404 and 500 error handling
- **Search & Filtering** - Advanced bug search and filtering capabilities
- **Pagination** - Efficient data pagination for large datasets
- **Data Validation** - Client-side and server-side validation
- **Security Features** - CSRF protection, XSS prevention, SQL injection protection

---

## 🚀 Technologies Used

### **Backend**
- **ASP.NET Core 8.0** - Modern web framework
- **Entity Framework Core** - ORM for database operations
- **ASP.NET Identity** - Authentication and authorization
- **SQL Server** - Primary database
- **C# 12** - Latest language features

### **Frontend**
- **Razor Pages/MVC** - Server-side rendering
- **Bootstrap 5.3** - Responsive CSS framework
- **Font Awesome 6.4** - Professional icon library
- **Custom CSS** - Modern design system with CSS variables
- **JavaScript ES6+** - Interactive functionality

### **Architecture**
- **Clean Architecture** - Separation of concerns
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Built-in ASP.NET Core DI
- **MVC Areas** - Organized code structure
- **ViewModels** - Proper data transfer objects
- 
---

## 🛠️ Getting Started

### **Prerequisites**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/)
- [Git](https://git-scm.com/)

### **Installation**

1. **Clone the repository**
   ```bash
   git clone https://github.com/PPStefanov/BugTrackingSystem.git
   cd BugTrackingSystem
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string**
   ```json
   // appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BugTrackingSystemDb;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Build and run the application**
   ```bash
   dotnet build
   dotnet run --project BugTrackingSystem.Web
   ```

6. **Access the application**
   - Open your browser and navigate to `https://localhost:5001`
   - Register a new account or use seeded admin credentials

---

## 📱 Screenshots

### Modern Dashboard
![Dashboard](docs/images/dashboard.png)
*Interactive dashboard with statistics cards and recent activity*

### Collapsible Sidebar
![Sidebar](docs/images/sidebar.png)
*Professional sidebar navigation with smooth animations*

### Bug Management
![Bug Management](docs/images/bug-management.png)
*Comprehensive bug tracking interface*

### Mobile Responsive
![Mobile](docs/images/mobile.png)
*Fully responsive design for all devices*

---

## 🏗️ Project Structure

```
BugTrackingSystem/
├── BugTrackingSystem.Data/          # Data access layer
├── BugTrackingSystem.Models/        # Entity models
├── BugTrackingSystem.Services.Core/ # Business logic
├── BugTrackingSystem.ViewModels/    # View models
├── BugTrackingSystem.Web/           # Web application
│   ├── Areas/                       # MVC Areas
│   ├── Controllers/                 # MVC Controllers
│   ├── Views/                       # Razor views
│   ├── wwwroot/                     # Static files
│   └── Program.cs                   # Application entry point
├── BugTrackingSystem.Tests/         # Unit tests
└── README.md
```

---

## 🔧 Configuration

### **Database Configuration**
The application uses Entity Framework Core with SQL Server. Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your-Connection-String-Here"
  }
}
```

### **Identity Configuration**
User roles and permissions are configured in `Program.cs`. Default roles include:
- **Administrator** - Full system access
- **User** - Standard bug reporting and viewing

### **Seeded Data**
The application includes data seeding for:
- Default user roles
- Sample applications
- Bug priorities and statuses
- Admin user account

---

## 🧪 Testing

Run the test suite:
```bash
dotnet test
```

The project includes:
- Unit tests for business logic
- Integration tests for controllers
- Service layer testing

---

## 🚀 Deployment

### **Production Deployment**
1. Update `appsettings.Production.json` with production settings
2. Set environment variables for sensitive data
3. Configure HTTPS certificates
4. Deploy to your preferred hosting platform (Azure, AWS, etc.)

### **Docker Support**
```dockerfile
# Dockerfile included for containerized deployment
docker build -t bug-tracking-system .
docker run -p 8080:80 bug-tracking-system
```

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Petar Stefanov**
- GitHub: [@PPStefanov](https://github.com/PPStefanov)
- LinkedIn: [Petar Stefanov](https://linkedin.com/in/petar-stefanov)

---

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- Bootstrap team for the responsive CSS framework
- Font Awesome for the professional icons
- Texcycle.bg for design inspiration

---

## 📊 Project Stats

- **Lines of Code**: 15,000+
- **Files**: 120+
- **Features**: 25+
- **Test Coverage**: 65%+
- **Performance**: A+ Grade

---

*Built with ❤️ using ASP.NET Core 8.0*
