using DistributedSaga.Contracts.Responses;

namespace DistributedSaga.AsyncSaga.Responses
{
    public class ILockAmountStaus : IStatusOk
    {
        public Guid LoanApplicationGuid { get; set; }
    }
}
