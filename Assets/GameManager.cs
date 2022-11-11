using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public StockTextManager stockTextManager;
    private Dictionary<int, GameObject> playerById = new Dictionary<int, GameObject>();

    [SerializeField]
    public EndManager em;
    public BigLaserManager blm;
    public float noActivityInterval = 60;
    public GameObject enoughActivityTextUI;
    public CameraShake cameraShake;
    public float dieWorldPause = 0.3f;
    public float swapWorldPause = 0.1f;
    public float joinWorldPause = 0.5f;
    public float pauseTimescale = 0.3f;

    private Coroutine enoughActivityCoroutine;

    private void Awake()
    {
        enoughActivityTextUI.SetActive(false);
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private IEnumerator delayWorldPause(float sec=0.5f)
    {
        Time.timeScale = pauseTimescale;
        yield return new WaitForSecondsRealtime(sec);
        Time.timeScale = 1;
    }


    private void Start()
    {
        enoughActivityCoroutine = StartCoroutine(awaitNoActivity(noActivityInterval));
    }

    // returns own id
    public int PlayerJoin(GameObject playerObject, Color color)
    {
        StartCoroutine(delayWorldPause(joinWorldPause));
        cameraShake.Shake(joinWorldPause, 0.25f);
        var id = stockTextManager.CreateStockText(color);
        playerById.Add(id, playerObject);
        return id;
    }

    public void PlayerSwap()
    {
        StartCoroutine(playerSwappedRoutine());
    }

    private IEnumerator playerSwappedRoutine()
    {
        yield return new WaitForSeconds(swapWorldPause);
        cameraShake.Shake(swapWorldPause, 0.25f);

    }

    private IEnumerator playerKilledRoutine(GameObject playerObj)
    {
        yield return new WaitForSeconds(0.01f);
        var player = playerObj.GetComponent<Player>();
        playerObj.transform.position = player.spawnPoint;
        player.TempInvincibility();
    }

    // return if respawn
    public bool PlayerKilled(int id)
    {
        StartCoroutine(delayWorldPause(dieWorldPause));
        cameraShake.Shake(dieWorldPause, 0.25f);

        restartAwaitNoActivity();
        var successful = stockTextManager.AttemptRemoveStock(id);
        if (successful)
        {
            StartCoroutine(playerKilledRoutine(playerById[id]));
            return true;

        }

        var num_alive = 0;
        int a_winning_player_id = -1;
        foreach (var player_id in playerById.Keys)
        {
            if (stockTextManager.CheckStock(player_id) > 0)
            {
                num_alive += 1;
                a_winning_player_id = player_id;
            }

        }
        if (num_alive == 1) {
            em.handle_end(string.Format("Player {0}", a_winning_player_id + 1));
        }
        if (num_alive == 0)
        {
            em.handle_end("Nobody");
        }

        return false;

    }

    void restartAwaitNoActivity()
    {
        if(enoughActivityCoroutine != null)
        {
            StopCoroutine(enoughActivityCoroutine);
            enoughActivityCoroutine = null;
        }
        blm.toggleLaserSpeedup(false);
        enoughActivityCoroutine = StartCoroutine(awaitNoActivity(noActivityInterval));
    }

    IEnumerator awaitNoActivity(float sec)
    {
        enoughActivityTextUI.SetActive(false);
        yield return new WaitForSeconds(sec);
        blm.toggleLaserSpeedup(true);
        enoughActivityTextUI.SetActive(true);
    }
}