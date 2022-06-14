using MongoDBRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHostedService<UserEditWorker>();
builder.Services.AddSingleton<IUserRepository,UserRepository>();
builder.Services.AddSingleton<ILoggingRepository, LoggingRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
