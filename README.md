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
8. Run load test `k6 run test/PaymentGateway.Api.Tests/load/load_test.js` (Make sure you have [k6](https://k6.io/open-source/) installed in your system)
   ![Load test](https://i.ibb.co/M1PJJ3W/Screenshot-2024-12-10-at-17-42-02.png)

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

# Build
![build](https://github.com/umr55766/payment-gateway-challenge-dotnet/actions/workflows/dotnet.yml/badge.svg)

# Test Coverage
![codecov](https://codecov.io/gh/umr55766/payment-gateway-challenge-dotnet/branch/main/graph/badge.svg)

![codecov](https://codecov.io/github/umr55766/payment-gateway-challenge-dotnet/graphs/tree.svg?token=A5OH2E0E3B)


# Assumptions
1. Created only Payments Repository and using it to store Payments data. (Decided to keep things simple with single repository, for now. [YAGNI](https://en.wikipedia.org/wiki/You_aren%27t_gonna_need_it))
2. Ideally, Card should be stored in another table and Payment should just have reference to it. This is more efficient and better way.
3. I would like to store Payments/Transactions in some sort of Ledger, so that we have full history of Payment lifecycle and can be used as reference/replay when needs arise.
4. We dont' necessarily need to store full card number as there is no requirements associated to it. I am storing it to just demonstrate that even if we need to store (let's say for scheduled payment/saved card/etc), since it's a very sensitive data and should never be store in plaintext format.
5. Fixed precision to 2 for now. Ideally it should be based on Currency
6. Using Decimal to store Amount, as it have better precision and best suited for handling financial data.
7. Not storing AuthorizationCode and Validation Errors, for now, as there is no requirements for it right now. Ideally we should be storing it for future/references. Maybe we need it for refund request, customer support ticket etc.
8. Authentication have not been implemented, as there is no requirements for it right now.
9. I have tried following test pyramid model, in which number of unit test > number of integration > number of end-to-end tests.
10. As finance/payments is an extensive domain. I am following Domain Driven Design. We have Core module which contains core parts/classes of the system.
11. Since software is (mostly) ever evolving, based on business needs, I have used Port & Adapters pattern, so that our solution is modular enough to evolve and adapt to ever-changing business needs. For example, right now we're using InMemoryPaymentsRepository, which uses `ConcurrentDictionary<Guid, Payment>` as the Database, if we need to migrate to use real DB we only need to replace this Repository with real repository and other parts of the code will work out-of-the-box.
![Port & Adapters](https://miro.medium.com/v2/resize:fit:1400/format:webp/0*3FZGIgynXuegHO4Y.)
12. I have integrated very basic Observability and Monitory, which logs, custom metrics and traces, just to re-iterate the importance of Observability and Monitor. In real world, of course, we'll be using something more efficient and durable.
13. There is contract test to ensure we don't contract for the merchants.
14. Smoke test is end-to-end test. I have kept this to minimal, in accordance with test pyramid model. Just 3, for 3 types of payment Authorized, Declined and Rejected.
15. I have followed Test driven development and made small, sustainable commits, that's why you might see a bigger number of commits.
16. I have integrated with Codecov to track and visualize test code coverage. I'm not targeting 100% code coverage purposefully [[reference]](https://en.wikipedia.org/wiki/Goodhart%27s_law)
17. I am using Github actions for a simple build pipeline.
18. Using [k6](https://k6.io/open-source/) for load testing
19. 


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
- [X] Load test - K6
- [X] Dockerize the service
- [X] Script to build, execute app and run tests
- [X] Add support for Observability https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel
- [X] Create Github repo with CI
- [X] Add CD
