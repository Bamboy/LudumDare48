using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Light2D boostLight;

    AudioSource audio;
    Rigidbody2D rb;
    float torqueAxis;
    float thrustAxis;

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();

        boostLight.enabled = false;
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
        Reset();
    }

    void OnQuit()
    {
#if UNITY_WEBGL
        Reset();
#else
        Application.Quit();
#endif

    }


    private void Update()
    {
        if (Time.timeScale == 0f)
            audio.volume = 0f;
        else
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
            boostLight.enabled = true;
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x * horizontalDragFactor, rb.velocity.y);
            boostParticles.Stop();
            boostLight.enabled = false;
        }
    }
    public AudioClip impactSingle;
    public Asteroidians.Assets.AudioSet impactAudios;
    public float velocityShakeThreshold = 10f;
    private ProCamera2DShake shaker;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (shaker == null)
            shaker = ProCamera2D.Instance.GetComponent<ProCamera2DShake>();
        if (collision.relativeVelocity.magnitude > velocityShakeThreshold)
        {
            AudioSource.PlayClipAtPoint(impactSingle, transform.position);
            AudioSource.PlayClipAtPoint(impactAudios.GetRandom(), transform.position);
            StartCoroutine(ImpactCoroutine(collision.relativeVelocity));
        }
    }

    IEnumerator ImpactCoroutine(Vector2 relVelocity)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
        shaker.Shake(0.3f, relVelocity, 10, 0.1f, -1, new Vector3(), 0.1f, true);
    }

    public bool isDead = false;

    void Reset()
    {
        FadeController.Singleton.FadeOut(delegate ()
        {
            SceneManager.LoadScene(0);
        });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead)
            return;

        if (other.tag == "Worm")
        {
            StopAllCoroutines();

            Highscores.AddScore(Mathf.FloorToInt(TerrainController.Singleton.GetPlayerDistance() / 3f));
            isDead = true;
            Reset();

        }
    }
}
