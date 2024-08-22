using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public delegate void HeaderButtonClickedDelegate();
    public static HeaderButtonClickedDelegate headerButtonClicked;

    public delegate Task ButtonClickedAsync(string buttonName);
    public static ButtonClickedAsync buttonClicked;

    [Header("Button Settings")]
    [SerializeField] Sprite defaultImage = null;
    [SerializeField] Sprite activeImage = null;
    [SerializeField] bool canSetButtonToActive = false;

    private void OnEnable()
    {
        headerButtonClicked += ActivateButtonSprite;
    }

    void Start()
    {
        if (canSetButtonToActive)
            GetComponent<Image>().sprite = activeImage;

        canSetButtonToActive = false;
    }

    public void HeaderButtonClicked()
    {
        canSetButtonToActive = true;
        headerButtonClicked?.Invoke();
        buttonClicked?.Invoke(gameObject.name);
    }
    private void ActivateButtonSprite()
    {
        if (canSetButtonToActive) gameObject.GetComponent<Image>().sprite = activeImage;
        else gameObject.GetComponent<Image>().sprite = defaultImage;

        canSetButtonToActive = false;
    }

    private void OnDestroy()
    {
        headerButtonClicked -= ActivateButtonSprite;
    }
}
