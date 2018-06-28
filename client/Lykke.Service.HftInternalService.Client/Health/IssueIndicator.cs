using JetBrains.Annotations;

namespace Lykke.Service.HftInternalService.Client.Health
{
    /// <summary>
    /// Issue description.
    /// </summary>
    [PublicAPI]
    public class IssueIndicator
    {
        /// <summary>
        /// Gets or sets the issue type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the issue reason, message or value.
        /// </summary>
        public string Value { get; set; }
    }
}