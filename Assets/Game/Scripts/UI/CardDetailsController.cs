using UnityEngine;
using TMPro;

public class CardDetailsController : MonoBehaviour
{
    [Header("Card Details Settings")]
    [SerializeField] TextMeshProUGUI playerPosition;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerScore;
    [SerializeField] TextMeshProUGUI playerTimestamp;
    [SerializeField] SOCardDetails cardDetails;

    void OnEnable()
    {
        this.playerPosition.text = cardDetails.playerPosition;
        this.playerName.text = cardDetails.playerName;
        this.playerScore.text = cardDetails.playerScore;
        this.playerTimestamp.text = cardDetails.playerTimestamp;
    }
}
