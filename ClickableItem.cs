using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableItem : MonoBehaviour {

	public virtual void OnPlayerClick(string objHit) {}

	public virtual void OnUse() {}
	public virtual void OnUse(string objUsing) {}

	public virtual void UnUse(string objUsing) {}

	public virtual bool CanBeUsed() {return false;}

}
