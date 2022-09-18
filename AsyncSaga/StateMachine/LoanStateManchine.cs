using DistributedSaga.AsyncSaga.Responses;
using DistributedSaga.AsyncSaga.states;
using DistributedSaga.AsyncSaga.Commands;
using DistributedSaga.Contracts.Events;
using DistributedSaga.Contracts.Responses;
using MassTransit;

namespace DistributedSaga.AsyncSaga.statemachine
{
    public class LoanStateMachine : MassTransitStateMachine<LoanState>
    {
        public LoanStateMachine()
        {

            InstanceState(x => x.CurrentState);

            SetCorrelationId();

            Initially(HandleLoanRequsted());

            During(LoanPrcessingPending, HandleProcessLoanCompleted(), HandleProcessLoanFaulted());

            During(AmountLockingPending, HandleLockAmountCompleted(), HandleLockAmountFaulted());

            During(LoanRevertingPending, HandleRevertLoanCompleted(), HandleRevertLoanFaulted());

            SetCompletedWhenFinalized();

        }

        public void SetCorrelationId()
        {
            Event(() => ProcessLoanCompleted, x => x.CorrelateById(context => context.Message.LoanApplicationGuid));
            Event(() => ProcessLoanFaulted, x => x.CorrelateById(context => 
            {
                if (context.Message.Message != null)
                    return context.Message.Message.LoanApplicationGuid;
                else
                    return Guid.NewGuid();
            }));

            Event(() => RevertLoanCompleted, x => x.CorrelateById(context => context.Message.LoanApplicationGuid));
            Event(() => RevertLoanFaulted, x => x.CorrelateById(context => {
                if (context.Message.Message != null)
                    return context.Message.Message.LoanApplicationGuid;
                else
                    return Guid.NewGuid();
            }));

            Event(() => LockAmountCompleted, x => x.CorrelateById(context => context.Message.LoanApplicationGuid));
            Event(() => LockAmountFaulted, x => x.CorrelateById(context => {
                if (context.Message.Message != null)
                    return context.Message.Message.LoanApplicationGuid;
                else
                    return Guid.NewGuid();
            }));


            Event(() => LoanAccepted, x => x.CorrelateById(context => context.Message.LoanApplicationGuid));
            Event(() => LoanRejected, x => x.CorrelateById(context => context.Message.LoanApplicationGuid));

            Event(() => LoanRequested, x =>
            {
                x.CorrelateById(context => context.Message.LoanApplicationGuid);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    await context.RespondAsync(new { context.Message.LoanApplicationGuid });
                }));
            });

        }

        public EventActivityBinder<LoanState, AsyncLoanRequested> HandleLoanRequsted()
        {
            return When(LoanRequested)
                // .Then(context => _logger.LogDebug("loan request submitted"))
                .Then(context => SaveLoanInfo(context.Saga, context))
                .TransitionTo(LoanPrcessingPending)
                .RespondAsync(context => context.Init<LoanStatus>(new { LoanApplicationGuid = context.Message.LoanApplicationGuid, State = "in progress" }))
                .PublishAsync(context => context.Init<IProcessLoan>(new
                {
                    context.Saga.CustomerGuid,
                    context.Saga.LoanApplicationGuid,
                    Amount = 100m
                }));
        }

        #region Process loan response
        public EventActivityBinder<LoanState, IProcessedLoanStatus> HandleProcessLoanCompleted()
        {
            return When(ProcessLoanCompleted)
                .ThenAsync(async context => { })
                .TransitionTo(AmountLockingPending)
                .PublishAsync(context => context.Init<ILockAmount>(new
                {
                    context.Saga.CustomerGuid,
                    Amount = 100m
                }));
        }

        public EventActivityBinder<LoanState, Fault<IProcessLoan>> HandleProcessLoanFaulted()
        {
            return When(ProcessLoanFaulted)
                .ThenAsync(async context => { })
                .TransitionTo(Rejected)
                .PublishAsync(context => context.Init<LoanRejected>(new
                {
                    context.Saga.CustomerGuid,
                    context.Saga.LoanApplicationGuid,
                    Amount = 100m
                }))
                .Finalize();
        }
        #endregion

        #region lock amount response 
        public EventActivityBinder<LoanState, ILockAmountStaus> HandleLockAmountCompleted()
        {
            return When(LockAmountCompleted)
                .TransitionTo(Accepted)
                .PublishAsync(context => context.Init<LoanAccepted>(new
                {
                    context.Saga.CustomerGuid,
                    context.Saga.LoanApplicationGuid,
                    Amount = 100m
                }))
                .Finalize();
        }

        public EventActivityBinder<LoanState, Fault<ILockAmount>> HandleLockAmountFaulted()
        {
            return When(LockAmountFaulted)
                .ThenAsync(async context => { })
                .TransitionTo(LoanRevertingPending)
                .PublishAsync(context => context.Init<IRevertLoan>(new
                {
                    context.Saga.CustomerGuid
                }));
        }
        #endregion


        #region revert loan response
        public EventActivityBinder<LoanState, ILoanRevertStaus> HandleRevertLoanCompleted()
        {
            return When(RevertLoanCompleted)
                .ThenAsync(async context => { })
                .PublishAsync(context => context.Init<LoanRejected>(new
                {
                    context.Saga.CustomerGuid,
                    context.Saga.LoanApplicationGuid,
                    Amount = 100m
                }))
                .Finalize();
        }

        public EventActivityBinder<LoanState, Fault<IRevertLoan>> HandleRevertLoanFaulted()
        {
            return When(RevertLoanFaulted)
                .ThenAsync(async context => { })
                .TransitionTo(Rejected)
                .ThenAsync(async context =>
                {
                    //do event sourcing
                })
                .PublishAsync(context => context.Init<LoanRejected>(new
                {
                    context.Saga.CustomerGuid,
                    context.Saga.LoanApplicationGuid,
                    Amount = 100m
                }))
                .Finalize();
        }

        #endregion


        //state
        public State LoanPrcessingPending { get; private set; }
        public State LoanRevertingPending { get; private set; }
        public State AmountLockingPending { get; private set; }

        public State Accepted { get; private set; }
        public State Rejected { get; private set; }

        //event
        public Event<AsyncLoanRequested> LoanRequested { get; private set; }

        public Event<IProcessedLoanStatus> ProcessLoanCompleted { get; private set; }
        public Event<Fault<IProcessLoan>> ProcessLoanFaulted { get; protected set; }

        public Event<ILoanRevertStaus> RevertLoanCompleted { get; private set; }
        public Event<Fault<IRevertLoan>> RevertLoanFaulted { get; private set; }

        public Event<ILockAmountStaus> LockAmountCompleted { get; private set; }
        public Event<Fault<ILockAmount>> LockAmountFaulted { get; private set; }

        public Event<LoanAccepted> LoanAccepted { get; private set; }
        public Event<LoanRejected> LoanRejected { get; private set; }


        private static void SaveLoanInfo(LoanState saga, BehaviorContext<LoanState, AsyncLoanRequested> context)
        {
            saga.CustomerGuid = context.Message.CustomerGuid;
            saga.LoanApplicationGuid = context.Message.LoanApplicationGuid;
            saga.SubmitDate = DateTime.Now;

            saga.CorrelationId = context.Message.LoanApplicationGuid;
        }

        private static Guid SelectCorrelation<T>( ConsumeContext<Fault<T>> context) where T : IStatusOk
        {
            if (context.Message.Message == null)
                return context.Message.Message.LoanApplicationGuid;
            else
                return Guid.NewGuid();
        }
    }
}