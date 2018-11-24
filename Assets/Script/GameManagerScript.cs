using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Host
//1ゲームの秒数設定
//各プレイヤーのスコア管理と，ゲームの終了を担当

public class GameManagerScript : MonoBehaviour {

	public bool gameStart = false;
	public bool gameEnd = false;

	public float gameTime = 90;	//1ゲーム5分の長さ．5分くらいかなあ．
	public int scoreA;
	public int scoreB;

	public Text TimeText;
	public Text AscoreText;
	public Text BscoreText;

	public AudioSource GameManageAudio;	//ゲーム終了時に効果音を再生
	public AudioSource GameEnd;

	void Start () {
		//GameManageAudio = gameObject.GetComponent<AudioSource> ();
	}

	public void GameStartOn(){
		gameStart = true;
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.S)){
			gameStart = true;
		}
		//全員の接続が確認出来たら
		if(gameStart){
			GameManageAudio.Play ();
			gameTime -= Time.deltaTime;	//時間経過で制限時間の残りを減らしていく
			float timeTextTime = Mathf.Floor (gameTime);	//uGUI表示時に小数点以下を捨てる

			//ゲームの残り時間がある限りuGUIを更新し続ける
			if(gameTime > 0){
				TimeText.text = "Time : " + timeTextTime.ToString ();
				AscoreText.text = "Player A : " + scoreA.ToString();
				BscoreText.text = "Player B : " + scoreB.ToString();
			}
			//制限時間が0以下になったらゲーム終了処理
			if(gameTime < 0){
				//制限時間がなくなってからずーっとリトライとか呼ばれ続けると面倒くさいのでboolを噛ます
				if (!gameEnd) {
					GameManageAudio.Play (); //終了効果音
					if (scoreA > scoreB) TimeText.text = "PlayerA WIN";	//PlayerAの方がスコアが高ければA勝利表示
					if (scoreB > scoreA) TimeText.text = "PlayerB WIN";	//PlayerBの方が　同上
					if (scoreB == scoreA) TimeText.text = "DRAW";		//プレイヤーのスコアが同じなら引き分け表示

					GameEnd.Play ();
					//AscoreText.text = null;	
					//BscoreText.text = null;
					gameEnd = true;		//処理が重複しないようにboolを切る
					// Invoke ("Retry", 10f);	//再起処理　ここは手動でもいいのかも
				}
			}
		}
	}
	// public void Retry(){
	// 	SceneManager.LoadScene ("HostServer");
	// }
}
