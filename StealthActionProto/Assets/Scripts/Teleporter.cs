using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{

    public Transform teleportExit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (teleportExit == null)
            return;
        //Debug.Log("ColliderTag2:" + collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
            Transform t = collision.gameObject.GetComponent<Transform>();
            t.position = teleportExit.position;
            t = Camera.main.GetComponent<Transform>();
            t.position = new Vector3(teleportExit.position.x, teleportExit.position.y, t.position.z);
            Debug.Log("Teleport");
        }
        
    }
}
