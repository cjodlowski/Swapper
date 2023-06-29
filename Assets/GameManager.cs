using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using UnityEngine.InputSystem.UI;

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
    public float countDownIntervalTimeS = 1;
    public PlayerInputManager playerInputManager;


    private bool doingCountDown = false;

    private Coroutine enoughActivityCoroutine;
    private float elapsedNoActivityTime;
    private List<bool> playersReadyToPlayMap = new List<bool> { false, false, false, false };
    private bool gameReady = false;

    private List<Player> players = new List<Player>();

    [Header("UI and TimeScale")]
    public static bool isRoundStarted;
    
    public GameObject pauseMenu;
    public static bool isPaused;
    public static int pausedPlayerId = -1;
    public TextMeshProUGUI countDownText;

    InputSystemUIInputModule inputSystemUIInputModule;

    private void Awake()
    {
        Time.timeScale = 1;

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        enoughActivityTextUI.SetActive(false);

        elapsedNoActivityTime = 0f;

    }

    //called when players join the game or respawn
    private IEnumerator playerJoinSequence(float sec=0.5f)
    {
        yield return new WaitUntil(() => !doingCountDown);
        cameraShake.Shake(joinWorldPause, 0.25f);
        GameManager.isPaused = true;
        yield return new WaitForSecondsRealtime(sec);
        GameManager.isPaused = false;
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
        isPaused = false;
        isRoundStarted = false;

        InputSystem.settings.SetInternalFeatureFlag("DISABLE_SHORTCUT_SUPPORT", true); //hopefully a fix for movement issues
        inputSystemUIInputModule = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<InputSystemUIInputModule>();
        enoughActivityCoroutine = StartCoroutine(GameStartSequence());
    }

    private IEnumerator GameStartSequence()
    {
        Debug.Log("Starting Count Down Sequence");
        doingCountDown = true;

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
        doingCountDown = false;

        Debug.Log("Normal Gameplay Commencing");
        isRoundStarted = true;
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
        //StartCoroutine(playerJoinSequence(dieWorldPause));
        //cameraShake.Shake(dieWorldPause, 0.25f);

        restartAwaitNoActivity();
        var successful = stockTextManager.AttemptRemoveStock(id);
        if (successful) //player hasnt lost last stock
        {
            StartCoroutine(playerKilledRoutine(playerById[id]));
            return true;

        }


        //checking end-state
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
            em.HandleEnd(string.Format("Player {0}", a_winning_player_id + 1));
        }
        if (num_alive == 0)
        {
            em.HandleEnd ("Nobody");
        }

        return false;

    }
    #endregion

    #region UI_METHODS

    public void Pause(int playerID, bool isPausing)
    {
        
        //if(isRoundStarted)
        //{
            if (isPausing)
            {
                isPaused = true;
                pausedPlayerId = playerID;

                Debug.Log(" ^^ Player " + playerID + " Paused");

                foreach (Player p in players)
                {
                    if (p.id != playerID)
                    {
                        p.playerInput.DeactivateInput();
                        Debug.Log(" ^^ Player " + p.id + " Deactivated");
                    }
                    else
                    {
                        p.playerInput.SwitchCurrentActionMap("UI");
                        Debug.Log(" ^^ Player " + p.id + " Controlling Input");

                        Debug.Log("Player is using " + p.playerInput.currentControlScheme);
                        inputSystemUIInputModule.actionsAsset = p.playerInput.actions;


                    }
                }
            }
            else
            {
                isPaused = false;
                pausedPlayerId = -1;

                foreach (Player p in players)
                {
                    if (p.id != playerID)
                    {
                        p.playerInput.ActivateInput();
                        Debug.Log(" ^^ Player " + p.id + " Reactivated");
                    }
                    else
                    {
                        p.playerInput.SwitchCurrentActionMap("Gameplay");
                        Debug.Log(" ^^ Player " + p.id + " Switched back to Gameplay");
                    }
                }
            }
            pauseMenu.SetActive(isPaused);
        //}
        
    }

    //Used for menu
    public void Resume()
    {
        isPaused = false;
        pausedPlayerId = -1;
        Time.timeScale = 1;

        foreach (Player p in players)
        {
            p.playerInput.ActivateInput();
            p.playerInput.SwitchCurrentActionMap("Gameplay");
        }

        pauseMenu.SetActive(isPaused);
    }

    public void BacktoMainMenu()
    {
        SceneManager.LoadScene("SelectGame");
    }

    #endregion

    void restartAwaitNoActivity()
    {
        elapsedNoActivityTime = 0;
        blm.toggleLaserSpeedup(false);
    }

    IEnumerator awaitNoActivity(float sec)
    {
        if(isRoundStarted && !isPaused)
        {
            if(elapsedNoActivityTime > sec)
            {
                blm.toggleLaserSpeedup(true);
                enoughActivityTextUI.SetActive(true);
                elapsedNoActivityTime = 0;
            } else
            {
                elapsedNoActivityTime += Time.deltaTime;
                enoughActivityTextUI.SetActive(false);
            }
        }
        yield return null;

    }
}
