using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetBuddy.Models
{
    [Flags]
    public enum DataType
    {
        None = 0,
        Transactions = 1,
        Transfers = 2,
        Categories = 4,
        All = Transactions | Transfers | Categories
    }
}
