using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class EndManager : MonoBehaviour
{
    [SerializeField]
    public GameObject endPanel;
    public GameObject endText;
    public GameObject endButton;

    public float endPauseSec;
    public float endSlowDownSec;
    [Range(0, 1)]
    public float endSlowDownRatio;

    private bool ended = false;
    // Start is called before the first frame update
    void Start()
    {
        endPanel.SetActive(false);
        endText.SetActive(false);
        endButton.SetActive(false);
    }

    public void HandleEnd(string winner)
    {
        StartCoroutine(endSequence());

        var text = endText.GetComponent<TMPro.TextMeshProUGUI>();
        text.text = string.Format("{0} won! Press button to restart", winner);

        ended = true;
    }

    private IEnumerator endSequence()
    {
        GameManager.isRoundStarted = false;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(endPauseSec);
        Time.timeScale = endSlowDownRatio;

        yield return new WaitForSecondsRealtime(endSlowDownSec);

        endPanel.SetActive(true);
        endText.SetActive(true);
        endButton.SetActive(true);

        Time.timeScale = 0;
    }

    public void restart(float sec)
    {
        StartCoroutine(restart_soon(sec));
    }

    IEnumerator restart_soon(float sec)
    {
        yield return new WaitForSeconds(sec);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
