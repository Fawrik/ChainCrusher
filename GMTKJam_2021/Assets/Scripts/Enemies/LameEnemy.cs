using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LameEnemy : EnemyBase
{
    protected Vector3 target;
    [Range(0.1f, 10)] public float movSpeed;
    

    protected override void Start()
    {
        base.Start();
        target = (player.transform.position - transform.position) * 100;
        transform.right = target;
        
        
    }

    private void Update()
    {
        if (!canmove) return;
        var step = movSpeed * Time.deltaTime;
        transform.position =
            Vector2.MoveTowards(transform.position, target, step);
    }

   
}