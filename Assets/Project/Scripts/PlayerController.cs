using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal;
using Com.LuisPedroFonseca.ProCamera2D;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Singleton { get; private set; }

    public float boostSpeed;
    public float horizontalDragFactor = 1;
    public float angleSpeed;
    public float maxAngle;

    public ParticleSystem boostParticles;

    AudioSource audio;
    Rigidbody2D rb;
    float torqueAxis;
    float thrustAxis;

    private void Awake()
    {
        if( Singleton == null )
            Singleton = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();

        boostParticles.Stop();
    }

    void OnRotate(InputValue value)
    {
        torqueAxis = -value.Get<float>();
    }

    void OnThrust(InputValue value)
    {
        thrustAxis = value.Get<float>();
    }

    void OnReset()
    {
        transform.position = Vector2.zero;
        rb.velocity = Vector2.up * 15;
        rb.rotation = 0;
        rb.angularVelocity = 0;
    }

    void OnQuit()
    {
        Application.Quit();
    }


    private void Update()
    {
        audio.volume = thrustAxis * 0.6f;
    }

    void FixedUpdate()
    {
        rb.AddTorque(torqueAxis * angleSpeed * Time.deltaTime);

        if (thrustAxis > 0)
        {
            Vector2 dir = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * Vector2.up;
            rb.AddForce(dir * boostSpeed * thrustAxis * Time.deltaTime);
            boostParticles.Play();
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x * horizontalDragFactor, rb.velocity.y);
            boostParticles.Stop();
        }
    }
    public AudioClip impactSingle;
    public Asteroidians.Assets.AudioSet impactAudios;
    public float velocityShakeThreshold = 10f;
    private ProCamera2DShake shaker;
    private void OnCollisionEnter2D( Collision2D collision )
    {
        if( shaker == null )
            shaker = ProCamera2D.Instance.GetComponent<ProCamera2DShake>();
        if( collision.relativeVelocity.magnitude > velocityShakeThreshold )
        {
            AudioSource.PlayClipAtPoint( impactSingle, transform.position );
            AudioSource.PlayClipAtPoint( impactAudios.GetRandom(), transform.position );
            StartCoroutine( ImpactCoroutine(collision.relativeVelocity) );
        }
    }

    IEnumerator ImpactCoroutine( Vector2 relVelocity )
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
        shaker.Shake( 0.3f, relVelocity, 10, 0.1f, -1, new Vector3(), 0.1f, true );
    }

    public bool isDead = false;
    private void OnTriggerEnter2D( Collider2D other )
    {
        if( isDead )
            return;

        if( other.tag == "Worm" )
        {
            StopAllCoroutines();
            Time.timeScale = 0;
            Highscores.AddScore( Mathf.FloorToInt( TerrainController.Singleton.GetPlayerDistance() / 3f ) );
            isDead = true;
        }
    }
}
