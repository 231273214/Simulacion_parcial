using UnityEngine;
using TMPro;

public class MissionNotifications : MonoBehaviour
{
    public TextMeshProUGUI notificationText;
    public float displayTime = 3f;

    private float hideTime;

    void Start()
    {
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Ocultar texto después del tiempo
        if (Time.time > hideTime && notificationText != null && notificationText.gameObject.activeInHierarchy)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    public void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
            hideTime = Time.time + displayTime;
        }
    }
}
