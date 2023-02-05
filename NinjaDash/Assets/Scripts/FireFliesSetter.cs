using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFliesSetter : MonoBehaviour
{
    [SerializeField] bool RefreshArea;
    [SerializeField] ParticleSystem fireflies;
    [SerializeField] PolygonCollider2D shapeRange;

    private void OnValidate()
    {
        /*Debug.Log("hello");
        Vector2 min = shapeRange.bounds.min;
        Vector2 max = shapeRange.bounds.max;

        float sizeX = max.x - min.x;
        float sizeY = max.y - min.y;

        var shape = fireflies.shape;
        shape.scale = new Vector3(sizeX, sizeY);
        shape.position = shapeRange.bounds.center;
        var emision = fireflies.emission;
        emision.rateOverTime = sizeX * sizeY * 0.01f;
*/
    }
}
