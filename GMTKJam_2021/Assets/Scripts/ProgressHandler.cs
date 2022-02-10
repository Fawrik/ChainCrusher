using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressHandler : MonoBehaviour
{
    public int currentProgressLevel = 1;
    public int progressLevelCap;

    public float currentExp;
    public float currentExpCap;
    public float expForLvl1;
    public float expForLvl2;
    public float expForLvl3;

    public Slider slider;
    public float fillAmount;
    public TextMeshProUGUI levelText;
    void Start()
    {
        currentProgressLevel = 1;
        currentExpCap = expForLvl2;
        slider = FindObjectOfType<Slider>();
        //fillAmount = 1 / currentExpCap;
        
    }

    private void Update()
    {
        
    }


    public void UpdateExp(EnemyType enemyType)
    {
        
        if (currentExp >= currentExpCap)
        {
            CheckIfLevelUp();
            return;
        }
        VisualizeProgress();
        switch (enemyType)
        {
            case EnemyType.Lame:
                currentExp += 1;
                break;

            case EnemyType.Creep:
                currentExp += 1.5f;
                break;

            case EnemyType.Shooter:
                currentExp += 2;
                break;

            default:
                break;
        }
        
        CheckIfLevelUp();
    }


    void CheckIfLevelUp()
    {
        if (currentExp >= currentExpCap && currentProgressLevel < progressLevelCap)
        {
            UpdateProgessValues();
        }     
    }

    public void UpdateProgessValues()
    {
        currentProgressLevel++;
        currentExp = 0;
        switch (currentProgressLevel)
        {
            case 0:
                currentProgressLevel = 0;
                UpdateProgessValues();
                break;
            case 1:
                currentExpCap = expForLvl1;
                break;
            case 2:
                currentExpCap = expForLvl2;
                break;
            case 3:
                currentExpCap = expForLvl3; 

                break;
            case 4:
                //Stop leveling up
                currentExpCap = Mathf.Infinity;
                break;
            default:
                break;
        }

        slider.value = 0;
        VisualizeProgress();
    }

    void VisualizeProgress()
    {
        
        slider.value += 1 / currentExpCap;
        levelText.text = "" + currentProgressLevel;

    }
}
