using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CollisionDetector : MonoBehaviour {
   
    private CarUserControl m_CarControl;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Ground")
        {
            m_CarControl = this.gameObject.GetComponentInParent<CarUserControl>();
            m_CarControl.StartToCharge();
            Debug.Log(this.gameObject.name + "   --- StartToCharge to charge  ----  " + other.gameObject.name);
        }

        /*
        if (other.gameObject.tag == "Car")
        {

            Debug.Log(this.gameObject.name + "   --- Car to charge  ----  " + other.gameObject.name);

        }
        */
    }
}
