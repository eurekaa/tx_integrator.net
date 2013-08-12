using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ultrapulito.Jarvix.Core {

    public class GeocodingException : Exception {
        
        public GeocodingException():base() { }

        public GeocodingException(string message) : base(message) { }

    }
}
