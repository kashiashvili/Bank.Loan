namespace Bank.Loan.Application.Infrastructure;

public interface IPendingLoanService
{
    void AddPendingLoan(ulong deliveryTag, long loanId);

    bool TryRemovePendingLoan(out ulong deliveryTag, long loanId);
}