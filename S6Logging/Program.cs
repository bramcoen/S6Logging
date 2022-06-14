using MongoDBRepository;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options => {
    //Check should be handled by the gateway
    options.AddPolicy("default", build => build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddControllers();

builder.Services.AddHostedService<UserEditWorker>();
builder.Services.AddSingleton<IUserRepository,UserRepository>();
builder.Services.AddSingleton<ILoggingRepository, LoggingRepository>();
var app = builder.Build();

app.UseRouting();
app.UseCors();
// Configure the HTTP request pipeline.
app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();
});
app.UseHttpsRedirection();


app.MapControllers();

app.Run();
