using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour //Mostly unused class
{
    public int maxHP = 3;
    public int HP;
    [HideInInspector] public Chain chain;

    void Start()
    {
        HP = maxHP;
    }

    public void Damage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            if (chain != null) chain.DisconnectObject();
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        chain = FindObjectOfType<Chain>();
    }
}