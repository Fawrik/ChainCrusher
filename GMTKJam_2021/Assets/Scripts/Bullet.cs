using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using System.Reflection;
using DG.Tweening;
using System.Collections.Generic;

//TODO: bullets persist after being reflected and colliding with enemy (but its really not important to fix rn bc it has no gameplay impact and also looks funny) -Gereon

public class Bullet : MonoBehaviour
{
    private BallCooldown ballCooldown;
    private BallProjection ballProjection;
    private EnemyBase shooter;
    private float bulletSpeed = 1;
    private bool reflected = false;
    public bool isPartOfChainReaction = false;
    public GameObject TextPopUpPrefab;
    private Vector3 targetPosition;
    private Vector3 reflectionLocation;
    public ParticleSystem reflectPS;
    public AudioClip bulletReflectSFX01;
    public AudioClip bulletReflectSFX02;
    public AudioClip bulletReflectSFX03;

    void Start()
    {
        ballCooldown = FindObjectOfType<BallCooldown>();
        ballProjection = FindObjectOfType<BallProjection>();

        targetPosition = (FindObjectOfType<CarController>().transform.position - transform.position) * 100;

        //Find a reference to the enemy that just shot that bullet
        var scanzone = Physics2D.OverlapCircleAll(transform.position, .2f);
        foreach (var colliderfound in scanzone) if (colliderfound.GetComponent<EnemyBase>()) shooter = colliderfound.GetComponent<EnemyBase>();

        Destroy(gameObject, 15); //bullet destroys itself after a while
    }

    void Update()
    {
        BulletTrajectory();
    }

    void BulletTrajectory()
    {
        float step = bulletSpeed * Time.deltaTime;
        //destroy if stationary
        if (transform.position == targetPosition) { Destroy(gameObject); }
        else { transform.position = Vector2.MoveTowards(transform.position, targetPosition, step); }



        if (shooter == null) shooter = FindObjectOfType<EnemyBase>();

        if (reflected && Vector3.Distance(transform.position, shooter.transform.position) < .1f)
        {
            shooter.EnemyDeath();
            int bonusPoints = Random.Range(50, 80);
            FindObjectOfType<UIBrain>().AddPoints(bonusPoints);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Egg")) //Check that the collision was with the ball
        {

            BulletIsReflected(other);
            if (!ballCooldown.canShoot)
            {
                MultiDestruction();
                ballCooldown.ResetTimer();
                print("its free real estate");
            }


            BallColorSwitch();
        }
        else
        {
            Destroy(gameObject);
        }
        void MultiDestruction()
        {
            Collider2D[] bulletsInTheArea = Physics2D.OverlapCircleAll(transform.position, 8);
            List<Bullet> bulletsInRadius = new List<Bullet>();
            foreach (var bullet in bulletsInTheArea)
            {
                if (bullet.GetComponent<Bullet>())
                {
                    bulletsInRadius.Add(bullet.GetComponent<Bullet>());
                }
            }
            foreach (Bullet bullet in bulletsInRadius)
            {
                bullet.isPartOfChainReaction = true;
                bullet.BulletIsReflected(other);
            }
        }
    }

    void BulletIsReflected(Collision2D other)
    {
        reflected = true;
        bulletSpeed *= 20; //If bullet is reflected, it goes woooosh in the face of the enemy
        gameObject.layer = 13; //put reflected bullet on a layer where it can't hurt the player anymore
        var newTrail = CopyComponent.GetCopyOf<TrailRenderer>(GameObject.Find("CarTrails").GetComponentInChildren<TrailRenderer>(), GameObject.Find("CarTrails").GetComponentInChildren<TrailRenderer>());
        if (!GetComponent<TrailRenderer>())
        {
            TrailRenderer t = gameObject.AddComponent<TrailRenderer>(newTrail);
            t.material = new Material(GameObject.Find("CarTrails").GetComponentInChildren<TrailRenderer>().material);

        }


        if (isPartOfChainReaction)
        {
            EnemyBase[] enemiesInScene = FindObjectsOfType<EnemyBase>();
            var enemyChoosen = enemiesInScene[Random.Range(0, enemiesInScene.Length)];
            shooter = enemyChoosen;
            targetPosition = enemyChoosen.transform.position;
        }
        else
        {
            if (shooter != null)
            {

                targetPosition = shooter.transform.position;
            }
            else if (FindObjectOfType<EnemyBase>())
            {
                shooter = FindObjectOfType<EnemyBase>();
                targetPosition = FindObjectOfType<EnemyBase>().transform.position;
            }
            else Destroy(gameObject);


            if (ballCooldown.canShoot)
            {
                ballProjection.rechargePS.Stop();
                ballProjection.rechargePS.Play();
            }
        }

        reflectionLocation = other.GetContact(0).point;
        GameObject textpopup = Instantiate(TextPopUpPrefab, reflectionLocation, Quaternion.identity);
        textpopup.GetComponentInChildren<TextMeshProUGUI>().colorGradient =
            new VertexGradient(Color.red, Color.red, Color.yellow, Color.yellow);
        int bonusPoints = Random.Range(500, 800);
        textpopup.GetComponent<PopupText>().AssignValue(bonusPoints);
        Instantiate(reflectPS, transform.position, Quaternion.identity);

        int i = Random.Range(0, 3);
        switch (i)
        {
            case 0:
                AudioSource.PlayClipAtPoint(bulletReflectSFX01, transform.position);
                break;
            case 1:
                AudioSource.PlayClipAtPoint(bulletReflectSFX02, transform.position);
                break;
            case 2:
                AudioSource.PlayClipAtPoint(bulletReflectSFX03, transform.position);
                break;

            default:
                break;
        }

        //TODO: freeze frame


    }
    public void BallColorSwitch()
    {
        GetComponent<SpriteRenderer>().color = Color.cyan; //Handle the rest of the bullet state, mostly plumbing code

    }

    void OnEnable()
    {
        MusicHandler.OnBeat += Bounce;
    }
    void OnDisable()
    {
        MusicHandler.OnBeat -= Bounce;
    }

    public void Bounce()
    {
        transform.DOPunchScale(Vector3.one * .7f, .2f, 10, 1);
    }
    private void OnDrawGizmos()
    {
        foreach (Bullet item in FindObjectsOfType<Bullet>())
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(item.transform.position, 5);
        }
    }
}

public static class CopyComponent
{
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }
    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }
}