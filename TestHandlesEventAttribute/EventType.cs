using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHandlesEventAttribute
{
    /// <summary>
    /// Master list of all events in our system, making it easy to find available 
    /// events and decorate our eventing attributes.
    /// </summary>
    /// <remarks>
    /// This could get unwieldy as events are added, but going to try it out.
    /// </remarks>
    public enum EventType
    {
        PostCreated,
        PostEdited,
        PostClosed,
        PostDeleted
    }
}
