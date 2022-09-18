using DistributedSaga.Contracts.Events;
using DistributedSaga.Contracts.Responses;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace DistributedSaga.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly IRequestClient<AsyncLoanRequested> _asyncLoanRequestClient;
    private readonly IRequestClient<SyncLoanRequested> _syncLoanRequestClient;
    private readonly IBus _bus;

    public TransactionController(ILogger<TransactionController> logger,
        IRequestClient<AsyncLoanRequested> loanRequestClient,
        IBus bus,
        IRequestClient<SyncLoanRequested> syncLoanRequestClient)
    {
        _logger = logger;
        _asyncLoanRequestClient = loanRequestClient;
        _bus = bus;
        _syncLoanRequestClient = syncLoanRequestClient;
    }

    [HttpGet()]
    public async Task<int> UseCourier()
    {


        var builder = new RoutingSlipBuilder(Guid.NewGuid());

        builder.AddActivity("Loan", new Uri("queue:loan_execute"), new { Amount = 10.0m });

        builder.AddActivity("LockAmount", new Uri("queue:lock-amount_execute"));

        builder.AddVariable("CustomerId", Guid.NewGuid());

        var routingSlip = builder.Build();

        await _bus.Execute(routingSlip);


        return 1;
    }


    [HttpGet()]
    public async Task<int> UseAsyncSaga()
    {

        var status = await _asyncLoanRequestClient
            .GetResponse<LoanStatus, AsyncLoanRequested>(new
            {
                CustomerGuid = Guid.NewGuid(),
                LoanApplicationGuid = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            });

        // await _bus.Publish<AsyncLoanRequested>(new
        // {
        //     CustomerGuid = Guid.NewGuid(),
        //     LoanApplicationGuid = Guid.NewGuid(),
        //     Timestamp = DateTime.UtcNow
        // });

        return 1;
    }

    [HttpGet()]
    public async Task<LoanStatus> UseSyncSaga()
    {


        var status = await _syncLoanRequestClient
            .GetResponse<LoanStatus, SyncLoanRequested>(new
            {
                CustomerGuid = Guid.NewGuid(),
                LoanApplicationGuid = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            });


        return status.Message as LoanStatus;
    }
}
