using DistributedSaga.AsyncSaga.Responses;
using DistributedSaga.AsyncSaga.Commands;
using MassTransit;

namespace DistributedSaga.AsyncSaga.consumers
{
    public class LoanRevertProcessorConsumer : IConsumer<IRevertLoan>
    {
        public async Task Consume(ConsumeContext<IRevertLoan> context)
        {
            //do work
            int lottary = new Random().Next();

            if ((lottary & 1) == 0)
            {
                await context.Publish<ILoanRevertStaus>(new
                {
                    context.Message.LoanApplicationGuid
                });
            }
            else
            {
                throw new Exception("User defined exception while reverting loan!");
            }
        }
    }
}