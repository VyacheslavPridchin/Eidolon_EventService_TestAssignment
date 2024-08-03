using System;
using System.Collections.Generic;

namespace Eidolon.Events.Storage.Abstract
{
    interface IEventsStorageProvider
    {
        bool HasEvents();
        Guid AddEvent(Event @event);
        Dictionary<Guid, Event> GetSavedEvents();
        void ReleaseEvents(List<Guid> ids);
    }
}