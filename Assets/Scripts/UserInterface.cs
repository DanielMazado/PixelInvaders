using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text enemiesText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject[] healthImages;
    [SerializeField] private Sprite[] heartImages;
    private static int SCORE = 0;
    private int prevLevel;
    private PlayerHealth playerHealth;

    // Para actualizar la puntuación y curar al jugador al inicio de una escena.
    private void Start()
    {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        playerHealth.FullHeal();

        if(EnemySpawner.Instance != null)
        {
            prevLevel = EnemySpawner.Instance.GetLevelID();
            levelText.text = prevLevel.ToString("D1");
        }
        
        scoreText.text = SCORE.ToString("D5");
    }

    private void Update()
    {
        if(EnemySpawner.Instance != null)
        {
            if(prevLevel != EnemySpawner.Instance.GetLevelID())
            {
                prevLevel = EnemySpawner.Instance.GetLevelID();
                levelText.text = prevLevel.ToString("D1");
            }
        }
        else
        {
            if(levelText.text != "X")
            {
                levelText.text = "X";
            }
        }
    }

    // Actualizar puntuación.
    public void AddScore(int amount = 0)
    {
        SCORE += amount;
        if(SCORE < 0) { SCORE = 0; }
        scoreText.text = SCORE.ToString("D5");
    }

    public int GetScore() { return SCORE; }

    // Actualizar enemigos restantes.
    public void UpdateEnemiesLeft(int amount, bool bossSpawned = false)
    {
        amount--;
        if(amount > -2)
        {
            enemiesText.text = "Enemies Remaining: " + amount.ToString("D2");
        }
        else
        {
            if(!bossSpawned)
            {
                enemiesText.text = "Tough Foe Incoming!";
            }
            else
            {
                enemiesText.text = "HP Remaining: 100";
            }
        }
    }

    // Actualizar vida del jefe.

    public void UpdateBossHP(int healthToSet)
    {
        if(healthToSet >= 0)
        {
            enemiesText.text = "HP Remaining: " + healthToSet;
        }
    }

    // Actualizar vida.
    public void UpdateLife(int life) 
    {
        if(life >= 0 || life <= 3) 
        {
            for(int i = 1; i <= healthImages.Length; i++) 
            {
                if (i > life)
                { 
                    healthImages[i-1].GetComponent<UnityEngine.UI.Image>().sprite = heartImages[0];
                }
                else 
                { 
                    healthImages[i-1].GetComponent<UnityEngine.UI.Image>().sprite = heartImages[1];
                }
            }
        }
    }
}