using DistributedSaga.SyncSaga.Commands;
using DistributedSaga.SyncSaga.Responses;
using MassTransit;

namespace DistributedSaga.SyncSaga.consumers
{
    public class LoanProcessorConsumer : IConsumer<IProcessLoan>
    {
        public async Task Consume(ConsumeContext<IProcessLoan> context)
        {
            //do work
            int lottary = new Random().Next();

            if ((lottary & 1) == 0)
            {
                await context.RespondAsync(new IProcessedLoanStatus
                {
                    LoanApplicationGuid = context.Message.LoanApplicationGuid
                });
            }
            else
            {

                throw new Exception("User defined exception while processing loan!");
            }
        }
    }
}