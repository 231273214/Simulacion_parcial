using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

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

    void Start()
    {
        dialogPanel.SetActive(false);
        acceptButton.onClick.AddListener(() => OnDecision(true));
        rejectButton.onClick.AddListener(() => OnDecision(false));
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
        acceptButton.gameObject.SetActive(false);
        rejectButton.gameObject.SetActive(false);

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

        acceptButton.gameObject.SetActive(true);
        rejectButton.gameObject.SetActive(true);
    }

    void OnDecision(bool accepted)
    {
        dialogPanel.SetActive(false);
        onDecisionMade?.Invoke(accepted);
        currentMission = null;
        onDecisionMade = null;
    }

    IEnumerator TypeDialogSequence(string[] dialogLines, bool showButtons)
    {
        acceptButton.gameObject.SetActive(false);
        rejectButton.gameObject.SetActive(false);

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

        // Solo mostrar botones si hay misión para ofrecer
        if (showButtons)
        {
            acceptButton.gameObject.SetActive(true);
            rejectButton.gameObject.SetActive(true);
        }
        else
        {
            // Si no hay misión, esperar 3 segundos y cerrar automáticamente
            yield return new WaitForSeconds(3f);
            OnDecision(false);
        }
    }
}
