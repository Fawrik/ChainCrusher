using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBrain : MonoBehaviour
{
    public int playerScore;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI healthText;
    public CarHealth carHealth;

    private void Start()
    {
        carHealth = FindObjectOfType<CarHealth>();
        playerScore = 0;
        Stats.ResetScore();
    }

    private void Update()
    {
        pointsText.text = $"Score : {playerScore.ToString()}";
        float perc = ((float)carHealth.currentHealth / (float)carHealth.maxHealth) * 100;
        healthText.text = $"Health : {(int)perc} %";
        
    }
    public void AddPoints(int value)
    {
        playerScore += value;
    }
}
