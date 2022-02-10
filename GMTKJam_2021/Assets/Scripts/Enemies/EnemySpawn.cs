using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class EnemySpawn : MonoBehaviour
{
    public List<Transform> spawnerPos;
    public float spawnRate = 3;
    public List<GameObject> spawnableEnemies;
    int diffLevel = 1;

    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;

    float spawnFrequency;
    float spawnFreqCap1;
    float spawnFreqCap2;
    float spawnFreqCap3;
    float difficultyTimer;
    float difficultyTimeCap;

    Vector2Int spawnEnemyTypeValueMinMax = new Vector2Int(0, 100);

    Vector2Int spawnEnmLameMinMax = new Vector2Int(0, 100);
    Vector2Int spawnEnmLurkerMinMax = new Vector2Int(0, 100);
    Vector2Int spawnEnmShooterMinMax = new Vector2Int(0, 100);

    public Vector2Int spawnPortalTypeValueMinMax = new Vector2Int(0, 100);

    public Vector2Int spawnPrtlSmallMinMax = new Vector2Int(0, 33);
    public Vector2Int spawnPrtlMedMinMax = new Vector2Int(34, 66);
    public Vector2Int spawnPrtlBigMinMax = new Vector2Int(67, 100);

    public GameObject portalToSpawn;

    private void Start()
    {
        StartCoroutine(IncreaseDiff());
        UpdateDifficulty();
    }

    void OnEnable()
    {
        spawnerPos.Clear();
        foreach (var spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            spawnerPos.Add(spawner.transform);
        }

        StartCoroutine(NewEnemySpawnTiming());
        StartCoroutine(IncreaseDiff());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(SpawnNewEnemy());
        }
    }

    IEnumerator NewEnemySpawnTiming()
    {
        

        StartCoroutine(SpawnNewEnemy());
        yield return new WaitForSeconds(spawnRate);
        StartCoroutine(NewEnemySpawnTiming());
    }

    IEnumerator IncreaseDiff()
    {
        if (spawnRate < 2)
        {
            yield break;
        }
        yield return new WaitForSeconds(10);
        print("difficulty increases");
        spawnRate -= .2f;
        StartCoroutine(IncreaseDiff());

    }

    IEnumerator SpawnNewEnemy()
    {
      
        ChoosePortalType();
        GameObject enemySpawnPortal = portalToSpawn;


        Transform spawnLocation = spawnerPos[Random.Range(0, spawnerPos.Count)];
        var spawnLocationActiveStatus = spawnLocation.GetComponent<SpawnIsActive>();

        while (spawnLocationActiveStatus.isActive == true)
        {           
            spawnLocation = spawnerPos[Random.Range(0, spawnerPos.Count)];
            spawnLocationActiveStatus = spawnLocation.GetComponent<SpawnIsActive>();

            
            yield return new WaitForSeconds(1f);

        }

        enemySpawnPortal = Instantiate(enemySpawnPortal, spawnLocation.position, Quaternion.identity);
        spawnLocation.GetComponent<SpawnIsActive>().isActive = true;
        EnemySpawnPortalMisc enmyPortalMisc = enemySpawnPortal.GetComponent<EnemySpawnPortalMisc>();

        List<GameObject> enemiesInPortalWait = new List<GameObject>();       

        for (int i = 1; i < enemySpawnPortal.transform.childCount; i++)
        {
            var enemytype = Instantiate(spawnableEnemies[Random.Range(0, spawnableEnemies.Count)], 
                enemySpawnPortal.transform.GetChild(i).position, Quaternion.identity);

            //Disable enemy stuff while spawn animation plays out
            ToggleEnemySpriteMask(enemytype, true); 
            ToggleEnemyLogic(enemytype, false);
            FadeOutlineColor(enemytype, .01f, 0);
            FadeOutlineColor(enemytype.transform.GetChild(0).gameObject, .98f, 0);

            enemiesInPortalWait.Add(enemytype);
        }

        Vector3 newSize = new Vector3(enmyPortalMisc.portalSizeMinMax.y, enmyPortalMisc.portalSizeMinMax.y, 1);
        enemySpawnPortal.transform.localScale = Vector3.zero;
        DOTween.To(() => enemySpawnPortal.transform.localScale, x => enemySpawnPortal.transform.localScale = x, newSize, 1f);
        //yield return new WaitUntil(() => enemySpawnPortal.transform.localScale == newSize);
        yield return new WaitForSeconds(1f);

        newSize = new Vector3(enmyPortalMisc.portalSizeMinMax.x, enmyPortalMisc.portalSizeMinMax.x, 1);
        DOTween.To(() => enemySpawnPortal.transform.localScale, x => enemySpawnPortal.transform.localScale = x, newSize, 1f);

        for (int i = 0; i < enemiesInPortalWait.Count; i++)
        {
            ToggleEnemySpriteMask(enemiesInPortalWait[i], false);
            FadeOutlineColor(enemiesInPortalWait[i], 1f, .8f);
            FadeOutlineColor(enemiesInPortalWait[i].transform.GetChild(0).gameObject, 1f, .8f);
        }
        
        
        yield return new WaitForSeconds(.8f);

        for (int i = 0; i < enemiesInPortalWait.Count; i++)
        {
            ToggleEnemyLogic(enemiesInPortalWait[i], true);
        }

        spawnLocation.GetComponent<SpawnIsActive>().isActive = false;
        enemiesInPortalWait.Clear();
        Destroy(enemySpawnPortal);           
    }

    void ToggleEnemySpriteMask(GameObject enemy, bool toggle)
    {
        if (toggle == true)
        {
            enemy.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            enemy.transform.GetChild(0).GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        else if (toggle == false)
        {
            enemy.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            enemy.transform.GetChild(0).GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
        }
    }

    void FadeOutlineColor(GameObject enemytype, float alpha, float time)
    {
       
        DOTween.To(() => enemytype.GetComponent<SpriteRenderer>().color, x => enemytype.GetComponent<SpriteRenderer>().color = x, new Color
            (enemytype.GetComponent<SpriteRenderer>().color.r,
            enemytype.GetComponent<SpriteRenderer>().color.g,
            enemytype.GetComponent<SpriteRenderer>().color.b, alpha), time);
        
    }

    void ToggleEnemyLogic(GameObject enemytype, bool toggle)
    {
        enemytype.GetComponent<EnemyBase>().canmove = toggle;
        enemytype.GetComponent<Collider2D>().enabled = toggle;

        if (toggle == true)
        {
            MusicHandler.OnBeat += enemytype.GetComponent<EnemyBase>().Bounce;
        }
        else if (toggle == false)
        {
            MusicHandler.OnBeat -= enemytype.GetComponent<EnemyBase>().Bounce;
        }
        
    }

    void ChoosePortalType()
    {
        int number = Random.Range(spawnPortalTypeValueMinMax.x, spawnPortalTypeValueMinMax.y);

        if (number >= spawnPrtlSmallMinMax.x && number <= spawnPrtlSmallMinMax.y)
        {
            portalToSpawn = spawn1;
        }
        else if (number >= spawnPrtlMedMinMax.x && number <= spawnPrtlMedMinMax.y)
        {
            portalToSpawn = spawn2;
        }
        else if (number >= spawnPrtlBigMinMax.x && number <= spawnPrtlBigMinMax.y)
        {
            portalToSpawn = spawn3;
        }


    }

    void UpdateDifficulty()
    {
        ProgressHandler progresHandler = FindObjectOfType<ProgressHandler>();

        switch (progresHandler.currentProgressLevel)
        {
            case 1:
                spawnPrtlSmallMinMax = new Vector2Int(0, 70);
                spawnPrtlMedMinMax = new Vector2Int(71, 100);
                spawnPrtlBigMinMax = new Vector2Int(101, 102);
                break;
            case 2:
                spawnPrtlSmallMinMax = new Vector2Int(0, 37);
                spawnPrtlMedMinMax = new Vector2Int(38, 74);
                spawnPrtlBigMinMax = new Vector2Int(75, 100);
                break;
            case 3:
                spawnPrtlSmallMinMax = new Vector2Int(0, 33);
                spawnPrtlMedMinMax = new Vector2Int(34, 66);
                spawnPrtlBigMinMax = new Vector2Int(67, 100);
                break;
            default:
                break;
        }
    }

}