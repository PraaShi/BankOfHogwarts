using BankOfHogwarts.Models.Enums;
namespace BankOfHogwarts.Models
{
    public class AdminDashboardReport
    {
        public int EmployeeCount { get; set; }
        public int CustomerCount { get; set; }
        public int MaleCustomerCount { get; set; }
        public int FemaleCustomerCount { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
        public decimal TotalLoansDisbursed { get; set; }
        public decimal InterestIncome { get; set; }
        public int ActiveAccountsCount { get; set; }
        public decimal AverageAccountBalance { get; set; }
        public double LoanApprovalRate { get; set; }
        public string TopBranch { get; set; }
        public List<TopCustomer> TopCustomers { get; set; }
        public int OverdueLoansCount { get; set; }
        public decimal AverageLoanAmount { get; set; }
        public Dictionary<TransactionType, int> TransactionVolumeByType { get; set; }
        public int NewCustomersCount { get; set; }
        public double LoansToDepositsRatio { get; set; }
    }

    public class TopCustomer
    {
        public int CustomerId { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
