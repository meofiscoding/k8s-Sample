using dotnetService.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<RabbitMQService>();
//builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(action => action.Configuration = builder.Configuration["RedisURL"]);

static void SubscribeToMessageQueue(WebApplication app)
{
    using var scope = app.Services.CreateAsyncScope();
    var rabbitMQService = scope.ServiceProvider.GetRequiredService<RabbitMQService>();

    var emailHandler = new EmailHandler(
       rabbitMQService.MsgChannel
   );

    rabbitMQService.SubscribeToQueue(emailHandler.MQEventHandler, RabbitMQService.SEND_EMAIL_QUEUE_NAME);
}

var app = builder.Build();

// Consume RabbitMQ
SubscribeToMessageQueue(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

