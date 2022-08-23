namespace DistributedSaga.courier.activityArgument;
using System;

 
public interface LoanArguments
{
    public Guid LoanApplicationGuid { get; set; }
    public decimal Amount { get; set; }
}