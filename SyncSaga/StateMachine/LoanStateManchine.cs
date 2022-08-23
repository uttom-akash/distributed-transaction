using DistributedSaga.SyncSaga.Responses;
using DistributedSaga.SyncSaga.Commands;
using DistributedSaga.Contracts.Events;
using DistributedSaga.Contracts.Responses;
using DistributedSaga.SyncSaga.states;
using MassTransit;
using MassTransit.Contracts;

namespace DistributedSaga.SyncSaga.statemachine
{
    public class LoanStateMachine : MassTransitStateMachine<LoanState>
    {
        public LoanStateMachine()
        {

            InstanceState(x => x.CurrentState);

            SetCorrelationId();

            Initially(ProcessLoanApplication());

            During(ProcessLoan.Pending, HandleProcessLoanCompleted(), HandleProcessLoanFaulted(), HandleProcessLoanExpired());

            During(LockAmount.Pending, HandleLockAmountCompleted(), HandleLockAmountFaulted(), HandleLockAmountExpired());

            During(RevertLoan.Pending, HandleRevertLoanCompleted(), HandleRevertLoanFaulted(), HandleRevertLoanExpired());

            SetCompletedWhenFinalized();
        }

        public void SetCorrelationId()
        {
            Event(() => LoanRequested, x => x.CorrelateById(context => context.Message.LoanApplicationGuid));

            Request(() => ProcessLoan, config => { config.Timeout = TimeSpan.Zero; });

            Request(() => LockAmount, config => { config.Timeout = TimeSpan.Zero; });

            Request(() => RevertLoan, config => { config.Timeout = TimeSpan.Zero; });

        }

        public EventActivityBinder<LoanState, SyncLoanRequested> ProcessLoanApplication()
        {
            return When(LoanRequested)
                // .Then(context => _logger.LogDebug("loan request submitted"))
                .Then(context => SaveLoanInfo(context.Saga, context))
                .Then(context => SaveRequestState(context.Saga, context))
                .TransitionTo(ProcessLoan.Pending)
                .Request(ProcessLoan, context => context.Init<IProcessLoan>(new
                {
                    context.Saga.CustomerGuid,
                    context.Saga.LoanApplicationGuid,
                    Amount = 100m
                }));
        }

        #region Process loan response
        public EventActivityBinder<LoanState, IProcessedLoanStatus> HandleProcessLoanCompleted()
        {
            return When(ProcessLoan.Completed)
                .ThenAsync(async context => { })
                .TransitionTo(LockAmount.Pending)
                .Request(LockAmount, context => context.Init<ILockAmount>(new
                {
                    context.Saga.CustomerGuid,
                    Amount = 100m
                }));
        }

        public EventActivityBinder<LoanState, Fault<IProcessLoan>> HandleProcessLoanFaulted()
        {
            return When(ProcessLoan.Faulted)
                .ThenAsync(async context => { })
                .TransitionTo(Rejected)
                .ThenAsync(async context =>
                {
                    var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
                    await endpoint.Send(new LoanStatus { LoanApplicationGuid = context.Saga.LoanApplicationGuid, State = "Faulted" }, r => r.RequestId = context.Saga.MyRequestId);
                })
                .Finalize();
        }

        public EventActivityBinder<LoanState, RequestTimeoutExpired<IProcessLoan>> HandleProcessLoanExpired()
        {
            return When(ProcessLoan.TimeoutExpired)
                .ThenAsync(async context => { })
                .TransitionTo(Rejected)
                .ThenAsync(async context =>
                {
                    var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
                    await endpoint.Send(new LoanStatus { LoanApplicationGuid = context.Saga.LoanApplicationGuid, State = "Faulted" }, r => r.RequestId = context.Saga.MyRequestId);
                })
                .Finalize();
        }
        #endregion


        #region lock amount response 
        public EventActivityBinder<LoanState, ILockAmountStaus> HandleLockAmountCompleted()
        {
            return When(LockAmount.Completed)
                .TransitionTo(Accepted)
                .ThenAsync(async context =>
                {
                    var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
                    await endpoint.Send(new LoanStatus { LoanApplicationGuid = context.Saga.LoanApplicationGuid, State = "Done" }, r => r.RequestId = context.Saga.MyRequestId);
                })
                .Finalize();
        }

        public EventActivityBinder<LoanState, Fault<ILockAmount>> HandleLockAmountFaulted()
        {
            return When(LockAmount.Faulted)
                .ThenAsync(async context => { })
                .TransitionTo(RevertLoan.Pending)
                .Request(RevertLoan, context => context.Init<IRevertLoan>(new
                {
                    context.Saga.CustomerGuid
                }));
        }

        public EventActivityBinder<LoanState, RequestTimeoutExpired<ILockAmount>> HandleLockAmountExpired()
        {
            return When(LockAmount.TimeoutExpired)
                .ThenAsync(async context => { })
                .Request(RevertLoan, context => context.Init<IRevertLoan>(new
                {
                    context.Saga.CustomerGuid
                }));
        }
        #endregion


        #region revert loan response
        public EventActivityBinder<LoanState, ILoanRevertStaus> HandleRevertLoanCompleted()
        {
            return When(RevertLoan.Completed)
                .ThenAsync(async context => { })
                .ThenAsync(async context =>
                {
                    var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
                    await endpoint.Send(new LoanStatus { LoanApplicationGuid = context.Saga.LoanApplicationGuid, State = "Faulted" }, r => r.RequestId = context.Saga.MyRequestId);
                })
                .Finalize();
        }

        public EventActivityBinder<LoanState, Fault<IRevertLoan>> HandleRevertLoanFaulted()
        {
            return When(RevertLoan.Faulted)
                .ThenAsync(async context => { })
                .TransitionTo(Rejected)
                .ThenAsync(async context =>
                {
                    //do event sourcing
                })
                .ThenAsync(async context =>
                {
                    var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
                    await endpoint.Send(new LoanStatus { LoanApplicationGuid = context.Saga.LoanApplicationGuid, State = "Faulted" }, r => r.RequestId = context.Saga.MyRequestId);
                })
                .Finalize();
        }

        public EventActivityBinder<LoanState, RequestTimeoutExpired<IRevertLoan>> HandleRevertLoanExpired()
        {
            return When(RevertLoan.TimeoutExpired)
                .ThenAsync(async context => { })
                .TransitionTo(Rejected)
                .ThenAsync(async context =>
                {
                    //do event sourcing
                })
                .ThenAsync(async context =>
                {
                    var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
                    await endpoint.Send(new LoanStatus { LoanApplicationGuid = context.Saga.LoanApplicationGuid, State = "Faulted" }, r => r.RequestId = context.Saga.MyRequestId);
                })
                .Finalize();
        }

        #endregion

        //state
        public State Prcessing { get; private set; }
        public State Accepted { get; private set; }
        public State Rejected { get; private set; }

        //event
        public Event<SyncLoanRequested> LoanRequested { get; private set; }

        //request
        public Request<LoanState, IProcessLoan, IProcessedLoanStatus> ProcessLoan { get; private set; }
        public Request<LoanState, IRevertLoan, ILoanRevertStaus> RevertLoan { get; private set; }
        public Request<LoanState, ILockAmount, ILockAmountStaus> LockAmount { get; private set; }



        private static void SaveRequestState(LoanState saga, BehaviorContext<LoanState> context)
        {
            saga.ProcessingId = Guid.NewGuid();
            saga.MyRequestId = context.RequestId;
            saga.ResponseAddress = context.ResponseAddress;
        }

        private static void SaveLoanInfo(LoanState saga, BehaviorContext<LoanState, SyncLoanRequested> context)
        {
            saga.CustomerGuid = context.Message.CustomerGuid;
            saga.LoanApplicationGuid = context.Message.LoanApplicationGuid;
            saga.SubmitDate = DateTime.Now;

            saga.CorrelationId = context.Message.LoanApplicationGuid;
        }
    }
}