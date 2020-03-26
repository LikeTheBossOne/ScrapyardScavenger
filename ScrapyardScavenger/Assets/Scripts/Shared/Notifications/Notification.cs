using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification
{

    public static int currentId = 0;

    public int ID { get; private set; }
    public String Message { get; private set; }
    public NotificationType Type { get; private set; }
    public float Timestamp { get; private set; }
    

    public Notification(String message, NotificationType type)
    {
        ID = currentId;

        Message = message;
        Type = type;

        Timestamp = Time.time;


        currentId++;
    }
}

public enum NotificationType
{
    Good = 0,
    Neutral,
    Bad,
    SIZE
}
