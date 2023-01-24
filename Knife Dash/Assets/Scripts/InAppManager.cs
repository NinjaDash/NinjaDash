using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class InAppManager : MonoBehaviour//,IStoreListener
{
    public static InAppManager Instance;

   
   

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
      
    }

    


  


    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.


   


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        if (m_StoreController == null) return;

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }



    public void ProcessPurchase(Product p)
    {
       
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", p.definition.id));

           // MessageBox.insta.showMsg("Purchase of" + p.definition.payout.quantity + " coins successful!", true);
            LocalData data = DatabaseManager.Instance.GetLocalData();
            data.coins += (int)p.definition.payout.quantity;
            DatabaseManager.Instance.UpdateData(data);
            UIManager.Instance.SetCoinText();
           
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.

      
    

       
    }
 
   

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
       // Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
       // MessageBox.insta.showMsg("Purchase Didn't Complete!", true);
    }

   
}



