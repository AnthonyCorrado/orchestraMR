using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class CustomHoldPlacing : Singleton<CustomHoldPlacing> {

    private Vector3 manipulationPreviousPosition;
    public float sensitivity;
    Rigidbody rb;

    void Start()
    {
        sensitivity = 2f;
        //rb = GetComponent<Rigidbody>();
    }

    void PerformManipulationStart(Vector3 position)
    {
        manipulationPreviousPosition = position;
    }

    //void OnTriggerEnter(Collider otherObject)
    //{
    //    Debug.Log("triggered entered: " + otherObject.name.StartsWith("spatial"));
    //    if (otherObject.)
    //    {
    //        Debug.Log("made it to the wall!!!");
    //        isContact = true;
    //    }
    //}

    //void OnTriggerExit(Collider otherObject)
    //{
    //    if (otherObject.name.StartsWith("spatial"))
    //    {
    //        isContact = false;
    //    }
    //}

    void PerformManipulationUpdate(Vector3 position)
    {

        if (GestureManager.Instance.IsManipulating)
        {
            /* TODO: DEVELOPER CODING EXERCISE 4.a */

            Vector3 moveVector = Vector3.zero;
            // 4.a: Calculate the moveVector as position - manipulationPreviousPosition. 
            moveVector = (position - manipulationPreviousPosition) * sensitivity;

            // 4.a: Update the manipulationPreviousPosition with the current position. 
            manipulationPreviousPosition = position;

            // 4.a: Increment this transform's position by the moveVector.
            //transform.parent.position = Vector3.Lerp(transform.parent.position, (transform.parent.position += adjustedMoveVector), 0.5f * Time.deltaTime);

             transform.parent.position += moveVector;

            //transform.position += moveVector;
        }
    }

    //public void ApplyPhysics()
    //{
    //    rb.isKinematic = false;
    //}
}
