namespace Server.Domain
{
    public class Event
    {
        public Event(string name, string type, DateTime time)
        {
            Name = name;
            Type = type;
            Time = time;
        }

        public string Name { get; }
        public string Type { get; }
        public DateTime Time { get; }

    }
}
