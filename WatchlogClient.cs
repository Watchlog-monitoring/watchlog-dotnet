using System;
using System.Net;
using System.Threading;

namespace WatchlogMetric
{
    public class WatchlogClient
    {
        private static readonly string AgentUrl;

        static WatchlogClient()
        {
            // اگر داخل کوبرنیتیز باشیم، KUBERNETES_SERVICE_HOST ست می‌شود
            var isKubernetes = !string.IsNullOrEmpty(
                Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST")
            );
            
            AgentUrl = isKubernetes
                ? "http://watchlog-node-agent:3774"
                : "http://127.0.0.1:3774";
        }

        private void SendMetric(string method, string metric, double value = 1)
        {
            if (double.IsNaN(value)) return;

            var uri = $"{AgentUrl}"
                    + $"?method={Uri.EscapeDataString(method)}"
                    + $"&metric={Uri.EscapeDataString(metric)}"
                    + $"&value={value}";

            new Thread(() =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Method = "GET";
                    request.Timeout = 1000; // 1 ثانیه

                    using var response = request.GetResponse(); // پاسخ را نادیده می‌گیریم
                }
                catch
                {
                    // خطاها را بی‌صدا نادیده بگیر
                }
            })
            { IsBackground = true }
            .Start();
        }

        public void Increment(string metric, double value = 1) 
            => SendMetric("increment", metric, value);

        public void Decrement(string metric, double value = 1) 
            => SendMetric("decrement", metric, value);

        public void Gauge(string metric, double value) 
            => SendMetric("gauge", metric, value);

        public void Percentage(string metric, double value)
        {
            if (value >= 0 && value <= 100)
                SendMetric("percentage", metric, value);
        }

        public void SystemByte(string metric, double value) 
            => SendMetric("systembyte", metric, value);
    }
}
