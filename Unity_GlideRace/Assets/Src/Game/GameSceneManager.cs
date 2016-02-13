using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using ScenesNames;

public class GameSceneManager : BaseObject {

    //参照^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private PlayerManager m_PlyMgr;  //プレイヤーマネージャ
    private UnityAction   m_fnGotoNextScene; //次のシーンへ移行する関数ポインタ
        
    //キャンバス関連^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private Canvas          m_Canvas;
    private TimeSprite      m_SoriteTimer;
    private NumberSprite    m_CountSprite;
    private RectTransform   m_StartSprite;

    //シーンステート^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private StateManager m_gameState;

    public const int STATE_MAX   = 4; //ステートの数

    public const int STATE_Ready = 0; //準備
    public const int STATE_Lace  = 1; //レース中
    public const int STATE_End   = 2; //ゲーム終了
    public const int STATE_Next  = 3; //次のシーンに移行する

    //タイマー^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private float m_timer; //（秒）


    //初期化===================================================================
    void Awake() {
        base.m_doNotPause = true;

        //PlayerManager取得----------------------------------------------------
        m_PlyMgr = GameObject.FindObjectOfType<PlayerManager>();


    }
	void Start() {
        //シーン移行関数取得---------------------------------------------------
        //if(Application.loadedLevel == SceneName.Game.ToInt()) {
        //    m_fnGotoNextScene = transform.root.GetComponent<SceneLoadManager>().NextScene;
        //}
        //if(m_fnGotoNextScene == null) Debug.Log("シーン移行関数がNULLです");

        //ステート設定---------------------------------------------------------
        UnityAction[] init = new UnityAction[STATE_MAX] {
            GameState00Init,
            GameState01Init,
            GameState02Init,
            null,
        };
        UnityAction[] update = new UnityAction[STATE_MAX] {
            GameState00Update,
            GameState01Update,
            GameState02Update,
            null,
        };
        m_gameState = new StateManager(STATE_MAX, init, update, true);
	    
        //タイマー-------------------------------------------------------------
        m_timer = 0f;
        //キャンバス関連
        CanvasRelationInit(); 
                
        //キャラセレクトからデータを受け取る
        int[] plymode = selectIcon.s_selectNo; // 0 = 参加しない　1 = 人間  2 = NPC
        int[] plychar = selectIcon.s_selectChara;

         // デバック用
        #if UNITY_EDITOR
        if(plymode[0] == 0) {
            plymode =  new int[] {1, 1, 2, 0}; // 0 = 参加しない　1 = 人間  2 = NPC
            plychar =  new int[] {0, 1, 2, 3};
        }
        #endif
        //*/
        
        //データを渡す
        m_PlyMgr.SetingPlyaerData(plymode, plychar);
        
    }

    //キャンバス関連初期化=====================================================
    private void CanvasRelationInit() {
        m_Canvas = transform.GetComponentInChildren<Canvas>();

        m_SoriteTimer = new TimeSprite();
        m_SoriteTimer.Initialize(m_Canvas.transform, "Timer");

        m_CountSprite = m_Canvas.transform.FindChild("Counter").GetComponent<NumberSprite>();
        m_CountSprite.Initialize();

        m_StartSprite = m_Canvas.transform.FindChild("Start").GetComponent<RectTransform>();
        m_StartSprite.gameObject.SetActive(false);
    }
    

    //更新=====================================================================
	void FixedUpdate() {
        m_gameState.Update();
	}

    //=========================================================================
    //  ステート別処理
    //=========================================================================
    //ステートRedy ============================================================
    private void GameState00Init() {
        m_timer = 5f;
        m_CountSprite.SetEnabled(true);
        m_CountSprite.SetEnabledImage(true);

        m_PlyMgr.SendRack(RackState.READY);
    }
    private void GameState00Update() {
        //カウンター
        m_timer -= Time.fixedDeltaTime;
        
        //カウンター　描画
        m_CountSprite.SetNumber((int)(m_timer + 1f));

        //カウンター　スケーリング
        Vector3 sca = m_CountSprite.transform.localScale;
        sca = Vector3.one * (m_timer - Mathf.Floor(m_timer));
        m_CountSprite.transform.localScale = sca;
        
        //次のステート
        if(m_timer <= 0f){
            m_CountSprite.SetEnabledImage(false);
            m_gameState.SetNextState(m_gameState.getState + 1);
        }
    }
    //ステートLace ============================================================
    private void GameState01Init() {
        m_timer = 0f;

        m_StartSprite.gameObject.SetActive(true);
        m_StartSprite.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        
        //Rack
        m_PlyMgr.SendRack(RackState.RACK1);
    }
    private void GameState01Update() {
        //タイマー
        m_timer += Time.fixedDeltaTime;
        //タイマー　描画
        m_SoriteTimer.DrawTimer(m_timer);

        //カウンター　スケーリング
        if(m_StartSprite.gameObject.activeSelf) {
            
            Vector3 sca = m_StartSprite.transform.localScale;
            sca += sca * 6f * Time.fixedDeltaTime;
            m_StartSprite.transform.localScale = sca;

            if(m_StartSprite.transform.localScale.x >= 20f) {
                m_StartSprite.gameObject.SetActive(false);
            }
        }

        //次のステート
        if(m_PlyMgr.ManAllGoal) {
            m_gameState.SetNextState(m_gameState.getState+1);
        }

    }
    //ステートEnd  ============================================================
    private void GameState02Init() {
        transform.root.GetComponent<SceneLoadManager>().ChangeScene(SceneName.Title.ToInt());
    }
    private void GameState02Update() {
    
    }
    //ステートNext ============================================================


}

