using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTransform;

    public BoxCollider2D mapBounds;

    private float xMin, xMax, yMin, yMax;
    private float camY, camX;
    private float camOrthsize;
    private float cameraRatio;
    private Camera mainCam;

    private Vector3 smoothPos;
    public float smoothSpeed = 0.25f;//speed of how camera smoothly follows player
    public float followOffset = 5;//offset to not center the player

    // Start is called before the first frame update
    void Start()
    {
        xMin = mapBounds.bounds.min.x;
        xMax = mapBounds.bounds.max.x;
        yMin = mapBounds.bounds.min.y;
        yMax = mapBounds.bounds.max.y;
        mainCam = GetComponent<Camera>();
        camOrthsize = mainCam.orthographicSize;
        cameraRatio = (xMax + camOrthsize) / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        camY = Mathf.Clamp(followTransform.position.y + followOffset, yMin + camOrthsize, yMax - camOrthsize);
        camX = Mathf.Clamp(followTransform.position.x, xMin + cameraRatio, xMax - cameraRatio);

        //smoothPos = Vector3.Lerp(this.transform.position, new Vector3(camX, camY, this.transform.position.z), smoothSpeed);
        smoothPos = Vector3.Lerp(this.transform.position, new Vector3(this.transform.position.x, camY, this.transform.position.z), smoothSpeed);

        this.transform.position = smoothPos;
    }
}
