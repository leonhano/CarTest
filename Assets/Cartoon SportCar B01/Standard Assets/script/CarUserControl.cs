using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void OriginalControl()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            Debug.Log("h = " + h + " y =" + v + " handbrake = " + handbrake);
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
        private void AutoDrive()
        {
            float h = UnityEngine.Random.value;//CrossPlatformInputManager.GetAxis("Horizontal");
            float v = UnityEngine.Random.value; //CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = 0f; // CrossPlatformInputManager.GetAxis("Jump");
            //Debug.Log("h = " + h + " y =" + v + " handbrake = " + handbrake);
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }

        private void FixedUpdate()
        {
            //OriginalControl();
            AutoDrive();
        }
    }
}
