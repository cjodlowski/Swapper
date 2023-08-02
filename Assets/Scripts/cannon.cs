using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannon : MonoBehaviour
{
    [SerializeField]
    public GameObject bulletPrefab;
    public Transform bulletParent;
    public float bulletSpeed;


    public int beatInterval = 1; //num of beats to wait before firing
    private float intervalS; //time between beats
    private float elapsedTime; //next firing time

    // Start is called before the first frame update
    void Start()
    {
        intervalS = Conductor.Instance.getBeatTimeInterval();
        elapsedTime = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isRoundStarted && !GameManager.isPaused)
        {
            if (elapsedTime > intervalS * beatInterval)
            {
                elapsedTime = 0f;
                var bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bulletParent);
                var rb = bulletObj.GetComponent<Rigidbody2D>();
                var rad = Mathf.Deg2Rad * (-transform.rotation.eulerAngles.z);
                rb.velocity = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * bulletSpeed;
                bulletObj.GetComponent<bullet>().SetVelocity(rb.velocity);
            } else
            {
                elapsedTime += Time.deltaTime;
            }
        }
    }
}
