using System;

namespace TestHandlesEventAttribute
{
    partial class Post
    {
        public enum EventTypes
        {
            Created,
            Edited,
            Closed,
            Deleted
        }

        [Event(EventTypes.Created)]
        public static event Action<Post, DefaultEventParameters> Created;


        public class EditedParameters : DefaultEventParameters
        {
            public string OldTitle { get; set; }
        }
        [Event(EventTypes.Edited)]
        public static event Action<Post, EditedParameters> Edited;


        // alternate way of declaring our events using collection

        public class ClosedParameters : DefaultEventParameters
        {
            public string CloseReason { get; set; }
        }
        [Event(EventTypes.Closed)]
        public static EventHandlerCollection<Post, ClosedParameters> Closed;


        [Event(EventTypes.Deleted)]
        public static EventHandlerCollection<Post, DefaultEventParameters> Deleted;
    }
}
