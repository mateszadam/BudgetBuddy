using BudgetBuddy.Services.Interfaces;
using ExcelDataReader;
using System.Data;
using System.IO;

namespace BudgetBuddy.Services
{
    public class ExcelStatementParser : IStatementParser
    {
        public ExcelStatementParser()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public DataTable Parse(string filePath)
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var result = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration { UseHeaderRow = false }
            });

            return result.Tables[0];
        }
    }
}