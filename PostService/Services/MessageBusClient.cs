using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PostService.Services
{
    public class MessageBusClient
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient()
        {
            var factory = new ConnectionFactory() { HostName = "172.17.0.2" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // تعريف الـ Queue في RabbitMQ
            _channel.QueueDeclare(queue: "postQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void PublishNewPost(PostCreatedDto postCreatedDto)
        {
            var message = JsonSerializer.Serialize(postCreatedDto);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: "postQueue", basicProperties: null, body: body);
            Console.WriteLine(" --> Post message sent to RabbitMQ");
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

    public class PostCreatedDto
    {
        public int PostId { get; set; }
        public string? Content { get; set; }
        public int UserId { get; set; }
    }
}
