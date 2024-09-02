using Bank.Loan.Application.Repository;
using Bank.Loan.Domain.Commands;
using MediatR;

namespace Bank.Loan.Application.Loan.Commands.Update;

public class UpdateLoanCommandHandler : IRequestHandler<UpdateLoanCommand, Unit>
{
    private readonly IRepository<Domain.Loan> _loanRepository;

    public UpdateLoanCommandHandler(IRepository<Domain.Loan> loanRepository)
    {
        _loanRepository = loanRepository;
    }
    
    public async Task<Unit> Handle(UpdateLoanCommand request, CancellationToken cancellationToken)
    {
        var entity =await _loanRepository.GetByIdAsync(request.ID, cancellationToken);

        entity.Update(request.Type, request.Amount, request.Ccy, request.MonthAmount);

        await _loanRepository.UpdateAsync(entity, cancellationToken);
        
        return Unit.Value;
    }
}