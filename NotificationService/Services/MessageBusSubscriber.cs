using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using NotificationService.Models;
using NotificationService.Data;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Services
{
    public class MessageBusSubscriber : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;

        public MessageBusSubscriber(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory() { HostName = "172.17.0.2" }; // استخدم اسم المضيف الصحيح لـ RabbitMQ
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // تعريف الـ Queue الخاصة بالمنشورات
            _channel.QueueDeclare(queue: "postQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            // تعريف الـ Queue الخاصة بالمستخدمين
            _channel.QueueDeclare(queue: "userQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            // مستهلك لمنشورات جديدة
            var postConsumer = new EventingBasicConsumer(_channel);
            postConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var postCreatedDto = JsonSerializer.Deserialize<PostCreatedDto>(message);
                if (postCreatedDto != null) // تحقق من أن postCreatedDto ليس null
                {
                    Console.WriteLine(" --> Received post message from RabbitMQ");

                    // إنشاء نطاق جديد (scope) لكل رسالة واردة
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

                        var notification = new Notification
                        {
                            UserId = postCreatedDto.UserId,
                            Message = $"A new post was created with content: {postCreatedDto.Content}",
                            SentAt = DateTime.UtcNow,
                            IsRead = false
                        };

                        dbContext.Notifications.Add(notification);
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    Console.WriteLine(" --> Received null post message, skipping.");
                }
            };

            // مستهلك لمستخدمين جدد
            var userConsumer = new EventingBasicConsumer(_channel);
            userConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // تحقق من عدم كون الرسالة null
                if (!string.IsNullOrEmpty(message))
                {
                    var userCreatedDto = JsonSerializer.Deserialize<UserCreatedDto>(message);

                    // تحقق من عدم كون userCreatedDto null قبل استخدامه
                    if (userCreatedDto != null)
                    {
                        Console.WriteLine(" --> Received user message from RabbitMQ");

                        // إنشاء نطاق جديد (scope) لكل رسالة واردة
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

                            var notification = new Notification
                            {
                                UserId = userCreatedDto.UserId,
                                Message = $"Welcome {userCreatedDto.Username}! Your email is {userCreatedDto.Email}.",
                                SentAt = DateTime.UtcNow,
                                IsRead = false
                            };

                            dbContext.Notifications.Add(notification);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
            };

            // بدء الاستماع لكل Queue
            _channel.BasicConsume(queue: "postQueue", autoAck: true, consumer: postConsumer);
            _channel.BasicConsume(queue: "userQueue", autoAck: true, consumer: userConsumer);
        }

        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }

    // DTO لمنشورات جديدة
    public class PostCreatedDto
    {
        public int PostId { get; set; }
        public string? Content { get; set; }
        public int UserId { get; set; }
    }

    // DTO لمستخدمين جدد
    public class UserCreatedDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}
