using System;

namespace TestHandlesEventAttribute
{
    partial class Post
    {
        [Event(EventType.PostCreated)]
        public static EventHandlerCollection<Post, EventArgs> Created;


        public class EditedArgs
        {
            public string OldTitle { get; set; }
        }
        [Event(EventType.PostEdited)]
        public static EventHandlerCollection<Post, EditedArgs> Edited;


        public class ClosedArgs
        {
            public string CloseReason { get; set; }
        }
        [Event(EventType.PostClosed)]
        public static EventHandlerCollection<Post, ClosedArgs> Closed;


        [Event(EventType.PostDeleted)]
        public static EventHandlerCollection<Post, EventArgs> Deleted;
    }
}
