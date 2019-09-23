using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Start Screen component, used to pick map and AI settings
public class StartScreen : MonoBehaviour
{
    public Dropdown mapDropdown;
    public Slider enemyCountSlider;
    public Toggle randomEnemies;
    public Text enemyCount;
    public int selectedMap;
    public int enemyQuantity;
    private List<string> mapTypes = new List<string>() { "Map of the Day", "Random", "Preset Seed" };

    private void Update()
    {
        enemyCount.text = enemyCountSlider.value.ToString(); // Display the enemy count
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
    public void StartGame()
    {
        SetMapType();
        SetAIQuantity();
        ToggleRandomEnemies();
        GameManager.instance.CreateNewGame();
        this.gameObject.SetActive(false);
    }

    // Quit game function when quit button is pressed
    public void QuitGame()
    {
        Application.Quit();
    }
}
