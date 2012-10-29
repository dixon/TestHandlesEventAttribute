using System;
using System.Runtime.CompilerServices;

namespace TestHandlesEventAttribute
{
    class Program
    {
        static void Main(string[] args)
        {
            HandlesEventAttribute.BindEvents();

            // these two calls use standard events
            var p = Post.Ask();
            p.Edit("How do I add an event handler via Reflection?");

            // these two use the EventHandlerCollection
            p.Close("Off-topic");
            p.Delete();

            Console.ReadKey();
        }

        [HandlesEvent(EventType.PostCreated)]
        static void PostCreated(Post p, EventArgs unused)
        {
            Console.WriteLine("New post created with Title = " + p.Title);
        }

        [HandlesEvent(EventType.PostEdited)]
        static void PostEdited(Post p, Post.EditedArgs args)
        {
            Console.WriteLine("Post edit; title changed from\n\t{0}\nto\n\t{1}", args.OldTitle, p.Title);
        }
        
        [HandlesEvent(EventType.PostClosed)]
        static void PostClosed(Post p, Post.ClosedArgs args)
        {
            Console.WriteLine("Post closed as " + args.CloseReason);
        }

        [HandlesEvent(EventType.PostDeleted)]
        static void PostDeleted(Post p, EventArgs unused)
        {
            Console.WriteLine("Post deleted at " + p.DeletionDate);
        }

        [HandlesEvent(EventType.PostDeleted)]
        static void AnotherPostDeleted(Post p, EventArgs unused)
        {
            Console.WriteLine("Same Post deleted at " + p.DeletionDate);
        }
        
    }

    
}
