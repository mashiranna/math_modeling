using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{

    public float moveSpeed;
    public float zoomSpeed;
    public float zoomMin, zoomMax;
    public float speedMin, speedMax;

    private Camera cam;

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        RemapSpeed();
    }

    void Update()
    {
        float vert = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        transform.Translate(new Vector3(hor, vert) * Time.deltaTime * moveSpeed);


        float scroll = -Input.mouseScrollDelta.y;

        if (scroll != 0)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + scroll * zoomSpeed, zoomMin, zoomMax);
            RemapSpeed();
        }


    }

    private void RemapSpeed()
    {
        moveSpeed = Mathf.Lerp(speedMin, speedMax, Mathf.InverseLerp(zoomMin, speedMax, cam.orthographicSize));
    }
}
