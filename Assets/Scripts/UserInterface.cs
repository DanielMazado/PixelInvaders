using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject[] healthImages;
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

    // Actualizar vida.
    public void UpdateLife(int life) 
    {
        if(life >= 0 || life <= 3) 
        {
            for(int i = 1; i <= healthImages.Length; i++) 
            {
                if (i > life) { healthImages[i-1].SetActive(false); }
                else { healthImages[i-1].SetActive(true); }
            }
        }
    }
}
