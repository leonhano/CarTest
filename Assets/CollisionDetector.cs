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
        m_CarControl = this.gameObject.GetComponentInParent<CarUserControl>();

        if (other.gameObject.tag != "Ground")
        {
            m_CarControl.StartToCharge();
            Debug.Log(this.gameObject.name + "   --- StartToCharge to charge  ----  " + other.gameObject.name);

        }

        if(transform != null)
        {
            var collider = other.gameObject.GetComponent<Collider>();
            
            if (collider != null)
            {
                var hitPoint = collider.ClosestPointOnBounds(transform.position);
                m_CarControl.TurnAround(hitPoint);
            }
                
        }
        
        /*
           
        if (other.gameObject.tag == "Car")
        {

            Debug.Log(this.gameObject.name + "   --- Car to charge  ----  " + other.gameObject.name);

        }
        */
    }
    
}
