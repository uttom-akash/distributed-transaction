namespace DistributedSaga.SyncSaga.Commands
{
    public interface ILockAmount
    {
        public Guid LoanApplicationGuid { get; set; }
        public Guid CustomerGuid { get; set; }
        public decimal Amount { get; set; }
    }
}
