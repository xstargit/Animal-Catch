using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon;

//Host
//オブジェクト自体が自分で管理する処理たち
//衝突押下音再生，ゴール到達判定，GameMangerへの得点加算

//あと衝突したトラッカーのプレイヤー判定はオブジェクトが行う　そのプレイヤに対してゴール時に得点加算する
//ゴール判定も当たり判定を使って済ませたいのでオブジェクト側で行う

public class ObjectScript : Photon.MonoBehaviour {

	public string lastPlayerName;
	public GameObject gameManager;					//ゲーム管理オブジェクト　ここではスコア管理を呼び出す
	public GameManagerScript gameManagerScript;	//スコアを管理しているスクリプト
	public NetworkControllerForHost networkInfo;	//大事そうだが使っていなさそうな変数

	public string colname;

	public AudioClip goalsound;
	public AudioSource audioSource;

	//ここでなにをしているのか難しくてわからぬ，，
	static public GameObject getChildGameObject(GameObject fromGameObject, string withName) {
		Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in ts) if (t.gameObject.name.IndexOf (withName) > -1) return t.gameObject;
		return null;
	}

	void Start () {
		//objectCollisionAudio = this.GetComponent<AudioSource> ();		//PC側で効果音再生の名残

		audioSource = GetComponent<AudioSource> ();
		//スコアの管理にアクセスするために取得
		gameManager = GameObject.Find("GameManagerForHostServer");
		gameManagerScript = gameManager.GetComponent<GameManagerScript> ();
	}

	void OnCollisionStay(Collision col){

		string objectName = col.gameObject.name;

		if (objectName == "Controller"){
			GameObject player = getChildGameObject (col.transform.gameObject, "Player");
			lastPlayerName = player.transform.name;
		}

		else if (objectName == "Goal"){
			switch (lastPlayerName) {
			case "Player2":
				gameManagerScript.scoreA += 10;
				Debug.Log("Updated Score A : " + gameManagerScript.scoreA);
				break;
			case "Player3":
				gameManagerScript.scoreB += 10;
				Debug.Log("Updated Score B : " + gameManagerScript.scoreB);
				break;
			}
			// Debug.Log ("破壊します");
			// audioSource.PlayOneShot (goalsound,0.7f);
			this.transform.parent = null;
			Destroy (this.gameObject); //スコア加算が終わったら自身を破壊
		}
	}
}