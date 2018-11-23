using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

//Host
//Trackerの衝突・入力判定
//結構いじるとことあり

public class TrackerScript : Photon.MonoBehaviour {

	public SteamVR_TrackedObject trackedObject;	//SteamVRでViveトラッカーを扱う宣言

	public bool pushSoundOnce = false;
	public bool pushfaileOnce = false;
	public bool releaseSoundOnce = false;
	public bool collisionTrigger;		//いまオブジェクトとぶつかっているか否かを保持　この間にボタン入力するとゲット
	public bool catchButtonTrigger;		//物理ボタン入力のために使用　PogoPin 3番の電圧状況を保持するフラグ

	public GameObject CollisionObject; 	//いまトラッカーにぶつかっているオブジェクトを保持しておく変数
	private NetworkControllerForHost networkInfo;

	static public GameObject getChildGameObject(GameObject fromGameObject, string withName) {
		Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in ts) if (t.gameObject.name.IndexOf (withName) > -1) return t.gameObject;
		return null;
	}

	void Start () {
		trackedObject = GetComponent<SteamVR_TrackedObject>();	//トラッカー制御スクリプトの取得
		networkInfo = GameObject.Find("GameManagerForHostServer").GetComponent<NetworkControllerForHost> ();
		collisionTrigger = false;								//衝突判定を切っておく
	}

	//オブジェクトと当たっている間は衝突判定フラグをオンにする　このフラグがオンの間にボタン入力したらゲット
	void OnCollisionStay(Collision col){
		if(col.gameObject.tag == "Object"){
			collisionTrigger = true;
			//当たっているオブジェクトを格納しておく　実際にゲットするのはキャッチ処理で行う
			CollisionObject = col.gameObject;
		}
	}

	//オブジェクトから離れたら衝突判定フラグをオフにする　
	void OnCollisionExit(Collision col){
		collisionTrigger = false;
	}

	void Update () {
		//ViveコントローラやViveトラッカーを取得，indexをふって格納
		var device = SteamVR_Controller.Input((int)trackedObject.index);

		//ViveトラッカーのPogoPinの電圧（物理ボタンの押下状況）を格納　Gripは3番目，Triggerは4番目のピン
		catchButtonTrigger = device.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip);

		//空振り時も効果音を鳴らしてあげる
		//if (Input.GetKey (KeyCode.C) && !collisionTrigger) {		//テスト用のキー操作
		if(catchButtonTrigger && !collisionTrigger){
			GameObject player = getChildGameObject (this.transform.gameObject, "Player");
			if(player != null){
				Debug.Log ("Call Sound [Missed]");
				if(!pushfaileOnce){
					networkInfo.PlaySound ("Missed", player.transform.name, "Controller");
					pushfaileOnce = true;
				}
			}
		}

		//衝突判定がオンになっている間にボタン入力が入ったらオブジェクトをゲットする
		//if (Input.GetKey (KeyCode.C) && collisionTrigger) {		//テスト用のキー操作
		if(catchButtonTrigger && collisionTrigger){				//本番用のトラッカーボタン操作
			GameObject player = getChildGameObject (this.transform.gameObject, "Player");
			if(player != null){
				Debug.Log ("Call Sound [Catch]");
				if (!pushSoundOnce) {
					networkInfo.PlaySound ("Catch", player.transform.name, "Controller");
					pushSoundOnce = true;
				}
			}
			CollisionObject.transform.parent = gameObject.transform; //ゲット処理　プレイヤのコントローラに追従するよう親子関係を紐づけ
		}
		//衝突判定がオンになっている間にボタン入力が入ったらオブジェクトをリリースする
		//if (!Input.GetKey (KeyCode.C)) {		//テスト用のキー操作
		if(!catchButtonTrigger){	
			pushSoundOnce = false;
			pushfaileOnce = false;
			GameObject player = getChildGameObject (this.transform.gameObject, "Player");
			if(player != null){
				Debug.Log ("Call Sound [Release]");
				if (!releaseSoundOnce) {
					networkInfo.PlaySound ("Release", player.transform.name, "Controller");
					releaseSoundOnce = true;
				}
			}
			CollisionObject.transform.parent = null; //リリース処理　親子関係を解消して追従しないようにする
		}

		if (catchButtonTrigger) {
			//releaseSoundOnce = false;
		}
	}

	void OnCollisionEnter (Collision col ){
		string objectName = col.transform.name;
		GameObject player = getChildGameObject (this.transform.gameObject, "Player");

		if(player != null){
			Debug.Log ("鳴らしまーす");
			string playerName = player.transform.name;
			Debug.LogFormat ("Call Sound [{0}] by [{1}]", objectName, playerName);
			networkInfo.PlaySound (objectName, playerName, "Object");
		}
	}
}
