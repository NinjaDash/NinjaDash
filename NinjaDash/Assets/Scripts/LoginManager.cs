using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class LoginManager : MonoBehaviour
{
    #region Singleton
    private static LoginManager _instance;
    public static LoginManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        if(!_instance)
        {
            _instance = this;
           // DontDestroyOnLoad(this.gameObject);
        }
        else
        {
           // Destroy(this.gameObject);
        }
    }
    #endregion


    [SerializeField] private bool isReady;
    [SerializeField] private string _WebClientID;
    [SerializeField] GameObject LoadingPanel;
    [SerializeField] TMPro.TMP_Text LoadingText;
    
   
    private void Start()
    {

        UIManager.Instance.gameObject.SetActive(false);
     
      
          
        
    }
    public void OnSignInClicked()
    {   
        LoadingPanel.SetActive(false);
        UIManager.Instance.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

  

  
}
