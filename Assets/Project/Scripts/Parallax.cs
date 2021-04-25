using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

[RequireComponent(typeof(SpriteRenderer))]
public class Parallax : MonoBehaviour
{
    public bool repeat = true;

    public float speed = 1;

    private float width;
    private float startPos;

    void Start()
    {
        startPos = transform.position.x;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        float dist = ProCamera2D.Instance.transform.position.x * speed;

        transform.position = new Vector3( startPos + dist, transform.position.y, transform.position.z );

        if( repeat )
        {
            float tmp = ProCamera2D.Instance.transform.position.x * (1f - speed);

            if( tmp > startPos + width )
                startPos += width;
            else if( tmp < startPos - width )
                startPos -= width;
        }
    }
}