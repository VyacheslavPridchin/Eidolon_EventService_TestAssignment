using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eidolon.Events.Storage.Abstract
{
    public abstract class EventsStorageProviderBase : ScriptableObject, IEventsStorageProvider
    {
        protected abstract void Initialize();
        protected abstract void ReadStorage();
        protected abstract void WriteStorage();

        public abstract bool HasEvents();
        public abstract Guid AddEvent(Event @event);
        public abstract Dictionary<Guid, Event> GetSavedEvents();
        public abstract void ReleaseEvents(List<Guid> ids);
    }
}