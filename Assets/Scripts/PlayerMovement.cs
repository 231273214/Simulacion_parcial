using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;

    private Vector2 moveInput;
    public Animator animator;
    private Rigidbody2D rb; // AGREGAR ESTA LÍNEA

    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // AGREGAR ESTA LÍNEA
        InputManager.Instance.OnMove += HandleMove;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnMove -= HandleMove;
    }

    void HandleMove(Vector2 input)
    {
        moveInput = input;
    }

    void Update()
    {
        // Movimiento del jugador
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0);
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // Animaciones
        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);

        // Aquí la velocidad REAL para Idle/Walk/Run
        animator.SetFloat("Speed", movement.magnitude);

        // Flip sólo si estás moviéndote horizontalmente
        HandleFlip();
    }

    public bool IsMoving()
    {
        // SOLO usar moveInput ya que no estás usando Rigidbody para movimiento
        return moveInput.magnitude > 0.1f;
    }

    void HandleFlip()
    {
        if (moveInput.x > 0 && !isFacingRight)
            Flip();
        else if (moveInput.x < 0 && isFacingRight)
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}