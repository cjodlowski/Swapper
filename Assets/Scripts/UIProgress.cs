using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class UIProgress : MonoBehaviour
{
    private Coroutine routine;
    public void StartEmpty(float duration)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        StartCoroutine(EmptyOverTime(duration));
    }

    private IEnumerator EmptyOverTime(float duration)
    {
        //Stopwatch stopWatch = new Stopwatch();
        var img = GetComponent<Image>();
        img.fillAmount = 1;

        var elapsed = 0.0f;
        img.fillAmount = 1;
        //stopWatch.Start();
        while (elapsed < duration)
        {
            //stopWatch.Stop();
            img.fillAmount = 1 - elapsed / duration;
            elapsed += 0.125f;
            //stopWatch.Start();
            yield return StartCoroutine(CoroutineUtils.WaitForSecondsExcludePause(0.125f));
            
        }
        img.fillAmount = 1;
        yield return StartCoroutine(CoroutineUtils.WaitForSecondsExcludePause(0.1f));
        img.fillAmount = 0;
        yield return StartCoroutine(CoroutineUtils.WaitForSecondsExcludePause(0.1f));
        img.fillAmount = 1;
        yield return StartCoroutine(CoroutineUtils.WaitForSecondsExcludePause(0.1f));
        img.fillAmount = 0;
        routine = null;
    }
}
