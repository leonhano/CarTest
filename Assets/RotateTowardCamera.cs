using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// make this object is always rotate toward camera;;
/// </summary>
public class RotateTowardCamera : MonoBehaviour {

    public Camera mainCamera = null;
    public string showText = "";

    private Text textComponent = null;

    // Use this for initialization
    void Start () 
    {
        textComponent = gameObject.GetComponentInChildren<Text>();
        showText = transform.parent.gameObject.name;
    }
        
	// Update is called once per frame
	void Update () {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera)
            OnRotateTowardCamera(mainCamera);

    }

    void OnRotateTowardCamera(Camera camera)
    {
        transform.rotation = Quaternion.LookRotation(camera.transform.forward, camera.transform.up);
    }

    private void FixedUpdate()
    {
        if (textComponent)
            textComponent.text = showText;
    }
}
