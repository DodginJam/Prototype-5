using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Difficulty : MonoBehaviour
{
    public Button CurrentButton
    { get; private set; }
    public GameManager GameManagerScript
    { get; private set; }
    [field: SerializeField] public float SpawnEveryXSeconds
    { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        CurrentButton = GetComponent<Button>();
        CurrentButton.onClick.AddListener(SetDifficultly);

        GameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetDifficultly()
    {
        GameManagerScript.StartGame(SpawnEveryXSeconds);
    }
}
