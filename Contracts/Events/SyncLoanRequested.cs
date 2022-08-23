namespace DistributedSaga.Contracts.Events
{
    public interface SyncLoanRequested
    {
        public Guid CustomerGuid { get; set; }
        public Guid LoanApplicationGuid { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}