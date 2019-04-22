using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationPoperties { get; private set; }

        public bool Revert { get; private set; }

        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            DestinationPoperties = destinationProperties;
            Revert = revert;
        }
    }
}
