using DistributedSaga.Contracts.Responses;

namespace DistributedSaga.SyncSaga.Responses
{
    public class ILoanRevertStaus : IStatusOk
    {
        public Guid LoanApplicationGuid { get; set; }
    }
}
