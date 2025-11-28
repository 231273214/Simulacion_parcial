using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuInicial : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject optionsPanel;

    [Header("Navigation")]
    public Button firstSelectedButton;   // Primer botón seleccionado en el menú principal
    public Button firstSelectedOptions;  // Primer botón seleccionado dentro de Opciones

    private bool navigatingOptions = false;
    private float navigationCooldown = 0.25f;
    private float lastNavigationTime = 0f;

    void Start()
    {
        // Seleccionar automáticamente el primer botón del menú
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);

        // Asegurar que el panel de opciones está desactivado al inicio
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    void Update()
    {
        if (!navigatingOptions)
            HandleMenuNavigation();       // Navegar menú principal
        else
            HandleOptionsNavigation();    // Navegar menú de opciones
    }

    // =====================================
    //         NAVEGACIÓN MENÚ PRINCIPAL
    // =====================================
    void HandleMenuNavigation()
    {
        float verticalInput =
            Input.GetAxisRaw("Vertical") +
            (Input.GetKeyDown(KeyCode.DownArrow) ? -1 : 0) +
            (Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0);

        if (Time.unscaledTime - lastNavigationTime >= navigationCooldown && verticalInput != 0)
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

        // Confirmar opción (Enter o A del mando)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
                selected.GetComponent<Button>().onClick.Invoke();
        }
    }

    // =====================================
    //         NAVEGACIÓN OPCIONES
    // =====================================
    void HandleOptionsNavigation()
    {
        float verticalInput =
            Input.GetAxisRaw("Vertical") +
            (Input.GetKeyDown(KeyCode.DownArrow) ? -1 : 0) +
            (Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0);

        if (Time.unscaledTime - lastNavigationTime >= navigationCooldown && verticalInput != 0)
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

        // Confirmar con Enter o A (Xbox)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
                selected.GetComponent<Button>().onClick.Invoke();
        }

        // Salir de opciones con B / Círculo / Escape
        if (Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOptions();
        }
    }

    // =====================================
    //         BOTONES DEL MENÚ
    // =====================================
    public void PlayGame()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenOptions()
    {
        if (optionsPanel == null) return;

        optionsPanel.SetActive(true);
        navigatingOptions = true;

        // Seleccionar botón inicial del panel de opciones
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedOptions.gameObject);
    }

    public void CloseOptions()
    {
        if (optionsPanel == null) return;

        optionsPanel.SetActive(false);
        navigatingOptions = false;

        // Volver al menú principal y seleccionar el primer botón
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}


