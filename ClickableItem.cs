using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableItem : MonoBehaviour {

	public virtual void OnPlayerClick(string objHit) {}

	public virtual void HighlightItem() {}
}
