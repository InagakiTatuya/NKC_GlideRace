using UnityEngine;
using System.Collections;

public class PlayerManager : BaseObject {
    
    private PlayerOperateV2[]   m_PlayerArr;        //管理するための配列

    private int m_charCnt; //参加人数
    private int m_manCnt;  //人間人数
    private int m_npcCnt;  //コンピュータ人数

    private int m_goalManCnt;
    private int m_goalNpcCnt;

    private bool m_fManAllGoal;

    //公開プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public bool ManAllGoal { get{ return m_fManAllGoal; } }

    ///////////////////////////////////////////////////////////////////////////
    //  公開関数
    ///////////////////////////////////////////////////////////////////////////
    //キャラクターのデータを受け取る===========================================
    //  受け取ったデータを使いデータベースから取り出し適用する
    //=========================================================================
    public void SetingPlyaerData(int[] aMode, int[] aChar) {
        Debug.Log("PlayerManager::SetingPlyaerData()");
        //プレイヤー取得
        if(m_PlayerArr == null) FindPlayer();

        //データを渡す
        for(int i = 0; i < m_PlayerArr.Length; i++) {
            //データ設定
            m_PlayerArr[i].SetCharData(Database.obj.GetPlayerCharaState(aChar[i]));
            //CPU Lv
            int npc = ((aMode[i] >= 2) ? (aMode[i] - 1) : (0));
            m_PlayerArr[i].SetNpcLv = npc;
            //アクティブ
            m_PlayerArr[i].SetActive(aMode[i] != 0);
        }

        //参加人数を設定
        SetPlayNum(aMode);
        
        //描画範囲を決定する関数を呼ぶ
        CameraSeting();
    }

    //ラックを設定する=========================================================
    //  全てのプレイヤーに対してラックを代入する
    //=========================================================================
    public void SendRack(RackState aRack) {
        for(int i = 0; i < m_PlayerArr.Length; i++) {
            m_PlayerArr[i].SetRack(aRack);
        }
    }

    //ゴール受け取り===========================================================
    //  プレイヤーからゴールをした信号を受け取る
    //=========================================================================
    public void ReportGoal(int aPlyNo) {
        if(m_PlayerArr[aPlyNo].isNpc) {
            m_goalNpcCnt++;
        }else{
            m_goalManCnt++;
        }

        //EndStateに移行するフラグ
        if(m_goalManCnt >= m_manCnt) {
            m_fManAllGoal = true;
        }

    }

    ///////////////////////////////////////////////////////////////////////////
    //  非公開関数
    ///////////////////////////////////////////////////////////////////////////
    //初期化===================================================================
    void Awake() {
        base.m_doNotPause = true;
        Initialize();
    }
	//void Start () { }
	//void Update () { }

    //初期化===================================================================
    private void Initialize() {
        m_charCnt = 0;
        m_manCnt  = 0;
        m_npcCnt  = 0;
        m_goalManCnt = 0;
        m_goalNpcCnt = 0;
        m_fManAllGoal = false;
    }
    
    //プレイヤー取得===========================================================
    //シーン上に配置されているプレイヤーオブジェクトを検索し取得する
    private void FindPlayer() {
        Debug.Log("PlayerManager::FindPlayer()");
        m_PlayerArr = GameObject.FindObjectsOfType<PlayerOperateV2>();
        
        #if UNITY_EDITOR
        if(m_PlayerArr.Length != Database.PLAYER_MAX) {
            Debug.LogError("Error：シーン上に存在するPlyerの数が異常デス");
            Debug.Break();
        }
        #endif

        //プレイヤーナンバー変更
        for(int i=0; i < m_PlayerArr.Length; i++) {
            m_PlayerArr[i].SetPlayerNo = (i+1);
        }

    }

    
    //参加人数を設定===========================================================
    private void SetPlayNum(int[] aMode) {
        Debug.Log("PlayerManager::SetPlayNum()");
        int manCnt = 0;
        int npcCnt = 0;
        for(int i=0; i < Database.CHAR_DATA_MAX; i++) {
            if(aMode[i] == 1) manCnt++;
            if(aMode[i] == 2) npcCnt++;
        }

        m_charCnt = manCnt + npcCnt;
        m_manCnt  = manCnt;
        m_npcCnt  = npcCnt;
    }

    //描画カメラ調整===========================================================
    private void CameraSeting() {
        Debug.Log("PlayerManager::CameraSeting()");
        if(m_charCnt == 0) {
            Debug.LogError("エラー：参加プレイヤーが０体でした。");
            return;
        }
        
        //描画範囲の大きさ取得
        int  camRectId = 0;
        if(m_manCnt <= 1) camRectId = Database.CAMRECTID_1Play;
        if(m_manCnt == 2) camRectId = Database.CAMRECTID_2Play;
        if(m_manCnt >= 3) camRectId = Database.CAMRECTID_3PlayOver;
        Rect manRect = Database.obj.GetCameraRect(camRectId);
        Rect npcRect = Database.obj.GetCameraRect(Database.CAMRECTID_NpcOrNon);
        //カメラに適用
        int manCnt = 0;
        for(int i=0; i < m_PlayerArr.Length; i++) {
            //ＮＰＣ
            if(m_PlayerArr[i].isNpc) {
                m_PlayerArr[i].SetCamRect(npcRect);
                break;
            }

            //人間
            if(!m_PlayerArr[i].isNpc) { manCnt++; }
            Rect rect = new Rect();
            rect.x = manRect.width  * ((manCnt-1) / 2);
            rect.y = manRect.height * ((manCnt-1) % 2);
            rect.width  = manRect.width;
            rect.height = manRect.height;
            m_PlayerArr[i].SetCamRect(rect);

        }

    }

}
