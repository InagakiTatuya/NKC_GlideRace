//Imageに登録して使用する
//2Dイメージに、設定した画像の形で光らせるシェーダー

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RayTrace : MonoBehaviour {

	//何秒で全てを光らせるか
	[SerializeField]
	private float transTime = 1.0f;

	private	const float	waitTime	= 0.5f;

	private Material m;
	private float timer;
	void Start () {
		m = GetComponent<Image>().material;
		if(m == null){
			this.enabled = false;
			return;
		}
		//光らせる位置を送信(0～１)
		m.SetFloat("_TransTime",0);
	}
	
	void Update () {
		if(m == null){
			this.enabled = false;
			return;
		}
		
		float t	= Mathf.Min(timer/transTime,1.0f);
		m.SetFloat("_TransTime",t);
		timer+=Time.deltaTime;

		//光終わったら停止
		if(timer >= transTime + waitTime){
			timer	=	0.0f;
			//this.enabled = false;
			//return;
		}
	}
}
