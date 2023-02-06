

## FVM Hyperspace - Testnet

### Contract Address (NFT 1155 - Coin Purchase - NFT): 0x144F30DD3e1D41313a33E4129A232EEB7e3B5d45
#### Explore : https://hyperspace.filfox.info/en/address/0x144F30DD3e1D41313a33E4129A232EEB7e3B5d45


### Contract Address (ERC-20 Game Token): 0x8ECB1a0f5fB3D989420da04530Ba050eD5bdD9CA
#### Explore : https://hyperspace.filfox.info/en/address/0x8ECB1a0f5fB3D989420da04530Ba050eD5bdD9CA

============================

### Script :
https://github.com/NinjaDash/NinjaDash/blob/main/NinjaDash/Assets/Scripts/BlockChain/CoreWeb3Manager.cs
https://github.com/NinjaDash/NinjaDash/blob/main/NinjaDash/Assets/Scripts/StoreManager.cs
https://github.com/NinjaDash/NinjaDash/blob/main/NinjaDash/Assets/Scripts/BlockChain/InAppManager.cs

============================

### Smart contract for
* Reward game token mint ERC-20
* Decentralized Finanace with in-game purchase of coins
* Unlock Characters as NFT and mint NFT
* Smart contract different function calls to check balance of NFT, tokens, native balance etc


### Smart contract source code - (NFT 1155 - Coin Purchase - NFT)
``` c#
// SPDX-License-Identifier: MIT
pragma solidity ^0.8.7;

import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/token/ERC20/ERC20.sol";
import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/utils/structs/EnumerableSet.sol";
import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/access/Ownable.sol";

contract NinjaDashEthToken is ERC20 {

  string constant ddp1F = "Xmt9";

  address public owner;
   IERC20  _token;

 
   modifier onlyOwner {
    require(owner == msg.sender); //if msg.sender != owner, then mint function will fail to execute.
    _;
}

    constructor() ERC20("NinjaDashToken", "NDT") {
      owner = msg.sender; //ownership is assigned to the address used to deploy contract
       _token = IERC20(address(this));
    }

 
    function mint(uint256 value) 
        public 
        onlyOwner
        returns (bool)
    {
        _mint(msg.sender, value  = value * 10 ** 18);
        return true;
    }

    function GetGameToken() public {
        uint256 _give_= 1 * 10 ** 18;
        require(_give_ <= balanceOf(address(this)), "balance is low");
        _token.transfer(msg.sender, _give_);
    }

    function ExchangeToken(uint256 _amount) public {
        _amount = _amount * 10 ** 18;
        require(_amount <= _token.balanceOf(msg.sender), "balance is low");
         transfer(msg.sender, _amount);
    }

     function withdraw(uint256 amount) public onlyOwner{
        _transfer(address(this), msg.sender, amount * 10 ** 18);
    }

        // Allow you to show how many tokens owns this smart contract
    function GetSmartContractBalance() external view returns(uint) {
        return _token.balanceOf(address(this));
    }

     // Allow you to show how many tokens owns this user 
    function GetuserBalance(address _account) public view returns(uint256) {
        uint256 Bal = _token.balanceOf(_account);
        return Bal;
    }

    function GetCurrentTime() public view returns(uint256 _result){
      return _result = block.timestamp;
    }
}
```

### Smart contract source code - (ERC-20 Game Token)

``` c#

// SPDX-License-Identifier: MIT
pragma solidity ^0.8.7;

//Importing ERC 1155 Token contract from OpenZeppelin
import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/token/ERC1155/ERC1155.sol";
import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/access/Ownable.sol";
import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/utils/Strings.sol";


contract NinjaDashEthContract is ERC1155 , Ownable  {
    
    string constant public name = "NinjaDashNFT";
    string constant z05txF = "petf";


    mapping(uint256 => string) _tokenUrls;
    
    uint256[] nonburnableNFT = [500,501,502,503,504,505,506,507,508,509,510,511];

    mapping(address => string) _NFTList;

    constructor() ERC1155("")  {}


    function buyCoins(uint256 _itemId) payable public /*onlyOwner*/{
    }

    //buy burnable nft
    function buyNonBurnItem(uint256 _tokenId, string memory _tokenUrl) public /*onlyOwner*/{
        //IMPORTANT Implement own security (set ownership to users). Not production ready contract
        require(_tokenId <= nonburnableNFT.length , "invalid item");
        _tokenUrls[nonburnableNFT[_tokenId]] = _tokenUrl;
        _mint(msg.sender, nonburnableNFT[_tokenId], 1, "");
           bytes memory a = abi.encodePacked(_NFTList[msg.sender], ",", Strings.toString(nonburnableNFT[_tokenId]));
       _NFTList[msg.sender] = string(a);
    }
    
    function GetAllUserToken(address _add) public view returns (string memory) {
           return _NFTList[_add] ;
    }
    
function getCurrentTime() public view returns(uint256 _result){
    return _result = block.timestamp;
}
 

    function uri(uint256 id) public view virtual override returns (string memory) {
        return _tokenUrls[id];
    }


    function withdraw(address _recipient) public payable onlyOwner {
    payable(_recipient).transfer(address(this).balance);
}
}
```
