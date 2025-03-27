using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    private static float speed = 2f; // Velocidad de movimiento
    private static float resetY = -18.3f; // Límite inferior
    private static float startY = 34.81f; // Posición exacta donde se teletransporta
    private static float epsilon = 0.01f; // Margen de error mínimo para evitar desfases

    void Update()
    {
        // Mover el fondo hacia abajo
        transform.position -= Vector3.up * speed * Time.deltaTime;

        // Si el fondo llega al límite inferior, teletransportarlo exactamente arriba
        if (transform.position.y <= resetY + epsilon)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Round(startY * 100f) / 100f, transform.position.z);
        }
    }
}