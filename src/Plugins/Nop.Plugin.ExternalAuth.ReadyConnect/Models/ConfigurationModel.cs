using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.ExternalAuth.ReadyConnect.Models
{
    /// <summary>
    /// Represents plugin configuration model
    /// </summary>
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExternalAuth.ReadyConnect.UseSandbox")]
        public bool UseSandbox { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.ReadyConnect.ClientId")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.ReadyConnect.ClientSecret")]
        public string ClientSecret { get; set; }

		[NopResourceDisplayName("Plugins.ExternalAuth.ReadyConnect.RedirectUri")]
		public string RedirectUrl { get; set; }
    }
}