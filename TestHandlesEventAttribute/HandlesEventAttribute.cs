using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TestHandlesEventAttribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HandlesEventAttribute : Attribute
    {
        public object[] EventTypes { get; private set; }

        public HandlesEventAttribute(params object[] eventTypes)
        {
            EventTypes = eventTypes;
        }

        /// <summary>
        /// Finds static methods decorated with <see cref="HandlesEventAttribute"/> and adds them to static events 
        /// based on a naming convention.
        /// </summary>
        /// <remarks>
        /// Naming convention: HandlesEvent takes enum parameters located within the class containing our events.
        /// Events have the same name as the enum values.
        /// 
        /// Example:
        /// 
        ///     [HandlesEvent(Post.EventTypes.Edited)]
        ///     static void PostEdited(Post.Events.EditedParameters p)
        /// 
        /// This method will be added to the Post.Events.Edited event.
        /// </remarks>
        public static void BindEvents(Assembly assemblyToSearch = null)
        {
            assemblyToSearch = assemblyToSearch ?? Assembly.GetExecutingAssembly();

            var pairs = from t in assemblyToSearch.GetTypes()
                        from m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                        where m.IsDefined(typeof(HandlesEventAttribute))

                        from @enum in m.GetCustomAttribute<HandlesEventAttribute>().EventTypes
                        let eventsType = @enum.GetType().DeclaringType

                        select new
                        {
                            HandlerInfo = m,
                            EventInfo = eventsType.GetEvent(@enum.ToString())
                        };

            foreach (var pair in pairs)
            {
                var hi = pair.HandlerInfo;
                var ei = pair.EventInfo;

                Debug.WriteLine("HandlesEventAttribute: adding {0}.{1} to {2}.{3}", hi.DeclaringType.FullName, hi.Name, ei.DeclaringType.FullName, ei.Name);

                ei.AddEventHandler(null, Delegate.CreateDelegate(ei.EventHandlerType, hi));
            }
        }
    }

    /// <summary>
    /// Doesn't use built-in event system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HandlesEvent_CollectionBackedAttribute : Attribute
    {
        public object[] EventTypes { get; private set; }

        public HandlesEvent_CollectionBackedAttribute(params object[] eventTypes)
        {
            EventTypes = eventTypes;
        }

        public static void BindEvents(Assembly assemblyToSearch = null)
        {
            assemblyToSearch = assemblyToSearch ?? Assembly.GetExecutingAssembly();

            var pairs = from t in assemblyToSearch.GetTypes()
                        from m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                        where m.IsDefined(typeof(HandlesEvent_CollectionBackedAttribute))

                        from enumValue in m.GetCustomAttribute<HandlesEvent_CollectionBackedAttribute>().EventTypes
                        let modelType = enumValue.GetType().DeclaringType
                        let eventCollection = modelType.GetField(enumValue.ToString(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)

                        group m by eventCollection into g

                        select new
                        {
                            EventCollectionFieldInfo = g.Key,
                            HandlerMethodInfos = g.Select(_ => _).ToArray()
                        };

            // construct an EventHandlerCollection for each set of events found
            foreach (var pair in pairs)
            {
                var delegates = pair.HandlerMethodInfos.Select(mi => Delegate.CreateDelegate(Expression.GetActionType(mi.GetParameters().Select(p => p.ParameterType).ToArray()), mi)).ToList();
                var collection = pair.EventCollectionFieldInfo.FieldType.GetConstructors().Single().Invoke(new[] { delegates });

                pair.EventCollectionFieldInfo.SetValue(null, collection);
            }
        }
    }

    public class DefaultEventParameters
    {
        public object CurrentUser { get; set; }
        public object CurrentSite { get; set; }

        // assumes this will be constructed in the main request thread
        public DefaultEventParameters()
        {
            CurrentUser = Current.User;
            CurrentSite = Current.Site;
        }

    }

    public class EventAttribute : Attribute
    {
        public object EventType { get; private set; }

        public EventAttribute(object eventType)
        {
            EventType = eventType;
        }
    }

    public class EventHandlerCollection<TSender, TParams> where TParams : DefaultEventParameters
    {
        public readonly Action<TSender, TParams>[] _handlers;

        public EventHandlerCollection(List<Delegate> handlers)
        {
            _handlers = handlers.Cast<Action<TSender, TParams>>().ToArray();
        }

        public void Fire(TSender sender, TParams parameters)
        {
            for (int i = 0; i < _handlers.Length; i++)
            {
                _handlers[i](sender, parameters);
            }
        }
    }


}
