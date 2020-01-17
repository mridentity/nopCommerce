using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.ReadyConnect
{
    /// <summary>
    /// Represents settings of the Facebook authentication method
    /// </summary>
    public class ReadyConnectExternalAuthSettings : ISettings
    {
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// Gets or sets OAuth2 client identifier
        /// </summary>
        public string ClientKeyIdentifier { get; set; }

        /// <summary>
        /// Gets or sets OAuth2 client secret
        /// </summary>
        public string ClientSecret { get; set; }
    }
}