using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();
    public static EventManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Subscribe(string eventName, Action listener)
    {
        if (!eventDictionary.ContainsKey(eventName))
            eventDictionary[eventName] = listener;
        else
            eventDictionary[eventName] += listener;
    }

    public void Unsubscribe(string eventName, Action listener)
    {
        if (eventDictionary.ContainsKey(eventName))
            eventDictionary[eventName] -= listener;
    }

    public void TriggerEvent(string eventName)
    {
        if (eventDictionary.ContainsKey(eventName))
            eventDictionary[eventName]?.Invoke();
    }
}