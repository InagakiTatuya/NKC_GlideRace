using UnityEngine;
using System.Collections;

public class PlayerManager : BaseObject {
    
    public const string NAME = "PlayerManager";

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
        m_PlayerArr = GameObject.FindObjectsOfType<PlayerOperateV2>();
        
        #if UNITY_EDITOR
        if(m_PlayerArr.Length != Database.PLAYER_MAX) {
            Debug.LogError("Error：シーン上に存在するPlyerの数が異常デス");
        }
        #endif

    }

}
