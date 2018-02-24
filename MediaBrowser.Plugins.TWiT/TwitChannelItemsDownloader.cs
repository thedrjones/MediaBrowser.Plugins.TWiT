using System;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using System.IO;
using System.Text;

namespace MediaBrowser.Plugins.TWiT
{
    public class TwitChannelItemsDownloader
    {
        private ILogger _logger;
        private readonly IHttpClient _httpClient;
        private readonly IXmlSerializer _xmlSerializer;

        public TwitChannelItemsDownloader(ILogger logManager, IXmlSerializer xmlSerializer, IHttpClient httpClient)
        {
            _logger = logManager;
            _xmlSerializer = xmlSerializer;
            _httpClient = httpClient;
        }

        public async Task<rss> GetStreamList(String queryUrl, int offset, CancellationToken cancellationToken)
        {
            using (var response = await _httpClient.GetResponse(new HttpRequestOptions
            {

                CancellationToken = cancellationToken,
                Url = queryUrl,
                EnableDefaultUserAgent = true,
                EnableHttpCompression = false,
                BufferContent = false,
                EnableKeepAlive = false

            }).ConfigureAwait(false))
            {
                using (var xml = response.Content)
                {
                    _logger.Info("Reading TWiT response with StreamReader");

                    using (var reader = new StreamReader(xml))
                    {
                        var str = reader.ReadToEnd();

                        _logger.Info("Deserializing TwiT response");

                        rss result = _xmlSerializer.DeserializeFromBytes(typeof(rss), Encoding.UTF8.GetBytes(str)) as rss;
                        _logger.Info("Deserialized TwiT response");
                        return result;
                    }
                }
            }
        }

    }
}
