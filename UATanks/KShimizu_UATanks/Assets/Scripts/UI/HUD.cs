using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Component that contains all HUD elements
public class HUD : MonoBehaviour
{
    public PlayerData playerData;
    public Text scoreValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerData != null)
        {
            scoreValue.text = playerData.score.ToString(); // Display the score on the HUD
        }
    }
}
