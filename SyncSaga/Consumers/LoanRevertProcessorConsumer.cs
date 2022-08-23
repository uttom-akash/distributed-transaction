using DistributedSaga.SyncSaga.Commands;
using DistributedSaga.SyncSaga.Responses;
using MassTransit;

namespace DistributedSaga.SyncSaga.consumers
{
    public class LoanRevertProcessorConsumer : IConsumer<IRevertLoan>
    {
        public async Task Consume(ConsumeContext<IRevertLoan> context)
        {
            //do work

            int lottary = new Random().Next();

            if ((lottary & 1) == 0)
            {
                await context.RespondAsync(new ILoanRevertStaus());

            }
            else
            {
                throw new Exception("User defined exception while reverting loan!");
            }
        }
    }
}