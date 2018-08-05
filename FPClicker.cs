using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Grabbing code based on https://answers.unity.com/questions/1268357/pick-upthrow-object.html
public class FPClicker : MonoBehaviour {

	private Camera cam;
	private RaycastHit hit;
	private ClickableItem clickedItem;

	private bool canGrab;
	public Transform guide;
	private GameObject grabbedObject;

	public float speed; 

	[SerializeField] private float FPCRange;
	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		clickedItem = null;
		canGrab = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(new Vector3((cam.pixelWidth-1)/2, (cam.pixelHeight-1)/2, 0));
			GameObject itm = null;

			if(Physics.Raycast(ray, out hit, FPCRange)) {
				itm = hit.collider.gameObject;
				//Debug.DrawRay(cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f)), itm.transform.position, Color.green, 3.0f, false);
				//Debug.DrawRay(ray, itm.transform.position, Color.green, 3.0f, false);
				clickedItem = itm.GetComponentInParent<ClickableItem>();
				if(clickedItem != null) {
					//TODO: Shader stuff here?
					Debug.Log("Hit a clickable item");
					clickedItem.OnPlayerClick(hit.collider.gameObject.name);
				}
				
				if(itm.tag == "Grabbable") {
	 				if(!grabbedObject)
						grabbedObject = itm;
					if(!canGrab) {
						ThrowDrop();
						grabbedObject = itm;
						Pickup();
					}
					else {
						Pickup();
					}
				 

				}

			}
		}

		if(!canGrab && grabbedObject) {
			grabbedObject.transform.position = guide.position;	
			//grabbedObject.transform.rotation = guide.rotation;	
		}

	}

	private void Pickup() {
		if(!grabbedObject) return;
		grabbedObject.layer = 10;
		//We set the object parent to our guide empty object
		grabbedObject.transform.SetParent(guide);

		//AG TODO: We might want to have a separate script, GrabbableItem, which has 3 float variables
		//xOffset, yOffset, and zOffset, that get applied to the transform once it's parented to the guide object.
		//That way we can change its position relative to the camera if we want to show off specific features (e.g. the rubber on the plunger)

		//Set gravity to false while holding it
		//TODO: Change GetComponent call to the more efficient version
		grabbedObject.GetComponent<Rigidbody>().useGravity = false;
		grabbedObject.GetComponent<Rigidbody>().drag = Mathf.Infinity;
		grabbedObject.GetComponent<Rigidbody>().angularDrag = Mathf.Infinity;
		//We apply the same rotation our main object (Camera) has
		grabbedObject.transform.localRotation = guide.rotation;

		//We reposition the object on our guide object
		grabbedObject.transform.position = guide.position;

		//TODO: Disable the collider on the object just in case it causes weird physics shit?

		canGrab = false;

	}


	//TODO: Do we add another function, Use(), for when we want to use the item in a spot and not throw it?
	private void Use() {}
	private void ThrowDrop() {
		if(!grabbedObject) return;
		Rigidbody rb = guide.GetChild(0).gameObject.GetComponent<Rigidbody>();
		//Set gravity to true again
		grabbedObject.GetComponent<Rigidbody>().useGravity = true;
		
		//Apply velocity on throwing	
		rb.mass = 1.0f;
		rb.drag = 0.1f;
		rb.angularDrag = 0.1f;
		rb.velocity = transform.forward * speed;

		//Set its layer back to default so we can collide with it like a regular object
		grabbedObject.layer = 0;
		//We don't have to do anything with the grabbed object field anymore
		grabbedObject = null;

		//Unparent the ball
		guide.GetChild(0).parent = null;

		canGrab = true;

	}

}
