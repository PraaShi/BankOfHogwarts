namespace BankOfHogwarts.Models.Enums
{
    public enum AccountStatus
    {
        Active,
        Inactive,
        PendingApproval,  // Add this status for pending accounts
        OnHold,
        Closed
    }
}
