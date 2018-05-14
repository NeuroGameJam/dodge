using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Statistics : MonoBehaviour
{
    public static Statistics Instance;
    List<TimedEvent> savedEvents;
    string folder;

    void Start ()
    {
        Instance = this;
        savedEvents = new List<TimedEvent>();
        folder = Path.Combine(Application.dataPath, "../") + "/Logs/";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        /*savedEvents.Add(new TimedEvent(2f, 4, 1.5f, 1.6f, Color.red, 1.7f, Color.green));
        savedEvents.Add(new TimedEvent(1f, 2, 1.6f, 1.4f, Color.yellow, 1.9f, Color.green));
        SaveToFile();*/
    }

    public void AddEvent(float timeStamp, int robots, float speed, float result, Color rColor)
    {
        savedEvents.Add(new TimedEvent(timeStamp, robots, speed, result, rColor));
    }

    public void SaveToFile()
    {
        string fileName = string.Format("Statistics - {0}.txt",
                             System.DateTime.Now.ToString("yyyy-MM-d_ HH-mm-ss") );
        string path = folder + fileName;

        if (!File.Exists(path))
        {
            string createText = "Statistics from Dodge" + System.Environment.NewLine;
            File.WriteAllText(path, createText);
        }

        foreach (TimedEvent tEve in savedEvents)
        {
            string content = string.Format("Timestamp:{0}s || Number of robots:{1} || Game Speed:{2}x || Result:{3}s",
                tEve.timeStamp, tEve.numberOfRobots, tEve.gameSpeed, tEve.result);
            File.AppendAllText(path, content + System.Environment.NewLine);
        }

        GameManager.Instance.Reset();
    }

    public List<TimedEvent> GetData()
    {
        return savedEvents;
    }
}
[System.Serializable]
public class TimedEvent
{
    public float timeStamp;
    public int numberOfRobots;
    public float gameSpeed;
    public float result;
    public Color resultColor;

    public TimedEvent(float timeStamp, int robots, float speed, float result, Color rColor)
    {
        this.timeStamp = timeStamp;
        this.numberOfRobots = robots;
        this.gameSpeed = speed;
        this.result = result;
        this.resultColor = rColor;
    }
}
