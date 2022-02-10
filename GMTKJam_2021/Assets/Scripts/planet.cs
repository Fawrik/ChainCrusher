using UnityEngine;
using Random = UnityEngine.Random;
public class Planet : MonoBehaviour
{
    float maxGravity = 22000f;
    float gravityRadius = 5f;
    public Color regularCol, attractionCol;
    Rigidbody2D player;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        Spawn();
        player = FindObjectOfType<CarController>().GetComponent<Rigidbody2D>();
        Destroy(gameObject, 30);
    }

    void Spawn()
    {
        int side = 1;

        // 0 = left/right, 1 = top/bottom
        if (Random.Range(0, 2) == 0)
        {
            // 0 = left, 1 = right
            if (Random.Range(0, 2) == 0)
            {
                side = -1;
            }

            float yPos = Random.Range(1f, 9f);
            transform.position = new Vector3(9 * side, yPos);
        }
        else
        {
            float yPos;
            // 0 = top, 1 = bottom
            if (Random.Range(0, 2) == 1)
            {
                yPos = 11f;
            }
            else
            {
                yPos = -1f;
            }
            float xPos = Random.Range(-8f, 8f);
            transform.position = new Vector3(xPos, yPos);
        }

        Vector3 direction = new Vector3(Random.Range(-3.5f, 3.5f), Random.Range(2f, 8f));
        transform.right = direction - transform.position;
    }

    void Update()
    {
        float dist = Vector2.Distance(transform.position, player.transform.position);
        if (dist < gravityRadius)
        {
            spriteRenderer.color = Color.Lerp(attractionCol, regularCol, dist / gravityRadius);
            float step = maxGravity / gravityRadius;
            Vector2 playerToPlanet = (transform.position - player.transform.position);
            float distanceToPlayer = playerToPlanet.magnitude;

            float value = (distanceToPlayer / gravityRadius);

            float pullForce = -step * distanceToPlayer + maxGravity;
            Vector2 gravityPull = playerToPlanet.normalized * (pullForce * Time.deltaTime);

            player.AddForce(gravityPull);
        }
        else
        {
            spriteRenderer.color = Color.Lerp(attractionCol, regularCol, dist / gravityRadius);
        }
        transform.position += transform.right * Time.deltaTime;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
