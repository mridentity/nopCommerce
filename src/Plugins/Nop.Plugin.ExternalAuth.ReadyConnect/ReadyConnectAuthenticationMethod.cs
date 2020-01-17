using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.ExternalAuth.ReadyConnect
{
    public class ReadyConnectAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public ReadyConnectAuthenticationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ReadyConnectAuthentication/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return ReadyConnectAuthenticationDefaults.VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new ReadyConnectExternalAuthSettings()
            {
                UseSandbox = true
            };

            _settingService.SaveSetting(settings);

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.ReadyConnect.ClientKeyIdentifier", "App ID/API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.ReadyConnect.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your ReadyConnect application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.ReadyConnect.ClientSecret", "App Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.ReadyConnect.ClientSecret.Hint", "Enter your app secret here. You can find it on your ReadyConnect application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.ReadyConnect.Instructions", "<p>To configure authentication with ReadyConnect, please follow these steps:<br/><br/><ol><li>Navigate to the <a href=\"https://members.ReadyConnect.com/\" target =\"_blank\" > ReadyMembers. If you don't already have a ReadyMembers account please create one. Then create a new confidential application with required information. Copy your App ID and App secret below.</p>");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ReadyConnectExternalAuthSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.ReadyConnect.ClientKeyIdentifier");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.ReadyConnect.ClientKeyIdentifier.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.ReadyConnect.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.ReadyConnect.ClientSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.ReadyConnect.Instructions");

            base.Uninstall();
        }

        #endregion    
    }
}