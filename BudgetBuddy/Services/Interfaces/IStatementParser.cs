using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BudgetBuddy.Services.Interfaces
{
    public interface IStatementParser
    {
        DataTable Parse(string filePath);
    }
}