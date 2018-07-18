using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPClicker : MonoBehaviour {

	private Camera cam;
	private RaycastHit hit;
	private ClickableItem clickedItem;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		clickedItem = null;

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(new Vector3((cam.pixelWidth-1)/2, (cam.pixelHeight-1)/2, 0));
			GameObject itm = null;

			if(Physics.Raycast(ray, out hit, 5000.0f)) {
				itm = hit.collider.gameObject;
				Debug.DrawRay(cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f)), itm.transform.position, Color.green, 3.0f, false);
				//Debug.DrawRay(ray, itm.transform.position, Color.green, 3.0f, false);
				clickedItem = itm.GetComponentInParent<ClickableItem>();
				if(clickedItem != null) {
					Debug.Log("Hit a clickable item");
					clickedItem.OnPlayerClick(hit.collider.gameObject.name);
				}
				
			}
		}
	}
}
