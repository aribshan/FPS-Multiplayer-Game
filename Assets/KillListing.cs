using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class KillListing : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI killerDisplay;
    [SerializeField] TextMeshProUGUI killedDisplay;
    [SerializeField] Image howImageDisplay;
    [SerializeField] Image backgroundImage;


    void Start()
    {
        Destroy(gameObject, 10f);
    }

    public void SetNamesAndHowImage(string killerName, string killedName, Sprite howImage, Color backgroundColor)
    {
        killedDisplay.text = killedName;
        killerDisplay.text = killerName;
        howImageDisplay.sprite = howImage;
        backgroundImage.color = backgroundColor;
    }

    
}
