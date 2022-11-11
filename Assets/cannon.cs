using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannon : MonoBehaviour
{
    [SerializeField]
    public GameObject bulletPrefab;
    public float bulletSpeed;

    public float intervalS = 1;
    private float timer = 0;
    private float intervalTime = 1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > intervalTime)
        {
            intervalTime = Time.time + 1;
            var bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var rb = bulletObj.GetComponent<Rigidbody2D>();
            var rad = Mathf.Deg2Rad * (-transform.rotation.eulerAngles.z);
            rb.velocity = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad))*bulletSpeed;
        }
    }
}
