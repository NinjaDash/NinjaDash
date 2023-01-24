using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParentPlacer : MonoBehaviour
{
    [SerializeField] string Parent_Name;
    private void OnEnable()
    {
        Transform parent = GameObject.Find(Parent_Name).transform;
        if (parent != null)
        {
            this.transform.parent = parent;
            this.enabled = false;
        }
    }
}
