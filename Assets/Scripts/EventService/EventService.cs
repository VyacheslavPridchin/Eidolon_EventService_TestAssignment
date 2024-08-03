using Eidolon.Events.Storage.Abstract;
using Eidolon.Network;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Eidolon.Events
{
    public class EventService : MonoBehaviour
    {
        [field: SerializeField]
        public EventsStorageProviderBase StorageProvider { get; private set; }

        [field: SerializeField]
        public NetworkManager NetworkManager { get; private set; }

        [SerializeField]
        private int cooldownBeforeSend;

        private bool isCooldownLaunched;

        private void Start()
        {
            Invoke("CheckUnsentEvents", 0.1f);
        }

        public void TrackEvent(string type, string data) => TrackEvent(new Event(type, data));

        public void TrackEvent(Event @event)
        {
            StorageProvider.AddEvent(@event);

            Debug.Log($"New event added: '{@event.Type}: {@event.Data}'");

            if (!isCooldownLaunched)
                Invoke("CheckUnsentEvents", 0.1f);
        }

        private void CheckUnsentEvents()
        {
            if (StorageProvider.HasEvents() & !isCooldownLaunched)
            {
                Debug.Log("Unsent events detected.");
                StartCoroutine(PostEventsWithCooldown());
            }
        }

        private IEnumerator PostEventsWithCooldown()
        {
            Debug.Log($"Cooldown for {cooldownBeforeSend} seconds has started.");
            isCooldownLaunched = true;
            yield return new WaitForSecondsRealtime(cooldownBeforeSend);

            if (!StorageProvider.HasEvents())
            {
                isCooldownLaunched = false;
                yield break;
            }

            PostEvents();
            isCooldownLaunched = false;
        }

        private async void PostEvents()
        {
            var events = StorageProvider.GetSavedEvents();
            Payload<List<Event>> payload = new Payload<List<Event>>("events", events.Values.ToList());
            var statusCode = await NetworkManager.PostAsync("events", payload);

            if (statusCode == System.Net.HttpStatusCode.OK)
            {
                Debug.Log($"Successfully sent {events.Count} events!");
                StorageProvider.ReleaseEvents(events.Keys.ToList());
            }
            else
            {
                Debug.Log($"An error occurred while sending events. Retry...");
                Invoke("CheckUnsentEvents", 0.1f);
            }
        }
    }
}