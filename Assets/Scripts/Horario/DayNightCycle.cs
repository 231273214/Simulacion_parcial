using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance;

    [Header("Configuración de Ciclo")]
    public float dayDuration = 60f;        // duración total del ciclo (día + noche) en segundos
    [Range(0f, 1f)] public float nightAlpha = 0.5f; // opacidad máxima del panel de noche

    [Header("UI Panel")]
    public Image nightPanel;

    private float timer = 0f;
    private bool isNight = false;

    void Awake()
    {
        // Configurar singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Avanzar tiempo
        timer += Time.deltaTime;
        float cycleProgress = timer / dayDuration;

        // Reiniciar ciclo si se completa
        if (cycleProgress >= 1f)
        {
            timer = 0f;
            cycleProgress = 0f;
        }

        // Determinar si es día o noche
        bool currentlyNight = cycleProgress > 0.5f;
        if (currentlyNight != isNight)
        {
            isNight = currentlyNight;
            if (isNight)
                Debug.Log("¡Comienza la noche!");
            else
                Debug.Log("¡Comienza el día!");
        }

        // Calcular alpha del panel de noche
        float alpha;
        if (!isNight) // día: alpha baja de nightAlpha ? 0
        {
            alpha = Mathf.Lerp(nightAlpha, 0f, cycleProgress * 2f);
        }
        else // noche: alpha sube de 0 ? nightAlpha
        {
            alpha = Mathf.Lerp(0f, nightAlpha, (cycleProgress - 0.5f) * 2f);
        }

        SetPanelAlpha(alpha);
    }

    void SetPanelAlpha(float alpha)
    {
        if (nightPanel != null)
        {
            Color c = nightPanel.color;
            c.a = alpha;
            nightPanel.color = c;
        }
    }

    // Método público para que otros scripts consulten si es de noche
    public bool IsNight()
    {
        return isNight;
    }
}



