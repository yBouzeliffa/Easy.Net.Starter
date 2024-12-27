# Easy.Net.Starter

**Easy.Net.Starter** is a .NET library designed to accelerate the setup of **Console** or **API** projects. It provides a ready-to-use configuration with:

- **Integrated logging** using **Serilog**.
- Centralized management of **settings** (`appsettings.json`) for different environments.
- Automatic setup of **Dependency Injection (DI)**.
- **Database support** (PostgreSQL with SnakeCase conventions).
- Quick configuration for **CORS**, middlewares, and API capabilities (Swagger, health checks, etc.).

---

## Key Features

1. **Logging with Serilog**:  
   Automatically logs to the console or files.

2. **Settings management**:  
   Loads settings from `appsettings.json` and exposes them via DI.

3. **PostgreSQL support**:  
   Easily add an Entity Framework context with default configuration.

4. **API Ready**:  
   Enable API capabilities (Swagger, controllers, health endpoints) with a single line.

5. **CORS**:  
   Simplified configuration for allowed origins, methods, and headers.

6. **Middleware management**:  
   Dynamically add custom middlewares using reflection.

---

## Installation

Add **Easy.Net.Starter** to your project via NuGet:

```bash
dotnet add package Easy.Net.Starter
```

---

## Usage Examples

### Initializing an API

```csharp
using Easy.Net.Starter.Api;
using Easy.Net.Starter.App;
using Easy.Net.Starter.ProgramsEntries;

var startupOptions = new StartupBuilder()
    .AddScoped<ITestServiceWithInterface, TestServiceWithInterface>()
    .AddHttpLoggerMiddleware()
    .Build();

var app = WebApiStarter.Start<CustomAppSettings>(args, startupOptions);

app.Run();
```

### Initializing with a Database

```csharp
using Easy.Net.Starter.Api;
using Easy.Net.Starter.App;
using Easy.Net.Starter.ProgramsEntries;

var startupOptions = new StartupBuilder()
    .AddDatabase<MyDatabaseContext>()
    .AddScoped<ITestServiceWithInterface, TestServiceWithInterface>()
    .AddHttpLoggerMiddleware()
    .Build();

var app = WebApiStarter.Start<CustomAppSettings>(args, startupOptions);

app.Run();
```

---

## Contribution

Contributions are welcome! If you’d like to suggest an improvement or report a bug, please open an issue or submit a Pull Request.

---

## License

This project is licensed under the [MIT](LICENSE).

---