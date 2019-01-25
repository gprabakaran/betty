using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Betty
{
    /// <summary>
    /// Use this to determine the weight of the passenger's luggage.
    /// </summary>
    public class LuggageScale : ILuggageScale
    {
        private Random _randomizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LuggageScale"/> class.
        /// </summary>
        public LuggageScale()
        {
            _randomizer = new Random(Environment.TickCount);
        }

        /// <summary>
        /// Gets the weight of the luggage currently on the conveyer belt.
        /// </summary>
        /// <returns>Returns the weight of the luggage.</returns>
        public Task<double> GetWeight()
        {
            return Task.FromResult(_randomizer.NextDouble() * 30);
        }
    }
}
