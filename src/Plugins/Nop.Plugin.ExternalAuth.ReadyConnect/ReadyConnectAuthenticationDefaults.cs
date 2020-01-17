namespace Nop.Plugin.ExternalAuth.ReadyConnect
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class ReadyConnectAuthenticationDefaults
    {
        /// <summary>
        /// Gets a name of the view component to display login button
        /// </summary>
        public const string VIEW_COMPONENT_NAME = "ReadyConnectAuthentication";

        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName = "ExternalAuth.ReadyConnect";

        /// <summary>
        /// Gets a name of error callback method
        /// </summary>
        public static string ErrorCallback = "ErrorCallback";
    }
}