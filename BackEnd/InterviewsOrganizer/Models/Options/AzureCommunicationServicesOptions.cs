namespace InterviewsOrganizer.Models.Options
{
    /// <summary>
    /// Strongly-typed configuration for Azure Communication Services Email.
    /// Bound from the "AzureCommunicationServices" configuration section.
    /// </summary>
    public class AzureCommunicationServicesOptions
    {
        public const string SectionName = "AzureCommunicationServices";

        /// <summary>
        /// ACS resource connection string. Must be supplied via user-secrets/environment
        /// variables/Key Vault in real environments, never committed to source control.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// The verified sender address, typically the Azure Managed Domain,
        /// e.g. DoNotReply@&lt;GUID&gt;.azurecomm.net
        /// </summary>
        public string SenderAddress { get; set; } = string.Empty;

        /// <summary>
        /// Base URL of the client application, used to build links (e.g. unsubscribe) in email templates.
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;
    }
}
