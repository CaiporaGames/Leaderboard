using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void CardButtonClickedDelegate();
    public static CardButtonClickedDelegate cardButtonClicked;

    [Header("Card Details Settings")]
    [SerializeField] GameObject detailPanel;

    private void Start()
    {
        cardButtonClicked += EnableCardDetailsPanel;
    }

    void EnableCardDetailsPanel()
    {
        detailPanel.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        cardButtonClicked -= EnableCardDetailsPanel;
    }
}
