using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image playerNameColor;

    private void Start()
    {
        PlayerData playerData = PlayerDataHandler.Instance.GetLocalPlayerData();
        
        playerNameText.text = playerData.name.ToString();
        playerNameColor.color = PlayerAttributes.Instance.GetColorFromColorId(playerData.colorId);
    }
}
