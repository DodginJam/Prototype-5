using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrail : MonoBehaviour
{
    private TrailRenderer currentTrial;
    private Camera mainCamera;
    private GameManager gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (currentTrial == null)
        {
            currentTrial = GetComponent<TrailRenderer>();
        }

        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerScript.ClickAndDragControlOnly == true)
        {
            TrailAtMouseLocation();
            TrailEmitOnMouseDown();
        }
    }

    void TrailAtMouseLocation()
    {
        if (gameManagerScript.IsGameActive)
        {
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            float z = 1;
            transform.position = mainCamera.ScreenToWorldPoint(new Vector3(x, y, z));
        }
    }

    public void TrailEmitOnMouseDown()
    {
        if (Input.GetMouseButton(0) && gameManagerScript.IsGameActive)
        {
            currentTrial.emitting = true;
        }
        else 
        {
            currentTrial.emitting = false;
        }
    }
}
