using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class EnemyBase : MonoBehaviour
{
    protected UIBrain uibrain;
    protected CameraBrain camBrain;
    protected CarController player;
    public AudioClip enemyDeathSFX01;
    public AudioClip enemyDeathSFX02;
    public AudioClip enemyDeathSFX03;
    public GameObject TextPopUpPrefab;
    public bool canmove = true;
    public ParticleSystem deathPS;
    public ProgressHandler progressHandler;

    public int Health
    {
        get => 0;
        set
        {
            if (Health <= 0) isAlive = false;
        }
    }

    public bool isAlive = true;
    public int pointValue = 100;

    protected virtual void Start()
    {
        player = FindObjectOfType<CarController>();
        uibrain = FindObjectOfType<UIBrain>();
        camBrain = FindObjectOfType<CameraBrain>();

        pointValue = pointValue + Random.Range(1, 50);
        progressHandler = FindObjectOfType<ProgressHandler>();
    }

    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Egg"))
        {
            EnemyDeath();
        }
    }

    public virtual void EnemyDeath()
    {
        uibrain.AddPoints(pointValue + Random.Range(pointValue / 5, pointValue));
        camBrain.ShakeCamera(.3f, 5f);
        GameObject textpopup = Instantiate(TextPopUpPrefab, transform.position, Quaternion.identity);
        textpopup.GetComponent<PopupText>().AssignValue(pointValue);

        //SFX Variation
        int i = Random.Range(0, 3);
        switch (i)
        {
            case 0:
                AudioSource.PlayClipAtPoint(enemyDeathSFX01, transform.position);
                break;
            case 1:
                AudioSource.PlayClipAtPoint(enemyDeathSFX02, transform.position);
                break;
            case 2:
                AudioSource.PlayClipAtPoint(enemyDeathSFX03, transform.position);
                break;

            default:
                break;
        }
        progressHandler.UpdateExp(GetComponent<EnemyMiscScript>().enemyType);
        Instantiate(deathPS, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
}