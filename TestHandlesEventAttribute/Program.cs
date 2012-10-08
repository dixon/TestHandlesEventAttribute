using System;
using System.Runtime.CompilerServices;

namespace TestHandlesEventAttribute
{
    class Program
    {
        static void Main(string[] args)
        {
            HandlesEventAttribute.BindEvents();
            HandlesEvent_CollectionBackedAttribute.BindEvents();

            // these two calls use standard events
            var p = Post.Ask();
            p.Edit("How do I add an event handler via Reflection?");

            // these two use the EventHandlerCollection
            p.Close("Off-topic");
            p.Delete();

            Console.ReadKey();
        }

        [HandlesEvent(Post.EventTypes.Created)]
        static void PostCreated(Post p, DefaultEventParameters parameters)
        {
            WriteCommonEventParameters(parameters);
            Console.WriteLine("New post created with Title = " + p.Title);
        }

        [HandlesEvent(Post.EventTypes.Edited)]
        static void PostEdited(Post p, Post.EditedParameters parameters)
        {
            WriteCommonEventParameters(parameters);
            Console.WriteLine("Post edit; title changed from\n\t{0}\nto\n\t{1}", parameters.OldTitle, p.Title);
        }
        
        [HandlesEvent_CollectionBacked(Post.EventTypes.Closed)]
        static void PostClosed(Post p, Post.ClosedParameters parameters)
        {
            WriteCommonEventParameters(parameters);
            Console.WriteLine("Post closed as " + parameters.CloseReason);
        }

        [HandlesEvent_CollectionBacked(Post.EventTypes.Deleted)]
        static void PostDeleted(Post p, DefaultEventParameters parameters)
        {
            WriteCommonEventParameters(parameters);
            Console.WriteLine("Post deleted at " + p.DeletionDate);
        }

        [HandlesEvent_CollectionBacked(Post.EventTypes.Deleted)]
        static void AnotherPostDeleted(Post p, DefaultEventParameters parameters)
        {
            WriteCommonEventParameters(parameters);
        }

        static void WriteCommonEventParameters(DefaultEventParameters parameters, [CallerMemberName] string memberName = "")
        {
            Console.WriteLine("\n{0} called with User {1} on Site {2}", memberName, parameters.CurrentUser, parameters.CurrentSite);
        }
    }

    
}
