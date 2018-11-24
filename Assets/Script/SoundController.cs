using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

//HostとCliantの仲介

public class SoundController : Photon.MonoBehaviour {

	[SerializeField]
	private int PlayerID;

	// for pass
	public Dictionary<string, float> requestedVolumeDictForPlayerA = new Dictionary<string,float> ();
	public Dictionary<string, float> requestedVolumeDictForPlayerB = new Dictionary<string,float> ();

	// // for recieve
	// public Dictionary<string, float> recievedVolumeDictForPlayerA = new Dictionary<string,float> ();
	// public Dictionary<string, float> recievedVolumeDictForPlayerB = new Dictionary<string,float> ();

	private PhotonView  m_photonView    = null;
	private string[] objectNameList = {"Cat","Chicken","Crow","Duck","Gorilla","Iwashi","Sheep","Pig","Dog"};

    public bool CompareDict<TKey, TValue>( Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
     {
         if (dict1 == dict2) return true;
         if ((dict1 == null) || (dict2 == null)) return false;
         if (dict1.Count != dict2.Count) return false;
 
         var valueComparer = EqualityComparer<TValue>.Default;
 
         foreach (var kvp in dict1)
         {
             TValue value2;
             if (!dict2.TryGetValue(kvp.Key, out value2)) return false;
             if (!valueComparer.Equals(kvp.Value, value2)) return false;
         }
         return true;
     }

	// Use this for initialization
	void Start () {
		m_photonView    = GetComponent<PhotonView>();

        foreach (string objectName in objectNameList)
        {
            Debug.Log(objectName);
            requestedVolumeDictForPlayerA.Add(objectName, 0);
            requestedVolumeDictForPlayerB.Add(objectName, 0);
        }

        if( !m_photonView.isMine )
        {
            return;
        }
	}
	
	//デバッグ　
	void Update () {

		Debug.Log("Volume of PlayerA");
        foreach (KeyValuePair<string, float> pair in requestedVolumeDictForPlayerA) {
            Debug.Log (pair.Key + " : " + pair.Value);
        }
        Debug.Log("Volume of PlayerB");
        foreach (KeyValuePair<string, float> pair in requestedVolumeDictForPlayerB) {
            Debug.Log (pair.Key + " : " + pair.Value);
        }

		if( !m_photonView.isMine )
        {
            return;
        }
	}

	//ポスト＆ゲットされたときのみではなくシリアル通信を行う　ここで送信＆受信　値が変わったときのみ動作
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting) {
            //データの送信
            if( !m_photonView.isMine )
        	{
            	return;
        	}

            foreach (string objectName in objectNameList)
            {
                stream.SendNext(requestedVolumeDictForPlayerA[objectName]);
            }
            foreach (string objectName in objectNameList)
            {
                stream.SendNext(requestedVolumeDictForPlayerB[objectName]);
            }
        } else {
            foreach (string objectName in objectNameList)
            {
            	requestedVolumeDictForPlayerA[objectName] = (float) stream.ReceiveNext();
            }
            foreach (string objectName in objectNameList)
            {
            	requestedVolumeDictForPlayerB[objectName] = (float)stream.ReceiveNext();
            }
        }
    }

    public void SetPlayerID(int m_Id){
    	this.PlayerID = m_Id;
    }
		
    public void SetVolumeForPalyerA(Dictionary<string, float> volumeInfo){
        if(!CompareDict(requestedVolumeDictForPlayerA, volumeInfo)){
            Debug.Log("called set volume A");

        	foreach (string key in volumeInfo.Keys) {
        		requestedVolumeDictForPlayerA[key] = volumeInfo[key];
        	}
        }
    }

    public void SetVolumeForPalyerB(Dictionary<string, float> volumeInfo){
        if(!CompareDict(requestedVolumeDictForPlayerB, volumeInfo)){
            Debug.Log("called set volume B");

            foreach (string key in volumeInfo.Keys) {
                requestedVolumeDictForPlayerB[key] = volumeInfo[key];
            }
        }
    }
}
