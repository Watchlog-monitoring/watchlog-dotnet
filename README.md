# Watchlog.Metric (.NET Client)

🔗 **Website**: [https://watchlog.io](https://watchlog.io)

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

## 🐳 Docker Setup

When running your .NET app in Docker, you can specify the agent URL explicitly:

```csharp
using WatchlogMetric;

// Create client with explicit agent URL for Docker
var watchlog = new WatchlogClient("http://watchlog-agent:3774");

watchlog.Increment("user_login", 1);
```

**Docker Compose Example:**
```yaml
version: '3.8'

services:
  watchlog-agent:
    image: watchlog/agent:latest
    container_name: watchlog-agent
    ports:
      - "3774:3774"
    environment:
      - WATCHLOG_APIKEY=your-api-key
      - WATCHLOG_SERVER=https://log.watchlog.ir
    networks:
      - app-network

  dotnet-app:
    build: .
    container_name: dotnet-app
    ports:
      - "5000:5000"
    depends_on:
      - watchlog-agent
    networks:
      - app-network

networks:
  app-network:
    driver: bridge
```

**Docker Run Example:**
```bash
# 1. Create network
docker network create app-network

# 2. Run Watchlog Agent
docker run -d \
  --name watchlog-agent \
  --network app-network \
  -p 3774:3774 \
  -e WATCHLOG_APIKEY="your-api-key" \
  -e WATCHLOG_SERVER="https://log.watchlog.ir" \
  watchlog/agent:latest

# 3. Run .NET app (make sure your code uses new WatchlogClient("http://watchlog-agent:3774"))
docker run -d \
  --name dotnet-app \
  --network app-network \
  -p 5000:5000 \
  my-dotnet-app
```

## 🔍 Environment Detection

The package automatically detects the runtime environment:

* **Local / non-K8s**: `http://127.0.0.1:3774`
* **Kubernetes**: `http://watchlog-node-agent.monitoring.svc.cluster.local:3774`

**Manual Override:** You can override the endpoint by passing `agentUrl` parameter to the constructor:

```csharp
var watchlog = new WatchlogClient("http://watchlog-agent:3774"); // Custom agent URL
```

**Important Notes:**
- When using Docker, use the container name as the hostname (e.g., `watchlog-agent`)
- Both containers must be on the same Docker network
- The agent must be running before your app starts
- If `agentUrl` is not provided, auto-detection will be used (local or Kubernetes)

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