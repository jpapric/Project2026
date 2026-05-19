namespace Server.Domain
{
    public class Belt
    {
        public float Speed { get; }
        public float Torque { get; }
        public bool Start_stop { get; }
        public enum Status
        {
            stopped = 0,
            prepared = 1,
            running = 2
        }
    }
}
