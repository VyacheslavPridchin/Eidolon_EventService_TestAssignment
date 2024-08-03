using System;

namespace Eidolon.Events
{
    [Serializable]
    public class Event
    {
        public Event(string type, string data)
        {
            Type = type;
            Data = data;
        }

        public string Type { get; set; }
        public string Data { get; set; }
    }
}