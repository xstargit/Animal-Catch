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

	//PC側で音を鳴らしていた頃の名残
	//public AudioSource objectCollisionAudio;
	//public AudioClip[] objectCollisionClip;
	public string playerName;
	string m_SceneName;	//大事そうだけどなにをしているのかわからない変数
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
		networkInfo = gameManager.GetComponent<NetworkControllerForHost> ();	//大事そうだが使っていなさそう
	}

	void OnCollisionStay(Collision col){
		//objectCollisionAudio.Play ();		//PC側で効果音再生の名残
		string objectName = this.transform.name;	//わからん　後のplayerNameと役割がかぶっていそう

		//ぶつかったコントローラの名前（PlayerAかBか）を取得
		GameObject player = getChildGameObject (col.transform.gameObject, "Player");

		colname = col.gameObject.name;

		//PlayerがSpawnされていなかったら発動しない
		if (player != null){
			playerName = player.transform.name;	//スコアをプレイヤーA・Bどちらに加算するのか判定するための名前格納
			Debug.Log (colname);

			//ここのif文が発動していないのか，（前のバージョンと見比べてほしい，，）
			//Goalタグがついたオブジェクトに当たってもなんにも起きない
			//タグじゃなくて名前で判定しようともしたけどできない
			//取得状況を見たくてpublicだらけにしてます，すません，，

			//ゴールに当たったらスコアを加算する処理
			if(colname == "Goal"){
				Debug.Log ("Goalに衝突");
				switch (playerName) {	//単純にプレイヤーAかBかオブジェクト名で判断しちゃう
				case "Player2":
					gameManagerScript.scoreA += 10;
					Debug.Log(gameManagerScript.scoreA);
					break;
				case "Player3":
					gameManagerScript.scoreB += 10;
					Debug.Log(gameManagerScript.scoreB);
					break;
				}
				Debug.Log ("破壊します");
				audioSource.PlayOneShot (goalsound,0.7f);
				Destroy (this.gameObject); //スコア加算が終わったら自身を破壊
			}
		}
	}
}
