using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverScene : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    void Update() => scoreText.text = $"Your score was {Stats.finalScore}. \n \n You did great !";
}
