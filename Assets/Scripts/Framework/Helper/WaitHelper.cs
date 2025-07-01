using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitHelper : MonoBehaviour
{
    private static Dictionary<float, WaitForSeconds> WaitCache = new();

    public static WaitForSeconds GetWait(float seconds)
    {
        float roundedSeconds = Mathf.Round(seconds * 10f) / 10f;
        
        if (WaitCache.TryGetValue(roundedSeconds, out var wait))
            return wait;

        return WaitCache[roundedSeconds] = new WaitForSeconds(roundedSeconds);
    }
}
