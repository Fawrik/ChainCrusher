using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiscScript : MonoBehaviour
{

    public bool isActive;
    public EnemyBase enemyBase;
    public EnemyType enemyType;

    private void Start()
    {
        GetOutLineColor();
        enemyBase = GetComponent<EnemyBase>();
    }

    void GetOutLineColor()
    {
        //outLineColor = GetComponent<SpriteRenderer>().material.color;
    }

   void ActivateEnemy()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            enemyBase = GetComponent<EnemyBase>();
        }

        
    }
}

public enum EnemyType { Lame, Creep, Shooter}
