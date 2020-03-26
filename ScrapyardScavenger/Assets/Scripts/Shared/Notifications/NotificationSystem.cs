using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationSystem : MonoBehaviour
{
    #region Singleton

    public static NotificationSystem Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public Queue<Notification> notifications;

    public delegate void NotificationAdded(Notification notification);
    public event NotificationAdded OnNotificationAdded;

    public delegate void NotificationRemoved(int id);
    public event NotificationRemoved OnNotificationRemoved;

    void Start()
    {
        notifications = new Queue<Notification>();
    }

    void Update()
    {
        if (notifications.Count == 0) return;


        bool removingNotifications = (notifications.Peek().Timestamp + 2 < Time.time);
        while (removingNotifications)
        {
            int removedID = notifications.Dequeue().ID;

            OnNotificationRemoved?.Invoke(removedID);

            if (notifications.Count == 0) break;

            removingNotifications = (notifications.Peek().Timestamp + 2 < Time.time);
        }
    }

    public void Notify(Notification notification)
    {
        notifications.Enqueue(notification);
        OnNotificationAdded?.Invoke(notification);
    }

}
