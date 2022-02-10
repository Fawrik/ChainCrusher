using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    float maxPush = 30000f;
    public float explosionRadius = 3f;
    public GameObject explosionVFX;
    public static float InvLinear(float maxVal, float length, float x)
    {
        float step = maxVal / length;
        return (-step * x) + maxVal;
    }

    public void ExplodeEnemies()
    {
        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
        explosion.GetComponent<ExplosionVFX>().ExploVFX(explosionRadius);
        print("exploded");
        
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (EnemyBase enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) > explosionRadius) continue;
            
            enemy.canmove = false;
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            rb.isKinematic = false;
            rb.mass = 15;
            rb.drag = 5;
            rb.angularDrag = 50;
            rb.gravityScale = 0;
            StartCoroutine(ResetEnemyMovement(enemy, rb));
            _Explode(rb);
        }
        IEnumerator ResetEnemyMovement(EnemyBase enemy, Rigidbody2D rb)
        {
            yield return new WaitForSeconds(1);
            if (rb != null)
            {
                rb.isKinematic = true;
                enemy.canmove = true;
            }
        }
       
    }
    void _Explode(Rigidbody2D other = null)
    {
        if (Vector3.Distance(transform.position, other.transform.position) < explosionRadius)
        {
            Vector2 toOther = other.transform.position - transform.position;
            float distance = toOther.magnitude;
            Vector2 direction = toOther.normalized;
            float pushForce = InvLinear(maxPush, explosionRadius, distance);

            Vector2 push = direction * pushForce;

           
            
            other.AddForce(push);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
