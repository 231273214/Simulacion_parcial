using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class InteractiveSceneChanger : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup uiPanel;
    public TextMeshProUGUI messageText;
    public float fadeDuration = 0.5f;
    public string displayText = "Presiona E o Button South para continuar";

    [Header("Interaction")]
    public float detectionRange = 2.5f;

    private bool isPlayerNear = false;

    void Start()
    {
        if (messageText != null)
            messageText.text = displayText;

        if (uiPanel != null)
        {
            uiPanel.alpha = 0f;
            uiPanel.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= detectionRange && !isPlayerNear)
        {
            isPlayerNear = true;
            ShowUI();
        }
        else if (distance > detectionRange && isPlayerNear)
        {
            isPlayerNear = false;
            HideUI();
        }

        // Detectar interacción: teclado o Input System (Button South)
        if (isPlayerNear && !IsTransitioning() &&
            (Keyboard.current.eKey.wasPressedThisFrame ||
             Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
        {
            ChangeScene();
        }
    }

    void ShowUI()
    {
        if (uiPanel != null)
        {
            uiPanel.gameObject.SetActive(true);
            StartCoroutine(FadeUI(0f, 1f, fadeDuration));
        }
    }

    void HideUI()
    {
        if (uiPanel != null)
        {
            StartCoroutine(FadeUI(1f, 0f, fadeDuration, true));
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }

    IEnumerator FadeUI(float from, float to, float duration, bool disableOnComplete = false)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (uiPanel != null)
                uiPanel.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        if (uiPanel != null)
            uiPanel.alpha = to;

        if (disableOnComplete)
            uiPanel.gameObject.SetActive(false);
    }

    bool IsTransitioning()
    {
        // Opcional: puedes agregar un bool si quieres evitar múltiples cambios rápidos de escena
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}




