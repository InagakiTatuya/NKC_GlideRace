using UnityEngine;
using System.Collections;

public class Database : SingletonCustom<Database> {

    //キャラクター^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public  const int         PLAYERCHAR_MAX = 4; //キャラクター数
    
    private GameObject[]      m_PlayerCharModelArr; //キャラクターモデル
    
    //ステータス
    private PlayerCharStateData[] m_CaraStateArr = new PlayerCharStateData[PLAYERCHAR_MAX] {
                new PlayerCharStateData(0 ,1f, 0.1f), //char01
                new PlayerCharStateData(1, 1f, 0.1f), //char02
                new PlayerCharStateData(2, 1f, 0.1f), //char03
                new PlayerCharStateData(3, 1f, 0.1f), //char04
            };

    //公開関数/////////////////////////////////////////////////////////////////
    //キャラクターモデル取得===================================================
    public GameObject GetPlayerModel(int aId) {
        return m_PlayerCharModelArr[aId];
    }

    //キャラクターステータス取得===============================================
    public PlayerCharStateData GetPlayerCharaState(int aIndex) {
        if(aIndex < 0 || aIndex >= PLAYERCHAR_MAX) {
            Debug.LogError("IndexOutOfRangeException");
            return new PlayerCharStateData();
        }
        return m_CaraStateArr[aIndex];
    }

    //非公開関数///////////////////////////////////////////////////////////////
    //初期化===================================================================
    void Awake() {
        BaseAwake(this);
    }

	void Start () {
	    
	}
	
    //モデル読み込み===========================================================
    private void LordPlayerModel() {
        m_PlayerCharModelArr = Resources.LoadAll<GameObject>("PlayerModel");
    }

}
