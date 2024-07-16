using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Gameplay varibles.
    [field: SerializeField] public List<GameObject> Targets
    { get; private set; } = new List<GameObject>();
    public float SpawnRate
    { get; private set; }
    public int Score 
    { get; private set; }
    public int Lives
    { get; private set; }
    public bool IsGameActive 
    { get; private set; }

    // UI
    public TextMeshProUGUI[] UI_ElementsToModify
    { get; private set; }
    public TextMeshProUGUI ScoreText 
    { get; private set; }
    public TextMeshProUGUI LifeText
    { get; private set; }
    public TextMeshProUGUI VolumeText
    { get; private set; }
    public GameObject GameStartUI
    { get; private set; }
    public GameObject GameOverUI
    { get; private set; }
    public GameObject PauseGameUI
    { get; private set; }
    public Slider VolumeSlider
    { get; private set; }
    public Toggle ControlsToggle
    { get; private set; }

    // Sound
    public AudioSource MusicPlayer
    { get; private set; }
    public float DefaultVolume
    { get; private set; } = 0.15f;
    public float MaxVolumeModifier
    { get; private set; } = 4;

    // Controls
    public bool ClickControlOnly
    { get; private set; }
    public bool ClickAndDragControlOnly
    { get; private set; }

    // VFX
    public MouseTrail MouseTrailScript
    { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        // Assign UI GameObjects TextMeshProGUI components with Variables, for the ones that need modifying during runtime.
        UI_ElementsToModify = GameObject.FindWithTag("UI").GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < UI_ElementsToModify.Length; i++)
        {
            if (UI_ElementsToModify[i].name == "ScoreText")
            {
                ScoreText = UI_ElementsToModify[i];
            }
            else if (UI_ElementsToModify[i].name == "LifeText")
            {
                LifeText = UI_ElementsToModify[i];
            }
        }

        // Set UI Variables to the respective parent GameObjects.
        GameStartUI = GameObject.FindWithTag("UI").transform.Find("StartGameUI").gameObject;
        GameOverUI = GameObject.FindWithTag("UI").transform.Find("EndGameUI").gameObject;
        PauseGameUI = GameObject.FindWithTag("UI").transform.Find("PauseGameUI").gameObject;

        MouseTrailScript = GameObject.Find("MouseTrail").GetComponent<MouseTrail>();

        // Set music player to preferred default volume level.
        MusicPlayer = GetComponent<AudioSource>();
        MusicPlayer.volume = DefaultVolume;

        // Set the Volume Slider to a starting value of 1, and let the maxValue of the Slider be a higher value.
        // The volume slider value will be used to multiply the Default Volume level. The higher the maxValue, the higher the Slider will allow the volume to reach.
        VolumeSlider = GameObject.FindWithTag("UI").transform.Find("StartGameUI/Volume/VolumeSlider").GetComponent<Slider>();
        VolumeText = GameObject.FindWithTag("UI").transform.Find("StartGameUI/Volume/VolumeText").GetComponent<TextMeshProUGUI>();
        VolumeSlider.maxValue = MaxVolumeModifier;
        // The listener assigned to the Slider OnValueChanged Event that calls SetVolume method, assiging the volume to a multiple of the slider value.
        VolumeSlider.onValueChanged.AddListener(sliderValue => SetVolumeFromSlider(sliderValue));
        VolumeSlider.value = 1;
        SetVolumeFromSlider(VolumeSlider.value);

        // Add a Event Listener to the toggle controls button, which will switch between click only and click and drag. Default it to click only on start.
        ControlsToggle = GameObject.FindWithTag("UI").transform.Find("StartGameUI/SwitchControls/ControlsToggle").GetComponent<Toggle>();
        ControlsToggle.onValueChanged.AddListener(toggleBool => ToggleControls(toggleBool));
        ControlsToggle.isOn = false;
        ToggleControls(ControlsToggle.isOn);
    }

    // Update is called once per frame
    void Update()
    {
        PauseGameOnInput(KeyCode.P);
    }

    void PauseGameOnInput(KeyCode inputKey)
    {
        // Can only pause the game during active gameplay.
        if (Input.GetKeyDown(inputKey) && Time.timeScale != 0 && IsGameActive)
        {
            Time.timeScale = 0;
            IsGameActive = false;
            PauseGameUI.SetActive(true);
            MusicPlayer.Pause();
        }
        else if (Input.GetKeyDown(inputKey) && Time.timeScale == 0)
        {
            Time.timeScale = 1;
            IsGameActive = true;
            PauseGameUI.SetActive(false);
            MusicPlayer.Play();
        }
    }

    public void ToggleControls(bool toggleBool)
    {
        if (toggleBool)
        {
            ClickControlOnly = false;
            ClickAndDragControlOnly = true;
        }
        else
        {
            ClickControlOnly = true;
            ClickAndDragControlOnly = false;
        }
    }

    public void SetVolumeFromSlider(float volumeSliderValue)
    {
        // The passed parameter represents the value the slider will have just been set to.
        MusicPlayer.volume = volumeSliderValue * DefaultVolume;
        VolumeText.text = $"Volume: {(float)System.Math.Round(volumeSliderValue, 2)}";
    }

    public void UpdateLives(int livesLost)
    {
        Lives -= livesLost;
        LifeText.text = $"Lives: {Lives}";

        if (Lives <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        IsGameActive = false;
        GameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame(float difficultySpawnRate)
    {
        GameStartUI.SetActive(false);

        Score = 0;
        Lives = 3;
        IsGameActive = true;
        SpawnRate = difficultySpawnRate;

        UpdateLives(0);
        UpdateScore(0);
        StartCoroutine(SpawnTarget());
    }

    public void UpdateScore(int scoreToAdd)
    {
        Score += scoreToAdd;
        ScoreText.text = $"Score: {Score}";
    }

    IEnumerator SpawnTarget()
    {
        while (IsGameActive)
        {
            yield return new WaitForSeconds(SpawnRate);
            int index = UnityEngine.Random.Range(0, Targets.Count);
            Instantiate(Targets[index]);
        }
    }
}
