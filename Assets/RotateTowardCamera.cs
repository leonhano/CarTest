using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// make this object is always rotate toward camera;;
/// </summary>
public class RotateTowardCamera : MonoBehaviour {

    public Camera mainCamera = null;
    public static string showText = "";
    private static string m_eventName = "";

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

    public static void SetText(string name, float speed, float distance, float money)
    {
        showText = name + "  Speed: " + speed.ToString("0.00") + "\n  Distance: " + distance.ToString("0.0")
                    + "\n  Money: " + money.ToString("c2") + "\n  Event: " + m_eventName;
    }
    public static void SetEventText(string eventName)
    {
        m_eventName = eventName;
    }
    
    public static void SetText(string text)
    {
        showText = text;
    }
}
