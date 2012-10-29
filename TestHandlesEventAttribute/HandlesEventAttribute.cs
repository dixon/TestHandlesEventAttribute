using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TestHandlesEventAttribute
{

    /// <summary>
    /// Doesn't use built-in event system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HandlesEventAttribute : Attribute
    {
        public EventType[] EventTypes { get; private set; }

        public HandlesEventAttribute(params EventType[] eventTypes)
        {
            EventTypes = eventTypes;
        }

        public static void BindEvents(Assembly assemblyToSearch = null)
        {
            assemblyToSearch = assemblyToSearch ?? Assembly.GetExecutingAssembly();

            // all events and handlers should be static; doesn't matter about visibility
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            // find the "events" - should be marked with an attribute, e.g. [Event(EventTypes.QuestionCreated)]
            var events = (from t in assemblyToSearch.GetTypes()
                          from f in t.GetFields(flags)
                          where f.IsDefined(typeof(EventAttribute))
                          select new
                          {
                              EventFieldInfo = f,
                              EventType = f.GetCustomAttribute<EventAttribute>().EventType
                          }).ToList();

            // find the handlers that should be called on each event, e.g. [HandlesEvent(EventTypes.QuestionCreated)]
            var handlers = (from t in assemblyToSearch.GetTypes()
                            from m in t.GetMethods(flags)
                            where m.IsDefined(typeof(HandlesEventAttribute))
                            select new
                            {
                                HandlerMethodInfo = m,
                                EventTypes = m.GetCustomAttribute<HandlesEventAttribute>().EventTypes
                            }).ToList();


            foreach (var e in events)
            {
                // all the methods that should be called when each event is fired
                var methods = handlers.Where(h => h.EventTypes.Contains(e.EventType)).Select(h => h.HandlerMethodInfo);

                // create something we can call
                var delegates = methods.Select(mi => Delegate.CreateDelegate(Expression.GetActionType(mi.GetParameters().Select(p => p.ParameterType).ToArray()), mi));

                // instantiate the "event" collection, passing in our handlers
                var collection = e.EventFieldInfo.FieldType.GetConstructors().Single().Invoke(new[] { delegates });

                // set the static event collection so it can be fired from our "normal" code
                e.EventFieldInfo.SetValue(null, collection);
            }
        }
    }

    public class EventHandlerCollection<TSender, TArgs> : IEnumerable<Action<TSender, TArgs>>
    {
        private readonly Action<TSender, TArgs>[] _handlers;

        public EventHandlerCollection(IEnumerable<Delegate> handlers)
        {
            _handlers = handlers.Cast<Action<TSender, TArgs>>().ToArray();
        }

        public IEnumerator<Action<TSender, TArgs>> GetEnumerator()
        {
            return ((IEnumerable<Action<TSender, TArgs>>)_handlers).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class EventHandlerCollectionExtensions
    {
        public static void Fire<TSender>(this EventHandlerCollection<TSender, EventArgs> handlers, TSender sender)
        {
            Fire(handlers, sender, EventArgs.Empty);
        }

        public static void Fire<TSender, TArgs>(this EventHandlerCollection<TSender, TArgs> handlers, TSender sender, TArgs args)
        {
            if (handlers == null) return;

            foreach (var handler in handlers)
            {
                handler(sender, args);
            }
        }
    }

}
