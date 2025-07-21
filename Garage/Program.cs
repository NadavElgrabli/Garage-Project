using Garage.Data;
using Garage.Factories;
using Garage.Handlers;
using Garage.Middlewares;
using Garage.Models;
using Garage.Repositories;
using Garage.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<ITreatmentService, RefuelService>();
builder.Services.AddSingleton<ITreatmentService, RechargeService>();
builder.Services.AddSingleton<ITreatmentService, InflateService>();
builder.Services.AddSingleton<ITreatmentRequestHandler, FuelRequestHandler>();
builder.Services.AddSingleton<ITreatmentRequestHandler, AirRequestHandler>();
builder.Services.AddSingleton<ITreatmentRequestHandler, ChargeRequestHandler>();
builder.Services.AddSingleton<IGarageRepository, GarageRepository>();
builder.Services.AddSingleton<GarageManagementService>();
builder.Services.AddSingleton<IListRepository, ListRepository>();
builder.Services.AddSingleton<ListProcessorService>();
builder.Services.AddSingleton<IValidationRepository, ValidationRepository>();
builder.Services.AddSingleton<ValidationService>();
builder.Services.AddSingleton<GarageOrchestratorService>();
builder.Services.AddSingleton<GarageState>();
builder.Services.AddSingleton<InMemoryDatabase>();
builder.Services.AddSingleton<ElectricCarFactory>();
builder.Services.AddSingleton<FuelCarFactory>();
builder.Services.AddSingleton(provider => new Dictionary<Type, IVehicleFactory>
{
    { typeof(AddElectricCarRequest), provider.GetRequiredService<ElectricCarFactory>() },
    { typeof(AddFuelCarRequest), provider.GetRequiredService<FuelCarFactory>() }
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// i added
var listProcessor = app.Services.GetRequiredService<ListProcessorService>();
_ = listProcessor.StartProcessingAsync(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>(); // i added

app.UseAuthorization();

app.MapControllers();

app.Run();