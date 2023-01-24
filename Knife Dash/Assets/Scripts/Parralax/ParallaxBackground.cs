using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] float multiplier = 0.0f;
    [SerializeField] bool horizontalOnly = true;
    private Transform cameraTransform;

    private Vector3 startCameraPos;
    private Vector3 startPos;

    void OnEnable()
    {
        cameraTransform = Camera.main.transform;
        startCameraPos = cameraTransform.position;
        startPos = Vector3.zero;
    }

    private void LateUpdate()
    {
        var position = startPos;
        if (horizontalOnly)
            position.x += multiplier * (cameraTransform.position.x - startCameraPos.x);
        else
            position += multiplier * (cameraTransform.position - startCameraPos);
        transform.position = position;
    }

}