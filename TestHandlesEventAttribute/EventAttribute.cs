using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHandlesEventAttribute
{
    public class EventAttribute : Attribute
    {
        public EventType EventType { get; private set; }

        public EventAttribute(EventType eventType)
        {
            EventType = eventType;
        }
    }
}
