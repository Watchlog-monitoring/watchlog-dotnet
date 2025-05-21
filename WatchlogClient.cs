using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WatchlogMetric
{
    public class WatchlogClient
    {
        private static readonly HttpClient _client = new HttpClient();
        private const string AgentUrl = "http://localhost:3774";

        private void SendMetric(string method, string metric, double value = 1)
        {
            if (double.IsNaN(value)) return;

            var uri = $"{AgentUrl}?method={method}&metric={Uri.EscapeDataString(metric)}&value={value}";

            Task.Run(async () =>
            {
                try
                {
                    await _client.GetAsync(uri);
                }
                catch
                {
                    // Silent failure
                }
            });
        }

        public void Increment(string metric, double value = 1) => SendMetric("increment", metric, value);
        public void Decrement(string metric, double value = 1) => SendMetric("decrement", metric, value);
        public void Gauge(string metric, double value) => SendMetric("gauge", metric, value);
        public void Percentage(string metric, double value)
        {
            if (value >= 0 && value <= 100)
                SendMetric("percentage", metric, value);
        }
        public void SystemByte(string metric, double value) => SendMetric("systembyte", metric, value);
    }
}
