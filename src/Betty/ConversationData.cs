using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Betty
{
    /// <summary>
    /// Conversation data for Betty.
    /// </summary>
    public class ConversationData
    {
        /// <summary>
        /// Gets or sets a flag indicating the customer has talked to Betty before.
        /// </summary>
        /// <value>True when the customer talked to Betty before in the same conversation; Otherwise false.</value>
        public bool ReturningCustomer { get; set; } = false;
    }
}
