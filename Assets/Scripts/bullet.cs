using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("BorderBox"))
        {
            transform.position = new Vector3(1000, 1000);
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("DeathTouch"))
        {
            transform.position = new Vector3(1000, 1000);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("DeathTouch"))
        {
            transform.position = new Vector3(1000, 1000);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("BorderBox"))
        {
            transform.position = new Vector3(1000, 1000);
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("DeathTouch"))
        {
            transform.position = new Vector3(1000, 1000);
            Destroy(gameObject);
        }
    }
}
