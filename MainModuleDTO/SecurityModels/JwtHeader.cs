using System;
using System.Collections.Generic;
using System.Text;

namespace MainModuleDTO.SecurityModels
{
    public class JwtHeader
    {
        // ReSharper disable once InconsistentNaming
        public string typ { get; set; }
        // ReSharper disable once InconsistentNaming
        public string alg { get; set; }
    }
}
