using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class GameSceneManager : BaseObject {

    private PlayerManager m_PlyMgr;

    //シーンステート
    private StateManager m_gameState;

    public const int STATE_MAX   = 4;
    public const int STATE_Ready = 0; //準備
    public const int STATE_Lace  = 1; //レース中
    public const int STATE_End   = 2; //ゲーム終了
    public const int STATE_Next  = 3; //次のシーンに移行する

    //タイマー
    private float           m_timer; //（秒）
    
    //キャンバス
    private Canvas          m_Canvas;
    private TimeSprite      m_SoriteTimer;

    //初期化===================================================================
    void Awake() {
       base.m_doNotPause = true;
    }
	void Start() {
    
        //ステート設定-------------------------------------
        UnityAction[] init = new UnityAction[STATE_MAX] {
            GameState00Init,
            GameState01Init,
            null,
            null,
        };
        UnityAction[] update = new UnityAction[STATE_MAX] {
            GameState00Update,
            GameState01Update,
            null,
            null,
        };
        m_gameState = new StateManager(STATE_MAX, init, update, true);
	    
        //タイマー
        m_timer = 0f;
        //キャンバス関連
        CanvasRelationInit();
        m_SoriteTimer = new TimeSprite();
        m_SoriteTimer.Initialize(m_Canvas.transform, "Timer");
    
        /*
        //PlayerManager取得
        
        //キャラセレクトからデータを受け取る
        //=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#
        //  工事中につき仮の処理
        //=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#
        int[] plymode = { 1, 2, 0, 0 }; // 0 = 参加しない　1 = 人間  2 = NPC
        int[] plychar = { 0, 1, 2, 3 };

        //PlyaerManagerに渡すためのデータを取り出す
        PlayerCharStateData[] data = 
            new PlayerCharStateData[Database.PLAYERCHAR_MAX];

        m_PlyMgr.SetPlyerCharData(data);
        */
    
    }
	
    //更新=====================================================================
	void FixedUpdate() {
        m_gameState.Update();
	}
    //ステート別処理===========================================================
    //ステートRedy ============================================================
    private void GameState00Init() {

    }
    private void GameState00Update() {

        m_gameState.SetNextState(m_gameState.getState + 1);
    }
    //ステートLace ============================================================
    private void GameState01Init() {

    }
    private void GameState01Update() {
        m_timer += Time.fixedDeltaTime * 30f;
        m_SoriteTimer.DrawTimer(m_timer);
    }
    //ステートEnd  ============================================================
    //ステートNext ============================================================

    //タイム描画===============================================================
    private void TimerInit() {
        m_timer = 0f;
    }

    private void CanvasRelationInit() {
        m_Canvas = transform.GetComponentInChildren<Canvas>();
    }
    

}

