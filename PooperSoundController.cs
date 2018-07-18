using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooperSoundController : ClickableItem {

	[SerializeField] private AudioSource soundSource;
	[SerializeField] private Animator anim;
	

	enum PlayerState {STOPPED, PLAYING, PAUSED, REWINDING, FASTFORWARDING};
	private PlayerState pstate;

	// Use this for initialization
	void Start () {
		pstate = PlayerState.STOPPED;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnPlayerClick(string objHit) {
		//Debug.Log("In OnPlayerClick");
		if(objHit.Contains("Handle")) {
			//Debug.Log("Clicked Handle");
			anim.SetTrigger("PlayTrigger");
		}
		else if(objHit.Contains("Seat")) {
			Debug.Log("Clicked Seat or SeatLid");
			string currentState = (anim.GetCurrentAnimatorClipInfo(0))[0].clip.name;
			//Debug.Log(currentState);
			if(currentState == "Idle") {
				anim.SetTrigger("StartRewindTrigger");
				Rewind(true);
			}
			else if (currentState == "LidLoweredIdle") {
				anim.SetTrigger("StopRewindTrigger");
				Rewind(false);
			}

		}
		else {
			Debug.Log("Huh?  Hit a " + objHit);
		}
	}

	void Play() {
		soundSource.Play();
		pstate = PlayerState.PLAYING;
	}

	void Pause() {
		//soundSource.Pause();
		//pstate = PlayerState.PAUSED;
	}

	void Rewind(bool DoOrDont) {
		//pstate = PlayerState.REWINDING;
		if(DoOrDont) {
			Debug.Log("Rewinding");
			soundSource.Pause();
			soundSource.timeSamples = audio.clip.samples - 1;
 			soundSource.pitch = -1;
 			soundSource.Play();
		}
		else {
			Debug.Log("Stopping Rewind");
		}
	}

	void FastForward() {
		//pstate = PlayerState.FASTFORWARDING;
	}


}
