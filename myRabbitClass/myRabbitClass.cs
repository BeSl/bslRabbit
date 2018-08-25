using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.InteropServices;

namespace bslRabbit
{
    [Guid("6844AACB-9194-46bf-81AF-9DA74EE687DC")]
    internal interface IRabbitMQ
    {
        [DispId(1)]
        bool SendMessage(string message, string Exchange, string RoutingKey, string Queue);
        string ReadMessage(string queueName);
        bool InitConnection(string UserName, string Password, string HostName, int Port);
    }

    [Guid("70DD7E62-7D82-4301-993C-B7D914330990"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IEventRabbit
    {
    }

    [Guid("69EE0677-884A-4eeb-A3BD-D407845C0C70"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IEventRabbit))]
    public class BslRabbit : IRabbitMQ 
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// ИмяСервера
        /// </summary>
        public String HostName { get; set; }

        /// <summary>
        /// Порт.
        /// </summary>
        public int Port { get; set; }


        public bool InitConnection(string UserName, string Password, string HostName, int Port)
        {
            /* if (string.IsNullOrEmpty(UserName)) UserName = "quest";
                if (string.IsNullOrEmpty(HostName)) HostName = "localhost";
                if (string.IsNullOrEmpty(Password)) Password = "quest";
                if (Port == 0) Port = 5672;
                */

            this.UserName = UserName;
            this.Password = Password;
            this.HostName = HostName;
            this.Port = Port;
            return true;
        }

        public bool SendMessage(string message, string Exchange,string RoutingKey,string Queue)
        {
            try
            {
   
                var factory = new ConnectionFactory()
                {
                    UserName = UserName,
                    HostName = HostName,
                    Password = Password,
                    Port = Port

                };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
                        var body = System.Text.Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: Exchange, routingKey: RoutingKey, basicProperties: null, body: body);
                    }
                }
            }
            catch (Exception e)
            {
                throw new COMException(e.ToString());
            }
            return true;
        }

        public string ReadMessage(string queueName)
        {
            string message = "";
            try
            {

                var factory = new ConnectionFactory()
                {
                    UserName = UserName,
                    HostName = HostName,
                    Password = Password,
                    Port = Port

                };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        BasicGetResult result = channel.BasicGet(queueName, true);

                        if (result != null)
                        {
                            byte[] body = result.Body;
                            message = System.Text.Encoding.UTF8.GetString(body);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new COMException(e.ToString());
            }

            return message;

        }

    } 
}
