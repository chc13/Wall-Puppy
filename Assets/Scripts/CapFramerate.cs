using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapFramerate : MonoBehaviour
{ 

    public int target = 144;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = target;
        
     }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
