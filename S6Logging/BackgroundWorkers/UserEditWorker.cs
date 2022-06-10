using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class UserEditWorker : BackgroundService
{
    ILogger _logger;
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
            if (message != null && message.Id != null)
            {
                await _loggingRepository.StoreLoggingActionAsync(new LoggingAction(System.Text.UTF8Encoding.UTF8.GetString(body), message.Id,"User has been modified"));
            }
            _logger.LogInformation("Processed a message in UserWorker");
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);


            _channel.BasicConsume(queue: "logging/user",
                                 autoAck: false,
                                 consumer: consumer);
        };
        return Task.CompletedTask;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        ConnectionFactory _connectionFactory = new ConnectionFactory()
        {
            HostName = _configuration.GetValue<string>("RabbitMQHostname"),
            UserName = _configuration.GetValue<string>("RabbitMQUsername"),
            Password = _configuration.GetValue<string>("RabbitMQPassword"),
            Port = 5672,
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
