using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooperSoundController : ClickableItem {

	[SerializeField] private AudioSource soundSource;
	[SerializeField] private Animator anim;
	[SerializeField] private GameObject playSymbol;
	[SerializeField] private GameObject pauseSymbol;
	[SerializeField] private GameObject fastForwardSymbol;
	[SerializeField] private GameObject rewindSymbol;

	private GameObject currentIcon = null;
	private GameObject prevIcon = null;

	enum PlayerState {STOPPED, PLAYING, PAUSED, REWINDING, FASTFORWARDING};
	private PlayerState pstate;

	public bool playClipOnAwake=false;


	// Use this for initialization
	void Awake () {
		pstate = PlayerState.STOPPED;
		soundSource = GetComponentInChildren<AudioSource>();
		if(playClipOnAwake) {
			soundSource.Play();
			playSymbol.SetActive(true);
			currentIcon = playSymbol;
			pstate = PlayerState.PLAYING;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnPlayerClick(string objHit) {
		//Debug.Log("In OnPlayerClick");
		if(objHit.Contains("Handle")) {
			//Debug.Log("Clicked Handle");
			anim.SetTrigger("PlayTrigger");
			if(pstate == PlayerState.PLAYING) Pause();
			else Play();
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
		//If there was an active icon before entering this state, deactivate it
		if(currentIcon != null) currentIcon.SetActive(false);

		//Play the clip and transition the FSM
		soundSource.Play();
		pstate = PlayerState.PLAYING;

		//Enable the Play icon and update the ref to the current icon
		playSymbol.SetActive(true);
		prevIcon = currentIcon;
		currentIcon = playSymbol;
	}

	void Pause() {
		
		//If there was an active icon before entering this state, deactivate it
		if(currentIcon != null) currentIcon.SetActive(false);

		//Pause the clip and transition the FSM
		soundSource.Pause();
		pstate = PlayerState.PAUSED;

		//Enable the Play icon and update the ref to the current icon
		pauseSymbol.SetActive(true);
		prevIcon = currentIcon;
		currentIcon = pauseSymbol;
	}

	void Rewind(bool DoOrDont) {
		
		if(DoOrDont) {
			if(currentIcon != null) currentIcon.SetActive(false);
			
			pstate = PlayerState.REWINDING;
			prevIcon = currentIcon;
			currentIcon = rewindSymbol;
			currentIcon.SetActive(true);
			
			Debug.Log("Rewinding");
			soundSource.Pause();
			//soundSource.timeSamples = soundSource.clip.samples - 1;
 			soundSource.pitch = -1;
 			soundSource.Play();
		}
		else {
			currentIcon.SetActive(false);
			currentIcon = prevIcon;
			currentIcon.SetActive(true);
			soundSource.pitch = 1;
			if(currentIcon == playSymbol) {
				pstate = PlayerState.PLAYING;

			}
			else if (currentIcon == pauseSymbol) {
				pstate = PlayerState.PAUSED;
				soundSource.Pause();
			}

			Debug.Log("Stopping Rewind");
			//Figure out what the old state was and switch back to it
			//SHOULD YOU BE ABLE TO REWIND WHILE PAUSED?
		}
	}

	void FastForward() {
		//pstate = PlayerState.FASTFORWARDING;
	}


}
