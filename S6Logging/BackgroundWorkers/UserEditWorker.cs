using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class UserEditWorker : BackgroundService
{
    ILogger _logger;
    ConnectionFactory _connectionFactory;
    IConnection _connection;
    IModel _channel;
    IConfiguration _configuration;
    ILoggingRepository _loggingRepository;
    public UserEditWorker(ILogger<UserEditWorker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _loggingRepository = new LoggingRepository(configuration);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = System.Text.Json.JsonSerializer.Deserialize<User>(body, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (message == null) return;
            await _loggingRepository.StoreLoggingActionAsync(new LoggingAction(System.Text.UTF8Encoding.UTF8.GetString(body), message.Id));
            _logger.LogInformation("Processed a message in UserWorker");
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: "logging/user",
                             autoAck: false,
                             consumer: consumer);

        return Task.CompletedTask;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQHostname"],
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            DispatchConsumersAsync = true
        };
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "logging/user",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        _channel.QueueBind(queue: "logging/user",
         exchange: "user",
         routingKey: "edit");
        _channel.BasicQos(0, 30, false);
        _logger.LogInformation($"Queue [hello] is waiting for messages.");

        return base.StartAsync(cancellationToken);
    }
}
