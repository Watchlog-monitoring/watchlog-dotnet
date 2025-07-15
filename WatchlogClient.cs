using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace WatchlogMetric
{
    public class WatchlogClient
    {
        private static readonly string AgentUrl;
        private static readonly HttpClient HttpClient;

        static WatchlogClient()
        {
            AgentUrl = DetermineServerUrl();
            HttpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(1)
                // HttpClient reuses connections by default (keep-alive enabled)
            };
        }

        private static bool IsRunningInK8s()
        {
            // Method 1: ServiceAccount Token
            if (File.Exists("/var/run/secrets/kubernetes.io/serviceaccount/token"))
                return true;

            // Method 2: cgroup
            try
            {
                var content = File.ReadAllText("/proc/1/cgroup");
                if (content.Contains("kubepods"))
                    return true;
            }
            catch
            {
                // ignore
            }

            // Method 3: DNS lookup
            try
            {
                Dns.GetHostEntry("kubernetes.default.svc.cluster.local");
                return true;
            }
            catch
            {
                // ignore
            }

            return false;
        }

        private static string DetermineServerUrl()
        {
            return IsRunningInK8s()
                ? "http://watchlog-node-agent.monitoring.svc.cluster.local:3774"
                : "http://127.0.0.1:3774";
        }

        private void SendMetric(string method, string metric, double value = 1)
        {
            if (double.IsNaN(value) || string.IsNullOrEmpty(metric))
                return;

            var uri = $"{AgentUrl}"
                    + $"?method={WebUtility.UrlEncode(method)}"
                    + $"&metric={WebUtility.UrlEncode(metric)}"
                    + $"&value={value}";

            // fire-and-forget; any errors are silently ignored
            _ = HttpClient.GetAsync(uri);
        }

        public void Increment(string metric, double value = 1)
        {
            if (value > 0)
                SendMetric("increment", metric, value);
        }

        public void Decrement(string metric, double value = 1)
        {
            if (value > 0)
                SendMetric("decrement", metric, value);
        }

        public void Gauge(string metric, double value)
        {
            SendMetric("gauge", metric, value);
        }

        public void Percentage(string metric, double value)
        {
            if (value >= 0 && value <= 100)
                SendMetric("percentage", metric, value);
        }

        public void SystemByte(string metric, double value)
        {
            if (value > 0)
                SendMetric("systembyte", metric, value);
        }
    }
}
