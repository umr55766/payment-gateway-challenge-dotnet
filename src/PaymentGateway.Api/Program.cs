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
