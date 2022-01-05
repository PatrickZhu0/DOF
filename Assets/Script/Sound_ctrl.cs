using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_ctrl : MonoBehaviour {

	// Use this for initialization



	AudioSource AS;

	static public GameObject father;
	static public AudioClip audioClip;
	static public GameObject soundObject_pre;
	static GameObject soundObject;



	void Awake (){
//		AS = transform.GetComponent<AudioSource> ();
		soundObject_pre = (GameObject)Resources.Load("SoundObject");
		father = gameObject;


	}


	void Start () {

//		soundObject = new GameObject();



	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Space)){

			soundObject = Instantiate(soundObject_pre) as GameObject;
			soundObject.transform.GetComponent<AudioSource> ().clip = (AudioClip)Resources.Load ("ogg/勇士/男鬼剑/sm_atk_01");
			soundObject.transform.GetComponent<AudioSource> ().Play ();

		}



	}


	static public void PlaySound(string address , bool loop , bool check){


		string[]  sArray=address.Split('/');

		soundObject = Instantiate(soundObject_pre) as GameObject;
		soundObject.transform.name = sArray [sArray.Length - 1];
		soundObject.transform.parent = father.transform;
		soundObject.transform.GetComponent<AudioSource> ().clip = (AudioClip)Resources.Load (address);
		soundObject.transform.GetComponent<AudioSource> ().Play ();

	}
}
