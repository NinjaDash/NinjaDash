using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPosHolder : MonoBehaviour
{
    public string UIIconCode;
    public RectTransform rect;
    public Vector2 OriginalPos;
    public bool isEditable;
    private void Awake()
    {
        if(rect==null)
            rect = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        LoadData();
    }
    private void OnValidate()
    {
        if(rect==null)
            rect = GetComponent<RectTransform>();
        OriginalPos = rect.anchoredPosition;
    }
    public void SaveData()
    {
        if (isEditable && this.gameObject.activeInHierarchy)
        {
            Debug.Log("saving data");
            PlayerPrefs.SetString(UIIconCode + "Pos", rect.anchoredPosition.x.ToString() + "/" + rect.anchoredPosition.y.ToString());
            PlayerPrefs.SetString(UIIconCode + "Scale", rect.localScale.x.ToString() + "/" + rect.localScale.y.ToString());
        }
        //LoadData();
    }
    public void LoadData()
    {
        
        Debug.Log("Loading data");
        string tempPos = PlayerPrefs.GetString(UIIconCode + "Pos", OriginalPos.x.ToString() + "/" + OriginalPos.y.ToString());
        string tempScale = PlayerPrefs.GetString(UIIconCode + "Scale", rect.localScale.x.ToString() + "/" + rect.localScale.y.ToString());


        string[] pos = tempPos.Split('/');
        string[] scale = tempScale.Split('/');


        rect.anchoredPosition = new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
        rect.localScale = new Vector2(float.Parse(scale[0]), float.Parse(scale[1]));
    }

    public void ResetPos()
    {
        if (this.gameObject.activeInHierarchy)
        {
            rect.anchoredPosition = OriginalPos;
            rect.localScale = Vector2.one;
        }
    }
}
