using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public float boostSpeed;
    public float horizontalDragFactor = 1;
    public float angleSpeed;
    public float maxAngle;

    public ParticleSystem boostParticles;

    Rigidbody2D rb;
    float torqueAxis;
    float thrustAxis;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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
}
