namespace DistributedSaga.AsyncSaga.Commands
{
    public interface IRevertLoan
    {
        public Guid LoanApplicationGuid { get; set; }
    }
}
