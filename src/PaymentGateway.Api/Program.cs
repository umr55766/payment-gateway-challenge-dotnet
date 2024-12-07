using System.Diagnostics;
using System.Diagnostics.Metrics;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

using PaymentGateway.Api.Domain.HttpClients;
using PaymentGateway.Api.Domain.Repositories;
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Domain.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<InMemoryPaymentsRepository>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<IBankClient, BankHttpClient>();
builder.Services.AddScoped<PaymentService>();

builder.Services.Configure<BankClientSettings>(builder.Configuration.GetSection("BankClient"));

var paymentMeter = new Meter("Payment", "1.0.0");
var paymentAttempts = paymentMeter.CreateCounter<int>("payment.count", description: "Counts the number of payments");
var makePaymentActivity = new ActivitySource("MakingPayment");
builder.Services.AddSingleton(paymentMeter);
builder.Services.AddSingleton(makePaymentActivity);
builder.Services.AddSingleton(paymentAttempts);


builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});
var otel = builder.Services.AddOpenTelemetry();
otel.WithMetrics(metrics =>
{
    // Metrics provider from OpenTelemetry
    metrics.AddAspNetCoreInstrumentation();
    
    // Custom metrics
    metrics.AddMeter(paymentMeter.Name);

    // ASP.NET Core specific metrics
    metrics.AddMeter("Microsoft.AspNetCore.Hosting");
    metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
});
otel.WithTracing(tracing =>
{
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    tracing.AddSource(makePaymentActivity.Name);
});
var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
if (otlpEndpoint != null)
{
    otel.UseOtlpExporter();
}

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
