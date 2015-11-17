//#############################################################################
//  最終編集者：稲垣達也
//
//  プレイヤーの操作と制御
//
//#############################################################################

#define PC_DEBUG

using UnityEngine;
using System.Collections;


public class PlayerOperate : MonoBehaviour {
    
    //Inspecterで編集^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    [SerializeField] private int m_plyNum = 1;   //プレイヤー番号


    //非公開変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //接触^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private int             GROUND_MASK;
    private float           HOVER            = 0.3f;   //地面から浮かせる距離
    private float           UNDERRAYDIS_MIN  = 0.5f;
    private float           FRONTRAYDIS_MIN  = 1.6f;
    private Raycaster       m_UnderRay;
    private Raycaster       m_FrontRay;
    private GravityStatus   m_Gravity;

    //ステータス^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private float           m_wait;
    private SpeedStatus     m_Speed;
    private GliderStatus    m_Glider;
    private GoalData        m_GoalData;
    private bool            m_fOnGround;
    
    //転回・ドリフト
    private const float     ROTSTEPNEXT = 60f;  //急カーブする数値
    private const float     ROTSTEP1    = 1.0f; //回転ステップ１
    private const float     ROTSTEP2    = 2.3f; //回転ステップ２
    private float           m_handleSeed;   //基礎の数値
    private bool            m_fDrift;       //ドリフト中
    private int             m_driftDir;     //ドリフト方向
    private Vector3         m_handleVec;    //ＸＺ空間上の移動方向
    private Vector3         m_forward;      //移動方向
    private Vector3         m_modeFrwrd;    //モデルの正面

    //入力情報^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private InputData       m_Input;     //押しているか
    private InputData       m_InputDown; //押した瞬間

    //簡略プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private Vector3 Pos {
        set{ transform.position = value; }
        get{ return transform.position;  }
    }
    private Vector3 LPos {
        set{ transform.localPosition = value; }
        get{ return transform.localPosition;  }
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
        set{ transform.rotation = value; }
        get{ return transform.rotation;  }
    }
    private Quaternion LRot {
        set{ transform.localRotation = value; }
        get{ return transform.localRotation;  }
    }
    private Vector3 LScale {
        set{ transform.localScale = value; }
        get{ return transform.localScale;  }
    }
    private Vector3 Forward {
        set { transform.forward = value; }
        get { return transform.forward;  }
    }

    //初期化===================================================================
    void Awake() {
    }

    void Start() {
        //接触
        GROUND_MASK = 0x1 << LayerMask.NameToLayer(LayerNames.Ground);
        m_UnderRay  = new Raycaster();
        m_FrontRay  = new Raycaster();
        m_Gravity   = new GravityStatus();
        //ステータス
        m_wait      = 0;
        m_Speed     = new SpeedStatus(0.08f, 0.8f, 0.03f); //データベースから代入
        m_Glider    = new GliderStatus();
        m_GoalData  = new GoalData();
        m_fOnGround = false;
        //回転・ドリフト
        m_handleSeed = 0;
        m_fDrift     = false;
        m_driftDir   = 0;
        m_handleVec  = Forward;
        m_forward    = Forward;
        m_modeFrwrd  = Forward;
        // 入力
        m_Input      = new InputData();
        m_InputDown  = new InputData();
    }

    //更新=====================================================================
    void Update() {

    }

    void FixedUpdate() {
        //入力情報リセット
        m_Input.Reset();
        m_InputDown.Reset();

        //入力受け取り
    #if PC_DEBUG
        InputPad.InputAllData(ref m_Input);
        InputPad.InputAllDownData(ref m_InputDown);
    #else
        InputPad.InputData(ref m_Input, m_plyNum);
        InputPad.InputDownData(ref m_InputDown, m_plyNum);
    #endif

        GravityFunc();

        AccelFunc();
        BrakeFunc();
        HandleFunc();
        //DriftFunc();  //ドリフト＊開発中
        GliderFunc();

        AutoDirectionFunc();
        AppSpeedFunc();

    }

    //非公開関数///////////////////////////////////////////////////////////////
    
    //重力処理=================================================================
    private void GravityFunc() {
        
        //地面接触判定
        float dis = UNDERRAYDIS_MIN + (-1 * m_Gravity.fall);
        if(m_UnderRay.Raycast(Pos, Vector3.down, dis, GROUND_MASK)) {
            m_fOnGround = true;
            m_Gravity.Reset();

            //着地
            m_forward.y = 0;
            m_forward.Normalize();

        } else {
            m_fOnGround = false;
        }
        
        //地面接触に対する処理
        if(m_fOnGround){
            //地面から浮かす
            SetPosY = Mathf.Min(Pos.y + HOVER, m_UnderRay.hitData.point.y + HOVER);
        
        } else {
            //重力処理
            m_Gravity.AddSeed();
            SetPosY = Pos.y + m_Gravity.fall;
        }
    }

    //加速処理=================================================================
    private void AccelFunc() {
        if(m_Input.brake) return;
        if(m_Input.accel) { 
            m_Speed.AddSeed(m_Speed.ACC);
        } else {
            m_Speed.SubSeed(m_Speed.ACC * 1.3f);
        }

    }
    //減速処理=================================================================
    private void BrakeFunc() {
        if(!m_Input.brake) return;
        m_Speed.SubSeed(m_Speed.ACC * 2.3f);
    }

    //ハンドル操作=============================================================
    private void HandleFunc() {
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        //旋回するときに減速するシステムがいる
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        if(m_fDrift) return;

        m_handleSeed += m_Input.axis.x;
        if(m_handleSeed > 0.0f && m_Input.axis.x < 0.0f) { m_handleSeed = 0.0f; }
        if(m_handleSeed < 0.0f && m_Input.axis.x > 0.0f) { m_handleSeed = 0.0f; }
        if(m_Input.axis.x > -0.1f && 0.1f > m_Input.axis.x) { m_handleSeed = 0.0f; }

        if((int)m_handleSeed != 0) {
            // Seed が 一定以上になると　急に曲がる
            //  ROTSTEPNEXT 未満　ROTSTEP1の倍　／　ROTSTEPNEXT 以上　ROTSTEP2の倍
            float rot = m_Speed.TURN * ((Mathf.Abs(m_handleSeed) < ROTSTEPNEXT)? ROTSTEP1 : ROTSTEP2);
            Vector3 axis = new Vector3(0, -1, 0) * Mathf.Sign(m_handleSeed);
            m_forward = m_modeFrwrd = VecRotation(m_forward, rot, axis);
        }
    }
    //ドリフト処理=============================================================
    private void DriftFunc() {
        //ドリフト判定
        if(m_InputDown.drift && (int)m_Input.axis.x != 0) {
            m_fDrift = true;
            m_driftDir = (int)Mathf.Sign(m_Input.axis.x);
        }
        
        if(!m_fDrift) return;

        //ドリフトカーブ処理
        if(m_Input.drift) { 
            //------------------------------------------

            //axis.x ==  0        ROTSTEP1
            //axis.x ==  driftDir ROTSTEP2
            //axis.x == -driftDir 回転しない

            //ドリフト方向と逆に入力した場合、まっすぐ進む
            if(m_Input.axis.x == -m_driftDir) return;

            float rot = m_Speed.TURN * (((int)Mathf.Sign(m_Input.axis.x) == 0) ? ROTSTEP1 : ROTSTEP2); 
            float sin_r = Mathf.Sin(rot / 2f);
            float cos_r = Mathf.Cos(rot / 2f);
            Vector3 axis = new Vector3(0, -1, 0) * m_driftDir;
            
            Quaternion f = new Quaternion(m_forward.x, m_forward.y, m_forward.z, 0.0f);
            Quaternion q = new Quaternion(sin_r *  axis.x, sin_r *  axis.y, sin_r *  axis.z, cos_r);
            Quaternion r = new Quaternion(sin_r * -axis.x, sin_r * -axis.y, sin_r * -axis.z, cos_r);
            Quaternion qr = r * f * q;
            m_forward = new Vector3(qr.x, qr.y, qr.z).normalized;

            //モデル方向
            f = new Quaternion(m_forward.x, m_forward.y, m_forward.z, 0.0f);
            qr = r * f * q; //もう一度回転させる
            m_modeFrwrd = new Vector3(qr.x, qr.y, qr.z).normalized;


            //------------------------------------------
        }else{
            m_fDrift = false;
            m_driftDir = 0;
        }

    }
    //グライダー処理===========================================================
    private void GliderFunc() {

    }

    //方向修正=================================================================
    private void AutoDirectionFunc() {
        if(!m_fOnGround) return;
        
        //移動先に垂直なレイを飛ばし坂や段差を乗り越えれる移動方向を決定する
        float sv = Mathf.Max(0.01f, m_Speed.value);
        Vector3 org = Pos + (m_forward * sv) + (Vector3.up * FRONTRAYDIS_MIN);
        float dis = UNDERRAYDIS_MIN + (m_Gravity.fall * -1) + FRONTRAYDIS_MIN;
        bool hit = m_FrontRay.Raycast(org, Vector3.down, dis, GROUND_MASK);

        if(hit) {
            m_forward = (m_FrontRay.hitData.point - m_UnderRay.hitData.point).normalized;
        }

    }

    
    //速度値の適用=============================================================
    private void AppSpeedFunc() {
        Pos += m_forward * m_Speed.value;
        Forward = m_modeFrwrd;
    }


    //回転処理=================================================================
    private Vector3 VecRotation(Vector3 aVec, float aRot, Vector3 aAxis) {
        float sin_r = Mathf.Sin(aRot / 2f);
        float cos_r = Mathf.Cos(aRot / 2f);
            
        Quaternion f = new Quaternion(aVec.x, aVec.y, aVec.z, 0.0f);
        Quaternion q = new Quaternion(sin_r *  aAxis.x, sin_r *  aAxis.y, sin_r *  aAxis.z, cos_r);
        Quaternion r = new Quaternion(sin_r * -aAxis.x, sin_r * -aAxis.y, sin_r * -aAxis.z, cos_r);
        Quaternion qr = r * f * q;
        return new Vector3(qr.x, qr.y, qr.z);
    }


    
    void OnDrawGizmos() {

        if(!Application.isPlaying) return;

        //レイキャスト
        Gizmos.color = new Color(0.8f, 0.8f, 0.8f);
        Vector3 pos = m_UnderRay.origin + m_forward * m_Speed.value;
        Gizmos.DrawRay(pos, Vector3.down * m_UnderRay.distance);
        pos = m_FrontRay.origin + m_forward * m_Speed.value;
        Gizmos.DrawRay(pos, Vector3.down * m_FrontRay.distance);

        //正面
        Gizmos.color = new Color(0.0f, 0.0f, 1.0f);
        Gizmos.DrawRay(Pos, m_forward.normalized);
        
        //移動先
        Gizmos.color = new Color(1.0f, 1.0f, 1.0f);
        Gizmos.DrawRay(Pos, m_forward * m_Speed.value);
    
    }

}
