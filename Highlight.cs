using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {

	private Shader std_shader;
	private Shader highlighted_shader;

	public float highlightRange;
	[SerializeField] private GameObject FPC;

	// Use this for initialization
	void Start () {
		std_shader = Shader.Find("Standard");
		highlighted_shader = Shader.Find("Outlined/Silhouetted Bumped Diffuse");
		//highlightRange = 50.0f;
	}


	void OnMouseOver()
	{
		if(Physics.Raycast(FPC.transform.position, (transform.position - FPC.transform.position).normalized, highlightRange)) {
			Debug.DrawLine(FPC.transform.position, (transform.position - FPC.transform.position).normalized, Color.green, 50.0f);
			//Debug.Log("Wax On");
			foreach(MeshRenderer mr in gameObject.GetComponentsInChildren<MeshRenderer>()) {
				mr.material.shader = highlighted_shader;
			}
		}
		else {
			//Debug.Log("Wax Off");
			foreach(MeshRenderer mr in gameObject.GetComponentsInChildren<MeshRenderer>()) {
				mr.material.shader = std_shader;
			}
		}
	}

	 void OnMouseExit()
	 {
			//Debug.Log("Wax Off");
			foreach(MeshRenderer mr in gameObject.GetComponentsInChildren<MeshRenderer>()) {
				mr.material.shader = std_shader;
			}
	 }
}
