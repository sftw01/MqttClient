using Microsoft.AspNetCore.SignalR;
using MqttClient.Client.Pages;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MqttClient.Shared;
using MqttClient.Server.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace MqttClient.Server.Services
{
    public class MqttService
    {

        private readonly IMqttClient _mqttClient= default!;
        private readonly IHubContext<HubMqtt> _hubContext = default!;

        //list of topics to subscribe
        private List<string> _topics = new List<string>();

        //add event for receiving messages from client
        public event Action<MqttMessageDto>? OnMessageReceived;


        public MqttService()
        {



            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            Console.WriteLine("MqttService.cs - created by constructor without hubContext");
        }


        public MqttService(IHubContext<HubMqtt> hubContext)
        {
            _hubContext = hubContext;
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            Console.WriteLine("MqttService.cs - created by constructor with hubContext");
        }


        public async Task PublishMessageAsync(MqttMessageDto message)
        {

            string topic = message.Topic;
            string? payload = message.Payload;

            var mqttFactory = new MqttFactory();     

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("broker.hivemq.com")
                    .Build();

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .Build();

                await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
                await mqttClient.DisconnectAsync();

                Console.WriteLine($"MqttService.cs - published message: T: {message.Topic}, P: {message.Payload}");

            }
        }



        public async Task SubscribeToTopicAsync(string topic)
        {

            //check if topic is already subscribed - if yes return, otherwise add to list of topics currently subscribed
            if (_topics.Contains(topic))
            {
                Console.WriteLine($"MqttService.cs - topic: {topic} already subscribed");
                return;
            }

            _topics.Add(topic);

            var mqttFactory = new MqttFactory();
            var mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("broker.hivemq.com").Build();

            mqttClient.ApplicationMessageReceivedAsync += HandleReceivedApplicationMessage;

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(topic))
                .Build();

            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine($"MqttService.cs - start subscrible topic: T: {topic}");

        }




        private async Task HandleReceivedApplicationMessage(MqttApplicationMessageReceivedEventArgs e)
        {

            var payloadReceive = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.Array ?? Array.Empty<byte>());

            Console.WriteLine($"MqttService.cs - HandreReceive  T: {e.ApplicationMessage.Topic}, P: {payloadReceive}");

            // Create new object to send to client - for fit to MqttMessageDto
            var receivedMessage = new MqttMessageDto
            {
                Topic = e.ApplicationMessage.Topic,
                Payload = payloadReceive
            };


            // send to all clients using SignalR Hub 
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", receivedMessage);
          

            //invoke event for receiving messages from client
            OnMessageReceived?.Invoke(receivedMessage);



        }

    }

}

