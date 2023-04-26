using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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
    public float swapWorldPause = 0.1f; //how long world pauses when players swap
    public float joinWorldPause = 0.5f;
    public float pauseTimescale = 0.3f;
    public TextMeshProUGUI countDownText;
    public float countDownIntervalTimeS = 1;
    public PlayerInputManager playerInputManager;


    private bool doingCountDown = false;

    private Coroutine enoughActivityCoroutine;
    private List<bool> playersReadyToPlayMap = new List<bool> { false, false, false, false };
    private bool gameReady = false;

    private List<Player> players = new List<Player>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        enoughActivityTextUI.SetActive(false);

    }

    //called when players join the game or respawn
    private IEnumerator playerJoinSequence(float sec=0.5f)
    {
        yield return new WaitUntil(() => !doingCountDown);
        cameraShake.Shake(joinWorldPause, 0.25f);
        Time.timeScale = pauseTimescale;
        yield return new WaitForSecondsRealtime(sec);
        Time.timeScale = 1;
    }

    public bool IsCurrentlyDoingCountDown() => doingCountDown;

    private bool playersReadyToPlay()
    {
        var playerCount = playerInputManager.playerCount;
        if (playerCount == 0)
        {
            return false;
        }
        var ready = true;
        for (int i = 0; i < playerCount; i++)
        {
            ready = ready && playersReadyToPlayMap[i];
        }
        return ready;
    }

    public void PlayerAnnounceReadyToPlay(int id, Player player)
    {
        Debug.Log("Try to announce ready " + id);
        if (!playersReadyToPlayMap[id])
        {
            player.CountdownHaloEnable(true);
            players.Add(player);
            playersReadyToPlayMap[id] = true;
            return;
        }

        if (playersReadyToPlay())
        {
            gameReady = true;
            foreach (var eachPlayer in players)
            {
                eachPlayer.CountdownHaloEnable(false);
            }
        }
    }


    private void Start()
    {
        enoughActivityCoroutine = StartCoroutine(GameStartSequence());
    }

    private IEnumerator GameStartSequence()
    {
        Debug.Log("Starting Count Down Sequence");
        doingCountDown = true;
        Time.timeScale = 0;

        Debug.Log("Waiting for players to be ready");
        countDownText.text = "play";

        yield return new WaitUntil(() => playersReadyToPlay());
        yield return new WaitUntil(() => gameReady);

        for (int i = 3; i > 0; i--)
        {
            countDownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(countDownIntervalTimeS);
        }
        countDownText.text = "";
        Time.timeScale = 1;
        doingCountDown = false;

        Debug.Log("Normal Gameplay Commencing");
        StartCoroutine(awaitNoActivity(noActivityInterval));
    }


    #region PLAYER_METHODS
    // returns own id
    public int PlayerJoin(GameObject playerObject, Color color)
    {
        StartCoroutine(playerJoinSequence(joinWorldPause));
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
        StartCoroutine(playerJoinSequence(dieWorldPause));
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
    #endregion

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
