using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannon : MonoBehaviour
{
    [SerializeField]
    public GameObject bulletPrefab;
    public float bulletSpeed;


    public int beatInterval = 1; //num of beats to wait before firing
    private float intervalS; //time between beats
    private float intervalTime; //next firing time

    // Start is called before the first frame update
    void Start()
    {
        intervalS = Conductor.Instance.getBeatTimeInterval();
        intervalTime = Time.time + Conductor.Instance.getTimeUntilNextBeat() + intervalS * (beatInterval - 1);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > intervalTime)
        {
            intervalTime = Time.time + intervalS * beatInterval;
            var bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var rb = bulletObj.GetComponent<Rigidbody2D>();
            var rad = Mathf.Deg2Rad * (-transform.rotation.eulerAngles.z);
            rb.velocity = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad))*bulletSpeed;
        }
    }
}
