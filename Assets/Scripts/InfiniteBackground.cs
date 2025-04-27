using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    public GameObject[] objects;
    private Transform[] backgrounds;
    public float speed = 1f; // Velocidad de movimiento

    private float backgroundHeight;

    void Start()
    {
        backgrounds = new Transform[2];
        for(int i = 0; i < objects.Length; i++)
        {
            backgrounds[i] = objects[i].GetComponent<Transform>();
        }
        // Calcula la altura del fondo basándose en el primer sprite
        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        // Mover todos los fondos hacia abajo
        foreach (var bg in backgrounds)
        {
            bg.transform.position -= Vector3.up * speed * Time.deltaTime;
        }

        // Comprobar si un fondo ha salido completamente por abajo
        foreach (var bg in backgrounds)
        {
            if (bg.transform.position.y <= -backgroundHeight)
            {
                // Encontrar el fondo más alto
                Transform highestBg = GetHighestBackground();

                // Reposicionar este fondo justo encima del más alto
                bg.transform.position = new Vector3(
                    bg.transform.position.x,
                    highestBg.position.y + backgroundHeight,
                    bg.transform.position.z
                );
            }
        }
    }

    // Encuentra el fondo que está más arriba
    Transform GetHighestBackground()
    {
        Transform highest = backgrounds[0];
        foreach (var bg in backgrounds)
        {
            if (bg.position.y > highest.position.y)
                highest = bg;
        }
        return highest;
    }
}
