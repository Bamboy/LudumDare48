﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float boostSpeed;

    public float angleSpeed;
    public float maxAngle;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float destAngle = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            destAngle += maxAngle;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            destAngle -= maxAngle;
        }

        float angle = Mathf.LerpAngle(transform.eulerAngles.z, destAngle, Time.deltaTime * angleSpeed);
        transform.eulerAngles = Vector3.forward * angle;

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
        {
            Vector2 dir = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * Vector2.up;
            rb.AddForce(dir * boostSpeed * Time.deltaTime);
        }
    }
}
