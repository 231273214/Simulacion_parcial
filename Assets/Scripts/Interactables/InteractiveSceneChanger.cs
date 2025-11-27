using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractiveSceneChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string targetSceneName;
    public string displayText = "Presiona E para ir a la primera simulación";

    [Header("Interaction Settings")]
    public float detectionRange = 2.5f;
    public KeyCode interactionKey = KeyCode.E;

    [Header("UI Components")]
    public CanvasGroup uiPanel;
    public Text messageText;
    public float fadeDuration = 0.5f;

    private Transform playerTransform;
    private bool isPlayerNear = false;
    private bool isUIVisible = false;
    private bool isTransitioning = false;

    void Start()
    {
        FindPlayer();
        SetupUI();

        // Asegurar que el tiempo esté corriendo
        Time.timeScale = 1f;
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player no encontrado, buscando en Update...");
        }
    }

    void SetupUI()
    {
        if (uiPanel != null)
        {
            uiPanel.alpha = 0f;
            uiPanel.gameObject.SetActive(false);
        }

        if (messageText != null)
        {
            messageText.text = displayText;
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        CheckPlayerDistance();

        if (isPlayerNear && Input.GetKeyDown(interactionKey) && !isTransitioning)
        {
            ChangeScene();
        }
    }

    void CheckPlayerDistance()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);

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
    }

    void ShowUI()
    {
        if (uiPanel != null && !isUIVisible)
        {
            isUIVisible = true;
            uiPanel.gameObject.SetActive(true);
            StartCoroutine(FadeUI(0f, 1f, fadeDuration));
        }
    }

    void HideUI()
    {
        if (uiPanel != null && isUIVisible)
        {
            isUIVisible = false;
            StartCoroutine(FadeUI(1f, 0f, fadeDuration, true));
        }
    }

    void ChangeScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName) && !isTransitioning)
        {
            isTransitioning = true;
            Debug.Log($"Cambiando a escena: {targetSceneName}");

            StartCoroutine(TransitionToScene());
        }
        else
        {
            Debug.LogError("Nombre de escena no asignado");
        }
    }

    IEnumerator TransitionToScene()
    {
        // Fade out de la UI
        if (uiPanel != null)
        {
            yield return StartCoroutine(FadeUI(uiPanel.alpha, 0f, 0.5f));
        }

        // Asegurar que el tiempo esté normal antes de cambiar escena
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Cargar la escena de manera asíncrona
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false;

        // Esperar a que la escena esté casi cargada
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    IEnumerator FadeUI(float fromAlpha, float toAlpha, float duration, bool disableOnComplete = false)
    {
        if (uiPanel == null) yield break;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            uiPanel.alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / duration);
            yield return null;
        }

        uiPanel.alpha = toAlpha;

        if (disableOnComplete && toAlpha == 0f)
        {
            uiPanel.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}