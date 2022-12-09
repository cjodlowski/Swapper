using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    PlayerInput playerInput;
    SpriteRenderer spriteRenderer;
    Controls controls;
    Vector2 move = Vector2.zero;
    Vector2 aimDir = Vector2.zero;
    Rigidbody2D rigidbody2D;
    GameManager gameManager;
    int id;
    bool dead = false;
    bool invince = false;
    bool dying = false;

    bool coolingDown = false;

    [SerializeField]
    public GameObject laserPrefab;
    public GameObject laserDebugPrefab;
    public GameObject laserFlagPrefab;
    public float maxLaserDistance = 100;
    //public bool showDebugLasers = false;
    public Vector3 spawnPoint;
    public float invinceFlashIntervalS = 1;
    public float startInvinceDurationS = 5;

    public float selfSwapInvinceDurationS = 1;
    public float laserCooldown = 0.3f;
    public UIProgress laserCooldownProgress;
    public int aimAssistOneSideAttempts = 5;
    public float aimAssistAngleSpacingDeg = 0.1f;



    float speed = 5;
    Color color;

    private Vector2 lastCurrentDir = Vector2.right;


    void Awake() {
        controls = new Controls();
        playerInput = gameObject.GetComponent<PlayerInput>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        transform.position = spawnPoint;

        color = PlayerColorPicker.Instance.PickColor();
        spriteRenderer.color = color;


    }

    private void Start()
    {
        transform.position = SpawnPointManager.Instance.GetSpawnPointByID(playerInput.playerIndex);

        gameManager = GameManager.Instance;
        id = gameManager.PlayerJoin(gameObject, color);
        TempInvincibility();
    }

    private void Update()
    {
        //Vector2 scaled_move = new Vector2(move.x, move.y) * Time.deltaTime * speed;
        //transform.Translate(scaled_move, Space.World);
        rigidbody2D.velocity = move * speed;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!dead)
        {
            move = ctx.ReadValue<Vector2>();
            rigidbody2D.velocity = move * speed;
        }

    }

    public void OnAim(InputAction.CallbackContext ctx)
    {
        if (!dead)
        {
            var aim_dir = ctx.ReadValue<Vector2>();
            aimDir = aim_dir;
            if (Mathf.Abs(aimDir.SqrMagnitude()) > 0.5f) {
                lastCurrentDir = aimDir;
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(aim_dir.y, aim_dir.x) * Mathf.Rad2Deg);
            }
        }

    }

    public void onMouseAim(InputAction.CallbackContext ctx)
    {
        if (!dead)
        {
            var aim_pos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
            var delta_pos = new Vector2(aim_pos.x - transform.position.x, aim_pos.y - transform.position.y);
            aimDir = new Vector2(delta_pos.x, delta_pos.y).normalized;

            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg);
        }

    }

    IEnumerator TimeoutCooldown()
    {
        laserCooldownProgress.StartEmpty(laserCooldown);
        yield return new WaitForSeconds(laserCooldown);
        coolingDown = false;
    }

    IEnumerator TimeoutDestroy(GameObject obj)
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(obj);
    }

    private List<Vector3> GetMaxLaserBouncePath(Vector3 startPos, Vector3 direction, float maxDist, int maxBounces, LayerMask stopMask, LayerMask mirrorMask)
    {
        var currDirection = direction;
        var currStartPos = startPos;
        var positions = new List<Vector3>() { startPos};

        for (var i = 0; i < maxBounces; i++)
        {
            var stop_hit = Physics2D.Raycast(currStartPos, currDirection, maxDist, stopMask);
            var mirror_hit = Physics2D.Raycast(currStartPos, currDirection, maxDist, mirrorMask);

            if (stop_hit.collider != null)
            {
                var laserStopIsCloser = mirror_hit.collider == null || (mirror_hit.collider != null && stop_hit.distance < mirror_hit.distance);
                if (laserStopIsCloser)
                {
                    positions.Add(stop_hit.point);
                    break;
                }

            }

            if (mirror_hit.collider != null)
            {
                positions.Add(mirror_hit.point);
                currDirection = Vector2.Reflect(currDirection, mirror_hit.normal);
                currStartPos = mirror_hit.point;
                continue;
            }

            Debug.LogError("LASER PATH WENT OUT OF BOUNDS");
            break;


        }

        return positions;
    }

    private void TriggerShoot()
    {
        if (!coolingDown && !dead)
        {
            coolingDown = true;

            var maxHits = 7;
            var stopMask = LayerMask.GetMask("BorderBox") | LayerMask.GetMask("LaserStop");
            var mirrorMask = LayerMask.GetMask("LaserMirror");
            var playerMask = LayerMask.GetMask("Player");
            var currentDir = Mathf.Abs(aimDir.SqrMagnitude()) < Mathf.Epsilon ? lastCurrentDir : aimDir;
            
            var weaponAdjustedStartPos = new Vector3(transform.position.x, transform.position.y) + new Vector3(currentDir.x, currentDir.y).normalized*.75f;
            var boundariesAdjustedStartPos = new Vector3(transform.position.x, transform.position.y) + new Vector3(currentDir.x, currentDir.y).normalized * transform.localScale.x;

            var early_stop_hit = Physics2D.Raycast( transform.position, currentDir, (weaponAdjustedStartPos - transform.position).magnitude, stopMask | mirrorMask);
            var early_player_hit = Physics2D.Raycast(boundariesAdjustedStartPos, currentDir, (weaponAdjustedStartPos - boundariesAdjustedStartPos).magnitude, playerMask);
            if (early_stop_hit.collider != null || early_player_hit.collider != null) {
                if (early_stop_hit.collider != null && early_player_hit.collider != null) {
                    if (early_player_hit.distance < early_stop_hit.distance) {
                        ConductSwap(early_player_hit.collider.transform);
                    }
                    else {

                    }
                }
                StartCoroutine(TimeoutCooldown());
                return;
            }

            var actualBouncePositions = new List<Vector3>();

            var numAttempts = Configuration.Instance.aimAssist ? aimAssistOneSideAttempts : 0;

            for (int fuzzyIndex = -numAttempts; fuzzyIndex  < numAttempts + 1; fuzzyIndex++) {

                var closeCurrentDir = Quaternion.Euler(0, 0, aimAssistAngleSpacingDeg * fuzzyIndex) * currentDir;
                
                var maxBouncePositions = GetMaxLaserBouncePath(weaponAdjustedStartPos, closeCurrentDir, maxLaserDistance, maxHits, stopMask, mirrorMask | mirrorMask);

                for(int i = 0; i < maxBouncePositions.Count - 1; i++)
                {
                    if (Configuration.Instance.debugLasers)
                    {
                        var laserObj = Instantiate(laserDebugPrefab, Vector3.zero, Quaternion.identity);
                        var laser = laserObj.GetComponent<laser>();
                        var currPos = maxBouncePositions[i];
                        var currDir = (maxBouncePositions[i + 1] - maxBouncePositions[i]).normalized;
                        laser.SetPositions(new List<Vector3>() { currPos, currPos+currDir }.ToArray());
                    }
                }

                var bouncePositions = new List<Vector3>() { maxBouncePositions[0] };

                var hitPlayer = false;
                for (int i = 0; i < maxBouncePositions.Count - 1; i++)
                {
                    var direction = maxBouncePositions[i + 1] - maxBouncePositions[i];
                    var player_hit = Physics2D.Raycast(maxBouncePositions[i], direction.normalized, direction.magnitude, playerMask);

                    if (player_hit.collider != null)
                    {
                        bouncePositions.Add(maxBouncePositions[i]);
                        bouncePositions.Add(player_hit.point);

                        if (player_hit.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                        {
                            hitPlayer = true;
                            actualBouncePositions = bouncePositions;
                            ConductSwap(player_hit.collider.transform);

                            if (Configuration.Instance.debugLasers)
                            {
                                var laserObj = Instantiate(laserDebugPrefab, Vector3.zero, Quaternion.identity);
                                var laser = laserObj.GetComponent<laser>();
                                var currPos = maxBouncePositions[i];
                                var currDir = (maxBouncePositions[i + 1] - maxBouncePositions[i]).normalized;
                                laser.SetPositions(new List<Vector3>() { currPos, currPos + currDir*2 }.ToArray());
                            }
                        }

                        break;
                    }
                    bouncePositions.Add(maxBouncePositions[i]);
                    if (i == maxBouncePositions.Count - 2)
                    {
                        bouncePositions.Add(maxBouncePositions[maxBouncePositions.Count - 1]);

                    }
                }
                
                if (hitPlayer)
                {
                    break;
                }

                if (fuzzyIndex == 0)
                {
                    actualBouncePositions = bouncePositions;
                    if (Configuration.Instance.debugLasers)
                    {
                        var laserObj = Instantiate(laserDebugPrefab, Vector3.zero, Quaternion.identity);
                        var laser = laserObj.GetComponent<laser>();
                        var currPos = weaponAdjustedStartPos;
                        laser.SetPositions(new List<Vector3>() { currPos, currPos + new Vector3(currentDir.x, currentDir.y) * 2 }.ToArray());
                    }
                }

            }

            var positions = actualBouncePositions.ToArray();
            DrawTempLaser(positions);       

            StartCoroutine(TimeoutCooldown());

        }
    }


    private void ConductSwap(Transform other) {
        var tempPos = transform.position;
        transform.position = other.position;
        other.position = tempPos;
        gameManager.PlayerSwap();
        if (Configuration.Instance.selfInvincibleAfterSwap) {
            ConfigureInvincibility(selfSwapInvinceDurationS);
        }
    }

    private void DrawTempLaser(Vector3[] positions) {
        if (positions.Length > 1)
        {
            Debug.Log("Drew Laser");
            var laserObj = Instantiate(laserPrefab, Vector3.zero, Quaternion.identity);
            var laser = laserObj.GetComponent<laser>();
            laser.SetColor(color);

            laser.SetPositions(positions);
            StartCoroutine(TimeoutDestroy(laserObj));

        }
    }


    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (!dead)
        {
            TriggerShoot();
        }
    }

    public void OnMouseShoot(InputAction.CallbackContext ctx)
    {
        if (!dead)
        {
            TriggerShoot();
        }
    }

    public void TempInvincibility()
    {
        ConfigureInvincibility(startInvinceDurationS);
    }

    public void ConfigureInvincibility(float duration) {
        if (invince) {
            return;
        }
        StartCoroutine(TempInvincibilityInner(duration));
    }


    private IEnumerator TempInvincibilityInner(float durationS)
    {
        invince = true;
        var trueColor = spriteRenderer.color;
        
        var timeLeftS = durationS;
        var colors = new List<Color>() { trueColor, Color.white };
        var idx = 0;
        while (timeLeftS > invinceFlashIntervalS)
        {
            spriteRenderer.color = colors[idx];
            idx = (idx + 1)%2;
            yield return new WaitForSeconds(invinceFlashIntervalS);
            timeLeftS -= invinceFlashIntervalS;
        }
        spriteRenderer.color = trueColor;
        invince = false;

    }

    //public void OnCollisionExit2D(Collision2D collision)
    //{
    //    rigidbody2D.velocity = move * speed;
    //}

    //public void OnCollisionStay2D(Collision2D collision)
    //{
    //    rigidbody2D.velocity = rigidbody2D.velocity.normalized * speed;
    //}

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("DeathTouch"))
        {
            die();

        }

    }

    private IEnumerator delayDie()
    {
        if (!dying)
        {
            dying = true;
            var respawns = gameManager.PlayerKilled(id);
            yield return new WaitForSeconds(0.01f);
            if (!respawns)
            {
                dead = true;
                spriteRenderer.enabled = false;
                transform.GetChild(0).gameObject.SetActive(false);
            }
            dying = false;
        }
        
    }

    private void die()
    {
        if (!dead && !invince)
        {
            StartCoroutine(delayDie());
        }
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("DeathTouch")))
        {
            die();
        }
    }

}

