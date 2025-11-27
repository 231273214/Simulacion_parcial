using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interacción")]
    public float interactRange = 1.5f;
    public LayerMask interactableLayer;

    [Header("UI")]
    public GameObject interactionUIPanel;
    public TextMeshProUGUI interactionText;

    void Start()
    {
        InputManager.Instance.OnInteract += HandleInteract;

        if (interactionUIPanel != null)
            interactionUIPanel.SetActive(false);
    }

    void Update()
    {
        // Mostrar texto si hay un interactuable en rango
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, interactableLayer);
        if (hit != null)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null && interactionUIPanel != null && interactionText != null)
            {
                interactionText.text = interactable.GetInteractText();
                interactionUIPanel.SetActive(true);
                return;
            }
        }

        if (interactionUIPanel != null)
            interactionUIPanel.SetActive(false);
    }

    void HandleInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, interactableLayer);
        if (hit != null)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null) interactable.Interact(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}

