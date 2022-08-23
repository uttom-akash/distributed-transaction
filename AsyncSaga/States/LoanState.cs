using MassTransit;

namespace DistributedSaga.AsyncSaga.states
{
    public class LoanState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }


        public Guid CustomerGuid { get; set; }
        public Guid LoanApplicationGuid { get; set; }
        public string? CurrentState { get; set; }

        public DateTime? SubmitDate { get; set; }
        public DateTime? Updated { get; set; }
    }

}