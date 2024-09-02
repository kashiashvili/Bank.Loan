using Bank.Loan.Domain.Enums;

namespace Bank.Loan.Domain;

public class Loan : Entity<long> //TODO add aggregateroot abstract class in common library and make this child
{
    public LoanType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Ccy { get; private set; }
    public int MonthAmount { get; private set; }
    public LoanStatus Status { get; private set; }
    
    public Loan(LoanType type, decimal amount, string ccy, int monthAmount, LoanStatus status)
    {
        Type = type;
        Amount = amount;
        Ccy = ccy;
        MonthAmount = monthAmount;
        Status = status;
    }
    
    public void Update(LoanType type, decimal amount, string ccy, int monthAmount)
    {
        Type = type;
        Amount = amount;
        Ccy = ccy;
        MonthAmount = monthAmount;
    }
    
    public void UpdateStatus(LoanStatus status)
    {
        Status = status;
    }
}