using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Cinemachine;


public class CarHealth : MonoBehaviour
{
    public AudioClip playerDeathSFX;
    private EggCollision eggObject;
    private BlackHole blackHole;
    private Volume ppvolume;
    public GameObject playerHitVFX;
    private Animator playerAnim;
    public CameraBrain camBrain;
    private MusicHandler musicHandler;
    public int maxHealth;
    public int currentHealth;
    private Vector3 screenCenter = new Vector3(0, 5, 0);

    float explosionRadius = 3f;
    public float damageInfluence = 0;

    float currentBlackHoleParticleSpeed = 1f;
    float currentBlackHoleParticleSize = .02f;
    float currentBLackHoleParticleOrbital = .2f;
    float currentBLackHoleParticleOrbitalOffset = .5f;

    public GameObject health01;
    public GameObject health02;
    public GameObject health03;
    public Material healthMat;

    private void Start()
    {
        eggObject = FindObjectOfType<EggCollision>();
        blackHole = FindObjectOfType<BlackHole>();
        playerAnim = GetComponent<Animator>();
        ppvolume = GameObject.Find("Volume2").GetComponent<Volume>();
        currentHealth = maxHealth;
        camBrain = FindObjectOfType<CameraBrain>();
        musicHandler = FindObjectOfType<MusicHandler>();


        Time.timeScale = 1;
        damageInfluence = 0;

        healthMat.color = new Color(0, 255, 0);

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        currentHealth--;

        switch (currentHealth)
        {
            case 2:
                health01.SetActive(false);
                healthMat.color = new Color(255, 255, 0);
                break;
            case 1:
                health02.SetActive(false);
                healthMat.color = new Color(255, 0, 0);
                break;
            case 0:
                health03.SetActive(false);
                break;
            default:
                break;
        }

        UpdateDamageInfuenceValues();
        StartCoroutine(ResetPos());
    }

    public IEnumerator ResetPos() //Damage sequence, it's a bit hefty
    {
        AudioSource.PlayClipAtPoint(playerDeathSFX, transform.position, 1); //SFX for player damage
        musicHandler.FadeMusic(0, .25f);
        //musicHandler.
        playerAnim.SetInteger("dmgVal", 1);

        Instantiate(playerHitVFX, transform.position, Quaternion.identity);

        camBrain.ShakeCamera(.3f, 5f);

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(.5f);
        Time.timeScale = 1;

        ParticleSystem particles = blackHole.GetComponentInChildren<ParticleSystem>();
        var main = particles.main;
        var velOverTime = particles.velocityOverLifetime;

        DOTween.To(() => ppvolume.weight, x => ppvolume.weight = x, 1, 1.5f); //Tweak the post-processing volume for dramatic effect
        // change effect of planet draw in speed;

        foreach (var enemy in FindObjectsOfType<EnemyBase>()) //Prevent all the current enemies from moving, emulating time stoping
        {
            enemy.canmove = false;
        }
        

        var enemySpawners = FindObjectOfType<EnemySpawn>().gameObject; //Deactivate the spawning of more enemies
        enemySpawners.SetActive(false);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<CarController>().enabled = false;
        eggObject.GetComponent<Collider2D>().enabled = false; //Deactivate all player input temporarily


        yield return new WaitForSeconds(.01f);
        playerAnim.SetInteger("dmgVal", 2);
        transform.DOMove(screenCenter, 1.75f).SetEase(Ease.InOutCubic); //Move to the center of the screen
        yield return new WaitForSeconds(2);
        GetComponent<Explosion>().ExplodeEnemies(); //Create explosion
        DOTween.To(() => ppvolume.weight, x => ppvolume.weight = x, 0, .7f); //Set the post processing back
        camBrain.ShakeCamera(1.25f, 10f);


        //StartCoroutine(nameof(MusicHandler.StartNewMusic)); // PLAY NEW MUSIC

        switch (damageInfluence)
        {
            case 0:
                musicHandler.changeMusic = true;
                musicHandler.currentMusic = musicHandler.midMusic;
                musicHandler.FadeMusic(.85f, 2f);
                break;

            case 1:
                musicHandler.changeMusic = true;
                musicHandler.currentMusic = musicHandler.lateMusic;
                musicHandler.FadeMusic(.85f, 2f);
                break;

            case 2:
                //musicHandler.currentMusic = musicHandler.midMusic;
                musicHandler.currentMusic = musicHandler.midMusic;
                musicHandler.FadeMusic(0f, .25f);
                break;

            default:
                break;
        }

        
        

        //Change Black Hole properties based on DamageInfluence
        DOTween.To(() => main.simulationSpeed, x => main.simulationSpeed = x, currentBlackHoleParticleSpeed, 2f);
        DOTween.To(() => main.startSizeMultiplier, x => main.startSizeMultiplier = x, currentBlackHoleParticleSize, 2f);
        DOTween.To(() => velOverTime.orbitalZMultiplier, x => velOverTime.orbitalZMultiplier = x, currentBLackHoleParticleOrbital, 2f);
        DOTween.To(() => velOverTime.orbitalOffsetZMultiplier, x => velOverTime.orbitalOffsetZMultiplier = x, currentBLackHoleParticleOrbitalOffset, 2f);

        playerAnim.SetInteger("dmgVal", 0);

        transform.DOMove(screenCenter, .1f).SetEase(Ease.InOutCubic);

        eggObject.GetComponent<Collider2D>().enabled = true; //Re-enable controls
        GetComponent<CarController>().enabled = true;
        GetComponent<Collider2D>().enabled = true;

        


        foreach (var enemy in FindObjectsOfType<EnemyBase>()) //Re-enable enemy spawning and allow enemies to move again
        {
            enemy.canmove = true;
        }
        enemySpawners.SetActive(true);
        damageInfluence++;

        if (currentHealth <= 0) //If die, display gameOver
        {
            Time.timeScale = .4f;
            yield return new WaitForSeconds(.5f);
            GetComponent<Explosion>().ExplodeEnemies();
            yield return new WaitForSeconds(.2f);
            Stats.finalScore = FindObjectOfType<UIBrain>().playerScore;
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameOver");
        }
        else if (currentHealth == 1)
        {
            CinemachineBasicMultiChannelPerlin cinMachPerlin = camBrain.vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinMachPerlin.m_AmplitudeGain = 0.2f;
        }

        blackHole.IncreaseBlackHoleSize();

    }

    void UpdateDamageInfuenceValues()
    {
        ParticleSystem particles = blackHole.GetComponentInChildren<ParticleSystem>();
        var main = particles.main;
        var velOverTime = particles.velocityOverLifetime;

        switch (damageInfluence)
        {
            case 0:
                GetComponent<Explosion>().explosionRadius = 10f;

                
                DOTween.To(() => main.simulationSpeed, x => main.simulationSpeed = x, 10, 3f);

                currentBlackHoleParticleSpeed = 4;
                currentBlackHoleParticleSize = 0.03f;
                currentBLackHoleParticleOrbital = .4f;
                currentBLackHoleParticleOrbitalOffset = .6f;

                musicHandler.bgBeatVal = 0.0025f;

                //main.simulationSpeed = 5;
                //black hole particle size
                //blakc hole particle speed
                //black hole particle pull-size
                // blackhole particle pull-speed

                break;
            case 1:
                GetComponent<Explosion>().explosionRadius = 15f;
                DOTween.To(() => main.simulationSpeed, x => main.simulationSpeed = x, 12, 3f);
                currentBlackHoleParticleSpeed = 8;
                currentBlackHoleParticleSize = 0.09f;
                currentBLackHoleParticleOrbital = .75f;
                currentBLackHoleParticleOrbitalOffset = 1f;
                musicHandler.bgBeatVal = 0.03f;

                break;

            case 2:
                Time.timeScale = .6f;
                GetComponent<Explosion>().explosionRadius = 100f;
                DOTween.To(() => main.simulationSpeed, x => main.simulationSpeed = x, 20, 3f);
                DOTween.To(() => main.startSizeMultiplier, x => main.startSizeMultiplier = x, 1.2f, 1f);


                musicHandler.bgBeatVal = 10f;


                break;
            default:
                break;
        }
    }

    private void OnGUI()
    {
        
    }
}