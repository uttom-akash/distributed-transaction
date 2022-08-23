namespace DistributedSaga.SyncSaga.Commands
{
    public interface IRevertLoan
    {
        public Guid LoanApplicationGuid { get; set; }
    }
}
