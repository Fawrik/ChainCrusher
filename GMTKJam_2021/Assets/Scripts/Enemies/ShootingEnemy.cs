using System.Collections;
using UnityEngine;

public class ShootingEnemy : CreepEnemy
{
    public GameObject projectile;
    public float projectileSpeed = 1f;
    public float shootingRate = 3;

    private void Awake()
    {
        StartCoroutine(DelayShoot());
    }

    IEnumerator DelayShoot()
    {
        yield return new WaitForSeconds(2.1f);
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        GameObject bulletClone = Instantiate(projectile, transform.position, Quaternion.identity);
        if (!canmove)
        {
            yield return new WaitForSeconds(10);
        }
        else
        {
            yield return new WaitForSeconds(shootingRate);
        }

        StartCoroutine(Shoot());
    }
}