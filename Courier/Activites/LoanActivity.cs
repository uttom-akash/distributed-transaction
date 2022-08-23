namespace DistributedSaga.courier.activites;

using DistributedSaga.courier.activityArgument;
using DistributedSaga.courier.activityLog;
using MassTransit;
using System.Threading.Tasks;

 
public class LoanActivity : IActivity<LoanArguments, LoanLogs>
{
    public LoanActivity()
    {
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<LoanArguments> context)
    {

        //do work

        var LoanId = NewId.NextGuid();

        return context.Completed(new { LoanId, Amount = 10m });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<LoanLogs> context)
    {
        // compensate if next fails

        return context.Compensated();
    }
}