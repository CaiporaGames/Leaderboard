using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] TextMeshProUGUI playerPosition;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerScore;
    [SerializeField] SOCardDetails cardDetails;
    //Here should have icon too.

    string timeStamp;

    public void InitializeCardFields(float playerPosition, string playerName, float playerScore, float playerTimestamp)
    {
        this.playerPosition.text = playerPosition.ToString();
        this.playerName.text = playerName;
        this.playerScore.text = playerScore.ToString();
        timeStamp = playerTimestamp.ToString();
    }

    public void CardButtonClicked()
    {
        cardDetails.playerPosition = playerPosition.text;
        cardDetails.playerName = playerName.text;
        cardDetails.playerScore = playerScore.text;
        cardDetails.playerTimestamp = timeStamp;

        GameManager.cardButtonClicked?.Invoke();
    }
}
