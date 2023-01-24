using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RopeLengthSetter : MonoBehaviour
{
    [SerializeField] [Range(1, 10)] float ropeLength;
    [SerializeField] LineRenderer rope;
    [SerializeField] Transform Body, Slash, Heart, Knot;
    [SerializeField] CapsuleCollider2D ropeCC, bodyCC;
    private void OnValidate()
    {
        if (rope)
        {
            rope.SetPosition(1, Vector3.down * ropeLength);
            Body.localPosition = Slash.localPosition = Knot.localPosition = Vector3.down * ropeLength;
            Heart.localPosition = Body.localPosition + Vector3.up * 1.5f;
            ropeCC.size = new Vector2(0.1f, ropeLength);
            ropeCC.offset = new Vector2(0, -ropeLength / 2);
            bodyCC.offset = new Vector2(0, -ropeLength);
        }
    }
}
