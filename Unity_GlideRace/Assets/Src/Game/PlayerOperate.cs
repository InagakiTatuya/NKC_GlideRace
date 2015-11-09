//#############################################################################
//  最終編集者：稲垣達也
//
//  プレイヤーの操作と制御
//
//#############################################################################

using UnityEngine;
using System.Collections;

public class PlayerOperate : MonoBehaviour {
    
    //公開 Static 変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public static float STA_GRAVITY = -0.019f;
    public static float STA_FALLMIN = -1.19f;

    //Inspecterで編集^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    [SerializeField] private int m_plyNum = 1;   //プレイヤー番号


    //非公開変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //接触^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private int         GROUND_MASK;
    private RaycastHit  m_RayHit;
    private float       m_graSeed;      //重力加速の基礎となる値
    private float       m_fall;

    //ステータス^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private float           m_wait;
    private SpeedStatus     m_Speed;
    private GliderStatus    m_Glider;
    private GoalData        m_GoalData;
    private bool            m_fOnGround;
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


    //初期化===================================================================
    void Awake() {
    }

    void Start() {
        //接触
        GROUND_MASK = 0x1 << LayerMask.NameToLayer(LayerNames.Ground);
        m_RayHit    = new RaycastHit();
        //ステータス
        m_wait      = 0;
        m_Speed     = new SpeedStatus(0.01f, 1f, 1f); //データベースから代入
        m_Glider    = new GliderStatus();
        m_GoalData  = new GoalData();
        m_fOnGround = false;
        // 入力
        m_Input      = new InputData();
        m_InputDown  = new InputData();
    }

    //更新=====================================================================
    void Update() {
        //入力受け取り
        InputPad.InputData(ref m_Input, m_plyNum);
        InputPad.InputDownData(ref m_Input, m_plyNum);
    }

    void FixedUpdate() {
        GravityFunc();
        AccelFunc();

        //入力情報リセット
        m_Input.Reset();
        m_InputDown.Reset();
    }

    //非公開関数///////////////////////////////////////////////////////////////
    
    //加速処理=================================================================
    private void AccelFunc() {
        //if(!m_Input.accel) return;
        if(!InputPad.AllAccel()) return;

        m_Speed.AddSeed();
        
        Vector3 vec = transform.forward;
        
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        //◇地面接地している
        //  □地面の角度から加算するベクトルを算出
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\

        //値の適用
        Pos += vec * m_Speed.value;
    }

    //重力処理=================================================================
    private void GravityFunc() {
        
        Debug.DrawRay(Pos,Vector3.down, Color.green, 0.5f);
        
        if(Physics.Raycast(Pos, Vector3.down, out m_RayHit, 0.5f, GROUND_MASK)) {
            m_fOnGround = true;
            m_fall    = 0;
            m_graSeed = 0;
        } else {
            m_fOnGround = false;
        }

        
        //この先地面に接触していない場合のみ処理
        if(m_fOnGround) return;
        //重力処理
        m_graSeed++;
        m_fall = STA_GRAVITY * m_graSeed;
        if(m_fall < STA_FALLMIN) m_fall = STA_FALLMIN;
        float y = Pos.y + m_fall;
        if(y < m_RayHit.point.y) { y = m_RayHit.point.y; }
        SetPosY = y;

    }


}
