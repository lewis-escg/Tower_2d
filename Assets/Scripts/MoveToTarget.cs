using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    private List<GameObject> movingObjects = new();
    public void Move(GameObject objectToMove, Vector3 target, float time, float zPositonDuringMove)
    {
        StartCoroutine(MoveObject(objectToMove, target, time, zPositonDuringMove)); // run async so we dont have to bother having a big library full of any active move functions and their necessary data. much easier
    }
    private IEnumerator MoveObject(GameObject objectToMove, Vector3 target, float time, float zPositonDuringMove)
    {

        if (movingObjects.Contains(objectToMove)) // if object is already moving
        {
            yield break;
        }

        if (time <= 0.0f)
        {
            objectToMove.transform.position = target; // if time less than or equal to zero, instant teleport
            yield break;
        }

        movingObjects.Add(objectToMove);

        float timer = 0.0f;
        Vector3 startPosition = objectToMove.transform.position;
        float horizontalDirection = Mathf.Sign(((startPosition - target).normalized).x); // get horizontal direction as -1 or 1
        float originalZPosition = startPosition.z;

        objectToMove.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidBody); // try getting a rigidbody component
        objectToMove.TryGetComponent<Player>(out Player playerScript); // try getting a playerscript component

        while (timer <= time) // loop until time is elapsed
        {
            if (objectToMove == null)
            {
                yield break; // if object is destroyed mid-movement
            }

            timer += Time.deltaTime; // increment timer
            Vector3 lerpPosition = Vector3.Lerp(startPosition, target, timer / time); // lerp position
            objectToMove.transform.position = new Vector3(lerpPosition.x, lerpPosition.y, zPositonDuringMove); // update position
            rigidBody.velocity = new Vector2(-horizontalDirection, 0); // set velocity to the direction we're going (sets correct direction for sprite) and resets gravity build-up

            yield return null; // wait for next frame
        }
        
        // once finished
        if (objectToMove != null) // if object still exists
        {
            objectToMove.transform.position = new Vector3(target.x, target.y, originalZPosition); // set the position just in case the movement went wrong or something
            movingObjects.Remove(objectToMove); // remove object from moving objects list
            if (rigidBody) // if the object has a rigidbody
            {
                rigidBody.velocity = new Vector2(0, 0); // set velocity to 0 to cancel out any build-up of gravity
            }
            if (playerScript) // if object has the player script (object is a player)
            {
                playerScript.LockControls = false; // unlock controls
            }
        }
    }
}
