using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineUtils
{

    public static IEnumerator WaitForSecondsExcludePause(float sec)
    {
        float elapsedTime = 0;
        while (elapsedTime < sec)
        {
            if(GameManager.isRoundStarted && !GameManager.isPaused)
            {
                elapsedTime += Time.unscaledDeltaTime;
            }
            yield return null;
        }

        yield break;
    }
}
