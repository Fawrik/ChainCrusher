using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using System.Collections;


public class BallProjection : MonoBehaviour
{
    public Rigidbody2D ballrb;
    CarController car;
    BallCooldown ballCooldown;
    [SerializeField] float ballThrowForce = 60;
    public ParticleSystem rechargePS;
    public Volume slomoVolume;
    private EggCollision eggCollision;

    public AudioClip chargeSFX;
    public AudioClip releaseSFX;

    public GameObject chargingSoundGO; //TODO Very hacky solution, fix this asap

    public LineRenderer dottedLineRenderer;
    public float timeStamp = 0f;

    public bool slomo = false;

    void Awake()
    {
        eggCollision = FindObjectOfType<EggCollision>();
        ballrb = FindObjectOfType<EggCollision>().GetComponentInParent<Rigidbody2D>();
        car = FindObjectOfType<CarController>();
        slomoVolume = GameObject.Find("SlomoVolume").GetComponent<Volume>();
        dottedLineRenderer = GameObject.Find("DottedLineRenderer").GetComponent<LineRenderer>();
        dottedLineRenderer.enabled = false;
        rechargePS = GameObject.Find("RechargeParticleSystem").GetComponent<ParticleSystem>();
        ballCooldown = GetComponent<BallCooldown>();
    }
    public float scalar = 6f;
    void Update()
    {
        if (!ballCooldown.canShoot)
        {
            return;
        }
        SpecialAttack();
    }
    void SpecialAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && ballCooldown.canShoot)
        {
            slomo = true;
            DOTween.To(() => slomoVolume.weight, x => slomoVolume.weight = x, 1, .4f);
            Time.timeScale = .4f;
            dottedLineRenderer.enabled = true;

            chargingSoundGO.GetComponent<AudioSource>().Play();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3[] positions = new Vector3[] { eggCollision.transform.position, transform.position + (car.transform.position - eggCollision.transform.position).normalized * scalar };
            dottedLineRenderer.SetPositions(positions);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            slomo = false;
            dottedLineRenderer.SetPositions(new Vector3[] { });
            dottedLineRenderer.enabled = false;
            DOTween.To(() => slomoVolume.weight, x => slomoVolume.weight = x, 0, .4f);
            Time.timeScale = 1f;
            ThrowBall();
            ballCooldown.StartTimer();
            AudioSource.PlayClipAtPoint(releaseSFX, transform.position);
            chargingSoundGO.GetComponent<AudioSource>().Stop();
        }
    }
    void ThrowBall() //Throw the ball in the direction of the player
    {
        Vector3 direction = (car.transform.position - eggCollision.transform.position);
        ballrb.AddForce(direction * ballThrowForce, ForceMode2D.Impulse);
        StartCoroutine(nameof(HugeBall));
        StartCoroutine(nameof(IFrames));
    }
    [ColorUsage(true, true)]
    public Color normalColor;
    [ColorUsage(true, true)]
    public Color dangerColor;
    IEnumerator HugeBall()
    {
        var ballBase = GameObject.Find("EggBase").transform;
        var eggMat = FindObjectOfType<EggCollision>().GetComponent<SpriteRenderer>().sharedMaterial;

        Vector3 scaledUp = new Vector3(3, 3, 3);

        eggMat.SetColor("_Color", dangerColor);
        ballBase.DOScale(scaledUp, .1f);
        eggMat.color = Color.red;
        yield return new WaitForSeconds(.12f);
        ballBase.DOScale(Vector3.one, .1f);
        yield return new WaitForSeconds(.12f);
        eggMat.SetColor("_Color", normalColor);
    }
    IEnumerator IFrames()
    {
        var carCol = GameObject.Find("CarBase").GetComponent<Collider2D>();
        carCol.isTrigger = true;
        //temporary invincibility
        yield return new WaitForSeconds(1f);
        carCol.isTrigger = false;


    }
}