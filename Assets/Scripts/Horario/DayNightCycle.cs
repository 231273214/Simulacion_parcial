using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0f, 24f)]
    public float timeOfDay = 12f;   
    public float dayLengthInMinutes = 10f;

    [Header("Lighting")]
    public Light sunLight;
    public Gradient lightColor;
    public AnimationCurve lightIntensity;

    [Header("2D Overlay")]
    public CanvasGroup nightOverlay;   

    public static DayNightCycle Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Avanza el tiempo del día
        timeOfDay += (24f / (dayLengthInMinutes * 60f)) * Time.deltaTime;
        if (timeOfDay >= 24f)
            timeOfDay = 0f;

        // Calcula qué tan de noche es
        float nightFactor = GetNightFactor();

        // Aplica el oscurecimiento
        if (nightOverlay != null)
            nightOverlay.alpha = Mathf.Lerp(0f, 0.7f, nightFactor);

        // Iluminación (opcional si usas Directional Light)
        UpdateLighting();
    }

    public bool IsNight()
    {
        // Consideramos noche entre 18:00 (6 PM) y 6:00 AM
        return timeOfDay >= 18f || timeOfDay <= 6f;
    }


    void UpdateLighting()
    {
        if (sunLight == null) return;

        float t = timeOfDay / 24f;

        // --- Luz del sol (color + base intensity desde curvas) ---
        Color sunCol = lightColor.Evaluate(t);
        float baseIntensity = lightIntensity.Evaluate(t);

        // Multiplicador para hacer la noche más oscura (ajusta 0.0 - 1.0)
        // valores recomendados: 0.0 = sin oscuridad extra, 0.5 = moderado, 0.85 = muy oscuro
        float nightDarkness = 0.85f;
        float nightFactor = GetNightFactor(); // 0 = día, 1 = noche

        // Reduce la intensidad del sol en la noche de forma no lineal para más contraste
        float intensity = Mathf.Lerp(baseIntensity, baseIntensity * (1f - nightDarkness), nightFactor);
        sunLight.color = Color.Lerp(sunCol, sunCol * 0.5f, nightFactor * 0.7f); // tiñe la luz más fría en la noche
        sunLight.intensity = intensity;

        // Rotación del sol (mismo que antes)
        sunLight.transform.rotation = Quaternion.Euler(
            (timeOfDay / 24f) * 360f - 90f,
            170f,
            0f
        );

        // --- Ambient (iluminación global) ---
        // Día claro -> casi blanco; noche -> azul muy oscuro
        Color dayAmbient = new Color(0.65f, 0.65f, 0.7f);
        Color nightAmbient = new Color(0.02f, 0.04f, 0.08f);
        RenderSettings.ambientLight = Color.Lerp(dayAmbient, nightAmbient, nightFactor);

        // --- Skybox exposure (si tienes uno que soporte _Exposure) ---
        if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_Exposure"))
        {
            // reduce exposición por la noche
            float skyExp = Mathf.Lerp(1.0f, 0.25f, nightFactor);
            RenderSettings.skybox.SetFloat("_Exposure", skyExp);
        }

        // --- Overlay UI: soporta CanvasGroup (recomendado) o Image (panel) ---
        if (nightOverlay != null)
        {
            // Si es CanvasGroup, usa alpha; si es Image, ajusta color alpha.
            var cg = nightOverlay as CanvasGroup;
            if (cg != null)
            {
                // Amplifica la oscuridad máxima con un multiplicador
                float maxAlpha = 0.85f; // ajustable: 0.7 - 0.95 para noches muy oscuras
                cg.alpha = Mathf.Lerp(0f, maxAlpha, nightFactor);
            }
            else
            {
                // Si en lugar de CanvasGroup asignaste un Image (panel), intenta obtenerlo:
                Image img = null;
                try { img = ((Image)(object)nightOverlay); } catch { img = null; }
                if (img != null)
                {
                    float maxAlpha = 0.85f;
                    Color c = img.color;
                    c.a = Mathf.Lerp(0f, maxAlpha, nightFactor);
                    img.color = c;
                }
            }
        }
    }

    // Devuelve 0..1 (0 día, 1 noche) con transiciones suaves en crepúsculos
    float GetNightFactor()
    {
        // Duraciones y rangos para transiciones - ajústalos como prefieras
        float nightStart = 18f;   // 18:00 empieza anochecer
        float nightFull = 21f;    // 21:00 noche completa
        float dawnStart = 4f;     // 04:00 noche aún
        float dawnEnd = 7f;       // 07:00 amanecer completo

        // Si estamos en la porción de noche "central"
        if (timeOfDay >= nightFull || timeOfDay <= dawnStart)
            return 1f;

        // Anochecer suave 18 -> 21
        if (timeOfDay >= nightStart && timeOfDay < nightFull)
            return Mathf.InverseLerp(nightStart, nightFull, timeOfDay);

        // Amanecer suave 04 -> 07 (inversa)
        if (timeOfDay > dawnStart && timeOfDay <= dawnEnd)
            return 1f - Mathf.InverseLerp(dawnStart, dawnEnd, timeOfDay);

        return 0f;
    }
}
