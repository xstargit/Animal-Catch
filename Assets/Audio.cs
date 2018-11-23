using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Test
//Photonなしでも衝突と再生が可能

public class Audio : MonoBehaviour {

	public AudioSource objectCollisionAudios;
	public AudioClip objectCollisionClip;
	
	// Update is called once per frame
	void Start () {
		objectCollisionAudios = gameObject.GetComponent<AudioSource> ();		//PC側で効果音再生の名残
		objectCollisionAudios.clip = objectCollisionClip;
	}
	void OnCollisionEnter (Collision col){
		Debug.Log ("なってまーす");
		objectCollisionAudios.Play ();		//PC側で効果音再生の名残
	}
}
