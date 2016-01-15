using UnityEngine;
using System.Collections;

public class moveUI : MonoBehaviour {
    //変化したいサイズを書く
    private readonly Vector3 scale = new Vector3(0.5f, 0.5f, 1.0f);

    //先行計算しておいた移動方向をインスペクタで代入する
    [SerializeField]
    private Vector3 moveVec;
    //先行計算しておいた移動方距離をインスペクタで代入する
    [SerializeField]
    private float moveLength;

    private float distance;

    private Transform myObj;
    private Transform targetObj;

	// Use this for initialization
	void Start () {
        targetObj = this.transform.parent.GetChild(0);
        myObj = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
        //移動方向に移動
        myObj.position += moveVec * Time.deltaTime;
        //現在の距離を計算
        distance = Vector3.Distance(myObj.position,targetObj.position);
        //線形保管を用いて指定のサイズから指定のサイズへ変更する
        myObj.localScale = Vector3.one * (1 - distance / moveLength) + scale * (distance / moveLength);

        //所定移動距離分移動したら動作を停止させる
        if (distance >= moveLength)
        {
            this.enabled = false;
        }
	}
}
