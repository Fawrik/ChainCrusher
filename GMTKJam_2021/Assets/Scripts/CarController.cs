using System;
using UnityEngine;
using DG.Tweening;

public class CarController : MonoBehaviour
{
    public enum ControlScheme //Do u want to control the player in a car-fashion or WASD
    {
        TRANSFORM,
        RIGIDBODY
    }

    public ControlScheme controlMode;
    [SerializeField]
    private float thrust;
    private Vector3 direction;
    private Vector3 maxVelocity;
    public GameObject carBase;

    [HideInInspector] public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        maxVelocity = rb.velocity;
        carBase = GameObject.Find("CarBase");

    }

    private void Update()
    {
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized; //Cache the input from Update and apply it in FixedUpdate

    }
 

    private void FixedUpdate()
    {
        switch (controlMode)
        {
            case ControlScheme.TRANSFORM:
                rb.AddForce(direction * thrust, ForceMode2D.Force);
                transform.up = rb.velocity.normalized;
                break;
            case ControlScheme.RIGIDBODY:
                if (Input.GetAxis("Vertical") > 0) rb.AddForce(transform.up * thrust);
                else if (Input.GetAxis("Vertical") < 0) rb.AddForce(-transform.up * thrust);

                //if (Input.GetAxis("Horizontal") > 0) transform.Rotate(0, 0, -turnRate * Time.fixedDeltaTime);
                //else if (Input.GetAxis("Horizontal") < 0) transform.Rotate(0, 0, turnRate * Time.fixedDeltaTime);

                break;
            default:
                throw new ArgumentOutOfRangeException();
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


}