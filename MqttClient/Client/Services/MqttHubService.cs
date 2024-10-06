using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MqttClient.Shared;

namespace MqttClient.Client.Services
{
    public class MqttHubService
    {

        //event for receiving messages from server
        public event Action<MqttMessageDto>? OnMessageReceiveds;


        private readonly HubConnection _hubConnection;


        public MqttHubService(NavigationManager navigationManager)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/hubmqtt"))
                .Build();

            //register method ReceiveMessage on client side
            _hubConnection.On<MqttMessageDto>("ReceiveMessage", ReceiveMessage);

            _hubConnection.StartAsync(); // Uruchom połączenie
        }


        public void ReceiveMessage(MqttMessageDto message)
        {
            Console.WriteLine("MqttHubService received object and invoke event");
            OnMessageReceiveds?.Invoke(message);
        }

        public async Task SubscribeTopic(string topic)
        {
            //invoke method SubscribeTopic on server by hub - mqttService
            await _hubConnection.SendAsync("SubscribeTopic", topic);
        }

        public async Task SendMessage(MqttMessageDto message)
        {
            //invoke method SendMessage on server by hub - mqttService
            await _hubConnection.SendAsync("SendMessage", message);
        }
        

    }
}
