using UnityEngine;
using System.Collections;

public struct LayerDef
{
    public const string Terrain = "Terrain";
    public const string Water = "Water";
}

/// <summary>
/// Enemy patrol script
/// </summary>
public class EnemyPatrol : MonoBehaviour {

    public float patrolSpeed = 2.5f;           //patrol speed;
    public float detectXDistance = 15f; //detect down to check whether water is too shallow to move;
    
    protected LayerMask raycastMask;        //raycast mask, ignore water layer;
     
    //for turning; 
    Quaternion newRotation;
    bool bStopAndRotate = false;        //stop and rotate flag, to avoid terrain;

	// Use this for initialization
	void Start () {
        //raycastMask |= (1 << LayerMask.NameToLayer(LayerDef.Water));
        raycastMask = ~raycastMask;
         
        newRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void FixedUpdate()
    {

        //turn if need; 
        if (Quaternion.Angle(transform.rotation, newRotation) > 1)
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime);
        else
            PatrolRaycast(); //if finish rotation, then check whether we can keep on moving;
             
        //if (!bStopAndRotate)
        //    transform.Translate(0, 0, patrolSpeed * Time.deltaTime);   //move based on patrol speed;
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
     
    void TurnAround(Vector3 hitPoint)
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

    //another way to do chase, by receive cmd;
    //trigger is collider collision;
    void InteractiveWithPlayer(GameObject player)
    {
        //collider with player, need chase player;
        ChasePlayer(player);
    }

    //chase player
    void ChasePlayer(GameObject player)
    {
        newRotation = Tools.LookAtPlayerOnYAxis(transform.position, player.transform.position);
        Debug.Log(this.name + "Interactive with player! begin chase!!!! ---> newRotation = " + newRotation);
    }
}
