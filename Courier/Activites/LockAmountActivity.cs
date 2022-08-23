namespace DistributedSaga.courier.activites
{
    using DistributedSaga.courier.activityArgument;
    using DistributedSaga.courier.activityLog;
    using MassTransit;
    using System;
    using System.Threading.Tasks;
     
    public class LockAmountActivity : IActivity<LockAmountArguments, LockAmountLogs>
    {
        static readonly Random _random = new Random();

        public async Task<ExecutionResult> Execute(ExecuteContext<LockAmountArguments> context)
        {

            // do work 

            // throw new InvalidOperationException("The card number was invalid");

            return context.Completed();
        }

        public async Task<CompensationResult> Compensate(CompensateContext<LockAmountLogs> context)
        {
            await Task.Delay(100);

            return context.Compensated();
        }
    }
}