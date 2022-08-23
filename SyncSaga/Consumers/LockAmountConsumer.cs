using DistributedSaga.SyncSaga.Commands;
using DistributedSaga.SyncSaga.Responses;
using MassTransit;

namespace DistributedSaga.SyncSaga.consumers
{
    public class LockAmountConsumer : IConsumer<ILockAmount>
    {

        public async Task Consume(ConsumeContext<ILockAmount> context)
        {
            //do work
            int lottary = new Random().Next();

            if ((lottary & 1) == 0)
            {
                await context.RespondAsync(new ILockAmountStaus());
            }
            else
            {
                throw new Exception("User defined exception while locking amount!");
            }
        }
    }
}