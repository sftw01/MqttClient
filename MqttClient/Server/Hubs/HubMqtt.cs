using Microsoft.AspNetCore.SignalR;
using MqttClient.Server.Services;
using MqttClient.Shared;
using System.Runtime.CompilerServices;

namespace MqttClient.Server.Hubs
{
    public class HubMqtt : Hub
    {
        //inject service for mqtt - to be able to send messages to the client

        //event for receiving messages from client
        public event Action<MqttMessageDto> OnMessageReceive;
        private readonly MqttService _mqttService;

        public HubMqtt(MqttService mqttService)
        {
            _mqttService = mqttService;          

        }

        public async Task ReceiveMessage(MqttMessageDto message)
        {
            Console.WriteLine("Hub received object and send to all clients");
            //hub received message from server and send to all clients
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
    
            Console.WriteLine("Hub connected");

        }

        //this method is called by the client trough the hub on server to send message
        public async Task SendMessage(MqttMessageDto message)
        {
            Console.WriteLine("Hub received object and send by _mqttService");
            await _mqttService.PublishMessageAsync(message);
        }



        //this method is called by the client trough the hub on server to subscribe to topic
        public async Task SubscribeTopic(string topic)
        {
            Console.WriteLine("Hub subscrible topic");
            await _mqttService.SubscribeToTopicAsync(topic);
        }


    }
}
