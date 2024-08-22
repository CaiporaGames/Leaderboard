using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Details", menuName = "Scriptable Objects / Card Details")]
public class SOCardDetails : ScriptableObject
{
    [HideInInspector] public string playerPosition = null;
    [HideInInspector] public string playerName = null;
    [HideInInspector] public string playerScore = null;
    [HideInInspector] public string playerTimestamp = null;

    private void OnDisable()
    {
        playerPosition = null;
        playerName = null;
        playerScore = null;
        playerTimestamp = null;
    }
}
