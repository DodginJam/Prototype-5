using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    // Gameplay variables.
    public Rigidbody CurrentRigidbody 
    { get; private set; }
    [field: SerializeField] public int ScoreValue
    { get; private set; }

    // Spawn Position Variables.
    public float XspawnRange
    { get; private set; } = 4;
    public float YspawnPosition
    { get; private set; } = -2;

    // Force Variables.
    public float ForceMin
    { get; private set; } = 10;
    public float ForceMax
    { get; private set; } = 15;
    public float TorqueRange
    { get; private set; } = 10;

    // Other Scripts references.
    public GameManager GameManagerScript 
    { get; private set; }

    // VFX Variables.
    [field: SerializeField] public ParticleSystem[] ExplosionEffects
    { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        // Grabbing references to other Scripts active in scene.
        GameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // Setting up the Target gameObject positions and self-acting forces ready for when it is Instantiated.
        CurrentRigidbody = GetComponent<Rigidbody>();
        transform.position = SpawnPositionGenerate();
        CurrentRigidbody.AddForce(UpwardForceGenerate(), ForceMode.Impulse);
        CurrentRigidbody.AddTorque(TorqueGenerate());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    void OnMouseDown()
    {
        // Can only click on objects and score points when the game is active and click only control scheme enables.
        if (GameManagerScript.IsGameActive && GameManagerScript.ClickControlOnly)
        {
            GameManagerScript.UpdateScore(ScoreValue);

            // VFX
            foreach (ParticleSystem effect in ExplosionEffects)
            {
                Instantiate(effect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
    

    private void OnMouseOver()
    {
        // Can only click on objects and score points when the game is active and click and hold control scheme enabled - therefore check on if mouse is held down here.
        if (Input.GetMouseButton(0) && GameManagerScript.IsGameActive && GameManagerScript.ClickAndDragControlOnly)
        {
            GameManagerScript.UpdateScore(ScoreValue);

            // VFX
            foreach (ParticleSystem effect in ExplosionEffects)
            {
                Instantiate(effect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Destroy all gameObjects that hit the sensor. If a non-bad object hit's the sensor, LoseLife.
        if (other.gameObject.CompareTag("DestroySensor"))
        {
            if (!gameObject.CompareTag("Bad") && GameManagerScript.IsGameActive)
            {
                GameManagerScript.UpdateLives(1);
            }

            Destroy(gameObject);
        }
    }

    Vector3 SpawnPositionGenerate()
    {
        return new Vector3(Random.Range(-XspawnRange, XspawnRange), YspawnPosition);
    }

    Vector3 UpwardForceGenerate()
    {
        return Vector3.up * Random.Range(ForceMin, ForceMax);
    }

    Vector3 TorqueGenerate()
    {
        return new Vector3(Random.Range(-TorqueRange, TorqueRange), Random.Range(-TorqueRange, TorqueRange), Random.Range(-TorqueRange, TorqueRange));
    }
}
