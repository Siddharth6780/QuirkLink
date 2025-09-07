# QuirkLink - Secure Link Sharing

A modern ASP.NET Core Web API application for generating, storing, and retrieving encrypted short links with optional QR code support and analytics.

## 🚀 Features

- **Secure Link Generation**: Create encrypted short links for any content
- **QR Code Support**: Automatic QR code generation for easy sharing
- **Flexible Expiration**: Choose from 5 minutes to 1 week expiration times
- **Modern UI**: Beautiful, responsive web interface
- **Redis Storage**: Fast, reliable data storage with Azure Redis Cache
- **Service Bus Integration**: Background processing for cleanup and analytics
- **RESTful API**: Clean API endpoints for programmatic access

## 🛠️ Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Redis Cache](https://azure.microsoft.com/services/cache/) (or local Redis instance)
- [Azure Service Bus](https://azure.microsoft.com/services/service-bus/) (optional, for background processing)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) (recommended)

## 📦 Installation

1. **Clone the repository:**
   ```bash
   git clone <your-repo-url>
   cd QuirkLink
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Configure application settings:**
   - Copy `appsettings.json` to `appsettings.Development.json`
   - Update connection strings and configuration values

## ⚙️ Configuration

Update `appsettings.json` with your Azure services configuration:

```json
{
  "QuirkLinkConfig": {
    "RedisConnectionString": "your-redis-connection-string",
    "ServiceBusConnectionString": "your-servicebus-connection-string",
    "CleanupQueueName": "cleanup-queue",
    "TrackingQueueName": "tracking-queue",
    "EncryptionKey": "your-32-character-encryption-key"
  }
}
```

## 🔨 Build Commands

### Development Build
```bash
# Build the solution
dotnet build QuirkLink.sln

# Build with specific configuration
dotnet build QuirkLink.sln --configuration Debug
```

### Release Build
```bash
# Build for production
dotnet build QuirkLink.sln --configuration Release

# Build and output to specific directory
dotnet build QuirkLink.sln --configuration Release --output ./publish
```

### Clean Build
```bash
# Clean previous builds
dotnet clean QuirkLink.sln

# Clean and rebuild
dotnet clean QuirkLink.sln && dotnet build QuirkLink.sln
```

## 🚀 Running the Application

### Development Server
```bash
# Run with hot reload (recommended for development)
dotnet run --project QuirkLink

# Run with specific environment
dotnet run --project QuirkLink --environment Development

# Run and watch for file changes
dotnet watch run --project QuirkLink
```

### Production Deployment
```bash
# Publish for deployment
dotnet publish QuirkLink --configuration Release --output ./publish

# Run published application
dotnet ./publish/QuirkLink.dll
```

## 📚 API Documentation

Once the application is running, visit:
- **Swagger UI**: `https://localhost:5001/swagger` (Development)
- **Web Interface**: `https://localhost:5001/` (Main UI)

### API Endpoints

#### Create QuirkLink
```http
POST /QuirkLink/link
Content-Type: application/json

{
  "content": "Your content here",
  "expireSeconds": 3600
}
```

#### Retrieve Content
```http
GET /QuirkLink/content/{token}
```

## 🎨 UI Features

The web interface provides:
- **Create Links**: User-friendly form with content input and expiration selection
- **Retrieve Content**: Token-based content retrieval
- **QR Codes**: Automatic generation and download
- **Copy Functions**: One-click copying of links and content
- **Responsive Design**: Works on desktop and mobile devices

### Expiration Options
- 5 Minutes
- 10 Minutes  
- 30 Minutes
- 1 Hour
- 6 Hours
- 12 Hours
- 24 Hours (default)
- 3 Days
- 1 Week

## 🏗️ Project Structure

```
QuirkLink/
├── Controllers/          # API Controllers
│   └── QuirkLinkController.cs
├── Models/              # Data models and DTOs
│   ├── QuirkLinkRequestModel.cs
│   ├── GenerateQuirkLinkResponseModel.cs
│   └── ...
├── Services/            # Business logic services
│   ├── Interfaces/      # Service contracts
│   ├── QuirkLinkService.cs
│   ├── AES256CryptoService.cs
│   ├── QrCodeService.cs
│   └── ...
├── wwwroot/            # Static web files
│   ├── css/
│   ├── js/
│   └── index.html
├── Properties/
│   └── launchSettings.json
├── appsettings.json
└── Program.cs
```

## 📋 Troubleshooting

### Common Issues

1. **Redis Connection Failed**
   - Verify Redis connection string in `appsettings.json`
   - Ensure Redis instance is running and accessible

2. **Service Bus Errors**
   - Check Service Bus connection string
   - Verify queue names exist in Azure Service Bus

3. **Build Errors**
   - Run `dotnet clean` followed by `dotnet restore`
   - Check .NET SDK version compatibility

4. **Port Already in Use**
   - Change port in `Properties/launchSettings.json`
   - Or kill existing process: `netstat -ano | findstr :5000`

### Logging

Application logs are available in:
- Console output (Development)
- Application Insights (Production, if configured)

## 🚀 Deployment Options

### Azure App Service
```bash
# Publish to Azure
dotnet publish --configuration Release
# Deploy using Azure CLI or Visual Studio
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
5. Submit a pull request

## 🔗 Useful Links

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Azure Redis Cache](https://azure.microsoft.com/services/cache/)
- [Azure Service Bus](https://azure.microsoft.com/services/service-bus/)
- [.NET 8.0 Documentation](https://docs.microsoft.com/dotnet)

---

**Happy Linking with QuirkLink! 🔗✨**
