using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float bulletSpeed;
    public Vector2 ogVelocity;

    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameManager.bullets.Add(this);
    }

    //roundabout way of storing the velocity set in cannon script here
    public void SetVelocity(Vector2 vel)
    {
        ogVelocity = vel;
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

    private void OnDisable()
    {
        GameManager.bullets.Remove(this);
    }
}
