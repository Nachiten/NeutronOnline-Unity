using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text localPlayerNameText;
    [SerializeField] private Image localPlayerColor;
    
    [SerializeField] private TMP_Text otherPlayerNameText;
    [SerializeField] private Image otherPlayerColor;

    private void Start()
    {
        PlayerData playerData = PlayerDataHandler.Instance.GetLocalPlayerData();
        
        localPlayerNameText.text = playerData.name.ToString();
        localPlayerColor.color = PlayerAttributes.Instance.GetColorFromColorId(playerData.colorId);
        
        PlayerData otherPlayerData = PlayerDataHandler.Instance.GetOtherPlayerData();
        
        otherPlayerNameText.text = otherPlayerData.name.ToString();
        otherPlayerColor.color = PlayerAttributes.Instance.GetColorFromColorId(otherPlayerData.colorId);
    }
}
