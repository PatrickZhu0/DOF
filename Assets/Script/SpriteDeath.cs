using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDeath : MonoBehaviour {

	// Use this for initialization






	public int deathFrame;












	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


		if (transform.GetComponent<SpritePlayCtrl>().prevFrameIndex == deathFrame - 1 && transform.GetComponent<SpritePlayCtrl>().currentFrameIndex != deathFrame - 1){

			Destroy (gameObject);

		}





	}
}
