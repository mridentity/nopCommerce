using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;
using AspNet.AspNetCore.Authentication.ReadyConnect;

namespace Nop.Plugin.ExternalAuth.ReadyConnect.Infrastructure
{
    /// <summary>
    /// Represents registrar of ReadyConnect authentication service
    /// </summary>
    public class ReadyConnectAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddReadyConnect(ReadyConnectDefaults.AuthenticationScheme, options =>
            {
                //set credentials
                var settings = EngineContext.Current.Resolve<ReadyConnectExternalAuthSettings>();
                options.ClientId = settings.ClientKeyIdentifier;
                options.ClientSecret = settings.ClientSecret;

                //store access and refresh tokens for the further usage
                options.SaveTokens = true;

                options.Scope.Add("rso_rid");   // rso_idp was the default RSO scope added prior to here in the ReadyConnect handler, if we don't remove it then both rso_idp and rso_rid will be supported.

                options.Scope.Add("email");
                options.Scope.Add("profile");
                options.Scope.Add("roles");
                options.Scope.Add("offline_access");


                //set custom events handlers
                options.Events = new ReadyConnectEvents
                {
                    //in case of error, redirect the user to the specified URL
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();

                        var errorUrl = context.Properties.GetString(ReadyConnectAuthenticationDefaults.ErrorCallback);
                        context.Response.Redirect(errorUrl);

                        return Task.FromResult(0);
                    }
                };
            });
        }
    }
}