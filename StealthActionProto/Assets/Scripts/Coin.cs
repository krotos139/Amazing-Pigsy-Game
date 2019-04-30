using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float speed;
    public float throwLength;
    public float maxLivingTime;

    private float dist = 0;
    private float livingTime = 0;

    void Update()
    {
        if (dist < throwLength)
        {
            dist += speed * Time.deltaTime;
            transform.Translate(0, dist, 0);
        }
        if (livingTime > maxLivingTime)
        {
            DestroyCoin();
        }
        else
        {
            livingTime += Time.deltaTime;
        }
    }

    void DestroyCoin()
    {
        Destroy(gameObject);
    }
}
