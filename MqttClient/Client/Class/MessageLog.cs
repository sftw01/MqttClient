using MudBlazor;

namespace MqttClient.Client.Class
{
    public class MessageLog
    {
        public string Title { get; set; } = "Info";
        public string Message { get; set; } = "Message";
        public Severity Severity { get; set; }

    }
}
