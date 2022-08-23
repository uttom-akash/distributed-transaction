namespace DistributedSaga.courier.activites
{
    using DistributedSaga.courier.activityArgument;
    using DistributedSaga.courier.activityLog;
    using MassTransit;

     
    public class LockAmountActivityDefinition :
        ActivityDefinition<LockAmountActivity, LockAmountArguments, LockAmountLogs>
    {
        public LockAmountActivityDefinition()
        {
            ConcurrentMessageLimit = 20;
        }
    }
}