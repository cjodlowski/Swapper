using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    [SerializeField]
    public GameObject endPanel;
    public GameObject endText;
    public GameObject endButton;

    private bool ended = false;
    // Start is called before the first frame update
    void Start()
    {
        endPanel.SetActive(false);
        endText.SetActive(false);
        endButton.SetActive(false);
    }

    private void Update()
    {
    }

    public void handle_end(string winner)
    {
        endPanel.SetActive(true);
        endText.SetActive(true);
        endButton.SetActive(true);

        var text = endText.GetComponent<TMPro.TextMeshProUGUI>();
        text.text = string.Format("{0} won! Press button to restart", winner);

        ended = true;
        //StartCoroutine(restart_soon(5));
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
