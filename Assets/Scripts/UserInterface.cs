using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    private static int SCORE = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Actualizar puntuación.
    public void AddScore(int amount = 0)
    {
        SCORE += amount;
        if(SCORE < 0) { SCORE = 0; }
        scoreText.text = "SCORE: " + SCORE.ToString("D5");
    }
}
