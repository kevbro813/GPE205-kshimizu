using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Start Screen component, used to pick map and AI settings
public class StartMenu : MonoBehaviour
{
    [Header("Sliders")]
    public Slider enemyCountSlider;
    public Slider columnSlider;
    public Slider rowSlider;
    [Space(10)]
    [Header("Text")]
    public Text enemyCount;
    public Text columnCount;
    public Text rowCount;
    [Space(10)]
    [Header("Toggle")]
    public Toggle randomEnemies;
    public Toggle multiplayer;
    [Space(10)]
    [Header("Dropdown")]
    public Dropdown mapDropdown;
    [Space(10)]
    [Header("Input Fields")]
    public InputField playerOneName;
    public InputField playerTwoName;
    [Space(10)]
    [Header("Images")]
    public Image continueImage;
    [Space(10)]
    [Header("Buttons")]
    public Button continueButton;
    [HideInInspector] public int selectedMap;
    [HideInInspector] public int enemyQuantity;
    private List<string> mapTypes = new List<string>() { "Map of the Day", "Random", "Preset Seed" };
    private void Start()
    {
        PopulateList();
    }
    private void Update()
    {
        enemyCount.text = enemyCountSlider.value.ToString(); // Display the enemy count
        columnCount.text = columnSlider.value.ToString(); // Display column value
        rowCount.text = rowSlider.value.ToString(); // Display row value
        if (GameManager.instance.isPreviousGame == true)
        {
            Color color = continueImage.color;
            color.a = 1.0f;
            continueImage.color = color;
            continueButton.enabled = true;
        }
        else
        {
            Color color = continueImage.color;
            color.a = 0.5f;
            continueImage.color = color;
            continueButton.enabled = false;
        }
    }
    void PopulateList()
    {
        mapDropdown.AddOptions(mapTypes);
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
    // Toggle whether to spawn random enemies or enemies based on a preset seed
    void SetRandomEnemies()
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
    // Set the size of the map
    public void SetMapSize()
    {
        GameManager.instance.mapGenerator.tileColumns = (int)columnSlider.value;
        GameManager.instance.mapGenerator.tileRows = (int)rowSlider.value;
    }
    // Determine if the game is multiplayer
    public void SetMultiplayer()
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
    // Set player names to the names entered into the input field
    public void SetPlayerNames()
    {
        if (multiplayer.isOn == true)
        {
            if (playerOneName.text != "")
            {
                GameManager.instance.playerOneName = playerOneName.text;
            }
            else
            {
                GameManager.instance.playerOneName = "Player One";
            }
            if (playerTwoName.text != "")
            {
                GameManager.instance.playerTwoName = playerTwoName.text;
            }
            else
            {
                GameManager.instance.playerTwoName = "Player Two";
            }
        }
        else
        {
            if (playerOneName.text != "")
            {
                GameManager.instance.playerOneName = playerOneName.text;
            }
            else
            {
                GameManager.instance.playerOneName = "Player One";
            }
        }
    }
    // Start game function when start button is pressed
    public void NewGame()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        SetMapType();
        SetMapSize();
        SetAIQuantity();
        SetRandomEnemies();
        SetMultiplayer();
        SetPlayerNames();
        GameManager.instance.gameState = "newgame";
        this.gameObject.SetActive(false);
    }
    public void ContinueGame()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        GameManager.instance.gameState = "resume";
        this.gameObject.SetActive(false);
    }
    // Quit game function when quit button is pressed
    public void QuitGame()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        GameManager.instance.gameState = "quit";
    }
}
