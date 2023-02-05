using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(3)]
public class Girl : MonoBehaviour
{
    public GameObject rope;
    public ParticleSystem PS_Slash;
    public ParticleSystem PS_Heart;
    public Rigidbody2D rb2d;
    public Collider2D cc2d;
    public Animator _animator;
    public Dialogue myDialogue;
    public AudioSource AS_Slash;
    private bool isFree;


    private void Awake()
    {
        if(LevelDataHolder.Instance)
        {
            LevelDataHolder.Instance.AmountOfHostages++;
        }
        float delay = Random.Range(0f, 5f);
        Invoke(nameof(DelayedAnimation), delay);
    }

    private void DelayedAnimation()
    {
        _animator.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Projectile"))
        {
            FreeSelf();
        }
        if (collision.CompareTag("Ground"))
        {
            if (isFree)
            {
                Debug.Log("Collided with" + collision.gameObject.name);
                _animator.SetBool("Free", true);
                Destroy(GetComponent<Rigidbody2D>());
                PS_Heart.Play();
                
                if(myDialogue.dialogue.Length > 0)
                {
                    UIManager.Instance.StartDialogue(myDialogue);
                }
            }
        }
        if(collision.CompareTag("Player") && isFree)
        {
            PS_Heart.Play();
        }
    }

    Coroutine _freeSelfCO;
    
    public void FreeSelf()
    {

        if (_freeSelfCO == null)
        {
            LevelDataHolder.Instance.GirlsSaved++;
            _freeSelfCO = StartCoroutine(FreeSelfCO());
        }
       
    }

    IEnumerator FreeSelfCO()
    {
        _animator.speed = 0;
        PS_Slash.Play();
        while (PS_Slash.isPlaying)
        {
            AS_Slash.Play();
            yield return new WaitForSeconds(0.1f);
        }
        AS_Slash.loop = false;
        yield return new WaitForSeconds(PS_Slash.main.duration);
        Destroy(rope);
        isFree = true;
        _animator.speed = 1;
        //transform.eulerAngles = Vector3.zero;
        _animator.SetBool("Flying", true);
        rb2d.gravityScale = 5;
        //cc2d.isTrigger = false;
        //this.gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");
        this.enabled = false;
    }    

}
    [System.Serializable]
    public class Dialogue
    {
        public Sprite pic;
        [TextArea(1,5)]
        public string[] dialogue;
    }
