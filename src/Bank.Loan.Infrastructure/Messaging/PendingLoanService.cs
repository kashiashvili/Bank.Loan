using System.Collections.Concurrent;
using Bank.Loan.Application.Infrastructure;

namespace Bank.Loan.Infrastructure.Messaging;

public class PendingLoanService : IPendingLoanService
{
    private readonly ConcurrentDictionary<long, ulong> _pendingLoans = new();

    public void AddPendingLoan(ulong deliveryTag, long loanId)
    {
        _pendingLoans[loanId] = deliveryTag;
    }

    public bool TryRemovePendingLoan(out ulong deliveryTag, long loanId)
    {
        return _pendingLoans.TryRemove(loanId, out deliveryTag);
    }
}