using System;
using UnityEngine;

public class CreepEnemy : EnemyBase
{
    [Range(.01f, 10)] public float moveSpeed;

    private void Update()
    {
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (!canmove) return;
        var step = moveSpeed * Time.deltaTime;
        transform.position =
            Vector2.MoveTowards(transform.position, player.transform.position, step);
        transform.right = player.transform.position - transform.position;
    }
}