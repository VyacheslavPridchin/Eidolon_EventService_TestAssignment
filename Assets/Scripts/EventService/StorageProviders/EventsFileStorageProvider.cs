using EasyButtons;
using Eidolon.Events.Storage.Abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Eidolon.Events.Storage
{
    [CreateAssetMenu(fileName = "EventsFileStorage", menuName = "Events Storage Providers/File Storage")]
    public class EventsFileStorageProvider : EventsStorageProviderBase
    {
        [field: Header("File Storage")]
        [field: SerializeField]
        public string FileName { get; private set; } = "EventsStorage";

        private Dictionary<Guid, Event> events = new Dictionary<Guid, Event>();
        private string filePath = string.Empty;

        private void OnEnable() => Initialize();

        protected override void Initialize()
        {
            filePath = Path.Combine(Application.persistentDataPath, "Storage", $"{FileName}.json");
            ReadStorage();
            Debug.Log($"The File storage with path '{filePath}' has been successfully initialized.");
        }

        protected override void ReadStorage() => ReadStorageAsync();

        private async void ReadStorageAsync()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    events = JsonConvert.DeserializeObject<Dictionary<Guid, Event>>(json);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    File.Delete(filePath);
                    Debug.LogWarning($"The storage '{filePath}' has been deleted.");
                }
            }
        }

        protected override void WriteStorage() => WriteStorageAsync();

        private async void WriteStorageAsync()
        {
            string json = JsonConvert.SerializeObject(events);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            try
            {
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
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