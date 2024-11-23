using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TMP_Text gridText;

    private object gridObject;

    private void Update()
    {
        gridText.text = gridObject.ToString();
    }

    public void SetGridObject(object _gridObject)
    {
        gridObject = _gridObject;
    }
}