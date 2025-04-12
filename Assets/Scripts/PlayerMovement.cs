using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 velocity;
    private Rigidbody2D rb;

    // Parámetros de movimiento
    private float maxSpeed = 4f;  // Velocidad máxima
    private float acceleration = 4f;  // Aceleración
    private float deceleration = 2f;  // Deceleración (frenado)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Obtener los inputs del jugador
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input.Normalize(); // Normalizar para que el movimiento diagonal no sea más rápido

        // Aplicar el movimiento
        ApplyMovement(input);
    }

    // Método para aplicar el movimiento basado en la entrada
    void ApplyMovement(Vector2 input)
    {
        // Aceleración: aumentar la velocidad en la dirección del input
        if (input.magnitude > 0)
        {
            velocity = Vector2.MoveTowards(velocity, input * maxSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // Frenado (deceleración)
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, deceleration * Time.deltaTime);
        }

        // Aplicar la velocidad calculada al Rigidbody
        rb.velocity = velocity;
    }
}
