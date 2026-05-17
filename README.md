# BudgetBuddy 💰

BudgetBuddy is a WPF (Windows Presentation Foundation) desktop application designed to help you track your personal finances. It allows users to import bank transaction history, categorize expenses, and visualize spending habits through an intuitive dashboard.

## 🚀 Features

- **Transaction Import**: Easily import bank statements from Excel files (`.xls`, `.xlsx`) using `ExcelDataReader`. The app parses transactions and transfers automatically.
- **Smart Categorization**: Transactions are automatically categorized based on saved aliases and location data.
- **Dashboard & Analytics**:
  - Visualize spending with charts.
  - Filter data by period (Week/Month) and specific dates.
  - View total spending summaries and breakdowns.
- **Data Persistence**: Automatically saves your transaction, transfer, and category data locally to JSON files (`KoltegData.json`, `TransferData.json`) in your "My Documents" folder.
- **Income Tracking**: Dedicated support for tracking incoming transfers (`Transfers`) separate from daily expenses (`Transactions`).

## 🛠️ Technologies Used

- **Framework**: .NET 10.0 (Windows)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Language**: C#
- **Data Format**: JSON (System.Text.Json)
- **Libraries**:
  - `ExcelDataReader` (v3.8.0) - For parsing Excel bank exports.

## 📖 Usage

1.  **Load Data**: Use the upload feature to select your bank's Excel export file. The application will process standard transaction rows.
2.  **Automatic Saving**: Your data is automatically saved to `KoltegData.json` and `TransferData.json` in your Documents folder, so it persists between sessions.
3.  **Categorization**: As you modify categories for specific places, the app learns and applies these to future transactions.
