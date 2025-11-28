using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;    // Asigna el Transform del jugador
    public Camera cam;          // Cámara a controlar

    [Header("Movimiento")]
    public float smoothSpeed = 5f; // Velocidad de seguimiento

    private float cameraHeight;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        cameraHeight = cam.transform.position.z; // Mantener la altura actual de la cámara
    }

    void Update()
    {
        if (player == null) return;

        // Posición objetivo (solo X e Y)
        Vector3 targetPos = new Vector3(player.position.x, player.position.y, cameraHeight);

        // Lerp suave hacia el jugador
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}

