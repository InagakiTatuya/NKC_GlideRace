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
    [SerializeField] public int m_plyNum = 1;   //プレイヤー番号

    //非公開変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //参照
    private CourseAnchors   m_CouresAncs; //コースアンカー

    //子オブジェクトの参照
    private Transform       m_Model;    //モデル
    private HeadUpDisplay   m_HUD;      //ヘッドアップディスプレイ

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
    private BoolArray32  m_TrgState;    //ステート変更フラグ
    private BoolArray32  m_NowState;    //ステート

    public const int STATE_OnGround      =  0; //地面に接触
    public const int STATE_UseGravity    =  1; //重力の影響を受ける
    public const int STATE_HitWoll       =  2; //壁に接触
    public const int STATE_Accel         =  3; //アクセル
    public const int STATE_Brake         =  4; //ブレーキ
    public const int STATE_Drift         =  5; //ドリフト
    public const int STATE_Boost         =  6; //ブースト
    public const int STATE_Jump          =  7; //ジャンプ台によるジャンプ
    public const int STATE_Glide          =  8; //滑空
    public const int STATE_Falter        =  9; //ひるみ
    public const int STATE_HeatUp        = 10; //ヒートゲージ上昇
    public const int STATE_HighHeat      = 11; //ハイヒート状態
    public const int STATE_Respwan       = 12; //復活
    public const int STATE_ReverseRun    = 13; //逆走

    //その他の変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //移動方向            PlayerOperate_Speed.cs に定義
    //コースに復帰関係    PlayerOerate_Spwan.cs に定義
    //入力情報            PlayerOperate_Input.cs に定義
    //プレイヤーのカメラ  PlayerOperate_Camera.cs に定義

    //簡略プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private Vector3 ModelScale {
        set { m_Model.localScale = value; }
        get { return m_Model.localScale; }
    }

    //初期化===================================================================
    void Awake() {
        //リネーム
        gameObject.name = "Player" + m_plyNum.ToString("00");
    }

    void Start() {
        //参照
        m_CouresAncs = GameObject.Find(CourseAnchors.GBJ_NAME).GetComponent<CourseAnchors>();
        
        //子オブジェクトの参照
        m_Model = transform.FindChild("Model").transform;
        m_HUD   = transform.FindChild("Canvas").GetComponent<HeadUpDisplay>();

        //ステータス
        m_wait       = 0;
        m_Gravity    = new GravityStatus();
        m_Speed      = new SpeedStatus(0.08f,0.03f,16f);
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
    
    }

    //更新=====================================================================
    void Update() { 
    }
    void FixedUpdate() {

        InputDataReset();   //入力情報リセット
        SetInput();         //入力受け取り
        
        PhysicsReycast();       //レイキャスト
        PhysicsGroundCheck();   //地面チャック
        PhysicsWallCheck();     //壁チェック
        PhysicsGravity();       //重力
        
        SpeedFunc();    //速度管理
        HandleFunc();   //ハンドル

        ActionGlider(); //滑空処理

        SlopeFunc();  //坂判定
        AppForward(); //移動方向適応
        AppSpeed();   //移動処理

        CameraFixdUpdate();     //カメラ更新
        SendToHeadUpDisplay();  //キャンバスにデータを適用
    }

    //=========================================================================
        //OnGround  -----------------------------------------------------------
        //UseGravity-----------------------------------------------------------
        //HitWoll   -----------------------------------------------------------
        //Accel     -----------------------------------------------------------
        //Brake     -----------------------------------------------------------
        //Drift     -----------------------------------------------------------
        //Boost     -----------------------------------------------------------
        //Jump      -----------------------------------------------------------
        //Glid      -----------------------------------------------------------
        //Falter    -----------------------------------------------------------
        //Respwan   -----------------------------------------------------------
        //ReverseRun-----------------------------------------------------------
    //=========================================================================

}
