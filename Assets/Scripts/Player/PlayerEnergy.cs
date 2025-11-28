using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Configuración de Energía")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyDrainRate = 2f; 
    public float lowEnergyThreshold = 30f;

    [Header("Efectos de Cansancio")]
    public float slowSpeedMultiplier = 0.6f;
    public float minSpeedBeforeExhaustion = 2f;

    [Header("UI")]
    public Slider energyBar;
    public Image energyBarFill;
    public Color normalColor = Color.green;
    public Color lowEnergyColor = Color.yellow;
    public Color criticalColor = Color.red;

    [Header("Recarga por Descanso")]
    public float energyRestoreRate = 10f; 
    public float timeToStartRestoring = 0.2f;

    private float idleTimer = 0f;
    private bool isRestoringEnergy = false;

    private PlayerMovement playerMovement;
    private AudioSource audioSource;
    private float originalSpeed;
    private bool isExhausted = false;
    private bool lowEnergyWarningPlayed = false;

    void Start()
    {
        currentEnergy = maxEnergy;
        playerMovement = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();

        if (playerMovement != null)
        {
            originalSpeed = playerMovement.moveSpeed;
        }

        UpdateUI();
    }

    void Update()
    {
        CheckIfIdle();

        // Si está quieto el tiempo suficiente, recargar energía
        if (currentEnergy > 0 && !isRestoringEnergy)
        {
            currentEnergy -= energyDrainRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        }
        if (isRestoringEnergy && currentEnergy < maxEnergy)
        {
            currentEnergy += energyRestoreRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        }

        CheckEnergyStatus();
        UpdateUI();

        // Debug
        if (Input.GetKeyDown(KeyCode.T))
        {
            AddEnergy(20);
        }

        if (currentEnergy > 0)
        {
            currentEnergy -= energyDrainRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        }

        
        CheckEnergyStatus();
        UpdateUI();

        // Debug
        if (Input.GetKeyDown(KeyCode.T)) 
        {
            AddEnergy(20);
        }
    }

    void CheckIfIdle()
    {
        if (playerMovement != null)
        {
            bool isMoving = playerMovement.IsMoving();

            if (!isMoving)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= timeToStartRestoring && !isRestoringEnergy)
                {
                    isRestoringEnergy = true;
                    Debug.Log("Comenzando recarga rápida de energía");
                }
            }
            else
            {
                idleTimer = 0f;
                isRestoringEnergy = false;
            }
        }
    }

    void CheckEnergyStatus()
    {
        if (currentEnergy <= lowEnergyThreshold && !lowEnergyWarningPlayed)
        {
            LowEnergyWarning();
            if (playerMovement != null)
            {
                // Reducir velocidad mientras la energía esté baja
                playerMovement.moveSpeed = Mathf.Max(originalSpeed * slowSpeedMultiplier, minSpeedBeforeExhaustion);
                isExhausted = true;
            }
        }
        else if (currentEnergy > lowEnergyThreshold)
        {
            lowEnergyWarningPlayed = false;

            if (playerMovement != null)
            {
                // Restaurar velocidad original cuando la energía ya no esté baja
                playerMovement.moveSpeed = originalSpeed;
                isExhausted = false;
            }
        }
    }

    public void EatFood(float energyAmount, float poisonChance = 0.2f)
    {
        bool isPoisoned = Random.value < poisonChance; // 20% de probabilidad de estar envenenada
        if (isPoisoned)
        {
            currentEnergy -= energyAmount; // resta energía si es venenosa
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            Debug.Log($"¡Comida envenenada! Pierdes {energyAmount} de energía.");
        }
        else
        {
            AddEnergy(energyAmount); // recupera energía normalmente
            Debug.Log($"Comida buena! Recuperas {energyAmount} de energía.");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            EatFood(20f, 0.25f); // 25% de probabilidad de que la comida sea venenosa
        }

    }



    void LowEnergyWarning()
    {
        lowEnergyWarningPlayed = true;
        Debug.Log("¡Energía baja! Encuentra comida.");
    }

    public void AddEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        Debug.Log($"Energía recuperada: +{amount}");
    }

    void UpdateUI()
    {
        if (energyBar != null)
        {
            energyBar.value = currentEnergy / maxEnergy;
        }

        if (energyBarFill != null)
        {
            if (currentEnergy <= 0)
                energyBarFill.color = criticalColor;
            else if (currentEnergy <= lowEnergyThreshold)
                energyBarFill.color = lowEnergyColor;
            else
                energyBarFill.color = normalColor;
        }
    }

    
    public bool IsExhausted()
    {
        return isExhausted;
    }

    public float GetEnergyPercentage()
    {
        return currentEnergy / maxEnergy;
    }
}
