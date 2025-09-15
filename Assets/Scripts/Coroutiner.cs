using System;
using System.Collections;
using UnityEngine;

public class Coroutiner : MonoBehaviour //CÓDIGO UTILITÁRIO PARA RODAR COISAS DE FORMA ASSINCRONA
{
    private static Coroutiner instance;

    private void Awake()
    {
        instance = this;
    }

    public static Coroutine DoCoroutine(IEnumerator coroutine)
    {
        return instance.StartCoroutine(coroutine);
    }

    public static Coroutine WaitForSeconds(float secondsToWait, Action callback)
    {
        return DoCoroutine(DoWaitForSeconds(secondsToWait, callback));
    }

    public static Coroutine WaitFrames(int frames, Action callback)
    {
        return DoCoroutine(DoWaitForFrames(frames, callback));
    }

    public static Coroutine WaitUntil(Func<bool> predicate, Action callback)
    {
        return DoCoroutine(DoWaitUntil(predicate, callback));
    }

    public static void StopDoCoroutine(Coroutine coroutine)
    {
        instance.StopCoroutine(coroutine);
    }

    private static IEnumerator DoWaitForFrames(int frames, Action callback)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        callback?.Invoke();
    }

    private static IEnumerator DoWaitForSeconds(float secondsToWait, Action callback)
    {
        yield return new WaitForSeconds(secondsToWait);
        callback?.Invoke();
    }

    private static IEnumerator DoWaitUntil(Func<bool> predicate, Action callback)
    {
        yield return new WaitUntil(predicate);
        callback?.Invoke();
    }
}
