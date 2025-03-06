using EFCore.First.API.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration)
);
builder.Services.AddCarter();
//builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HRContext>(c =>
    c.UseSqlServer("Server=THE_HA9;Database=HRDatabase;Integrated Security=True;TrustServerCertificate=True;"));
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
app.MapCarter();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.Run();

Log.CloseAndFlush();
