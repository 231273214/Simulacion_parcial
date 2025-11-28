using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Pause")]
    public GameObject pauseUI;
    public Button firstSelectedButton; // botón inicial (por ejemplo "Resume")

    private bool isPaused = false;
    private bool pauseButtonPressed = false;

    // Cooldown para evitar mover demasiado rápido el menú
    private float navigationCooldown = 0.25f;
    private float lastNavigationTime = 0f;

    void Update()
    {
        HandlePauseInput();
        if (isPaused) HandleMenuNavigation();
    }

    // ==========================
    //     PAUSA / DESPAUSA
    // ==========================
    void HandlePauseInput()
    {
        // Teclado (ESC)
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        // Botón de pausa en gamepads
        bool pauseInput = Input.GetKey("joystick button 7") || Input.GetKey("joystick button 9");

        if (pauseInput && !pauseButtonPressed)
        {
            pauseButtonPressed = true;
            TogglePause();
        }
        else if (!pauseInput)
        {
            pauseButtonPressed = false;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            // Seleccionar automáticamente el primer botón del menú
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }
    }

    // ==========================
    //   NAVEGACIÓN DEL MENÚ
    // ==========================
    void HandleMenuNavigation()
    {
        // Dirección hacia arriba o abajo
        float verticalInput =
            Input.GetAxisRaw("Vertical") +
            (Input.GetKeyDown(KeyCode.DownArrow) ? -1 : 0) +
            (Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0);

        // Evitar repetición demasiado rápida
        if (Time.unscaledTime - lastNavigationTime < navigationCooldown)
            return;

        if (verticalInput != 0)
        {
            lastNavigationTime = Time.unscaledTime;

            Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();

            if (current != null)
            {
                Selectable next = verticalInput < 0 ? current.FindSelectableOnDown() : current.FindSelectableOnUp();

                if (next != null)
                {
                    EventSystem.current.SetSelectedGameObject(next.gameObject);
                }
            }
        }

        // Botón de confirmar (A - Xbox / X - PlayStation)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                selected.GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    // ==========================
    //   BOTONES DEL MENÚ
    // ==========================
    public void OnGameResumePress()
    {
        TogglePause();
    }

    public void OnGameOptionsPress()
    {
        Debug.Log("Opciones (todavía no implementado)");
    }

    public void OnGameExitPress()
    {
        Application.Quit();
    }

    public void VolverEscena()
    {
        SceneManager.LoadScene(2);
    }
}


