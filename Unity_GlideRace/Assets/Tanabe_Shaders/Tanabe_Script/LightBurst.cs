//Imageに登録して使用する
//2Dイメージに光帯を通過させるシェーダーコントロールプログラム

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class lightburst : MonoBehaviour {

	//光帯が通過するまでの時間
	[SerializeField]
	private float transTime = 0.25f;
	//光帯が通過するまでの待機時間
	[SerializeField]
	private float period = 3.0f;

	private Material m;
	private float timer;
	private bool b;
	void Start () {
		m = GetComponent<Image>().material;
		if(m == null){
			this.enabled = false;
			return;
		}
		//光帯の位置を送る(0～1.15)
		m.SetFloat("_TransTime",0);
	}
	
	void Update () {
		if(m == null){
			this.enabled = false;
			return;
		}
		
		if(b){//光帯通過
			float t;
			t=timer/transTime*1.15f;
			m.SetFloat("_TransTime",t);
			if(timer>=transTime){
				timer=0;
				b=!b;
				m.SetFloat("_TransTime",0);
			}
		}else{//待機
			if(period<=timer){
				timer=0;
				b=!b;
			}
		}
		timer+=Time.deltaTime;
	}
}
