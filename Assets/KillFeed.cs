using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KillFeed : MonoBehaviour
{
    public static KillFeed instance;
    [SerializeField] GameObject KillListingWithImagePrefab;
    [SerializeField] Sprite[] howImages;

    private void Start()
    {
        instance = this;
    }

    public void AddNewKillListingWithHowImage(string killer, string killed, int howIndex, Color backgroundColor)
    {
        GameObject temp = Instantiate(KillListingWithImagePrefab, transform);
        temp.transform.SetSiblingIndex(0);
        KillListing tempListing = temp.GetComponent<KillListing>();
        tempListing.SetNamesAndHowImage(killer, killed, howImages[howIndex], backgroundColor);
    }
}