using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnInfoUI : MonoBehaviour
{
    [SerializeField] private List<Color> colors;
    [SerializeField] private TMP_Text playerTurnInfo;
    [SerializeField] private TurnSystem turnSystem;

    private void Start()
    {
        turnSystem.OnStateChanged += OnStateChanged;
        OnStateChanged();
    }

    private void OnStateChanged()
    {
        int playerNumber = turnSystem.GetCurrentPlayerIndex() + 1;
        string movingPiece = turnSystem.IsMovingElectron() ? "Electron" : "Neutron";

        Color playerColor = colors[playerNumber - 1];
        Color pieceColor = turnSystem.IsMovingElectron() ? playerColor : colors[2];
        
        string playerTurnString = StringUtils.GenerateColorString(playerColor, $"Player {playerNumber} turn\n");
        string movingPieceString = StringUtils.GenerateColorString(pieceColor, $"Moving {movingPiece}");
        
        playerTurnInfo.text = playerTurnString + movingPieceString;
    }
}
