using Discord.Net.Rest;
using System;
using System.Net;

namespace ValheimDiscord.Discord
{
    public static class MonoRestClientProvider
    {
        public static readonly RestClientProvider Instance = Create();

        public static RestClientProvider Create(bool useProxy = false, IWebProxy webProxy = null)
        {
            return url =>
            {
                try
                {
                    return new MonoRestClient(url);
                }
                catch (PlatformNotSupportedException ex)
                {
                    throw new PlatformNotSupportedException("The default RestClientProvider is not supported on this platform.", ex);
                }
            };
        }
    }
}
