using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.AspNetCore.Authentication.ReadyConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Plugin.ExternalAuth.ReadyConnect.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.ExternalAuth.ReadyConnect.Controllers
{
    public class ReadyConnectAuthenticationController : BasePluginController
    {
        #region Fields

        private readonly ReadyConnectExternalAuthSettings _readyConnectExternalAuthSettings;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOptionsMonitorCache<ReadyConnectOptions> _optionsCache;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ReadyConnectAuthenticationController(ReadyConnectExternalAuthSettings readyConnectExternalAuthSettings,
            IAuthenticationPluginManager authenticationPluginManager,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOptionsMonitorCache<ReadyConnectOptions> optionsCache,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _readyConnectExternalAuthSettings = readyConnectExternalAuthSettings;
            _authenticationPluginManager = authenticationPluginManager;
            _externalAuthenticationService = externalAuthenticationService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _optionsCache = optionsCache;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion        
        
        #region Methods
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ClientId = _readyConnectExternalAuthSettings.ClientKeyIdentifier,
                ClientSecret = _readyConnectExternalAuthSettings.ClientSecret
            };

            return View("~/Plugins/ExternalAuth.ReadyConnect/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _readyConnectExternalAuthSettings.ClientKeyIdentifier = model.ClientId;
            _readyConnectExternalAuthSettings.ClientSecret = model.ClientSecret;
            _settingService.SaveSetting(_readyConnectExternalAuthSettings);

            //clear ReadyConnect authentication options cache
            _optionsCache.TryRemove(ReadyConnectDefaults.AuthenticationScheme);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public IActionResult Login(string returnUrl)
        {
            var methodIsAvailable = _authenticationPluginManager
                .IsPluginActive(ReadyConnectAuthenticationDefaults.SystemName, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
            if (!methodIsAvailable)
                throw new NopException("ReadyConnect authentication module cannot be loaded");

            if (string.IsNullOrEmpty(_readyConnectExternalAuthSettings.ClientKeyIdentifier) ||
                string.IsNullOrEmpty(_readyConnectExternalAuthSettings.ClientSecret))
            {
                throw new NopException("ReadyConnect authentication module not configured");
            }

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "ReadyConnectAuthentication", new { returnUrl = returnUrl })
            };
            authenticationProperties.SetString(ReadyConnectAuthenticationDefaults.ErrorCallback, Url.RouteUrl("Login", new { returnUrl }));

            return Challenge(authenticationProperties, ReadyConnectDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            //authenticate ReadyConnect user
            var authenticateResult = await HttpContext.AuthenticateAsync(ReadyConnectDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = ReadyConnectAuthenticationDefaults.SystemName,
                AccessToken = await HttpContext.GetTokenAsync(ReadyConnectDefaults.AuthenticationScheme, "access_token"),
                Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            //authenticate Nop user
            return _externalAuthenticationService.Authenticate(authenticationParameters, returnUrl);
        }

        #endregion
    }
}
