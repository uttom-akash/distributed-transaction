namespace DistributedSaga.courier.activites;

using DistributedSaga.courier.activityArgument;
using DistributedSaga.courier.activityLog;
using MassTransit;

 
public class LoanActivityDefinition :
    ActivityDefinition<LoanActivity, LoanArguments, LoanLogs>
{
    public LoanActivityDefinition()
    {
        ConcurrentMessageLimit = 10;
    }
}
