using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

public class WeatherManager : MonoBehaviour, IGameManager
{
    public float cloudValue { get; private set; }
    public ManagerStatus status { get; private set; }
    // Add cloud value here (listing 10.8)
    private NetworkService network;
    public void Startup(NetworkService service)
    {
        Debug.Log("Weather manager starting...");
        network = service;
        StartCoroutine(network.GetWeatherXML(OnXMLDataLoaded));
        StartCoroutine(network.GetWeatherJSON(OnJSONDataLoaded));
        status = ManagerStatus.Initializing;
    }

    public void OnXMLDataLoaded(string data)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(data);
        XmlNode root = doc.DocumentElement;
        XmlNode node = root.SelectSingleNode("clouds");
        string value = node.Attributes["value"].Value;
        cloudValue = Convert.ToInt32(value) / 100f;
        Debug.Log($"Value: {cloudValue}");
        Messenger.Broadcast(GameEvent.WEATHER_UPDATED);
        status = ManagerStatus.Started;
    }
    public void OnJSONDataLoaded(string data)
    {
        JObject root = JObject.Parse(data);
        JToken clouds = root["clouds"];
        cloudValue = (float)clouds["all"] / 100f;
        Debug.Log($"Value: {cloudValue}");
        Messenger.Broadcast(GameEvent.WEATHER_UPDATED);
        status = ManagerStatus.Started;
    }
    public void LogWeather(string name)
    {
        StartCoroutine(network.LogWeather(name, cloudValue, OnLogged));
    }
    private void OnLogged(string response)
    {
        Debug.Log(response);
    }
}