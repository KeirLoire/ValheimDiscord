﻿using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ValheimDiscord.Model;

namespace ValheimDiscord.Discord
{
    public sealed class MonoRestClient : IRestClient, IDisposable
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        private CancellationToken _cancelToken;
        private bool _isDisposed;

        public MonoRestClient(string baseUrl)
        {
            _baseUrl = baseUrl;

            _client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false
            });

            SetHeader("accept-encoding", "gzip, deflate");

            _cancelToken = CancellationToken.None;
        }

        public void SetHeader(string key, string value)
        {
            _client.DefaultRequestHeaders.Remove(key);

            if (value != null)
                _client.DefaultRequestHeaders.Add(key, value);
        }

        public void SetCancelToken(CancellationToken cancelToken)
        {
            _cancelToken = cancelToken;
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken cancelToken, bool headerOnly, string reason = null,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null)
        {
            string uri = Path.Combine(_baseUrl, endpoint);
            using (var restRequest = new HttpRequestMessage(GetMethod(method), uri))
            {
                if (reason != null)
                    restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
                if (requestHeaders != null)
                    foreach (var header in requestHeaders)
                        restRequest.Headers.Add(header.Key, header.Value);
                return await SendInternalAsync(restRequest, cancelToken, headerOnly).ConfigureAwait(false);
            }
        }
        public async Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken cancelToken, bool headerOnly, string reason = null,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null)
        {
            string uri = Path.Combine(_baseUrl, endpoint);
            using (var restRequest = new HttpRequestMessage(GetMethod(method), uri))
            {
                if (reason != null)
                    restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
                if (requestHeaders != null)
                    foreach (var header in requestHeaders)
                        restRequest.Headers.Add(header.Key, header.Value);
                restRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
                return await SendInternalAsync(restRequest, cancelToken, headerOnly).ConfigureAwait(false);
            }
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, CancellationToken cancelToken, bool headerOnly, string reason = null,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null)
        {
            var uri = Path.Combine(_baseUrl, endpoint);
            #pragma warning disable IDISP004
            var restRequest = new HttpRequestMessage(GetMethod(method), uri);
            #pragma warning restore IDISP004

            if (reason != null)
                restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
            if (requestHeaders != null)
                foreach (var header in requestHeaders)
                    restRequest.Headers.Add(header.Key, header.Value);
            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));

            foreach (var p in multipartParams ?? ImmutableDictionary<string, object>.Empty)
            {
                switch (p.Value)
                {
                    #pragma warning disable IDISP004
                    case string stringValue:
                        { content.Add(new StringContent(stringValue, Encoding.UTF8, "text/plain"), p.Key); continue; }
                    case byte[] byteArrayValue:
                        { content.Add(new ByteArrayContent(byteArrayValue), p.Key); continue; }
                    case Stream streamValue:
                        { content.Add(GetStreamContent(streamValue), p.Key); continue; }
                    case MultipartFile fileValue:
                        {
                            var streamContent = GetStreamContent(fileValue.Stream);

                            if (fileValue.ContentType != null)
                                streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileValue.ContentType);

                            content.Add(streamContent, p.Key, fileValue.Filename);
                    #pragma warning restore IDISP004

                            continue;
                        }
                    default:
                        throw new InvalidOperationException($"Unsupported param type \"{p.Value.GetType().Name}\".");
                }
            }

            restRequest.Content = content;
            return SendInternalAsync(restRequest, cancelToken, headerOnly);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    _client.Dispose();
                _isDisposed = true;
            }
        }
        private async Task<RestResponse> SendInternalAsync(HttpRequestMessage request, CancellationToken cancelToken, bool headerOnly)
        {
            using (var cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancelToken, cancelToken))
            {
                cancelToken = cancelTokenSource.Token;
                HttpResponseMessage response = await _client.SendAsync(request, cancelToken).ConfigureAwait(false);

                var headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault(), StringComparer.OrdinalIgnoreCase);
                var stream = (!headerOnly || !response.IsSuccessStatusCode) ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false) : null;

                return new RestResponse(response.StatusCode, headers, stream);
            }
        }

        private static readonly HttpMethod Patch = new HttpMethod("PATCH");
        private HttpMethod GetMethod(string method)
        {
            switch (method)
            {
                case "DELETE":
                    return HttpMethod.Delete;
                case "GET":
                    return HttpMethod.Get;
                case "PATCH":
                    return Patch;
                case "POST":
                    return HttpMethod.Post;
                case "PUT":
                    return HttpMethod.Put;
            };

            throw new ArgumentOutOfRangeException(nameof(method), $"Unknown HttpMethod: {method}");
        }

        static StreamContent GetStreamContent(Stream stream)
        {
            if (stream.CanSeek)
                stream.Position = 0;

            #pragma warning disable IDISP004
            return new StreamContent(stream);
            #pragma warning restore IDISP004
        }
    }
}
