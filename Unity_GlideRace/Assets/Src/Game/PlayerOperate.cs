﻿//#############################################################################
//  最終編集者：稲垣達也
//
//  プレイヤーの操作と制御
//
//#############################################################################


using UnityEngine;
using System.Collections;


public partial class PlayerOperate : MonoBehaviour {

    //Inspecterで編集^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    [SerializeField] public int m_plyNum = 1;   //プレイヤー番号

    //非公開変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //参照
    private CourseAnchors   m_CouresAncs;

    //子オブジェクトの参照
    private Transform       m_Model;    //モデル
    private HeadUpDisplay   m_HUD;      //ヘッドアップディスプレイ
    
    //接触^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private int           GROUND_MASK;
    private const float   HOVER           = 0.3f; //地面から浮かせる距離
    private const float   UNDERRAYDIS_MIN = 0.5f; //足元レイの最小の長さ
    private const float   FRONTRAYDIS_MIN = 1.6f; //移動先レイの最小の長さ
    private const float   SLOPE = 45f / 180f * Mathf.PI; //上れる坂の角度
    private Raycaster     m_UnderRay;         //足元レイ
    private Raycaster     m_FrontDownRay;     //移動先レイ
    private Raycaster     m_FrontRay;         //移動方向レイ
    private GravityStatus m_Gravity;          //重力ステータス

    //ステータス^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private RackState     m_Rack;         //ラックステート
    private float         m_wait;         //重さ
    private SpeedStatus   m_Speed;        //速度に関するステータス
    private GliderStatus  m_Glider;       //滑空に関するステータス
    private HeatStatus    m_Heat;         //ヒートに関するステータス
    private GoalData      m_GoalData;     //ゴールデータ
    private bool          m_fUseGravity;  //重力の影響を受けるか
    private bool          m_fOnGround;    //地面に接触
    private bool          m_fHitWoll;     //壁に接触
    private bool          m_fJumpBoost;   //ジャンプ台を踏んだ
    private bool          m_fReverseRun;  //逆走
    private bool          m_fRespwan;     //復活判定
    private AnchorData    m_AncData;      //最後に通ったアンカー
    private AnchorData    m_AncDataGround;//地面に接触中に通ったアンカー

    //転回・ドリフト^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private const float   ROTSTEPNEXT  = 60.0f; //急カーブする数値
    private const float   ROTSTEP1     =  1.0f; //回転ステップ１
    private const float   ROTSTEP2     =  2.3f; //回転ステップ２
    private float         m_handleSeed;   //基礎の数値
    private bool          m_fDrift;       //ドリフト中
    private int           m_driftDir;     //ドリフト方向
    private Vector3       m_handleDir;    //ＸＺ空間上の移動方向
    private Vector3       m_forward;      //移動方向
    private Vector3       m_modeFrwrd;    //モデルの正面

    //滑空^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private bool          m_fGliding;     //滑空中

    //コースに復帰するための変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //PlayerOerate_Spwan.cs に定義
    //入力情報^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //PlayerOperate_Input に定義
    
    //簡略プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private Vector3 Pos {
        set { transform.position = value; }
        get { return transform.position; }
    }
    private Vector3 LPos {
        set { transform.localPosition = value; }
        get { return transform.localPosition; }
    }
    private float SetPosX {
        set { transform.position = new Vector3(value, Pos.y, Pos.z); }
    }
    private float SetPosY {
        set { transform.position = new Vector3(Pos.x, value, Pos.z); }
    }
    private float SetPosZ {
        set { transform.position = new Vector3(Pos.x, Pos.y, value); }
    }
    private Quaternion Rot {
        set { transform.rotation = value; }
        get { return transform.rotation; }
    }
    private Quaternion LRot {
        set { transform.localRotation = value; }
        get { return transform.localRotation; }
    }
    private Vector3 LScale {
        set { transform.localScale = value; }
        get { return transform.localScale; }
    }
    private Vector3 Forward {
        set { transform.forward = value; }
        get { return transform.forward; }
    }

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
        
        //カメラ
        CameraStart();
        
        //接触
        GROUND_MASK      = 0x1 << LayerMask.NameToLayer(LayerNames.Ground);
        m_UnderRay       = new Raycaster();
        m_FrontDownRay   = new Raycaster();
        m_FrontRay       = new Raycaster();
        m_Gravity        = new GravityStatus();

        //ステータス
        m_Rack           = RackState.READY;
        m_wait           = 0;
        m_Speed          = new SpeedStatus(0.08f, 0.8f, 0.03f); //データベースから代入するように改良する
        m_Glider         = new GliderStatus();
        m_Heat           = new HeatStatus();
        m_GoalData       = new GoalData();
        m_fUseGravity    = true;
        m_fOnGround      = false;
        m_fHitWoll       = false;
        m_fReverseRun    = false;
        m_AncData        = m_CouresAncs.GetAnc(0);
        m_AncDataGround  = m_CouresAncs.GetAnc(0);

        //回転・ドリフト
        m_handleSeed = 0;
        m_fDrift     = false;
        m_driftDir   = 0;
        m_handleDir  = Forward;
        m_forward    = Forward;
        m_modeFrwrd  = Forward;

        //復帰する処理用
        SpwanStart();

        //入力
        InputStart();
    }

    //更新=====================================================================
    void Update() {

    }

    void FixedUpdate() {

        InputDataReset();   //入力情報リセット
        SetInput();         //入力受け取り

        GravityFunc();

        AccelFunc();
        BrakeFunc();
        HandleFunc();
        //DriftFunc();  //ドリフト＊開発中
        GliderFunc(); //滑空    ＊未開発

        SlopeFunc();    //坂処理
        WallCheck();    //壁チェック
        AppSpeedFunc();

        SearchAnchor();

        SpwanFunc();

        CameraFixdUpdate();     //カメラ
        SendToHeadUpDisplay();  //GUIにデータを渡す
    }

    //非公開関数///////////////////////////////////////////////////////////////

    //入力データ管理===========================================================
    //  PlayerOperate_Input.cs に定義

    //アクセル・回転ドリフト処理===============================================
    //  PlayerOperate_Accele.cs に定義

    //重力処理=================================================================
    private void GravityFunc() {

        //地面接触判定
        float dis = UNDERRAYDIS_MIN + (-1 * m_Gravity.fall);
        if(m_UnderRay.Raycast(Pos, Vector3.down, dis, GROUND_MASK)) {
            //着地
            m_fOnGround = true;
            m_Gravity.Reset();
            m_forward.y = 0;
            m_forward.Normalize();

        } else {
            m_fOnGround = false;
        }

        //地面接触に対する処理
        if(m_fOnGround) {
            //地面から浮かす
            SetPosY = Mathf.Min(Pos.y + HOVER, m_UnderRay.hitData.point.y + HOVER);

        }

         //重力処理
        if(m_fUseGravity && !m_fOnGround) {
            m_Gravity.AddSeed();
            SetPosY = Pos.y + m_Gravity.fall;
        }
    }
    
    //ジャンプ処理=============================================================
    private void JumpBoost() {
        if(!m_fJumpBoost) return;

        

    }

    //グライダー処理===========================================================
    private void GliderFunc() {
        
        //滑空開始
        if(m_Input.glide && !m_fGliding && !m_fOnGround) {
            m_fUseGravity = false;
            m_fGliding    = true;
            //[グライダーエフェクト発生処理をここに書く]
        }

        //グラインドゲージ回復
        if(m_fOnGround) {
            m_Glider.AddValue(GliderStatus.ADDVALUE);
        }

        //ココから先滑空処理---------------------------------------------------
        if(!m_fGliding) return;

        //グラインドゲージ減少
        m_Glider.SubValue(GliderStatus.SUDVALUE);

        //グラインド終了条件
        //  グライドボタンを離した || ゲージなくなった
        bool end = (!m_Input.glide || m_Glider.value <= 0f);

        //グラインド終了処理
        if(end) {
            m_fUseGravity = true;
            m_fGliding    = false;
        }


    }

    //Ｙ方向修正===============================================================
    //  足元のレイキャスト情報と移動先のレイキャスト情報から
    //  坂の角度を計算して移動先ベクトルを再設定する
    //=========================================================================
    private void SlopeFunc() {
        if(!m_fOnGround) return;

        bool fhit = false;
        bool fSlope = false;

        //移動先に垂直なレイを飛ばす
        float sv = Mathf.Max(0.01f, m_Speed.value);
        m_FrontDownRay.origin    = Pos + (m_forward * sv) + (Vector3.up * FRONTRAYDIS_MIN);
        m_FrontDownRay.direction = Vector3.down;
        m_FrontDownRay.distance  = (-1 * m_Gravity.fall) + FRONTRAYDIS_MIN;
        m_FrontDownRay.layerMask = GROUND_MASK;
        fhit = m_FrontDownRay.Raycast();

        if(fhit) {
            float rot = Mathf.Acos(Vector3.Dot(Vector3.up, m_FrontDownRay.hitData.normal));
            fSlope = (0f <= rot && rot <= SLOPE);
        } else {
            return;
        }

        //移動方向修正
        if(fSlope) {
            m_forward = (m_FrontDownRay.hitData.point - m_UnderRay.hitData.point).normalized;
        }

    }

    //壁判定===================================================================
    //  移動先方向にレイキャストを飛ばして、取得した情報から壁があるかどうかを
    //  判定する
    //=========================================================================
    private void WallCheck() {
        bool fhit = false;

        //正面にレイを飛ばす
        fhit = m_FrontRay.Raycast(Pos + Vector3.up * 0.5f, m_handleDir, m_Speed.value, GROUND_MASK);
        if(fhit) {
            float rot = Mathf.Acos(Vector3.Dot(Vector3.up, m_FrontRay.hitData.normal));
            float y = m_FrontRay.hitData.normal.y;
            m_fHitWoll = ((y <= 0) || (y > 0 && rot > SLOPE));
        } else {
            m_fHitWoll = false;
        }
    }

    //速度値の適用=============================================================
    private void AppSpeedFunc() {
        Pos += m_forward * m_Speed.value;
        Forward = m_modeFrwrd;
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

        //Debug.Log("frntAncNo=" + frntNo + "\n" +"backAncNo=" + backNo );

        //前のアンカー
        if(frntNo < m_CouresAncs.ancArrSize && frntNo >= 0) {
            AnchorData data = m_CouresAncs.GetAnc(frntNo);
            float sqrDis = (data.point - Pos).sqrMagnitude;
            if(sqrDis < m_CouresAncs.powAncCollSize) {
                m_AncData = data;
                if(m_fOnGround) { m_AncDataGround = data; }
                m_fReverseRun = false; //逆走していない
                return;//前のアンカーを優先して処理する
            }
        }

        //後のアンカー
        if(backNo < m_CouresAncs.ancArrSize && backNo >= 0) {
            AnchorData data = m_CouresAncs.GetAnc(backNo);
            float sqrDis = (data.point - Pos).sqrMagnitude;
            if(sqrDis < m_CouresAncs.powAncCollSize) {
                m_AncData = data;
                if(m_fOnGround) { m_AncDataGround = data; }
                m_fReverseRun = true; //逆走している
            }
        }
    }

    //コースに復帰するフラグとコースに復帰する処理=============================
    //PlayerOperate_Spwan.cs に定義

    //GUI更新==================================================================
    //  現在のステートをHeadUpDisplayに渡す
    //=========================================================================
    private void SendToHeadUpDisplay() {
        m_HUD.SetGaugeGlider(m_Glider.value / GliderStatus.MAX);
        m_HUD.SetGaugeHeat  (m_Heat.value   / HeatStatus.MAX);
    }

    ///////////////////////////////////////////////////////////////////////////
    //  Trigger
    ///////////////////////////////////////////////////////////////////////////
    public void OnChildTriggerForJump() {
        Debug.Log("OnTrigger hit JumpTrigger");
        m_fJumpBoost = true;
    }

    ///////////////////////////////////////////////////////////////////////////
    //  ヘルパー関数
    ///////////////////////////////////////////////////////////////////////////

    //回転処理=================================================================
    //　渡されたベクトルを３Ｄ回転させる
    //  第１引数：回転させるベクトル
    //  第２引数：回転させる量
    //  第３引数：回転軸
    //=========================================================================
    private Vector3 VecRotation(Vector3 aVec, float aRot, Vector3 aAxis) {
        float sin_r = Mathf.Sin(aRot / 2f);
        float cos_r = Mathf.Cos(aRot / 2f);

        Quaternion f = new Quaternion(aVec.x, aVec.y, aVec.z, 0.0f);
        Quaternion q = new Quaternion(sin_r *  aAxis.x, sin_r *  aAxis.y, sin_r *  aAxis.z, cos_r);
        Quaternion r = new Quaternion(sin_r * -aAxis.x, sin_r * -aAxis.y, sin_r * -aAxis.z, cos_r);
        Quaternion qr = r * f * q;
        return new Vector3(qr.x, qr.y, qr.z);
    }

    //回転処理=================================================================
    //　渡されたベクトルを二つのベクトルのなす角の量で回転させる
    //  第１引数：回転させるベクトル
    //  第２引数：なす角を作るベクトル１
    //  第３引数：なす角を作るベクトル２
    //  第４引数：回転軸
    //=========================================================================
    private Vector3 VecRotationEx(Vector3 aVec, Vector3 aV1, Vector3 aV2, Vector3 aAxis) {
        Vector3 vec   = aVec;
        float   angle = Mathf.Acos(Vector3.Dot(aV1, aV2));
        Vector3 acxis = Vector3.Cross(aV1, aV2);
        float   pm    = (acxis.y < 0) ? -1 : +1;
        return VecRotation(vec, angle, aAxis * pm);
    }


    //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=
    //ココより下は、デバック用 
    //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=
#if UNITY_EDITOR
    //GUI
    void OnGUI() {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("State = " + System.Enum.GetName(typeof(RackState), m_Rack));
        if(GUILayout.Button("To " + System.Enum.GetName(typeof(RackState), 0))) { m_Rack = (RackState)0; }
        if(GUILayout.Button("To " + System.Enum.GetName(typeof(RackState), 1))) { m_Rack = (RackState)1; }
        if(GUILayout.Button("To " + System.Enum.GetName(typeof(RackState), 2))) { m_Rack = (RackState)2; }
        if(GUILayout.Button("To " + System.Enum.GetName(typeof(RackState), 3))) { m_Rack = (RackState)3; }
        GUILayout.EndVertical();
    }


    //Gizmos===================================================================
    void OnDrawGizmos() {

        if(!Application.isPlaying) return;

        //レイキャスト
        Gizmos.color = new Color(0.8f, 0.4f, 0.8f);
        Gizmos.DrawRay(m_UnderRay.origin, m_UnderRay.direction * m_UnderRay.distance);
        Gizmos.color = new Color(0.8f, 0.4f, 0.0f);
        Gizmos.DrawRay(m_UnderRay.hitData.point, m_UnderRay.hitData.normal);

        Gizmos.color = new Color(0.4f, 0.8f, 0.8f);
        Gizmos.DrawRay(m_FrontDownRay.origin, m_FrontDownRay.direction * m_FrontDownRay.distance);
        Gizmos.color = new Color(0.4f, 0.8f, 0.0f);
        Gizmos.DrawRay(m_FrontDownRay.hitData.point, m_FrontDownRay.hitData.normal);

        //正面
        Gizmos.color = new Color(0.0f, 0.0f, 1.0f);
        Gizmos.DrawRay(Pos, m_forward.normalized);

        //移動先
        Gizmos.color = new Color(1.0f, 1.0f, 1.0f);
        Gizmos.DrawRay(Pos, m_forward * m_Speed.value);

    }
#endif

}
