using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Betty
{
    /// <summary>
    /// Use this to determine the weight of the passenger's luggage.
    /// </summary>
    public interface ILuggageScale
    {
        /// <summary>
        /// Gets the weight of the luggage currently on the conveyer belt.
        /// </summary>
        /// <returns>Returns the weight of the luggage.</returns>
        Task<double> GetWeight();
    }
}
