using UnityEngine;
using System.Collections;

public class Database : SingletonCustom<Database> {
    //プレイヤー^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public  const int         PLAYER_MAX = 4;     //参加可能なプレイヤー数

    //キャラクター^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public  const int         CHAR_DATA_MAX = 4; //キャラクター数
    private GameObject[]      m_PlayerCharModelArr; //キャラクターモデル

    //ステータス
    private PlayerCharStateData[] m_CaraStateArr = new PlayerCharStateData[CHAR_DATA_MAX] {
                new PlayerCharStateData(0 ,1f, 0.1f), //char01
                new PlayerCharStateData(1, 1f, 0.1f), //char02
                new PlayerCharStateData(2, 1f, 0.1f), //char03
                new PlayerCharStateData(3, 1f, 0.1f), //char04
            };

    //カメラ描画範囲^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private Rect[] m_CameraRectArr = {
        new Rect(0.0f, 0.0f, 0.0f, 0.0f), //ＮＰＣ用
        new Rect(0.0f, 0.0f, 1.0f, 1.0f), //１人用
        new Rect(0.0f, 0.0f, 1.0f, 0.5f), //２人用
        new Rect(0.0f, 0.0f, 0.5f, 0.5f), //３，４人用
                                     };

    public const int CAMRECTID_NpcOrNon    = 0;
    public const int CAMRECTID_1Play       = 1;
    public const int CAMRECTID_2Play       = 2;
    public const int CAMRECTID_3PlayOver   = 3;

    //公開関数/////////////////////////////////////////////////////////////////
    //キャラクターモデル取得===================================================
    public GameObject GetPlayerModel(int aId) {
        if(m_PlayerCharModelArr == null) { LordPlayerModel(); }
        return m_PlayerCharModelArr[aId];
    }

    //キャラクターステータス取得===============================================
    public PlayerCharStateData GetPlayerCharaState(int aIndex) {
        if(aIndex < 0 || aIndex >= CHAR_DATA_MAX) {
            Debug.LogError("IndexOutOfRangeException");
            return new PlayerCharStateData();
        }
        return m_CaraStateArr[aIndex];
    }

    //カメラ描画範囲===========================================================
    public Rect GetCameraRect(int aCamRectId) {
        return m_CameraRectArr[aCamRectId];
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
        m_PlayerCharModelArr = Resources.LoadAll<GameObject>("Prefab/PlayerModel");
    }

}
