using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannon : MonoBehaviour
{
    [SerializeField]
    public GameObject bulletPrefab;
    public float bulletSpeed;

    private float intervalS = 1;
    private float intervalTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        intervalTime = Time.time + Conductor.Instance.getTimeUntilNextBeat();
        intervalS = Conductor.Instance.getBeatTimeInterval();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > intervalTime)
        {
            intervalTime = Time.time + intervalS;
            var bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var rb = bulletObj.GetComponent<Rigidbody2D>();
            var rad = Mathf.Deg2Rad * (-transform.rotation.eulerAngles.z);
            rb.velocity = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad))*bulletSpeed;
        }
    }
}
