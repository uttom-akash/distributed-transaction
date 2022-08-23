namespace DistributedSaga.Contracts.Responses
{
    public interface LoanRejected
    {
        public Guid LoanApplicationGuid { get; set; }
    }
}