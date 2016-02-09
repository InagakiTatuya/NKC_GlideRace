using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class GameSceneManager : BaseObject {
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
    
        
        //キャラセレクトからデータを受け取る
        
        //データベースからデータを受け取る

        //操作キャラをインスタンス
        
        //ステータス変更
        
        //モデル変更
	    
    
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
        m_timer += Time.fixedDeltaTime;
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


//#############################################################################
//  最終更新者：稲垣達也
//  GameSceneManager内で使用するために作成
//  Timerを表示するための処理等を定義する
//
//#############################################################################
public class TimeSprite {

    private NumberSprite[]  m_TimeSpriteArr; //時間描画
    private const int UNITY_MAXMUN = 8; //管理する数

    private const float ONE_UNIT_SIZE = 60;

    //初期化===================================================================
    public void Initialize(Transform aCanvasTra, string aPath) {
        m_TimeSpriteArr = aCanvasTra.FindChild(aPath).GetComponentsInChildren<NumberSprite>();
        
        for(int i=0; i < m_TimeSpriteArr.Length; i++) {
            m_TimeSpriteArr[i].transform.localPosition = new Vector3(ONE_UNIT_SIZE * (i + 1), 0, 0);
        }
        
    }

    //タイム描画===============================================================
    //  タイマーをスプライトに適応する
    //=========================================================================
    public void DrawTimer(float aTime) {
        int m = (int)aTime / 60;
        int s = (int)aTime - (60 * m);
        int f = (int)((aTime - (m + s)) * 100);
        m_TimeSpriteArr[0].SetNumber(m / 10);
        m_TimeSpriteArr[1].SetNumber(m % 10);
        m_TimeSpriteArr[2].SetNumber(10); 
        m_TimeSpriteArr[3].SetNumber(s / 10);
        m_TimeSpriteArr[4].SetNumber(s % 10);
        m_TimeSpriteArr[5].SetNumber(10); 
        m_TimeSpriteArr[6].SetNumber(f / 10);
        m_TimeSpriteArr[7].SetNumber(f % 10);
        

    }
}


