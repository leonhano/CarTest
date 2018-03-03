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
        private GameObject m_carGO;
        private Rigidbody m_Rigidbody;

        private float m_totalDistance = 0;
        private bool m_bStopToCharge;    //stop to charge
        private float m_targetTime = 3.0f;
        public float m_CHARGETIME = 1f;
        public int m_RandomSeed = 1;
       // public const float ChangeTime = 2f;

        private Vector3 m_prePosition = Vector3.zero;


        //for turning; 
        Quaternion newRotation;
        bool bStopAndRotate = false;        //stop and rotate flag, to avoid terrain;
        public float patrolSpeed = 2.5f;           //patrol speed;
        public float detectXDistance = 15f; //detect down to check whether water is too shallow to move;
        protected LayerMask raycastMask;        //raycast mask, ignore water layer;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            m_carGO = m_Car.gameObject;
            m_Rigidbody = GetComponent<Rigidbody>();

            m_prePosition = m_carGO.transform.position;
            m_bStopToCharge = false;
            m_targetTime = m_CHARGETIME;

            newRotation = transform.rotation;
            raycastMask = ~raycastMask;
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
            float h = GetRandomNumber((m_RandomSeed * -1), m_RandomSeed); //CrossPlatformInputManager.GetAxis("Horizontal");
            float v = GetRandomNumber((m_RandomSeed * -1), m_RandomSeed); //CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = 0f; // CrossPlatformInputManager.GetAxis("Jump");
            //Debug.Log("h = " + h + " y =" + v + " handbrake = " + handbrake);

            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }

        private void StopDrive()
        {
            m_Car.Move(0, -10, 100, 10);
        }
        
        public void StartToCharge()
        {
            m_bStopToCharge = true;
            StopDrive();

            RotateTowardCamera.SetEventText("Charge!");
            m_targetTime = m_CHARGETIME;
        }

        public void FinishToCharge()
        {
            m_bStopToCharge = false;
            //m_targetTime = CHARGETIME;
            Debug.Log(this.gameObject.name + "   --- Finish to charge  ----");
            RotateTowardCamera.SetEventText("Go!Go!");
        }

        
        private void FixedUpdate()
        {
            m_targetTime -= Time.deltaTime;
            float dist = Vector3.Distance(m_carGO.transform.position, m_prePosition);
            m_totalDistance += dist;


            //turn if need; 
            if (Quaternion.Angle(transform.rotation, newRotation) > 1)
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime);
            else
                PatrolRaycast(); //if finish rotation, then check whether we can keep on moving;


            if (!m_bStopToCharge)   {
                if (m_targetTime < 0)
                {
                    AutoDrive();
                    m_targetTime = UnityEngine.Random.Range(1, 8);
                }
            }
            else
            {
                
                if (dist < 1.0f)
                {
                    FinishToCharge();
                }

            }
            m_prePosition = m_carGO.transform.position;

            RotateTowardCamera.SetText(m_carGO.name, GetSpeed(), m_totalDistance, UnityEngine.Random.Range(0, 100));
        }

        void PatrolRaycast()
        {
            bStopAndRotate = false;
            Vector3 rayDirection = transform.rotation * Vector3.forward;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, detectXDistance, raycastMask))
            {
                bStopAndRotate = true;
                TurnAround(hit.point);
                //Debug.Log("Shallow waterr, can't move!");             //else, just turn;
            }
            else
            {
                newRotation = transform.rotation;
            }
        }

        public void TurnAround(Vector3 hitPoint)
        {

            Quaternion hitRotation = Tools.LookAtPlayerOnYAxis(transform.position, hitPoint);
            newRotation = hitRotation * Quaternion.AngleAxis(180, transform.up);
            //Debug.Log(this.name + "  : TurnAround ---> newRotation = " + newRotation);

            //         Vector3 targetAngles = hitTransform.eulerAngles + 180f * Vector3.up; // what the new angles should be
            // 
            //         newRotation = Quaternion.Euler(targetAngles);
            if (newRotation == transform.rotation)
            {
                Debug.LogError(this.name + "Wrong, why new rotation is same as transform roatation = " + newRotation);
                return;
            }
        }

        public float GetSpeed()
        {
            return m_Rigidbody.velocity.magnitude;
        }

        //Function to get random number
        private static readonly System.Random getrandom = new System.Random();

        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }
    }

}
