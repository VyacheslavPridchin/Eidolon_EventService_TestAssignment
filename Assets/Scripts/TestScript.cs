using EasyButtons;
using Eidolon.Events;
using Eidolon.Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = Eidolon.Events.Event;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    private EventService eventService;

    [Button]
    private void AddRandomEvent()
    {
        string[] events = { "levelStart", "rewardReceived", "coinsSpent", "levelEnd" };
        string[] data = { "level", "reward", "coins", "level" };
        int index = Random.Range(0, events.Length);
        eventService.TrackEvent(new Event(events[index], $"{data[index]}:{Random.Range(0, 100)}"));
    }

    [Button]
    private void ShowData()
    {
        var events = eventService.StorageProvider.GetSavedEvents();
        string output = "Events data:\n";

        foreach (var item in events)
            output += $"{item.Key}: '{item.Value.Type}: {item.Value.Data}'\n";

        Debug.Log(output);
    }

    [Button]
    private void ShowJSONPayload()
    {
        var events = eventService.StorageProvider.GetSavedEvents();
        Payload<List<Event>> payload = new Payload<List<Event>>("events", events.Values.ToList());
        Debug.Log(payload.GetJson());
    }

    [Button]
    private void ClearData()
    {
        var events = eventService.StorageProvider.GetSavedEvents();
        eventService.StorageProvider.ReleaseEvents(events.Keys.ToList());
        Debug.Log("Clear complete!");
    }
}
