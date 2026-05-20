using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class MockModel
    {
        public class HistoryEvent
        {
            public System.DateTime Timestamp { get; set; }
            public string EventType { get; set; }  
            public string EventDescription { get; set; }
            public bool IsAlarm { get; set; }
        }

        public class AlarmEvent
        {
            public System.DateTime Timestamp { get; set; }
            public string AlarmName { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }  
        }
    }
}
