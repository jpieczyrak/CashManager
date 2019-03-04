using System;
using System.IO;
using System.Net;

using log4net;

namespace CashManager.Utils
{
    public class WebReader
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(WebReader)));

        public string Read(string url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(url))
                    {
                        if (stream != null)
                        {
                            var reader = new StreamReader(stream);
                            string content = reader.ReadToEnd();
                            return content;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Value.Info($"Could not read from url: {url}", e);
            }

            return string.Empty;
        }
    }
}