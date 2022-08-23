namespace DistributedSaga.AsyncSaga.Commands
{
    public interface IProcessLoan
    {
        public Guid CustomerGuid { get; set; }
        public Guid LoanApplicationGuid { get; set; }
        public decimal Amount { get; set; }
    }
}
