namespace DistributedSaga.Contracts.Responses
{
    public class LoanStatus
    {
        public Guid LoanApplicationGuid { get; set; }

        public string State { get; set; }
    }
}