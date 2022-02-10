using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class EggCollision : MonoBehaviour
{
    private BallCooldown ballCooldown;
    private BallProjection ballProjection;
    public Color normalColor;
    public Color rechargingColor;
    public SpriteRenderer ballRD;

    public CinemachineCollisionImpulseSource inpulseSource;

    private void Start()
    {
        ballRD = GetComponent<SpriteRenderer>();
        ballCooldown = FindObjectOfType<BallCooldown>();
        ballProjection = FindObjectOfType<BallProjection>();
        inpulseSource = GetComponent<CinemachineCollisionImpulseSource>();
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        //TODO: the sreenshake didnt show up on my machine when testing, only after increasing the values to 5f for impulse did i see something. -Gereon
        if (other.collider.GetComponent<EnemyBase>() && !ballCooldown.canShoot)
        {

            ballProjection.rechargePS.Stop();
            ballProjection.rechargePS.Play();

            inpulseSource.GenerateImpulse(8f);

        }
        if (other.gameObject.GetComponent<Bullet>())
        {
            inpulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = .2f;
            inpulseSource.GenerateImpulse(1.2f);
        }
        else if (other.gameObject.GetComponent<EnemyBase>())
        {
            inpulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = .35f;
            inpulseSource.GenerateImpulse(1.75f);
        }
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

    private void Update()
    {
        ballRD.color = ballCooldown.canShoot ? normalColor : rechargingColor;
    }
}