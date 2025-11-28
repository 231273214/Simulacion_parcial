using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogPanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogText;
    public Button acceptButton;
    public Button rejectButton;

    [Header("Configuración")]
    public float typingSpeed = 0.05f;

    private MissionData currentMission;
    private System.Action<bool> onDecisionMade;
    private bool buttonsActive = false;

    void Start()
    {
        dialogPanel.SetActive(false);

        // Asignar listeners a los botones
        if (acceptButton != null)
            acceptButton.onClick.AddListener(() => OnDecision(true));

        if (rejectButton != null)
            rejectButton.onClick.AddListener(() => OnDecision(false));
    }

    void Update()
    {
        // Solo procesar input si los botones están activos y el diálogo visible
        if (!buttonsActive || !dialogPanel.activeInHierarchy) return;

        // Input de teclado y gamepad
        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame))
        {
            OnDecision(true);
        }
        else if (Keyboard.current.escapeKey.wasPressedThisFrame ||
                 (Gamepad.current != null && Gamepad.current.bButton.wasPressedThisFrame))
        {
            OnDecision(false);
        }
    }

    public void ShowDialog(string npcName, string[] dialogLines, MissionData mission, System.Action<bool> callback)
    {
        currentMission = mission;
        onDecisionMade = callback;

        dialogPanel.SetActive(true);
        npcNameText.text = npcName;

        StartCoroutine(TypeDialogSequence(dialogLines));
    }

    IEnumerator TypeDialogSequence(string[] dialogLines)
    {
        // Ocultar botones al inicio
        if (acceptButton != null) acceptButton.gameObject.SetActive(false);
        if (rejectButton != null) rejectButton.gameObject.SetActive(false);
        buttonsActive = false;

        // Mostrar diálogo letra por letra
        foreach (string line in dialogLines)
        {
            dialogText.text = "";
            foreach (char letter in line.ToCharArray())
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(1f);
        }

        // Mostrar botones al final del diálogo
        if (acceptButton != null) acceptButton.gameObject.SetActive(true);
        if (rejectButton != null) rejectButton.gameObject.SetActive(true);
        buttonsActive = true;

        // Seleccionar automáticamente para gamepad
        if (Gamepad.current != null && acceptButton != null)
        {
            acceptButton.Select();
        }
    }

    public void OnDecision(bool accepted)
    {
        if (!buttonsActive) return;

        dialogPanel.SetActive(false);
        onDecisionMade?.Invoke(accepted);
        currentMission = null;
        onDecisionMade = null;
        buttonsActive = false;
    }
}