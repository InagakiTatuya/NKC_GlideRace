using UnityEngine;
using System.Collections;

public class PlayerManager : BaseObject {

    private const int           PLAYER_MAX = 4;     //プレイヤーの数
    private PlayerOperateV2[]   m_PlayerArr;        //管理するための配列

    //公開関数=================================================================
    //キャラクターのデータを受け取る===========================================
    //  受け取ったデータを使いデータベースから取り出し適用する
    //=========================================================================
    public void SetPlyerCharData(PlayerCharStateData[] aPlayerCharStateArr) {
        //プレイヤー取得
        if(m_PlayerArr == null) FindPlayer();
        
        //データを解読し、渡す

        //描画範囲を決定する関数を呼ぶ

    }

    //非公開関数===============================================================
    //初期化===================================================================
    void Awake() {
        base.m_doNotPause = true;
    }

	void Start () {

    }
	
	void Update () {
	
	}
    
    //プレイヤー取得===========================================================
    //シーン上に配置されているプレイヤーオブジェクトを検索し取得する
    private void FindPlayer() {
        m_PlayerArr = new PlayerOperateV2[PLAYER_MAX];
        for(int i = 0; i < PLAYER_MAX; i++) {
            m_PlayerArr[i] = GameObject.
                Find("Player0" + (i + 1)).GetComponent<PlayerOperateV2>();
        }
    }

}
