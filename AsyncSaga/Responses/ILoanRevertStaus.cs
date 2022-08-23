using DistributedSaga.Contracts.Responses;

namespace DistributedSaga.AsyncSaga.Responses
{
    public class ILoanRevertStaus : IStatusOk
    {
        public Guid LoanApplicationGuid { get; set; }
    }
}
