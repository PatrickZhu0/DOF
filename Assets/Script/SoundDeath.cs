using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDeath : MonoBehaviour {

	// Use this for initialization
	void Start () {





	}
	
	// Update is called once per frame
	void Update () {


		if (!transform.GetComponent<AudioSource>().isPlaying){

			Destroy (gameObject);

		}




	}
}
