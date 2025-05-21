# Watchlog.Metric (.NET Client)

**Watchlog.Metric** is a lightweight and non-blocking .NET client for sending custom metrics to your [Watchlog](https://watchlog.io/) server.

## 📦 Installation

Install the package via NuGet:

```bash
dotnet add package Watchlog.Metric
```

---

## 🚀 Quick Start

### 1. Import the namespace

```csharp
using WatchlogMetric;
```

### 2. Create an instance of the Watchlog client

```csharp
var watchlog = new WatchlogClient(); // No configuration needed
```

### 3. Send metrics

```csharp
watchlog.Increment("user_login", 1);
watchlog.Decrement("cart_items", 2);
watchlog.Gauge("active_users", 120);
watchlog.Percentage("disk_usage", 73.5);
watchlog.SystemByte("memory_usage", 1048576);
```

> All operations are **asynchronous**, **non-blocking**, and **fail-safe** — they never throw exceptions or print logs.

---

## 🛡️ Features

- ✔️ Fully async-safe using `Task.Run`
- ✔️ No logs, no exceptions
- ✔️ Tiny footprint, pure .NET
- ✔️ Works in ASP.NET Core, background services, or console apps

---

## 💡 Example: ASP.NET Core Integration

```csharp
public class HomeController : Controller
{
    private readonly WatchlogClient _watchlog = new WatchlogClient();

    public IActionResult Index()
    {
        _watchlog.Increment("homepage_visits");
        return View();
    }
}
```

---

## 📄 License

MIT License