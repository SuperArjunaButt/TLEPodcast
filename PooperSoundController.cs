/*
 *	Handles the play/pause/rewind/fast forward functions of the toilet podcast clips,
 *	triggers the appropriate animations, and enables/disables the icons for each function. *
 */

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
	private PlayerState prevState;
	private PlayerState currentState;

	public bool playClipOnAwake=false;


	// Use this for initialization
	void Awake () {
		currentState = PlayerState.STOPPED;
		soundSource = GetComponentInChildren<AudioSource>();
		if(playClipOnAwake) {
			soundSource.Play();
			playSymbol.SetActive(true);
			currentIcon = playSymbol;
			currentState = PlayerState.PLAYING;
			prevState = PlayerState.STOPPED;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Handles player interaction with the toilets.
	//Checks to see if we've clicked on the flushing handle
	//or if we've clicked on the seat.
	//If we clicked on the handle, play or pause depending 
	//on the value of the PlayerState enum.
	//If we clicked on the seat, rewind the clip or stop rewinding
	//based on PlayerState.  
	public override void OnPlayerClick(string objHit) {
		//Debug.Log("In OnPlayerClick");
		if(objHit.Contains("Handle")) {
			//Debug.Log("Clicked Handle");
			anim.SetTrigger("PlayTrigger");
			// if(currentState == PlayerState.PLAYING || currentState == PlayerState.REWINDING || currentState == PlayerState.FASTFORWARDING) Pause();
			// else Play();

			if(currentState == PlayerState.FASTFORWARDING) {
				// if(prevState == PlayerState.PAUSED) {
				// 	Pause();
				// }
				// if(prevState == PlayerState.PLAYING) {
				// 	Play();
				// }
			}
			else if(currentState == PlayerState.REWINDING) {
				Rewind(false);
			}
			else if(currentState == PlayerState.PLAYING) {
				Pause();
			}
			else if (currentState == PlayerState.STOPPED) {
				soundSource.Play();
				playSymbol.SetActive(true);
				currentIcon = playSymbol;
				currentState = PlayerState.PLAYING;
				prevState = PlayerState.STOPPED;
			}
			else if(currentState == PlayerState.PAUSED) {

				Play();
			}
		}
		else if(objHit.Contains("Seat")) {
			Debug.Log("Clicked Seat or SeatLid");
			string currentState = (anim.GetCurrentAnimatorClipInfo(0))[0].clip.name;
			//Debug.Log(currentState);
			if(currentState == "Idle") {
				Rewind(true);
			}
			else if (currentState == "LidLoweredIdle") {
				Rewind(false);
			}

		}
		else {
			Debug.Log("Huh?  Hit a " + objHit);
		}
	}

	public override void UnUse(string objUsing) {
		OnUse(objUsing);
	}

	//Fast forward
	public override void OnUse(string objUsing) {
		if(objUsing.Contains("plunger")) {
			//If there was an active icon before entering this state, deactivate it
			if(currentIcon != null) currentIcon.SetActive(false);

			if(currentState == PlayerState.FASTFORWARDING) {
				soundSource.Pause();
				soundSource.pitch = 1;			//Set the pitch back (and therefore playback rate) to normal
				
				currentIcon = prevIcon; 		//Figure out if we were playing or pausing
				currentIcon.SetActive(true);	//Activate whatever the old icon was (play or pause)

				if(currentIcon == playSymbol) { //Update the PlayerState enum and play or pause
					prevState = currentState;
					currentState = PlayerState.PLAYING;
					soundSource.Play();
				}
				else if (currentIcon == pauseSymbol) {
					prevState = currentState;
					currentState = PlayerState.PAUSED;
				}
			}
			else {
				//Change icons
				prevIcon = currentIcon;
				currentIcon = fastForwardSymbol;
				currentIcon.SetActive(true);

				//Change player state
				prevState = currentState;
				currentState = PlayerState.FASTFORWARDING;

				//Fast forward
				soundSource.Pause();
				soundSource.pitch = 2;
				soundSource.Play();
			}
		}
	}

	void Play() {
		//If there was an active icon before entering this state, deactivate it
		if(currentIcon != null) currentIcon.SetActive(false);

		//Sanity check: Reset pitch to standard
		soundSource.pitch = 1;

		//Play the clip and transition the FSM
		soundSource.Play();
		prevState = currentState;
		currentState = PlayerState.PLAYING;

		//Enable the Play icon and update the ref to the current icon
		playSymbol.SetActive(true);
		prevIcon = currentIcon;
		currentIcon = playSymbol;
	}

	void Pause() {
		
		//If there was an active icon before entering this state, deactivate it
		if(currentIcon != null) currentIcon.SetActive(false);

		//Pause the clip and transition the FSM
		Debug.Log("In Pause call to soundSource");
		soundSource.Pause();
		prevState = currentState;		
		currentState = PlayerState.PAUSED;

		//Enable the Pause icon and update the ref to the current icon
		pauseSymbol.SetActive(true);
		prevIcon = currentIcon;
		currentIcon = pauseSymbol;
	}

	//Rewinds the clip.  Passes it a bool to see if we were already rewinding, in
	//which case we go back to either playing the clip or staying paused.
	void Rewind(bool DoOrDont) {
		if(currentState != PlayerState.FASTFORWARDING) {
			
			if(DoOrDont) { //Rewind the clip
				anim.SetTrigger("StartRewindTrigger");
				if(currentIcon != null) currentIcon.SetActive(false);
				
				prevState = currentState;
				currentState = PlayerState.REWINDING;
				prevIcon = currentIcon;
				currentIcon = rewindSymbol;
				currentIcon.SetActive(true);
				
				Debug.Log("Rewinding");
				soundSource.Pause();
				//soundSource.timeSamples = soundSource.clip.samples - 1;
				soundSource.pitch = -2;
				soundSource.Play();
			}
			else { //Stop rewinding the clip
				//From the AudioSource documentation (for 2018.2.0):
				//"If AudioSource.clip is set to the same clip that is playing then the clip will sound like it is 
				//re-started. AudioSource will assume any Play call will have a new audio clip to play."
				//If we were in the play state, we need to pause the clip, change the pitch, then play (or keep it paused if we were in the pause state).
				//Otherwise it will start from the beginning if we just call Play() after coming back from rewinding or fast forwarding.
				anim.SetTrigger("StopRewindTrigger");
				soundSource.Pause();
				soundSource.pitch = 1;			//Set the pitch back (and therefore playback rate) to normal
				
				currentIcon.SetActive(false);	//Disable the rewind icon
				currentIcon = prevIcon; 		//Figure out if we were playing or pausing
				currentIcon.SetActive(true);	//Activate whatever the old icon was (play or pause)

				if(currentIcon == playSymbol) { //Update the PlayerState enum and play or pause
					prevState = currentState;					
					currentState = PlayerState.PLAYING;
					soundSource.Play();
				}
				else if (currentIcon == pauseSymbol) {
					prevState = currentState;					
					currentState = PlayerState.PAUSED;
				}


				Debug.Log("Stopping Rewind");			
			}
		}
	}

	void FastForward() {
		if(currentState != PlayerState.REWINDING) {
			//Update player state
			
			soundSource.pitch = 2;
			if(currentState == PlayerState.PAUSED) {
				soundSource.Play();
			}
			prevState = currentState;
			currentState = PlayerState.FASTFORWARDING;

			//Update the icon
			prevIcon = currentIcon;
			currentIcon.SetActive(false);
			currentIcon = fastForwardSymbol;
			currentIcon.SetActive(true);
			
		}
	}

	public override bool CanBeUsed() {
		return currentState != PlayerState.REWINDING;
	}

}
