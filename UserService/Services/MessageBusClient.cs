using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace UserService.Services
{
    public class MessageBusClient
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient()
        {
            var factory = new ConnectionFactory() { HostName = "172.17.0.2" }; // استخدم اسم المضيف الصحيح لـ RabbitMQ
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // تعريف الـ Queue في RabbitMQ
            _channel.QueueDeclare(queue: "userQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void PublishNewUser(UserCreatedDto userCreatedDto)
        {
            var message = JsonSerializer.Serialize(userCreatedDto);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: "userQueue", basicProperties: null, body: body);
            Console.WriteLine(" --> User message sent to RabbitMQ");
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

    public class UserCreatedDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}
