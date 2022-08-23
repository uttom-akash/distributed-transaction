using MassTransit;

namespace DistributedSaga.SyncSaga.states
{
    public class LoanState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }

        // sync request and respons correlation
        public Guid ProcessingId { get; set; }
        public Guid? MyRequestId { get; set; }
        public Uri ResponseAddress { get; set; }

        public Guid CustomerGuid { get; set; }
        public Guid LoanApplicationGuid { get; set; }
        public string? CurrentState { get; set; }

        public DateTime? SubmitDate { get; set; }
        public DateTime? Updated { get; set; }
    }

}