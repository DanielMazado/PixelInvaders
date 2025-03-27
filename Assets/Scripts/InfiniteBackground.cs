using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    private static float speed = 2f; // Velocidad de movimiento
    private static float resetY = -18.3f; // Límite inferior
    private static float startY = 34.81f; // Posición a la que se teletransportará

    void Update()
    {
        // Mover el fondo hacia abajo
        transform.position -= Vector3.up * speed * Time.deltaTime;

        // Si el fondo alcanza el límite inferior, se teletransporta arriba
        if (transform.position.y <= resetY)
        {
            transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        }
    }
}