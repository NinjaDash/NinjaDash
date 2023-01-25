using Cysharp.Threading.Tasks;
using Defective.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CoreWeb3Manager : MonoBehaviour
{
    #region Singleton
    public static CoreWeb3Manager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    public const string abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"values\",\"type\":\"uint256[]\"}],\"name\":\"TransferBatch\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"TransferSingle\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"value\",\"type\":\"string\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"URI\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_add\",\"type\":\"address\"}],\"name\":\"GetAllUserToken\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"accounts\",\"type\":\"address[]\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"}],\"name\":\"balanceOfBatch\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_itemId\",\"type\":\"uint256\"}],\"name\":\"buyCoins\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"_tokenUrl\",\"type\":\"string\"}],\"name\":\"buyNonBurnItem\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getCurrentTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"_result\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"amounts\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeBatchTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"uri\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_recipient\",\"type\":\"address\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"}]";

    public const string abiToken = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"ExchangeToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"GetCurrentTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"_result\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"GetGameToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"GetSmartContractBalance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_account\",\"type\":\"address\"}],\"name\":\"GetuserBalance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"subtractedValue\",\"type\":\"uint256\"}],\"name\":\"decreaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"addedValue\",\"type\":\"uint256\"}],\"name\":\"increaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

   

    // address of contract
    public const string contract = "0x27015D7B4aee78Ab0356B6b07Fa2736020751a97";
    public const string contractToken = "0xf0c285bd506aED1bC737346752D4401EcFc9bFFC";

    const string chain = "";
    const string network = "";
    const string chainId = "1313161555";
    static string networkRPC= "https://testnet.aurora.dev/";



    public float[] coinCost = { 0.025f, 0.050f, 0.075f, 0.1f, 0.050f };

    public static string userBalance = "0";
    public static string userTokenBalance = "0";

    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    private int expirationTime;
    private string account;

    [SerializeField] TMP_Text _status;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject[] toEnableObjectsAfterLogin;
    [SerializeField] GameObject[] toDisableObjectsAfterLogin;

   
   


    private void Start()
    {
        //LoginWallet();
        //TestIT();

        Application.targetFrameRate = 45;
        
    }

   
    public async void LoginWallet()
    {
        _status.text = "Connecting...";
#if !UNITY_EDITOR
        Web3Connect();
        OnConnected();
#else
        // get current timestamp
        int timestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // set expiration time
        int expirationTime = timestamp + 60;
        // set message
        string message = expirationTime.ToString();
        // sign message
        string signature = await Web3Wallet.Sign(message);
        // verify account
        string account = await EVM.Verify(message, signature);
        int now = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // validate
        if (account.Length == 42 && expirationTime >= now)
        {
            // save account
            PlayerPrefs.SetString("Account", account);

            print("Account: " + account);
            _status.text = "connected : " + account;
            CheckUserBalance();



            if (DatabaseManager.Instance)
            {
                DatabaseManager.Instance.GetData(true);
            }
            // load next scene
        }
        
        for (int i = 0; i < toDisableObjectsAfterLogin.Length; i++)
        {
            toDisableObjectsAfterLogin[i].SetActive(false);
        }
        loadingPanel.SetActive(true);
        SingletonDataManager.userethAdd = account;
        CovalentManager.insta.GetNFTUserBalance();
        getTokenBalance();


        // Debug.Log("LIST OF PUZZLE: " + await CheckPuzzleList());

#endif

    }

    public void EnablePlayPanels()
    {
        for (int i = 0; i < toEnableObjectsAfterLogin.Length; i++)
        {
            toEnableObjectsAfterLogin[i].SetActive(true);
        }
        loadingPanel.SetActive(false);
    }

    async private void OnConnected()
    {
        account = ConnectAccount();
        while (account == "")
        {
            await new WaitForSecondsRealtime(2f);
            account = ConnectAccount();
        };
        account = account.ToLower();
        // save account for next scene
        PlayerPrefs.SetString("Account", account);
        _status.text = "connected : " + account;
        // reset login message
        SetConnectAccount("");
        CheckUserBalance();

        SingletonDataManager.userethAdd = account;


        if (DatabaseManager.Instance)
        {
            DatabaseManager.Instance.GetData(true);
        }
        // load next scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);


        loadingPanel.SetActive(true);
        for (int i = 0; i < toDisableObjectsAfterLogin.Length; i++)
        {
            toDisableObjectsAfterLogin[i].SetActive(false);
        }
        getTokenBalance();
        //CoinBuyOnSendContract(0);
    }


    #region BuyCoins
    async public void CoinBuyOnSendContract(int _pack)
    {
        if (MessageBox.insta) MessageBox.insta.showMsg("Coin purchase process started\nThis can up to minute", false);

        object[] inputParams = { _pack };

        float _amount = coinCost[_pack];
        float decimals = 1000000000000000000; // 18 decimals
        float wei = _amount * decimals;
        print(Convert.ToDecimal(wei).ToString() + " " + inputParams.ToString() + " + " + Newtonsoft.Json.JsonConvert.SerializeObject(inputParams));
        // smart contract method to call
        string method = "buyCoins";

        // array of arguments for contract
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        // value in wei
        string value = Convert.ToDecimal(wei).ToString();
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {


#if !UNITY_EDITOR
            string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
            Debug.Log(response);
#else
            // string response = await EVM.c(method, abi, contract, args, value, gasLimit, gasPrice);
            // Debug.Log(response);
            string data = await EVM.CreateContractData(abi, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contract, value, data, gasLimit, gasPrice);


            Debug.Log(response);
#endif

            if (!string.IsNullOrEmpty(response))
            {
                transID = response;
                InvokeRepeating("CheckTransactionStatus", 1, 5);
                if (MessageBox.insta) MessageBox.insta.showMsg("Your Transaction has been recieved\nCoins will reflect to your account once it is completed!", true);
            }

            if (DatabaseManager.Instance)
            {
                DatabaseManager.Instance.AddTransaction(response, "pending", _pack);
            }


        }
        catch (Exception e)
        {
            if (MessageBox.insta) MessageBox.insta.showMsg("Transaction Has Been Failed", true);
            Debug.Log(e, this);
        }
    }
    #endregion

    #region NonBurnNFTBuy
    async public void NonBurnNFTBuyContract(int _no, string _uri)
    {


        //string uri = "ipfs://bafyreifebcra6gmbytecmxvmro3rjbxs6oqosw3eyuldcwf2qe53gbrpxy/metadata.json";

        Debug.Log("Non Burn NFT Buy  " + _no +  "URI : "+_uri);

        object[] inputParams = { _no, _uri };

        string method = "buyNonBurnItem"; // buyBurnItem";// "buyCoins";

        // array of arguments for contract
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        // value in wei
        string value = "";// Convert.ToDecimal(wei).ToString();
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {

#if !UNITY_EDITOR
                string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
                Debug.Log(response);
#else
            //string response = await EVM.Call(chain, network, contract, abi, args, method, args);
            //Debug.Log(response);
            string data = await EVM.CreateContractData(abi, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contract, "0", data, gasLimit, gasPrice);
            Debug.Log(response);

#endif

           
            if (CovalentManager.insta)
            {
                CovalentManager.insta.GetNFTUserBalance();
            }


            if (MessageBox.insta) MessageBox.insta.showMsg("Your Transaction has been recieved\nIt will reflect to your account once it is completed!", true);


            if (MyNFTCollection.insta)
            {
                MyNFTCollection.insta.DeductCoins(DatabaseManager.Instance.allMetaDataServer[_no + 1].cost);              
               

                MyNFTCollection.insta.DisableLastSelectedButton();
                MyNFTCollection.insta.SetNewData(false);
            }

            

            if (!string.IsNullOrEmpty(response))
            {
                CheckUserBalance();
            }

        }
        catch (Exception e)
        {
            Debug.Log(e, this);
            if (MessageBox.insta)
            {

                MessageBox.insta.showMsg("Server Error", true);
                
            }

        }
    }
    async public void NonBurnNFTPuzzleBuyContract( string _uri)
    {


        //string uri = "ipfs://bafyreifebcra6gmbytecmxvmro3rjbxs6oqosw3eyuldcwf2qe53gbrpxy/metadata.json";

        object[] inputParams = {  _uri };

        string method = "mintPuzzleNFTItem"; // buyBurnItem";// "buyCoins";

        // array of arguments for contract
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        // value in wei
        string value = "";// Convert.ToDecimal(wei).ToString();
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {

#if !UNITY_EDITOR
                string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
                Debug.Log(response);
#else
            //string response = await EVM.Call(chain, network, contract, abi, args, method, args);
            //Debug.Log(response);
            string data = await EVM.CreateContractData(abi, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contract, "0", data, gasLimit, gasPrice);
            Debug.Log(response);

#endif
            if (CovalentManager.insta)
            {
                CovalentManager.insta.GetNFTUserBalance();
            }

            if (MessageBox.insta) MessageBox.insta.showMsg("Your Transaction has been recieved\nIt will reflect to your account once it is completed!", true);
           
            
            
        }
        catch (Exception e)
        {
            Debug.Log(e, this);
            if (MessageBox.insta) MessageBox.insta.showMsg("Server Error", true);
           
        }
    }
    #endregion


    #region CheckTime
    public async Task<string> CheckTimeStatus()
    {
        // smart contract method to call
        string method = "getCurrentTime";
        // array of arguments for contract
        object[] inputParams = { };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args,networkRPC);
            Debug.Log(response);
            return response;

        }
        catch (Exception e)
        {
            Debug.Log(e, this);
            return "";
        }
    }


    public List<string> nftList = new List<string>();

    public async Task<string> CheckPuzzleList()
    {
        // smart contract method to call
        nftList = new List<string>();
        nftList.Clear();
        string method = "GetAllUserToken";
        // array of arguments for contract
        object[] inputParams = { PlayerPrefs.GetString("Account") };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        Debug.Log("CheckPuzzleList ===================");
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);
            Debug.Log("CheckPuzzleList =================== Now");
            Debug.Log(response);
            string[] splitArray = response.Split(char.Parse(",")); //return one word for each string in the array
                                                                   //here, splitArray[0] = Give; splitArray[1] = me etc...

            for (int i = 0; i < splitArray.Length; i++)
            {
                if (string.IsNullOrEmpty(splitArray[i])) continue;
                nftList.Add(splitArray[i]);
            }

           
            return response;

        }
        catch (Exception e)
        {
            Debug.Log(e, this);
            return "";
        }
    }
    #endregion

    #region CheckNFTBalance

    public string balanceNFT;
    async public Task<string> CheckNFTBalance()
    {
        int first = 500;
        int skip = 0;
        try
        {
            string response = await EVM.AllErc1155(chain, network, PlayerPrefs.GetString("Account"), contract, first, skip);
            // string response = await EVM.BalanceOf(chain, network, PlayerPrefs.GetString("Account"), contract, first, skip);
            Debug.Log(response);
            balanceNFT = response;         
            return response;
        }
        catch (Exception e)
        {
            Debug.Log(e, this);
            return null;
        }
    }
    #endregion

    #region CheckUserBalance
    async public void CheckUserBalance()
    {
        try
        {

            string response = await EVM.BalanceOf(chain, network, PlayerPrefs.GetString("Account"),networkRPC);
            if (!string.IsNullOrEmpty(response))
            {
                float wei = float.Parse(response);
                float decimals = 1000000000000000000; // 18 decimals
                float eth = wei / decimals;
                // print(Convert.ToDecimal(eth).ToString());
                Debug.Log(Convert.ToDecimal(eth).ToString());
                userBalance = Convert.ToDecimal(eth).ToString();
                if (InAppManager.Instance)
                {
                    InAppManager.Instance.SetBalanceText();                    
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e, this);
        }
    }
    #endregion

    #region CheckTRansactionStatus
    private string transID;
    async public void CheckTransactionStatus()
    {
        try
        {
            string txConfirmed = await EVM.TxStatus(chain, network, transID, networkRPC);
            print(txConfirmed); // success, fail, pending
            if (txConfirmed.Equals("success") || txConfirmed.Equals("fail"))
            {

                // NonBurnNFTBuyContract(0, "ipfs://bafyreigkpnryq6t53skpbmfylegrp7wl3xkegzxq7ogimvnkzdceisya4a/metadata.json");
                CancelInvoke("CheckTransactionStatus");
                if (DatabaseManager.Instance)
                {
                    DatabaseManager.Instance.ChangeTransactionStatus(transID, txConfirmed);
                }

                
            }

        }
        catch (Exception e)
        {
            Debug.Log(e, this);
        }
    }
    async public void CheckDatabaseTransactionStatus(string Id)
    {
        try
        {
            string txConfirmed = await EVM.TxStatus(chain, network, Id,networkRPC);
            print(txConfirmed); // success, fail, pending
            if (txConfirmed.Equals("success") || txConfirmed.Equals("fail"))
            {
                //CancelInvoke("CheckTransactionStatus");
                if (DatabaseManager.Instance)
                {
                    DatabaseManager.Instance.ChangeTransactionStatus(Id, txConfirmed);
                }
            }



        }
        catch (Exception e)
        {
            Debug.Log(e, this);
        }
    }

    #endregion

    #region getMetaData
    async public void getMetaData()
    {

        try
        {
            string response = await ERC1155.URI(chain, network, contract, "400");
            Debug.Log(response);
        }
        catch (Exception e)
        {
            Debug.Log(e, this);
        }
    }
    #endregion

    #region NFTUploaded

    public void purchaseItem(int _id, bool _skin)
    {
        Debug.Log("purchaseItem");

        MetadataNFT meta = new MetadataNFT();


        meta.itemid = DatabaseManager.Instance.allMetaDataServer[_id].itemid;
        meta.name = DatabaseManager.Instance.allMetaDataServer[_id].name;
        meta.description = DatabaseManager.Instance.allMetaDataServer[_id].description;
        meta.image = DatabaseManager.Instance.allMetaDataServer[_id].imageurl;

        if (MessageBox.insta) MessageBox.insta.showMsg("NFT purchase process started\nThis can up to minute", false);

        //StartCoroutine(UploadNFTMetadata(Newtonsoft.Json.JsonConvert.SerializeObject(meta), _id, _skin));
        NonBurnNFTBuyContract(_id, "test");
    }
    public void uploadPuzzleNFT(string jsonData,string url)
    {
        Debug.Log("purchaseItem");

        MetadataNFT meta = new MetadataNFT();


        meta.jsonData = jsonData;
        meta.name = "Puzzle";
        meta.description = "Puzzle Desc.";
        meta.image = url;

        StartCoroutine(UploadNFTPuzzleMetadata(Newtonsoft.Json.JsonConvert.SerializeObject(meta)));
        //NonBurnNFTPuzzleBuyContract(url);
    }   
    IEnumerator UploadNFTMetadata(string _metadata, int _id, bool _skin)
    {
        if (MessageBox.insta) MessageBox.insta.showMsg("NFT purchase process started\nThis can up to minute", false);
        Debug.Log("Creating and saving metadata to IPFS..." + _metadata);
        Debug.Log("Sending ID To SERVER " + _id);
        WWWForm form = new WWWForm();
        form.AddField("meta", _metadata);

        using (UnityWebRequest www = UnityWebRequest.Post("https://api.nft.storage/store", form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + ConstantManager.nftStorage_key);
            www.timeout = 40;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log("UploadNFTMetadata upload error " + www.downloadHandler.text);

               
                    if (MessageBox.insta) MessageBox.insta.showMsg("Server error\nPlease try again", true);
                
                www.Abort();
                www.Dispose();
            }
            else
            {
                Debug.Log("UploadNFTMetadata upload complete! " + www.downloadHandler.text);

                JSONObject j = new JSONObject(www.downloadHandler.text);
                if (j.HasField("value"))
                {
                    //Debug.Log("Predata " + j.GetField("value").GetField("ipnft").stringValue);
                    SingletonDataManager.nftmetaCDI = j.GetField("value").GetField("url").stringValue; //ipnft
                    //SingletonDataManager.tokenID = j.GetField("value").GetField("ipnft").stringValue; //ipnft
                    Debug.Log("Metadata saved successfully");
                    
                    if (!_skin) NonBurnNFTBuyContract(_id, j.GetField("value").GetField("url").stringValue);
                }
            }
        }
    }
    IEnumerator UploadNFTPuzzleMetadata(string _metadata)
    {
        if (MessageBox.insta) MessageBox.insta.showMsg("NFT purchase process started\nThis can up to minute", false);
        Debug.Log("Creating and saving metadata to IPFS..." + _metadata);
        WWWForm form = new WWWForm();
        form.AddField("meta", _metadata);

        using (UnityWebRequest www = UnityWebRequest.Post("https://api.nft.storage/store", form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + ConstantManager.nftStorage_key);
            www.timeout = 40;
            yield return www.SendWebRequest();

          

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log("UploadNFTMetadata upload error " + www.downloadHandler.text);

                
                if (MessageBox.insta) MessageBox.insta.showMsg("Server error\nPlease try again", true);
                
                www.Abort();
                www.Dispose();
            }
            else
            {
                Debug.Log("UploadNFTMetadata upload complete! " + www.downloadHandler.text);

                JSONObject j = new JSONObject(www.downloadHandler.text);
                if (j.HasField("value"))
                {
                    //Debug.Log("Predata " + j.GetField("value").GetField("ipnft").stringValue);
                    SingletonDataManager.nftmetaCDI = j.GetField("value").GetField("url").stringValue; //ipnft
                    //SingletonDataManager.tokenID = j.GetField("value").GetField("ipnft").stringValue; //ipnft
                    Debug.Log("Metadata saved successfully");
                    //PurchaseItem(cost, _id);
                    NonBurnNFTPuzzleBuyContract(j.GetField("value").GetField("url").stringValue);
                }
            }
        }
    }
    #endregion


    #region Token

    async public void ExchangeToken(int _pack)
    {

        if (MessageBox.insta) MessageBox.insta.showMsg("Exchange token process started", false);

        float decimals = 1000000000000000000; // 18 decimals
        float wei = (_pack) * decimals;

        object[] inputParams = { contractToken, Convert.ToDecimal(wei).ToString() };


        // smart contract method to call
        string method = "transfer";

        // array of arguments for contract
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        // value in wei
        string value = "0";
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {


#if !UNITY_EDITOR
            string response = await Web3GL.SendContract(method, abiToken, contractToken, args, value, gasLimit, gasPrice);
            Debug.Log(response);
#else
            // string response = await EVM.c(method, abi, contract, args, value, gasLimit, gasPrice);
            // Debug.Log(response);
            string data = await EVM.CreateContractData(abiToken, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contractToken, value, data, gasLimit, gasPrice);


            Debug.Log(response);
#endif

            if (!string.IsNullOrEmpty(response))
            {
                // InvokeRepeating("CheckTransactionStatus", 1*Time.timeScale, 5*Time.timeScale);


                if (MessageBox.insta) MessageBox.insta.showMsg("Coin exchanged successfully", true);
                DatabaseManager.Instance.AddCoins(_pack);



            }

        }
        catch (Exception e)
        {
            if (MessageBox.insta) MessageBox.insta.showMsg("Transaction Has Been Failed", true);
            Debug.Log(e, this);
        }
    }
    public async UniTaskVoid getTokenBalance()
    {
        HERE:
        // smart contract method to call
        string method = "balanceOf";
        // array of arguments for contract
        object[] inputParams = { PlayerPrefs.GetString("Account") };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contractToken, abiToken, method, args, networkRPC);
            Debug.Log(response);
            try
            {
                float wei = float.Parse(response);
                float decimals = 1000000000000000000; // 18 decimals
                float eth = wei / decimals;
                // print(Convert.ToDecimal(eth).ToString());
                var tokenBalance = Convert.ToDecimal(eth).ToString();
                userTokenBalance = tokenBalance;
                Debug.Log("Token Bal : " + Convert.ToDecimal(eth).ToString() + " | " + response);

                if (UIManager.Instance) { UIManager.Instance.SetTokenBalanceText(); }

                //if (StoreManager.insta) StoreManager.insta.UpdateBalance();
            }
            catch (Exception)
            {
            }


        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        await UniTask.Delay(6000, true);
        goto HERE;

    }

    async public void getDailyToken()
    {

        object[] inputParams = { };
        string method = "GetGameToken"; // buyBurnItem";// "buyCoins";

        // array of arguments for contract
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        // value in wei
        string value = "";// Convert.ToDecimal(wei).ToString();
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        string response = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {

#if !UNITY_EDITOR
                response = await Web3GL.SendContract(method, abiToken, contractToken, args, value, gasLimit, gasPrice);
                Debug.Log(response);
#else
            string data = await EVM.CreateContractData(abiToken, method, args);
            response = await Web3Wallet.SendTransaction(chainId, contractToken, "0", data, gasLimit, gasPrice);
            Debug.Log(response);
#endif

        }
        catch (Exception e)
        {
            Debug.Log("error" + e);
            if (MessageBox.insta) MessageBox.insta.showMsg("Server Error", true);
            return;
        }

        if (!string.IsNullOrEmpty(response))
        {
            MessageBox.insta.showMsg("Token will be credited soon", true);
            // CheckTransactionStatusWithTransID(response, 1);

        }
        else
        {
           // if (MessaeBox.insta) MessaeBox.insta.showMsg("Server Error", true);
            Debug.Log("In check blank");
        }

    }

    #endregion



}
