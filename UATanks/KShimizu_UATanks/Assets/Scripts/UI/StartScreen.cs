using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        enemyCount.text = enemyCountSlider.value.ToString();
    }
    public void SetMapType()
    {
        selectedMap = mapDropdown.value;
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
    public void SetAIQuantity()
    {
        enemyQuantity = (int)enemyCountSlider.value;
        // TODO: Finish AI quantity
        GameManager.instance.maxEnemies = enemyQuantity;
    }
    private void Start()
    {
        PopulateList();
    }

    void PopulateList()
    {
        mapDropdown.AddOptions(mapTypes);
    }
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
    public void StartGame()
    {
        SetMapType();
        SetAIQuantity();
        ToggleRandomEnemies();
        GameManager.instance.CreateNewGame();
        this.gameObject.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
