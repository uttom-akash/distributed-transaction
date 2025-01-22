# Distributed Transaction in Microservices

This repository demonstrates the implementation of distributed transaction patterns in microservices using **MassTransit**. The key patterns are the **Saga** state machine and **Routing Slip**, both essential for coordinating complex distributed workflows.

## Features

- **Saga Patterns**: Implements both **synchronous** and **asynchronous** saga patterns using MassTransit.
- **Routing Slip**: Uses MassTransit Courier for routing slip-based transactions.
- **Transactional Consistency**: Ensures consistency across distributed services with distributed transactions.

## Core Components

- **LockAmount.cs**: Manages the locking of transaction amounts.
- **ProcessLoan.cs**: Handles the loan processing logic within the distributed transaction.
- **RevertLoan.cs**: Contains the logic for reverting a loan if needed.
- **UnlockAmount.cs**: Manages the unlocking of amounts after transaction completion or failure.

## Installation

To get started, clone this repository:

```bash
// rabbitmq setup
setup rabbitmq

// setup project
git clone https://github.com/uttom-akash/distributed-transaction.git
dotnet restore
dotnet run
```

check swagger pageq