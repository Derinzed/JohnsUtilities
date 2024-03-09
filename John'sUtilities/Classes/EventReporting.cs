using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using JohnUtilities.Classes;

namespace JohnUtilities.Model.Classes
{
    public class EventType{
    }
    public struct BaseEventTypes {

        public static readonly EventType Startup = new EventType();
        public static readonly EventType Close = new EventType();
        public static readonly EventType ErrorNotice = new EventType();
        public static readonly EventType SuccessNotice = new EventType();
    }

    public class Event
    {
        public Event(EventType eventType)
        {
            EventType = eventType;
        }
        public void Invoke(string message)
        {
            EventH?.Invoke(this, new NotificationEventArgs(message));
        }
        public EventType EventType;
        public event EventHandler<NotificationEventArgs> EventH;
    }
    public class EventReporting
    {
        private EventReporting()
        {
            Instance = this;
            CreateEventType(BaseEventTypes.Startup);
            CreateEventType(BaseEventTypes.Close);
            CreateEventType(BaseEventTypes.ErrorNotice);
            CreateEventType(BaseEventTypes.SuccessNotice);
        }

        public void CreateEventType(EventType eventType)
        {
            Events.Add(new Event(eventType));
        }
        public void InvokeEvent(EventType eventType, string message)
        {
            Event Item = Events.FirstOrDefault(x => x.EventType == eventType);
            if(Item == null)
            {
                Logging.WriteLogLine("Event type does not exist and cannot be triggered.");
                return;
            }
            Logging.WriteLogLine("Invoking event With message: " + message, LoggingLevel.Debug);
            Item.Invoke(message);
        }
        public void SubscribeToEvent(EventType eventType, Action<Object, NotificationEventArgs> function)
        {   
            Events.First(x => x.EventType == eventType).EventH += function.Invoke;
        }
        public void UnsubscribeToEvent(EventType eventType, Action<Object, NotificationEventArgs> function)
        {
            Events.First(x => x.EventType == eventType).EventH -= function.Invoke;
        }

        static public EventReporting GetEventReporter()
        {
            if(Instance == null)
            {
                return new EventReporting();
            }
            return Instance;
        }

        private static EventReporting Instance;

        List<Event> Events = new List<Event>();
    }
}
