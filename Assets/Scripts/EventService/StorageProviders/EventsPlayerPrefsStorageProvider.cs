using EasyButtons;
using Eidolon.Events.Storage.Abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eidolon.Events.Storage
{
    [CreateAssetMenu(fileName = "EventsPlayerPrefsStorage", menuName = "Events Storage Providers/Player Preferences Storage")]
    public class EventsPlayerPrefsStorageProvider : EventsStorageProviderBase
    {
        [field: Header("Player Preferences Storage")]
        [field: SerializeField]
        public string Key { get; private set; } = "EventsStorage";

        private Dictionary<Guid, Event> events = new Dictionary<Guid, Event>();

        private void OnEnable() => Initialize();

        protected override void Initialize()
        {
            ReadStorage();
            Debug.Log($"The PlayerPrefs storage with key '{Key}' has been successfully initialized.");
        }

        protected override void ReadStorage()
        {
            if (PlayerPrefs.HasKey(Key))
            {
                try
                {
                    string json = PlayerPrefs.GetString(Key);
                    events = JsonConvert.DeserializeObject<Dictionary<Guid, Event>>(json);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    PlayerPrefs.DeleteKey(Key);
                    Debug.LogWarning($"The storage '{Key}' has been deleted.");
                }
            }
        }

        protected override void WriteStorage()
        {
            string json = JsonConvert.SerializeObject(events);

            PlayerPrefs.SetString(Key, json);
        }

        public override bool HasEvents() => events != null && events.Count > 0;

        public override Guid AddEvent(Event @event)
        {
            Guid id = Guid.NewGuid();
            events[id] = @event;

            WriteStorage();

            return id;
        }

        public override Dictionary<Guid, Event> GetSavedEvents() => events;

        public override void ReleaseEvents(List<Guid> ids)
        {
            foreach (Guid id in ids)
                events.Remove(id);

            WriteStorage();
        }

#if UNITY_EDITOR
        [Button]
        private void Clear()
        {
            events.Clear();
            WriteStorage();
        }

        [Button]
        private void ShowData()
        {
            var events = GetSavedEvents();
            string output = "Events data:\n";

            foreach (var item in events)
                output += $"{item.Key}: '{item.Value.Type}: {item.Value.Data}'\n";

            Debug.Log(output);
        }
#endif
    }
}