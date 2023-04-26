using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameSelect : MonoBehaviour
{
    // Start is called before the first frame update
    public void changeScene(int idx)
    {
        SceneManager.LoadScene(idx);
    }
}
