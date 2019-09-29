using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Start Screen component, used to pick map and AI settings
public class StartGame : MonoBehaviour
{
    public Dropdown mapDropdown;
    public Slider enemyCountSlider;
    public Toggle randomEnemies;
    public Text enemyCount;
    public Slider columnSlider;
    public Slider rowSlider;
    public Text columnCount;
    public Text rowCount;
    public Toggle multiplayer;
    [HideInInspector] public int selectedMap;
    [HideInInspector] public int enemyQuantity;
    private List<string> mapTypes = new List<string>() { "Map of the Day", "Random", "Preset Seed" };

    private void Update()
    {
        enemyCount.text = enemyCountSlider.value.ToString(); // Display the enemy count
        columnCount.text = columnSlider.value.ToString(); // Display column value
        rowCount.text = rowSlider.value.ToString(); // Display row value
    }
    // Set the type of map to be generated
    public void SetMapType()
    {
        selectedMap = mapDropdown.value; // Set selectedMap integer variable to the corresponding dropdown value
        if (selectedMap == 0)
        {
            GameManager.instance.mapGenerator.mapType = MapGenerator.MapType.MapOfTheDay;
        }
        if (selectedMap == 1)
        {
            GameManager.instance.mapGenerator.mapType = MapGenerator.MapType.Random;
        }
        if (selectedMap == 2)
        {
            GameManager.instance.mapGenerator.mapType = MapGenerator.MapType.PresetSeed;
        }
    }

    // Set the number of AI that will be spawned
    public void SetAIQuantity()
    {
        enemyQuantity = (int)enemyCountSlider.value;
        GameManager.instance.maxEnemies = enemyQuantity;
    }
    private void Start()
    {
        PopulateList();
    }
    // Populate the dropdown menu with map type options
    void PopulateList()
    {
        mapDropdown.AddOptions(mapTypes);
    }
    // Toggle whether to spawn random enemies or enemies based on a preset seed
    void ToggleRandomEnemies()
    {
        if (randomEnemies.isOn == true)
        {
            GameManager.instance.isRandomEnemy = true;
        }
        else
        {
            GameManager.instance.isRandomEnemy = false;
        }
    }
    // Start game function when start button is pressed
    public void BeginGame()
    {
        SetMapType();
        SetMapSize();
        SetAIQuantity();
        ToggleRandomEnemies();
        ToggleMultiplayer();
        this.gameObject.SetActive(false);
        GameManager.instance.CreateNewGame();
    }
    public void SetMapSize()
    {
        GameManager.instance.mapGenerator.tileColumns = (int)columnSlider.value;
        GameManager.instance.mapGenerator.tileRows = (int)rowSlider.value;
    }
    public void ToggleMultiplayer()
    {
        if (multiplayer.isOn == true)
        {
            GameManager.instance.isMultiplayer = true;
        }
        else
        {
            GameManager.instance.isMultiplayer = false;
        }
    }
    // Quit game function when quit button is pressed
    public void QuitGame()
    {
        Application.Quit();
    }
}
