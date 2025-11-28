using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCController : MonoBehaviour
{
    [Header("Configuración NPC")]
    public string npcName = "Superviviente";
    public float interactionRange = 3f;
    private DialogUI dialogUI;

    [Header("Referencias")]
    public GameObject interactionIndicator;
    public Transform player;

    [Header("Diálogo")]
    public string[] firstMissionDialog = {
    "¡Gracias a Dios que llegaste! La ciudad ha estado bajo el control de los zombies por semanas...",
    "Los supervivientes estamos escondidos, pero necesitamos ayuda para recuperar el área.",
    "¿Podrías ayudarnos a eliminar algunos de estos monstruos?"
    };

    public string[] secondMissionDialog = {
    "¡Increíble! Lograste eliminar a esos zombies...",
    "Pero hay más amenazas por ahí. Necesitamos que limpies un área más grande.",
    "¿Estás listo para un desafío mayor?"
};

    public string[] thirdMissionDialog = {
    "Eres todo un héroe... La zona está mucho más segura ahora.",
    "Pero tenemos un problema: hay una horda acercándose.",
    "Necesitamos que sobrevivas el mayor tiempo posible. ¿Puedes hacerlo?"
};

    public string[] allMissionsCompletedDialog = {
    "¡Eres increíble! Has completado todas las misiones.",
    "La ciudad está mucho más segura gracias a ti.",
    "Vuelve más tarde, quizás tengamos más trabajo para ti."
};

    private bool isPlayerInRange = false;
    private List<MissionData> availableMissions = new List<MissionData>();
    private bool canInteract = true;

    void Start()
    {
        dialogUI = FindObjectOfType<DialogUI>();

        if (interactionIndicator != null)
            interactionIndicator.SetActive(false);

        // Obtener misiones disponibles del MissionManager
        UpdateAvailableMissions();
    }

    void Update()
    {
        if (player == null) return;

        // Verificar distancia con el player
        float distance = Vector2.Distance(transform.position, player.position);
        bool wasInRange = isPlayerInRange;
        isPlayerInRange = distance <= interactionRange;

        // Mostrar/ocultar indicador
        if (interactionIndicator != null)
        {
            if (isPlayerInRange && !wasInRange)
            {
                interactionIndicator.SetActive(true);
                UpdateInteractionPrompt();
            }
            else if (!isPlayerInRange && wasInRange)
                interactionIndicator.SetActive(false);
        }

        // Interacción con E, Click o Gamepad
        if (isPlayerInRange && canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame ||
                Mouse.current.leftButton.wasPressedThisFrame ||
                (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame))
            {
                InteractWithPlayer();
            }
        }
    }

    void UpdateInteractionPrompt()
    {
        // Alternativa si usas UI Canvas en lugar de TextMeshPro
        if (interactionIndicator != null)
        {
            UnityEngine.UI.Text promptText = interactionIndicator.GetComponentInChildren<UnityEngine.UI.Text>();
            if (promptText != null)
            {
                if (Gamepad.current != null)
                    promptText.text = "Presiona A para hablar";
                else
                    promptText.text = "Presiona E para hablar";
            }
        }
    }
    void UpdateAvailableMissions()
    {
        if (MissionManager.Instance != null)
        {
            availableMissions = MissionManager.Instance.GetAvailableMissions();
            Debug.Log($"Misiones disponibles en NPC: {availableMissions.Count}");
        }
    }

    void InteractWithPlayer()
    {
        if (!canInteract) return;

        canInteract = false;

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null) playerMovement.enabled = false;

        UpdateAvailableMissions();

        if (dialogUI != null)
        {
            string[] dialogToUse = GetAppropriateDialog();
            MissionData missionToOffer = availableMissions.Count > 0 ? availableMissions[0] : null;

            dialogUI.ShowDialog(npcName, dialogToUse, missionToOffer, OnMissionDecision);
        }
        else
        {
            Debug.LogError("DialogUI no encontrado!");
            // Reactivar movimiento si no hay UI
            if (playerMovement != null) playerMovement.enabled = true;
            canInteract = true;
        }
    }

    string[] GetAppropriateDialog()
    {
        // Verificar progreso de misiones
        bool firstCompleted = IsMissionCompleted("KILL_10_ZOMBIES");
        bool secondCompleted = IsMissionCompleted("KILL_25_ZOMBIES");
        bool thirdCompleted = IsMissionCompleted("SURVIVE_3_MINUTES");

        // Determinar qué diálogo usar
        if (!firstCompleted)
            return firstMissionDialog;
        else if (firstCompleted && !secondCompleted)
            return secondMissionDialog;
        else if (secondCompleted && !thirdCompleted)
            return thirdMissionDialog;
        else
            return allMissionsCompletedDialog;
    }

    bool IsMissionCompleted(string missionID)
    {
        if (MissionManager.Instance != null)
        {
            MissionData mission = MissionManager.Instance.allMissions.Find(m => m.missionID == missionID);
            return mission != null && mission.state == MissionState.Completed;
        }
        return false;
    }

    List<MissionData> GetCompletedMissions()
    {
        if (MissionManager.Instance != null)
        {
            return MissionManager.Instance.GetCompletedMissions();
        }
        return new List<MissionData>();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    void OnMissionDecision(bool accepted)
    {
        // Reactivar movimiento del player
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null) playerMovement.enabled = true;

        if (accepted && availableMissions.Count > 0)
        {
            MissionManager.Instance.AcceptMission(availableMissions[0].missionID);
            Debug.Log("¡Misión aceptada!");
        }
        else
        {
            Debug.Log("Misión rechazada");
        }

        canInteract = true; // Permitir interacción nuevamente
    }
}
