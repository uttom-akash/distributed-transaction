namespace DistributedSaga.AsyncSaga.Commands
{
    public interface IUnlockAmount
    {
        public Guid CustomerGuid { get; set; }
        public decimal Amount { get; set; }
    }
}
