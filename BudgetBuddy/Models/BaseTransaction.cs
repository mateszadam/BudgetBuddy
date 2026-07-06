using System;

namespace BudgetBuddy.Models
{
    public abstract class BaseTransaction
    {
        public Guid Id { get; init ;  } = Guid.NewGuid();
        public int Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string FormattedDate => Date.ToString("yyyy. MM. dd.");

        public override bool Equals(object? obj)
        {
            if (obj is BaseTransaction other)
            {
                return Description == other.Description;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Description.GetHashCode();
        }

        public static bool operator ==(BaseTransaction? left, BaseTransaction? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(BaseTransaction? left, BaseTransaction? right)
        {
            return !(left == right);
        }
    }
}