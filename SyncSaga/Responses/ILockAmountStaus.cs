using DistributedSaga.Contracts.Responses;

namespace DistributedSaga.SyncSaga.Responses
{
    public class ILockAmountStaus : IStatusOk
    {
        public Guid LoanApplicationGuid { get; set; }
    }
}
