using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour

{
    float speedX;
    float speedY;

    private Vector2 velocity;

    public float maxSpeed; //Velocidad m�xima de la nave
    public float minSpeed; //Velocidad m�nima, si la velocidad est� por debajo de este valor mientras no se est� pulsando un direcci�n, esta se reduce a 0

    public float snapFactor; //Si la nave cambia de direcci�n de manera brusca, se aplica este valor como bonus a la aceleraci�n para vencer la velocidad anterior

    public float baseAcceleration; //Aceleraci�n base de la nave
    public float brakingSpeed; //Factor que determina el ritmo al que decrece la velocidad al dejar de mover la nave

    void Update()
    {
        Vector3 pos = transform.position;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input.Normalize();
        input.x *= math.abs(Input.GetAxis("Horizontal"));
        input.y *= math.abs(Input.GetAxis("Vertical"));

        speedX = speedCalculator(speedX, input.x);
        speedY = speedCalculator(speedY, input.y);

        velocity = new Vector2(speedX, speedY);
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
            speedX = velocity.x;
            speedY = velocity.y;
        }

        pos.x += speedX * Time.deltaTime;
        pos.y += speedY * Time.deltaTime;

        transform.position = pos;

    }

    float speedCalculator(float speed, float input)
    {
        float acceleration;

        if (input != 0)
        {
            if (input * speed >= 0)
            {
                acceleration = baseAcceleration * input;
            }
            else
            {
                acceleration = baseAcceleration * snapFactor * input; //Factor de rebote
            }
        }
        else
        {
            acceleration = -speed * brakingSpeed; // Factor de frenado
        }

        speed += Time.deltaTime * acceleration;

        // Detener la velocidad si es muy peque�a
        if (Mathf.Abs(speed) < minSpeed && input == 0)
        {
            speed = 0;
        }

        return Mathf.Clamp(speed, -maxSpeed, maxSpeed); // Limita la velocidad
    }
}
