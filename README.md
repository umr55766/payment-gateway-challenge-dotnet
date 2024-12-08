# Instructions for candidates

This is the .NET version of the Payment Gateway challenge. If you haven't already read this [README.md](https://github.com/cko-recruitment/) on the details of this exercise, please do so now. 

## Template structure
```
src/
    PaymentGateway.Api - a skeleton ASP.NET Core Web API
test/
    PaymentGateway.Api.Tests - an empty xUnit test project
imposters/ - contains the bank simulator configuration. Don't change this

.editorconfig - don't change this. It ensures a consistent set of rules for submissions when reformatting code
docker-compose.yml - configures the bank simulator
PaymentGateway.sln
```

Feel free to change the structure of the solution, use a different test library etc.

# Get Started
1. Make sure you have docker setup & docker-compose installed in your machine
2. Run `docker-compose up --build` (to build and run the containers)
3. You can access the swagger [here](http://localhost:5001/swagger/index.html)
![Swagger](https://i.ibb.co/ZJmLSpR/Screenshot-2024-12-09-at-11-42-14.png)
4. Access Aspire monitoring dashboard [here](http://localhost:18888/login?t=4a90e99a2bc2149d43c9f83a5ccf2963) (Get latest access token from docker logs)
![Custom metrics](https://i.ibb.co/DfVq40z/Screenshot-2024-12-09-at-11-38-55.png)
![Traces](https://i.ibb.co/SXHsd3W/Screenshot-2024-12-09-at-11-39-08.png)
![Logs](https://i.ibb.co/VVz6bzP/Screenshot-2024-12-09-at-11-39-18.png)
5. Run smoke test `./test/PaymentGateway.Api.Tests/smoke/smoke_test.sh`
![Smoke](https://i.ibb.co/mtkDkC5/Screenshot-2024-12-09-at-11-42-32.png)
6. Run contact test `node test/PaymentGateway.Api.Tests/contract/contract_test.js` (Make sure you have node installed in your system)
![Contract](https://i.ibb.co/dt4MS7x/Screenshot-2024-12-09-at-11-42-44.png)
7. Run unit test `dotnet test` (Make sure you have .NET 8 SDK installed in your machine - [Refer here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))
![Unit test](https://i.ibb.co/PjW7t6F/Screenshot-2024-12-09-at-12-12-24.png)

### Sample Payload for testing

Successful Payment
```
{
  "cardNumber": "2222405343248877",
  "expiryMonth": "04",
  "expiryYear": "2025",
  "currency": "GBP",
  "amount": 100,
  "cvv": "123"
}
```
Failed Payment
```
{
  "cardNumber": "2222405343248112",
  "expiryMonth": "01",
  "expiryYear": "2026",
  "currency": "USD",
  "amount": 60000,
  "cvv": "456"
}
```
You can use Swagger for testing APIs [here](http://localhost:5001/swagger/index.html)

# Test Coverage
![codecov](https://codecov.io/gh/umr55766/payment-gateway-challenge-dotnet/branch/main/graph/badge.svg)

![codecov](https://codecov.io/github/umr55766/payment-gateway-challenge-dotnet/graphs/tree.svg?token=A5OH2E0E3B)


# TODO
- [X] Make payment endpoint
- [X] Make payment request validation
- [X] Currency validation
- [X] Right value to store Money - Value, Currency ISO, Precision
- [X] Standardize the Errors
- [X] Get payment endpoint
- [X] Ensure thread safety
- [X] Contract test - Swagger, Json
- [X] Integration test - Newman, Postman
- [ ] Load test - K6
- [X] Dockerize the service
- [X] Script to build, execute app and run tests
- [X] Add support for Observability https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel
- [X] Create Github repo with CI
- [X] Add CD
