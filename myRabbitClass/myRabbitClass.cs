using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.InteropServices;

namespace myRabbitClass
{
    [Guid("6844AACB-9194-46bf-81AF-9DA73EE687DC")]
    internal interface IMyClass
    {
        [DispId(1)]
        //4. описываем методы которые можно будет вызывать из вне
        string SendMessage(string message,
                                       string UserName,
                                       string Password,
                                       string HostName,
                                       int port,
                                       string Exchange,
                                       string RoutingKey,
                                       string Queue);
        string RecieveMessage(string message,
                                       string UserName,
                                       string Password,
                                       string HostName,
                                       int port,
                                       string Exchange,
                                       string RoutingKey,
                                       string Queue
                                       );
    }

    [Guid("70DD7E62-7D82-4301-993C-B7D919430990"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
    public interface IMyEvents
#pragma warning restore CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
    {
    }
    //6. описываем класс реализующий интерфейсы(GUID получаем и записываем с помощью утилиты guidgen.exe)
    [Guid("69EE0677-884A-4eeb-A3BD-D407844C0C70"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IMyEvents))]
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
    public class BslRabbit : IMyClass //название нашего класса MyClass
#pragma warning restore CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
    {
        public string SendMessage(string message,
                                       string UserName,
                                       string Password,
                                       string HostName,
                                       int port,
                                       string Exchange,
                                       string RoutingKey,
                                       string Queue
                                       )
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = HostName,
                    UserName = UserName,
                    Password = Password,
                    Port = port
                };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                        var body = System.Text.Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: Exchange,
                                            routingKey: RoutingKey,
                                            basicProperties: null,
                                            body: body);
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return "Data successfully delivered!";
        }


        public string RecieveMessage(string message,
                                       string UserName,
                                       string Password,
                                       string HostName,
                                       int port,
                                       string Exchange,
                                       string RoutingKey,
                                       string Queue
                                       )
        {
            var factory = new ConnectionFactory()
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password,
                Port = port
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = System.Text.Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: Queue, autoAck: false, consumer: consumer);
                return message;

            }
        }

    } 
}
