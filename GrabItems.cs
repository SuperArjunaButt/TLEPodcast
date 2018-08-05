using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Grab items script
//Based on the post in Unity Answers from https://answers.unity.com/questions/1268357/pick-upthrow-object.html

public class GrabItems : MonoBehaviour {

	public float speed = 1;
	public bool canGrab = true;
	public GameObject grabbedObject;
	public Transform guide;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			if(!canGrab) {
				ThrowDrop();
			}
			else {
				Pickup();
			}
		}

		if(!canGrab && grabbedObject)
			grabbedObject.transform.position = guide.position;	
	}
	
	// //We can use trigger or collision
	//AG: Maybe keep this commented out, but bring in a new function that talks to/is callable from FPClicker
	//so it selects grabbedObject based on a Raycast.
	// void OnTriggerEnter(Collider col) {
	// 	if(col.gameObject.tag == "Grabbable") {
	// 		if(!grabbedObject)
	// 			grabbedObject = col.gameObject;
	// 	}
	// }

	// void OnTriggerExit(Collider col) {
	// 	if(col.gameObject.tag == "Grabbable") {
	// 		if(canGrab)
	// 			grabbedObject = null;
	// 	}
	// }

	public void checkForGrabbedObject(Collider col) {
		if(col.gameObject.tag == "Grabbable") {
	 		if(!grabbedObject)
				grabbedObject = col.gameObject;
	 	}
		//TODO: This is not yet matched up with the calls ot OnTriggerEnter and Exit
	}

	private void Pickup() {
		if(!grabbedObject) return;

		//We set the object parent to our guide empty object
		grabbedObject.transform.SetParent(guide);

		//AG TODO: We might want to have a separate script, GrabbableItem, which has 3 float variables
		//xOffset, yOffset, and zOffset, that get applied to the transform once it's parented to the guide object.
		//That way we can change its position relative to the camera if we want to show off specific features (e.g. the rubber on the plunger)

		//Set gravity to false while holding it
		//TODO: Change GetComponent call to the more efficient version
		grabbedObject.GetComponent<Rigidbody>().useGravity = false;

		//We apply the same rotation our main object (Camera) has
		grabbedObject.transform.localRotation = transform.rotation;

		//We reposition the object on our guide object
		grabbedObject.transform.position = guide.position;

		//TODO: Disable the collider on the object just in case it causes weird physics shit?

		canGrab = false;

	}


	//TODO: Do we add another function, Use(), for when we want to use the item in a spot and not throw it?

	private void ThrowDrop() {
		if(!grabbedObject) return;

		//Set gravity to true again
		grabbedObject.GetComponent<Rigidbody>().useGravity = true;

		//We don't have to do anything with the grabbed object field anymore
		grabbedObject = null;

		//Apply velocity on throwing
		guide.GetChild(0).gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;

		//Unparent the ball
		guide.GetChild(0).parent = null;

		canGrab = true;

	}
}
