using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal;
using Com.LuisPedroFonseca.ProCamera2D;

public class PlayerController : MonoBehaviour
{
    public float boostSpeed;
    public float horizontalDragFactor = 1;
    public float angleSpeed;
    public float maxAngle;

    public ParticleSystem boostParticles;

    AudioSource audio;
    Rigidbody2D rb;
    float torqueAxis;
    float thrustAxis;

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

    public Asteroidians.Assets.AudioSet impactAudios;
    public float velocityShakeThreshold = 10f;
    private ProCamera2DShake shaker;
    private void OnCollisionEnter2D( Collision2D collision )
    {
        if( shaker == null )
            shaker = ProCamera2D.Instance.GetComponent<ProCamera2DShake>();
        if( collision.relativeVelocity.magnitude > velocityShakeThreshold )
        {
            shaker.Shake( 0.4f, collision.relativeVelocity );
            AudioSource.PlayClipAtPoint( impactAudios.GetRandom(), transform.position );
        }
    }
}
