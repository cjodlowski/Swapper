using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance { get; private set; }
    //public PlayerInputManager playerInputManager;
    [SerializeField]
    public GameObject player1Marker;
    public GameObject player2Marker;
    public GameObject player3Marker;
    public GameObject player4Marker;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    public Vector3 GetSpawnPointByID(int id)
    {
        return id switch
        {
            0 => player1Marker.transform.position,
            1 => player2Marker.transform.position,
            2 => player3Marker.transform.position,
            3 => player4Marker.transform.position,
            _ => throw new UnityException("Spawn point not found by id: " + id)
        };
    }

    
}
