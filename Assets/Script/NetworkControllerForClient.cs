using UnityEngine;
using System.Collections.Generic;
using Photon;

//Client
//Client側のネットワーク管理
//受け取った音量情報をもとに音量を変化

public class NetworkControllerForClient : Photon.MonoBehaviour
{
	[SerializeField]
	private const string ROOM_NAME  = "RoomA";
	private static PhotonView ScenePhotonView;

	private SoundController SoundInfo;

	private string playerName = "";

	private AudioSource TrackerAudio;												//実際に鳴らす音源
	private List<AudioSource> objectOnceAudioSources = new List<AudioSource>();		//当たった時の一回きりの音声
	private List<AudioSource> objectRepeatAudioSources = new List<AudioSource>();	//立体音響用のリピート音声

	public AudioClip[] controllerAudioClips;
	public AudioClip[] objectOnceAudioClips;
	public AudioClip[] objectRepeatAudioClips;	//こちらはボリューム情報のみ受け取って独立して再生（ホスト側に処理はない）

	// For Debug
	private string clipName = "";

	void Start()
	{
		PhotonNetwork.ConnectUsingSettings( "v.1.0.0" );

		TrackerAudio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

		foreach (AudioClip clip in objectOnceAudioClips) {
			AudioSource tempAudio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
			tempAudio.clip = clip;
			objectOnceAudioSources.Add(tempAudio);
		}

		//格納されているものをもとにAudioSourceを生成→ボリューム情報を受け取って音量を変化
		foreach (AudioClip clip in objectRepeatAudioClips) {
			AudioSource tempAudio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
			tempAudio.clip = clip;
			tempAudio.loop = true;
			tempAudio.volume = 0.0f;
			objectRepeatAudioSources.Add(tempAudio);
		}

		foreach (AudioSource source in objectRepeatAudioSources) {
			source.Play();
		}
		
		ScenePhotonView = this.GetComponent<PhotonView>();
		TrackerAudio = GetComponent<AudioSource> ();

		SoundInfo = GameObject.Find("SoundManager").GetComponent<SoundController>();
	}

	void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		GUILayout.Label(playerName);
		// GUILayout.Label(clipName); // for Debug
	}

	void OnJoinedLobby()
	{
		PhotonNetwork.JoinOrCreateRoom( ROOM_NAME, new RoomOptions(), TypedLobby.Default );
	}

	void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		PhotonNetwork.CreateRoom( ROOM_NAME );
	}

	void OnJoinedRoom()
	{
		int playerID = PhotonNetwork.player.ID;
		ScenePhotonView.RPC("SpawnObject", PhotonTargets.MasterClient, playerID);
		playerName = "Player" + playerID;
		SoundInfo.SetPlayerID(playerID);
		Debug.Log(playerName);
	}

	void Update(){
		//ボリューム変化
		if(playerName != ""){
			if(playerName == "Player2"){
				foreach (AudioSource source in objectRepeatAudioSources) {
					foreach (KeyValuePair<string, float> pair in SoundInfo.requestedVolumeDictForPlayerA) {
						if(pair.Key == source.clip.name){
            				source.volume = pair.Value;
            				break;
						}
        			}
				}
			}

			else if(playerName == "Player3"){
				foreach (AudioSource source in objectRepeatAudioSources) {
					foreach (KeyValuePair<string, float> pair in SoundInfo.requestedVolumeDictForPlayerB) {
						if(pair.Key == source.clip.name){
            				source.volume = pair.Value;
            				break;
						}
        			}
				}
			}
			
		}
	}

	//Host側のPlaySoundに呼ばれたら実行
	[PunRPC]
	void PlayControllerSound(string ClipName, string r_PlayerName)
	{
		if (r_PlayerName == this.playerName) {
			clipName = ClipName;

			foreach (AudioClip clip in controllerAudioClips) {
				if (clip.name == ClipName) {
					TrackerAudio.clip = clip;
					TrackerAudio.Play ();
					Handheld.Vibrate ();
				}
			}
		}
	}

	[PunRPC]
	void PlayObjectSound(string ClipName, string r_PlayerName)
	{
		if (r_PlayerName == this.playerName) {
			clipName = ClipName;

			foreach (AudioSource source in objectOnceAudioSources) {
				if (source.clip.name == ClipName) {
					if(!source.isPlaying){
						source.Play ();
						Handheld.Vibrate ();	//ここに振動入れたけど効果あるのかな
					}
				}
			}
		}
	}

	public string GetPlayerName(){
		return this.playerName;
	}
}