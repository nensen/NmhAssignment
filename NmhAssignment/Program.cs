using Assignment.Services;
using KeyValueWebApi.Services;
using Microsoft.EntityFrameworkCore;
using NmhAssignment.DbContexts;
using RabbitMQ.Client;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<KeyValueStorage>();

/*  Setup rabbitMQ connection that will be injected to services
 *
 *  Wait 15 seconds if running in debug from VS, because rabbit mq even when started is not initialized yet, 
 *  so we need to wait until it is initialized or the application would crash,
 *  There is probably better solution but I already spent like a hour searching how to deal with this problem that 
 *  should not exist in a first place, but nothing really worked.
 *  I prefer not to spend another hour dealing with this issue on demo app, not worth a effort, sorry. */
Thread.Sleep(15000);
builder.Services.AddSingleton<IConnection>(sp =>
{
    ConnectionFactory factory = new ConnectionFactory()
    {
        HostName = "rabbitmq",
        Port = 5672,
        UserName = "guest",
        Password = "guest",
        RequestedHeartbeat = TimeSpan.FromSeconds(60),
        AutomaticRecoveryEnabled = true
    };
    var connection = factory.CreateConnection();

    // Create queue
    using var channel = connection.CreateModel();
    channel.QueueDeclare(queue: "calculation",
          durable: false,
          exclusive: false,
          autoDelete: false,
          arguments: null);

    return connection;
});
builder.Services.AddHostedService<RabbitMqConsumerService>();

// Setup postgres EF
builder.Services.AddDbContext<ApplicationDbContext>(options =>
           options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresAppDatabase")));

var app = builder.Build();

// Run EF migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}


// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();