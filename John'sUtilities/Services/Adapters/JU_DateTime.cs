using System;
using System.Collections.Generic;
using System.Text;
using JohnUtilities.Services.Interfaces;

namespace JohnUtilities.Services.Adapters
{
    public class JU_DateTime : IJU_DateTime
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
