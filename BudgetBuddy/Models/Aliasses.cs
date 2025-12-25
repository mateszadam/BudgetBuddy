using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetBuddy.Classes
{

        public class Aliasess
        {
            public string Type { get; set; } = string.Empty;
            public string[] Places { get; set; } = Array.Empty<string>();

            public Aliasess(string t, string[] p)
            {
                Type = t;
                Places = p ?? Array.Empty<string>();
            }
            public Aliasess()
            {
            }
        }
    
}
