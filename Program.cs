
using DistributedSaga.Contracts.Events;
using DistributedSaga.courier.activites;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton(KebabCaseEndpointNameFormatter.Instance);

builder.Services.AddMassTransit(cfg =>
{

    //courier
    cfg.AddActivitiesFromNamespaceContaining<LoanActivity>();

    //async saga statesmachine
    cfg.AddSagaStateMachine<DistributedSaga.AsyncSaga.statemachine.LoanStateMachine, DistributedSaga.AsyncSaga.states.LoanState>()
        .InMemoryRepository();

    cfg.AddConsumersFromNamespaceContaining<DistributedSaga.AsyncSaga.consumers.LoanProcessorConsumer>();

    cfg.AddRequestClient<AsyncLoanRequested>();

    //sync saga statesmachine
    cfg.AddSagaStateMachine<DistributedSaga.SyncSaga.statemachine.LoanStateMachine, DistributedSaga.SyncSaga.states.LoanState>()
        .InMemoryRepository();

    cfg.AddConsumersFromNamespaceContaining<DistributedSaga.SyncSaga.consumers.LoanProcessorConsumer>();

    cfg.AddRequestClient<SyncLoanRequested>();

    //common
    cfg.SetKebabCaseEndpointNameFormatter();

    cfg.UsingRabbitMq((context, x) =>
    {
        x.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        x.ConfigureEndpoints(context, KebabCaseEndpointNameFormatter.Instance);
    });
});

builder.Services.AddHostedService<MassTransitConsoleHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public class MassTransitConsoleHostedService :
        IHostedService
{
    readonly IBusControl _bus;

    public MassTransitConsoleHostedService(IBusControl bus)
    {
        _bus = bus;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _bus.StopAsync(cancellationToken);
    }
}