using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitstop1 : MonoBehaviour
{
    //hitstop test script 1

    public float duration = 1f;
    float pendingFreezeDuration = 0f;
    bool isFrozen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(pendingFreezeDuration>0 && !isFrozen)
        {
            StartCoroutine(doFreeze());
        }
        
    }

    public void Freeze()
    {
        pendingFreezeDuration = duration;
    }

    IEnumerator doFreeze()
    {
        isFrozen = true;
        var original = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = original;
        pendingFreezeDuration = 0;
        isFrozen = false;
    }
}
