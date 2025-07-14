using Garage.Repositories;
using Garage.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<ITreatmentService, RefuelService>();
builder.Services.AddSingleton<ITreatmentService, RechargeService>();
builder.Services.AddSingleton<ITreatmentService, InflateService>();
builder.Services.AddSingleton<GarageService>();
builder.Services.AddSingleton<IGarageRepository, GarageRepository>();
builder.Services.AddSingleton<IListRepository, ListRepository>();
builder.Services.AddSingleton<ListProcessorService>();


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