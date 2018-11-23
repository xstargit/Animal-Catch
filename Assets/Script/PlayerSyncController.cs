using UnityEngine;
using System.Collections.Generic;

//Host
//距離計算と立体音響調整

public class PlayerSyncController : MonoBehaviour
{
	//わからん
    public Dictionary<string, float> objectDistanceDict = new Dictionary<string,float> ();

	//音量情報
    private static float volumeMin = 0.0f;
    private static float volumeMax = 0.5f;
    private static float distanceMin = 0.0f;
    private static float distanceMax;

	//Radiusの調整用
    private SphereCollider colliderInfo;	

    void Start()
	{
        var childTransform = GameObject.Find("GameMap").GetComponentsInChildren<Transform>();
        foreach (Transform child in childTransform)
		{
            if (child.gameObject.tag == "Object")
            {
                objectDistanceDict.Add(child.name, 0);
            }
        }

		//Radiusのサイズに合わせてMapしてくれる
        colliderInfo = transform.GetComponent<SphereCollider>();
        distanceMax = (float) colliderInfo.radius;
    }

	//立体音響用Colliderに入ってる間は再生
    void OnCollisionStay(Collision col){
        if(col.gameObject.tag == "Object"){
            string target_name = col.transform.name;
            float distance = (float) Vector3.Distance(col.transform.position, this.transform.position);
            float lerped_distance = Mathf.InverseLerp (distanceMin, distanceMax, distance);
            float volume = Mathf.Lerp(volumeMax, volumeMin, lerped_distance);

            objectDistanceDict[target_name] = volume;
        }
    }

    void OnCollisionExit(Collision col){
        if(col.gameObject.tag == "Object"){
            string target_name = col.transform.name;
            objectDistanceDict[target_name] = 0;
        }
    }

	//わからん
	void Update()
	{
		// if (!photonView.isMine)
		// {
		// transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
		// transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
		// }
		// foreach (KeyValuePair<string, float> pair in objectDistanceDict) {
		//     Debug.Log (pair.Key + " : " + pair.Value);
		// }
	}
}