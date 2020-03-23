using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameNotificationUI : MonoBehaviour
{
    public GameObject notificationPanel;

    void Start()
    {
        NotificationSystem.Instance.OnNotificationAdded += NotificationAdded;
        NotificationSystem.Instance.OnNotificationRemoved += NotificationRemoved;
        foreach (var notif in NotificationSystem.Instance.notifications)
        {
            NotificationAdded(notif);
        }

    }

    void Update()
    {
    }

    void OnDestroy()
    {
        NotificationSystem.Instance.OnNotificationAdded -= NotificationAdded;
        NotificationSystem.Instance.OnNotificationRemoved -= NotificationRemoved;
    }

    #region Notification Events

    void NotificationAdded(Notification notification)
    {
        int count = NotificationSystem.Instance.notifications.Count;
        RectTransform rt = notificationPanel.GetComponent<RectTransform>();
        float height = notificationPanel.GetComponent<RectTransform>().sizeDelta.y;

        // Move down previous notifs
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition += new Vector3(0, -height, 0);
        }


        GameObject notifObj = Instantiate(notificationPanel, gameObject.transform);
        notifObj.name = notification.ID.ToString();

        notifObj.transform.localPosition = new Vector3(-10, 0, 1);
        notifObj.GetComponentInChildren<Text>().text = notification.Message;

        Color color = Color.grey;
        switch (notification.Type)
        {
            case NotificationType.Bad:
                color = Color.red - new Color(0, 0, 0, .4f);
                break;
            case NotificationType.Neutral:
                color = Color.grey - new Color(0, 0, 0, .4f); ;
                break;
            case NotificationType.Good:
                color = Color.green - new Color(0, 0, 0, .4f); ;
                break;
            default:
                break;
        }
        notifObj.GetComponent<Image>().color = color;

    }

    void NotificationRemoved(int id)
    {
        Destroy(transform.GetChild(0).gameObject);
        // Destroy(GameObject.Find(id.ToString()));
    }

    #endregion
}
