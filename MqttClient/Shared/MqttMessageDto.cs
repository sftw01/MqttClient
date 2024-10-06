using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient.Shared
{
    public class MqttMessageDto
    {
        public string Topic { get; set; } = string.Empty;
        public string? Payload { get; set; } = string.Empty;

    }
}
