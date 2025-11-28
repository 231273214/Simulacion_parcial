using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 2f;
    public LayerMask interactableLayer; 

    private PlayerController controls;

    void Awake()
    {
        controls = new PlayerController();
        controls.Player.Interact.performed += ctx => TryInteract();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void TryInteract()
    {
        // Detecta todos los objetos dentro del rango
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, interactableLayer);

        if (hit == null)
        {
            Debug.Log("Nada para interactuar en el círculo.");
            return;
        }

        IInteractable interactable = hit.GetComponent<IInteractable>();

        if (interactable == null)
        {
            Debug.Log("El objeto no tiene IInteractable");
            return;
        }

        Debug.Log("Interactuando con: " + hit.name);

        // Llama al método del objeto interactivo
        interactable.Interact(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
