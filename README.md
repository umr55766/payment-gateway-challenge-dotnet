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
- [ ] Script to build, execute app and run tests
- [X] Add support for Observability https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel
- [X] Create Github repo with CI
- [X] Add CD