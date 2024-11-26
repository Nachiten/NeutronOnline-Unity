using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnInfoUI : MonoBehaviour
{
    [SerializeField] private List<Color> colors;
    [SerializeField] private TMP_Text playerTurnInfo;
    [SerializeField] private TurnSystem turnSystem;
    
    private string[] playerNames;
    
    private void Start()
    {
        SetupPlayerNames();
        
        turnSystem.OnStateChanged += OnStateChanged;
        OnStateChanged();
    }

    private void SetupPlayerNames()
    {
        playerNames = new string[2];
        
        playerNames[0] = PlayerDataHandler.Instance.GetPlayerDataFromPlayerIndex(0).name.ToString();
        playerNames[1] = PlayerDataHandler.Instance.GetPlayerDataFromPlayerIndex(1).name.ToString();
    }

    private void OnStateChanged()
    {
        int playerIndex = turnSystem.GetCurrentPlayerIndex();
        string playerName = playerNames[playerIndex];
        
        // Convert first letter of name to upper case
        playerName = char.ToUpper(playerName[0]) + playerName.Substring(1);
        
        string movingPiece = turnSystem.IsMovingElectron() ? "Electron" : "Neutron";

        Color playerColor = colors[playerIndex];
        Color pieceColor = turnSystem.IsMovingElectron() ? playerColor : colors[2];
        
        string playerTurnString = StringUtils.GenerateColorString(playerColor, $"{playerName}'s turn\n");
        string movingPieceString = StringUtils.GenerateColorString(pieceColor, $"Moving {movingPiece}");
        
        playerTurnInfo.text = playerTurnString + movingPieceString;
    }
}
