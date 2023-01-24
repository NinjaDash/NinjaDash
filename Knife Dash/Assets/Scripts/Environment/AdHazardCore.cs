using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdHazardCore : MonoBehaviour , IDamageable
{
    public bool AdPlayed;
    public float loopDuration;
    public float loopDirection = 1;
    public Vector3 LaserSize;
    public AdHazardType _adHazardType;
    public Collider2D myCollider;
    [SerializeField] Transform LaserSpawnPoint;
    [SerializeField] Transform LaserParent;
    [SerializeField] List<AdLaser> lasers = new List<AdLaser>();
    int tween = -1;

    private void Start()
    {

        switch (_adHazardType)
        {
            #region moving ad
            case AdHazardType.Moving:
                foreach (var item in lasers)
                {
                    item.transform.localScale = LaserSize;
                    item.transform.localPosition = new Vector3(0, LaserSize.y / 2, 0);
                }
               tween = LeanTween.moveLocalY(this.gameObject, LaserSize.y, loopDuration).setLoopPingPong().id;
                break;
            #endregion

            #region Rotating Ad
            case AdHazardType.Rotator:
                foreach (var item in lasers)
                {
                    item.transform.localScale = LaserSize;
                    item.transform.localPosition = Vector3.zero;
                }
                tween = LeanTween.rotateAround(LaserParent.gameObject, Vector3.back * loopDirection, 360, loopDuration).setLoopClamp().id;
                break;
            #endregion

            #region Static Ad 
            case AdHazardType.Straight:
                foreach (var item in lasers)
                {
                    item.transform.localScale = LaserSize;
                    item.transform.localPosition = new Vector3(0, LaserSize.y / 2, 0);
                }
                tween = LeanTween.scaleY(LaserParent.gameObject, 0, loopDuration).setLoopPingPong().id;
                break;
            #endregion
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !AdPlayed)
        {
            collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(2f, transform.position);
            Debug.Log("Play Ad Here");
            RetractAll();
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            RetractAll();
            collision.gameObject.SetActive(false);
        }
    }

    public void RetractAll()
    {
        foreach (var item in lasers)
        {
            item.Retract();
        }
        LaserParent.parent = LaserSpawnPoint;
        LeanTween.scale(LaserSpawnPoint.gameObject, Vector3.zero, 1);
        myCollider.enabled = false;
        AdPlayed = true;
        if (tween != -1)
        {
            LeanTween.cancel(tween);
        }
    }

    public void ApplyDamage(float damage, Vector3 position)
    {
        RetractAll();
    }
}

public enum AdHazardType
{
    Straight,
    Rotator,
    Moving,
}
