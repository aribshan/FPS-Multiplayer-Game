using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace D3xter1922.Scoreboards
{
    public class ScoreboardEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI entryNameText = null;
        [SerializeField] private TextMeshProUGUI entryKillsText = null;
        [SerializeField] private TextMeshProUGUI entryDeathsText = null;

        public void Initialize(ScoreboardEntryData scoreboardEntryData)
        {
            entryNameText.text = scoreboardEntryData.entryName;
            entryKillsText.text = scoreboardEntryData.entryKills.ToString();
            entryDeathsText.text = scoreboardEntryData.entryDeaths.ToString();
        }

    }
}

