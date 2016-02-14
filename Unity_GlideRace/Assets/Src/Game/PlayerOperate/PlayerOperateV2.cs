//#############################################################################
//  最終編集者：稲垣達也
//
//  プレイヤーの操作と制御
//
//#############################################################################
using UnityEngine;
using System.Collections;

public partial class PlayerOperateV2 : BaseObject {
    //Inspecterで編集^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    [SerializeField] public int m_plyNo = 1;    //プレイヤー番号
    [SerializeField] public int m_NpcLv = 0;    //CPU

    //非公開変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private PlayerManager   m_PlyMgr;  //プレイヤーマネージャ
    //子オブジェクトの参照
    private Transform       m_Model;    //モデル
    private HeadUpDisplay   m_HUD;      //ヘッドアップディスプレイ

    //コースアンカー^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private CourseAnchors m_CouresAncs;   //コースアンカーの参照
    private AnchorData    m_AncData;      //最後に通ったアンカー
    private AnchorData    m_AncDataGround;//地面に接触中に通ったアンカー

    //ステータス^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private float         m_wait;       //重さ
    private GravityStatus m_Gravity;    //重力ステータス
    private SpeedStatus   m_Speed;      //速度に関するステータス
    private JumpStatus    m_Jump;       //ジャンプに関するステータス
    private GliderStatus  m_Glider;     //滑空に関するステータス
    private HeatStatus    m_Heat;       //ヒートに関するステータス
    private RackState     m_Rack;       //ラックステート
    private GoalData      m_GoalData;   //ゴール時のデータ

    //ステート^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private BoolArray32  m_TrgState;    //瞬間ステート
    private BoolArray32  m_NowState;    //ステート

    public const int STATE_OnGround      =  0; //地面に接触
    public const int STATE_UseGravity    =  1; //重力の影響を受ける
    public const int STATE_HitWoll       =  2; //壁に接触
    public const int STATE_Accel         =  3; //アクセル
    public const int STATE_Brake         =  4; //ブレーキ
    public const int STATE_Drift         =  5; //ドリフト
    public const int STATE_Boost         =  6; //ブースト
    public const int STATE_Jump          =  7; //ジャンプ台によるジャンプ
    public const int STATE_Glide         =  8; //滑空
    public const int STATE_Falter        =  9; //ひるみ
    public const int STATE_HeatUp        = 10; //ヒートゲージ上昇
    public const int STATE_HighHeat      = 11; //ハイヒート状態
    public const int STATE_Respwan       = 12; //復活
    public const int STATE_ReverseRun    = 13; //逆走
    public const int STATE_Turn          = 14; //ターン

    //トリガーヒット^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //  m_TrgState で使用する
    public const int TRGGER_Boost        = 16;
    public const int TRGGER_Jump         = 17;
    public const int TRGGER_Heat         = 18;
    public const int TRGGER_Turn         = 19;
    public const int TRGGER_Goal         = 20;

    //その他の変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //アクション          PlayerOperateV2_Action.cs に定義
    //カメラを管理        PlayerOperateV2_Camera.cs に定義
    //入力情報            PlayerOperateV2_Input.cs に定義
    //移動方向            PlayerOperateV2_Move.cs に定義
    //プレイヤーのカメラ  PlayerOperateV2_Camera.cs に定義
    //コースに復帰        PlayerOerateV2_Spwan.cs に定義
    //ターン              PlayerOerateV2_Turn.cs に定義

    //簡略プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private Vector3 ModelScale {
        set { m_Model.localScale = value; }
        get { return m_Model.localScale; }
    }
    
    //公開プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public int GetPlayerNo{ get{ return m_plyNo;  } }
    public int SetPlayerNo{ 
        set{ 
            m_plyNo = value; 
            Rename();
        }
    }

    public int GetNpcLv { get{ return m_NpcLv;  } }
    public int SetNpcLv { set{ m_NpcLv = value; } }

    public bool isNpc { get{ return (m_NpcLv > 0); } } //ＮＰＣか否か

    ///////////////////////////////////////////////////////////////////////////
    //  公開関数
    ///////////////////////////////////////////////////////////////////////////
    //データ設定===============================================================
    public void Initialize(PlayerCharStateData aData){
        //モデル変更
        this.ModeChange(Database.obj.GetPlayerModel(aData.modelId));
        
        //アンカー
        m_AncData        = m_CouresAncs.GetAnc(0);
        m_AncDataGround  = m_CouresAncs.GetAnc(0);

        //ステータス
        m_wait  = aData.wait;
        m_Gravity    = new GravityStatus();
        m_Speed      = new SpeedStatus(aData.accel, aData.turn, aData.maxSpeed);
        m_Jump       = new JumpStatus();
        m_Glider     = new GliderStatus();
        m_Heat       = new HeatStatus();
        m_Rack       = RackState.READY;
        m_GoalData   = new GoalData();

        //ステート
        m_TrgState = new BoolArray32(false);
        m_NowState    = new BoolArray32(false);

        InputStart();   //入力
        CameraStart(); //カメラ

        SpeedStart();   //速度・方向
        StartPhysics(); //重力やレイキャスト
        SpwanStart();   //復帰する処理用
        TurnnStart();   //ターン
        
        GoalStart();
    }

    //アクティブ設定===========================================================
    public void SetActive(bool aActive) {
        gameObject.SetActive(aActive);
    }

    //ラック変更===============================================================
    public void SetRack(RackState aRackState) {
        m_Rack = aRackState;

        InputLock = (m_Rack == RackState.READY);
    }


    ///////////////////////////////////////////////////////////////////////////
    //  非公開関数
    ///////////////////////////////////////////////////////////////////////////
    //初期化===================================================================
    void Awake() {
        //PlayerManager取得----------------------------------------------------
        m_PlyMgr = GameObject.FindObjectOfType<PlayerManager>();
        //子オブジェクトの参照
        m_HUD   = transform.FindChild("Canvas").GetComponent<HeadUpDisplay>();
        //アンカー
        m_CouresAncs     = GameObject.Find(CourseAnchors.GBJ_NAME).GetComponent<CourseAnchors>();
        
        //カメラ関連初期化        
        CameraAwke();
    }

    void Start() { }

    //リネーム=================================================================
    private void Rename() {
        gameObject.name = "Player" + m_plyNo.ToString("00");
    }

    //モデル変更===============================================================
    private void ModeChange(GameObject aNowModel) {
        //古いモデル削除
        Transform  old   = transform.FindChild("Model");
        if(old != null) { GameObject.Destroy(old); }
        //新しいモデルを追加
        GameObject model = Instantiate(aNowModel);
        model.transform.SetParent(transform, false);
        m_Model = model.transform;
    }

    //更新=====================================================================
    //void Update() { }
    void FixedUpdate() {

        InputDataReset();   //入力情報リセット
        InputFixdUpdate();  //入力受け取り
        
        PhysicsReycast();       //レイキャスト
        PhysicsGroundCheck();   //地面チャック
        PhysicsWallCheck();     //壁チェック
        PhysicsGravity();       //重力

        TurnFunc();
        
        SpeedFunc();    //速度管理
        ActionBoost();  //ブースト処理
        HandleFunc();   //ハンドル

        ActionGlider(); //滑空処理

        SlopeFunc();  //坂判定

        Anima();        //アニメーション処理

        AppForward(); //移動方向適応
        AppSpeed();   //移動処理
        
        SearchAnchor(); //アンカー検知
        SpwanFunc();    //復帰処理

        GoalFixdUpdate(); //ゴール判定とその後の処理
        
        CameraFixdUpdate();     //カメラ更新
        SendToHeadUpDisplay();  //キャンバスにデータを適用

        TriggerReset();
    }


    //アンカー検知=============================================================
    //  最後に通ったアンカー情報を記憶する
    //  逆走判定フラグもここに書いてある
    //=========================================================================
    private void SearchAnchor() {
        //前後のアンカーをの距離を測る
        //一定距離になったらアンカーデータに保存
        //後ろの場合逆走フラグを出す

        //ラックによる進む方向
        int n = ((m_Rack == RackState.RACK1) ? (1) : (-1));
        int frntNo = m_AncData.indexNo + n;
        int backNo = m_AncData.indexNo - n;
        
        AnchorData  data    = null;  //前後のアンカーデータ
        float       sqrDis  = 0f;    //アンカーとの距離
        bool        frntHit = false; //アンカーに接触（前）
        bool        backHit = false; //アンカーに接触（後）
        bool        reverse = m_NowState[STATE_ReverseRun]; //逆走判定

        //前のアンカー --------------------------------------------------------
        if(frntNo < m_CouresAncs.ancArrSize && frntNo >= 0) {
            data    = m_CouresAncs.GetAnc(frntNo);
            sqrDis  = (data.point - traPos).sqrMagnitude;
            frntHit = (sqrDis < m_CouresAncs.powAncCollSize);
        }
        if(frntHit) {
            //データ読み込み
            m_AncData = data;
            if(m_NowState[STATE_OnGround]) { m_AncDataGround = data; }
            //逆走判定
            reverse = false; //逆走していない
        }

        //後アンカー ----------------------------------------------------------
        if(!frntHit && backNo < m_CouresAncs.ancArrSize && backNo >= 0) {
            data    = m_CouresAncs.GetAnc(backNo);
            sqrDis  = (data.point - traPos).sqrMagnitude;
            backHit = (sqrDis < m_CouresAncs.powAncCollSize);
        }
        if(backHit) {
            //データ読み込み
            m_AncData = data;
            if(m_NowState[STATE_OnGround]) { m_AncDataGround = data; }
            //逆走判定
            reverse = true; //逆走している
        }

        //逆走ステート変更
        m_TrgState[STATE_ReverseRun] = (!m_NowState[STATE_ReverseRun] && reverse);
        m_NowState[STATE_ReverseRun] = reverse;

    }

    ///////////////////////////////////////////////////////////////////////////
    //  Trigger
    ///////////////////////////////////////////////////////////////////////////
    private void TriggerReset() {
         m_TrgState[TRGGER_Boost] = false;
         m_TrgState[TRGGER_Jump ] = false;
         m_TrgState[TRGGER_Heat ] = false;
         m_TrgState[TRGGER_Turn ] = false;
         m_TrgState[TRGGER_Goal ] = false;
    }

    public void OnChildTriggerEnter(Collider col) {
        #if UNITY_EDITOR
        if(col.tag != TagNames.Untagged) Debug.Log("Hit trigger : tag = " + col.tag);
        #endif
        if(col.tag == TagNames.BoostTrigger) m_TrgState[TRGGER_Boost] = true;
        if(col.tag == TagNames.JumpTrigger ) m_TrgState[TRGGER_Jump ] = true;
        if(col.tag == TagNames.HeatTrigger ) m_TrgState[TRGGER_Heat ] = true;
        if(col.tag == TagNames.TurnTrigger ) m_TrgState[TRGGER_Turn ] = true;
        if(col.tag == TagNames.StartAndGoal) m_TrgState[TRGGER_Goal ] = true;
    }


}
