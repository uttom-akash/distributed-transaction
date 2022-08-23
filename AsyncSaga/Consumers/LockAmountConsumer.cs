using DistributedSaga.AsyncSaga.Responses;
using DistributedSaga.AsyncSaga.Commands;
using MassTransit;

namespace DistributedSaga.AsyncSaga.consumers
{
    public class LockAmountConsumer : IConsumer<ILockAmount>
    {

        public async Task Consume(ConsumeContext<ILockAmount> context)
        {
            //do work
            int lottary = new Random().Next();

            if ((lottary & 1) == 0)
            {
                await context.Publish<ILockAmountStaus>(new { });
            }
            else
            {
                throw new Exception("User defined exception while locking amount!");
            }
        }
    }
}