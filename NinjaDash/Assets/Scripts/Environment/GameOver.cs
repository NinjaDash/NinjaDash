using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(2)]
public class GameOver : MonoBehaviour
{

    [SerializeField] bool HasFinished;
    private void Awake()
    {
        if(LevelDataHolder.Instance && !HasFinished)
            LevelDataHolder.Instance.gameOverScript = this;
    }

    public GameObject wall;
    public void toggleWall(bool state)
    {
        if (wall)
        {
            Debug.Log("can pass now" + state);
            HasFinished = state;
            //CanFinish = !state;
            //wall.SetActive(state);
            //UIManager.Instance.DoorOpenIcon(state);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (HasFinished)
            {
                Debug.Log("Player Touching door");
                CharacterController2D cc = collision.GetComponent<CharacterController2D>();
                GameDone(cc, true);
            }
            else
            {
                wall.SetActive(true);
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && wall.activeSelf)
        {
            wall.SetActive(false);
        }
    }


    public void GameDone(CharacterController2D cc, bool GameWon)
    {
        Debug.Log("Game done");
        StopAllCoroutines();
        //LeanTween.cancelAll();
        
        PlayerInputs.canControl = false;
        cc.enabled = false;
        cc.canMove = false;
        PlayerInputs.canControl = false;
        cc.invincible = true;
        cc.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        LevelDataHolder.Instance.HowManyStarts(cc.life, HasFinished);
        LevelDataHolder.Instance.SaveStarsAmount();
        UIManager.Instance.showGameOverPanel(GameWon);
    }

    public void FinishEarly()
    {
        Debug.Log("Player Touching door");
        LevelManager.showAd = false;       
        CharacterController2D cc = PlayerInputs.Instance.myCC;
        GameDone(cc, true);
    }
   
}
