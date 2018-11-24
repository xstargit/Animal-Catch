using UnityEngine;
using System.Collections.Generic;
using Photon;

//Host
//Hostのネットワーク管理
//RoomへのJoinやJoin時の検知　ログを表示

public class NetworkControllerForHost : Photon.MonoBehaviour
{
	[SerializeField]
	private string  m_resourcePath_A  = "";
	[SerializeField]
	private string  m_resourcePath_B  = "";
	[SerializeField]
	private GameObject[] objectList = null; // We lerp towards this

	private const string ROOM_NAME  = "RoomA";

	private static PhotonView ScenePhotonView;
	public static int playerID;
	private string m_SceneName;

	private GameObject PlayerA = null;
	private GameObject PlayerB = null;

	private SoundController SoundInfo;

	private PlayerSyncController controllerInfoForPayerA;
	private PlayerSyncController controllerInfoForPayerB;

	static public GameObject getChildGameObject(GameObject fromGameObject, string withName) {
		Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in ts) if (t.gameObject.name.IndexOf (withName) > -1) return t.gameObject;
		return null;
	}

	void Start()
	{
		PhotonNetwork.ConnectUsingSettings( "v.1.0.0" );		//Photonのばじょん
		ScenePhotonView = this.GetComponent<PhotonView>();		//PhotonViewの取得
		SoundInfo = GameObject.Find("SoundManager").GetComponent<SoundController>();	//Soundオブジェクトの湯徳
		controllerInfoForPayerA = null;	//わからん
		controllerInfoForPayerB = null;	//わからん
		
		PlayerA = (GameObject) Resources.Load("Prefabs/Player2");	//プレイヤーの生成
		PlayerB = (GameObject) Resources.Load("Prefabs/Player3");	//プレイヤーの生成
	}

	void Update()
	{
		Debug.Log(PhotonNetwork.connectionStateDetailed.ToString());	//Photonとの接続状況

		// Debug.Log("controllerInfoForPayerA is " + controllerInfoForPayerA);
		// Debug.Log("controllerInfoForPayerB is " + controllerInfoForPayerB);

		//接続しているならば
		if(controllerInfoForPayerA != null){
			// foreach (KeyValuePair<string, float> pair in controllerInfoForPayerA.objectDistanceDict) {
  			//          	Debug.Log (pair.Key + " : " + pair.Value);
   			//      	}
			// controllerInfoForPayerA.objectDistanceDict
			SoundInfo.SetVolumeForPalyerA(controllerInfoForPayerA.objectDistanceDict);
		}

		//接続しているならば
		if(controllerInfoForPayerB != null){
			SoundInfo.SetVolumeForPalyerB(controllerInfoForPayerB.objectDistanceDict);
		}

	}

	void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}

	void OnJoinedLobby()
	{
		PhotonNetwork.JoinOrCreateRoom( ROOM_NAME, new RoomOptions(), TypedLobby.Default );
	}

	void OnJoinedRoom()
	{
		if (PhotonNetwork.playerList.Length == 1)
		{
			playerID = PhotonNetwork.player.ID;
			SoundInfo.SetPlayerID(playerID);
		}

		Debug.Log("playerID: " + playerID);
	}

	void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		PhotonNetwork.CreateRoom( ROOM_NAME );
	}

	//接続されたらプレイヤIDをDebugに表示
	void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected: " + player);
	}

	public void PlaySound(string ClipName, string PlayerName, string mode){
		//ボタン押下で再生
		if(mode == "Controller")
		{
			ScenePhotonView.RPC("PlayControllerSound", PhotonTargets.Others, ClipName, PlayerName);
		}
		//衝突で再生（単発）
		else if(mode == "Object")
		{
			ScenePhotonView.RPC("PlayObjectSound", PhotonTargets.Others, ClipName, PlayerName);
		}
	}

	 //for release (using vibe)
	 [PunRPC]
	 void SpawnObject(int PlayerID)
	 {
	 	GameObject[] cotrollers = GameObject.FindGameObjectsWithTag("Controller"); 
	 	foreach (GameObject controller in cotrollers) {
	 		var renderModel = controller.GetComponentInChildren<SteamVR_RenderModel> ();
	 		if (renderModel != null) {
				Debug.Log ("rendeOK");
	 			if(getChildGameObject(controller, "Player") == null)
	 			{
					Debug.Log ("getchildnull");
	 				string renderModelName = renderModel.renderModelName;
	 				if (renderModelName != null && renderModelName.IndexOf ("{htc}vr_tracker_vive_1_0") > -1) {
						Debug.Log ("renderNameIndexOK");
	 					if (PlayerID == 2)
	 					{
	 						GameObject n_player = Instantiate( PlayerA, controller.transform.position, Quaternion.identity);
	 						n_player.transform.name = "Player" + PlayerID;
	 						n_player.transform.parent = controller.transform;
	 						controllerInfoForPayerA =  n_player.GetComponent<PlayerSyncController>();
	 						Debug.Log("activated player A");
	 						// is_ativate_PalyerA = true;
	 						break;
	 					}
	 					else if (PlayerID == 3)
	 					{
	 						GameObject n_player = Instantiate( PlayerB, controller.transform.position, Quaternion.identity);
	 						n_player.transform.name = "Player" + PlayerID;
	 						n_player.transform.parent = controller.transform;
	 						controllerInfoForPayerB =  n_player.GetComponent<PlayerSyncController>();
	 						Debug.Log("activated player B");
	 						// is_ativate_PalyerB = true;
	 						break;   
	 					}
	 				}
	 			}
	 		}
	 	}
	 }
	
	// for debug (not using vibe)
	 /*
	[PunRPC]
	void SpawnObject(int PlayerID)
	{
		GameObject[] cotrollers = GameObject.FindGameObjectsWithTag("Controller"); 
		foreach (GameObject controller in cotrollers) {
			if(getChildGameObject(controller, "Player") == null)
			{
				if (PlayerID == 2)
				{
					GameObject n_player = Instantiate( PlayerA, controller.transform.position, Quaternion.identity);
					n_player.transform.name = "Player" + PlayerID;
					n_player.transform.parent = controller.transform;
					controllerInfoForPayerA =  n_player.GetComponent<PlayerSyncController>();
					Debug.Log("activated player A");
					// is_ativate_PalyerA = true;
					break;
				}
				else if (PlayerID == 3)
				{
					GameObject n_player = Instantiate( PlayerB, controller.transform.position, Quaternion.identity);
					n_player.transform.name = "Player" + PlayerID;
					n_player.transform.parent = controller.transform;
					controllerInfoForPayerB =  n_player.GetComponent<PlayerSyncController>();
					Debug.Log("activated player B");
					// is_ativate_PalyerB = true;
					break;   
				}
			}
		}
	}
	*/
}