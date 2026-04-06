namespace LuyenTap
{
    public class LoanInterestResult
    {
        public decimal InterestRate { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal TotalPayable { get; set; }
    }

    public class TinhLaiSuat
    {
        public bool TryCalculate(decimal loanAmount, int loanTermMonths, out LoanInterestResult? result, out string errorMessage)
        {
            result = null;
            errorMessage = string.Empty;

            if (loanAmount <= 0)
            {
                errorMessage = "Loan amount must be greater than 0.";
                return false;
            }

            if (loanTermMonths <= 0)
            {
                errorMessage = "Loan term must be greater than 0 month.";
                return false;
            }

            var yearlyRate = GetInterestRateByTerm(loanTermMonths);
            var interestAmount = loanAmount * yearlyRate * loanTermMonths / 12m;

            result = new LoanInterestResult
            {
                InterestRate = yearlyRate,
                InterestAmount = Math.Round(interestAmount, 2),
                TotalPayable = Math.Round(loanAmount + interestAmount, 2)
            };

            return true;
        }

        private static decimal GetInterestRateByTerm(int loanTermMonths)
        {
            if (loanTermMonths <= 12)
            {
                return 0.08m;
            }

            if (loanTermMonths <= 36)
            {
                return 0.10m;
            }

            if (loanTermMonths <= 60)
            {
                return 0.12m;
            }

            return 0.135m;
        }
    }
}
