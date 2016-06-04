using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    [SerializeField]
    private GameObject PlayerObject;
    
    public float xMin, xMax, yMin, yMax;
    public  bool moveX, moveY;

	void Update ()
    {      
         
	    if (xMin >= PlayerObject.transform.localPosition.x)
        {
            moveX = false;
        }
        else if (xMax <= PlayerObject.transform.localPosition.x)
        {
            moveX = false;
        }
        else
        {
            moveX = true;
        }

        if (yMin >= PlayerObject.transform.localPosition.y)
        {
            moveY = false;
        }
        else if (yMax <= PlayerObject.transform.localPosition.y) // + 3f
        {
            moveY = false;
        }
        else
        {
            moveY = true;
        }

        if (moveX && moveY)
        {
            transform.localPosition = new Vector3(PlayerObject.transform.localPosition.x, PlayerObject.transform.localPosition.y, -14f); //
        }
        else if (!moveX && moveY)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, PlayerObject.transform.localPosition.y, -14f); //
        }
        else if (moveX && !moveY)
        {
            transform.localPosition = new Vector3(PlayerObject.transform.localPosition.x, transform.localPosition.y, -14f);
        }

	}
}
