using System;
using System.Collections.Generic;
using System.Text;
using JohnUtilities.Services.Interfaces;

namespace JohnUtilities.Services.Adapters
{
    public class NNS_DateTime : INNS_DateTime
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
