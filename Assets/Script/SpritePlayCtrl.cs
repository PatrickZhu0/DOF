using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;  
using System.Text;  
using System;

public class SpritePlayCtrl : MonoBehaviour {
	public bool isFlip;
	public int startFrame;
	public int endFrame;
	public float frameRate;
	public int currentFrameIndex;
	public int prevFrameIndex;

	void Start (){



	}

	void Update (){
		prevFrameIndex = currentFrameIndex;
		foreach(Transform child in gameObject.transform){
			currentFrameIndex = child.GetComponent<FrameByFramePlay> ().currentFrameIndex;
		}

		if (isFlip)
			transform.localScale = new Vector3(-1 , transform.localScale.y , transform.localScale.z );
		else
			transform.localScale = new Vector3(1 , transform.localScale.y , transform.localScale.z );
	}


	public void ResetSprite (){
		foreach(Transform child in gameObject.transform){
			child.GetComponent<FrameByFramePlay> ().ResetSprite ();
			child.GetComponent<FrameByFramePlay> ().isFlip = isFlip;
		}
	}

	public void PlaySpriteAnimation(int start , int end , float rate , bool flip){
		startFrame = start;
		endFrame = end;
		frameRate = 1f / rate;
		isFlip = flip;
		ResetSprite ();
		foreach(Transform child in gameObject.transform){
			child.GetComponent<FrameByFramePlay> ().startFrame = start;
			child.GetComponent<FrameByFramePlay> ().endFrame = end;
			child.GetComponent<FrameByFramePlay> ().frameRate = 1f / rate;
			child.GetComponent<SpriteRenderer> ().sprite = child.GetComponent<FrameByFramePlay>().spriteArray[start - 1];
		}
	}




}
