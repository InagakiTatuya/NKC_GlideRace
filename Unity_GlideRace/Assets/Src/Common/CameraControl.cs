//#############################################################################
//  最終更新者：稲垣達也
//  カメラの操作を簡易化する
//  
//  位置、焦点、上方向を設定して操作する
//  
//#############################################################################
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour {

    //変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private Camera m_Cam;
    
    private Vector3 m_Pos;  //位置
    private Vector3 m_At;   //焦点
    private Vector3 m_Up;   //上方向
    
    private bool    m_useFixedUpdate = false;   //FixdUpdateで動くようにするか
    private bool    m_isLocalPos     = false;   //座標をローカルとして扱うか
    private bool    m_isLocalRot     = false;   //回転値をローカルとして扱うか
    private UnityAction<Vector3>    m_fnSetTransPos; //座標に代入する関数
    private UnityAction<Quaternion> m_fnSetTransRot; //回転値に代入する関数

    //スムージング^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private bool    m_fSmoothing;
    private Vector3 m_tarPos;
    private Vector3 m_tarAt;
    private Vector3 m_tarUp;

    private float   m_SmoothPosSpeed = 0.1f;
    private float   m_SmoothRotSpeed = 0.1f;

    //プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public Camera camera { get { return m_Cam; } }
    
    public Vector3 pos { get { return m_Pos; } set { m_Pos = value; } }
    public Vector3 at  { get { return m_At;  } set { m_At  = value; } }
    public Vector3 up  { get { return m_Up;  } set { m_Up  = value; } }

    public bool useFixedUpdate  { get { return m_useFixedUpdate; } }
    public bool isLocalPos      { get { return m_isLocalPos;     } }
    public bool isLocalRot      { get { return m_isLocalRot;     } }

    public bool smoothing { get { return m_fSmoothing; } set { m_fSmoothing = value; } }
    public Vector3 tarPos { get { return m_tarPos; } set { m_tarPos = value; } }
    public Vector3 tarAt  { get { return m_tarAt;  } set { m_tarAt  = value; } }
    public Vector3 tarUp  { get { return m_tarUp;  } set { m_tarUp  = value; } }

    public float smoothPosSpeed { get { return m_SmoothPosSpeed;  } 
                                  set { m_SmoothPosSpeed = value; } }
    public float smoothRotSpeed { get { return m_SmoothRotSpeed;  } 
                                  set { m_SmoothRotSpeed = value; } }

    
    
    //非公開関数///////////////////////////////////////////////////////////////
    //初期化===================================================================
    void Awake() {
        Init();
    }
    void Start() {
    
    }
    
    //更新=====================================================================
    void Update() { 
        if( m_useFixedUpdate) return;
        SmoothingMove();
        App();
    }
    void FixedUpdate() {
        if(!m_useFixedUpdate) return;
        SmoothingMove();
        App();
    }


    //公開関数/////////////////////////////////////////////////////////////////
    //セッター=================================================================
    //  pos at up をまとめて設定する
    //=========================================================================
    public void SetValues(Vector3 aPos, Vector3 aAt, Vector3 aUp) {
        m_Pos = aPos;    m_At = aAt;    m_Up = aUp;
    }
    //セッター=================================================================
    //  tarPos tarAt tarUp をまとめて設定する
    //=========================================================================
    public void SetTargetValues(Vector3 aPos, Vector3 aAt, Vector3 aUp) {
        m_tarPos = aPos;    m_tarAt = aAt;    m_tarUp = aUp;
    }
    
    //更新する関数を切り替える=================================================
    //  FixdUpdateで更新ｓるか
    //  true:FixdUpdeteで更新 false:Updateで更新
    //=========================================================================
   public void DoWantFixdUpdate(bool aValue) {
        m_useFixedUpdate = aValue;
   }

    //ローカル・ワールド切り替え===============================================
    //座標のローカル／ワールド切り替え=========================================
    //  管理座標をローカル座標として扱うか
    //  true:ローカル座標 false:ワールド座標
    //=========================================================================
    public void DoWantLocalPos(bool aValue) {
        m_isLocalPos = aValue;
        if(m_isLocalPos) {
            m_fnSetTransPos = ((a) => { transform.localPosition = a; });
        } else {
            m_fnSetTransPos = ((a) => { transform.position = a; });
        }
    }
    //回転値のローカル／ワールド切り替え=======================================
    //  管理座標をローカル座標として扱うか
    //  true:ローカル座標 false:ワールド座標
    //=========================================================================
    public void DoWantLocalRot(bool aValue) {
        m_isLocalRot = aValue;
        if(m_isLocalRot) {
            m_fnSetTransRot = ((a) => { transform.localRotation = a; });
        } else {
            m_fnSetTransRot = ((a) => { transform.rotation = a; });
        }
    }

    //継承関数/////////////////////////////////////////////////////////////////
    //初期化関数===============================================================
    protected void Init() {
        m_Cam = GetComponent<Camera>();

        DoWantLocalPos(false);
        DoWantLocalRot(false);
    }
	

    //スムージング=============================================================
    //  ターゲットに指定された座標(tarPos)や焦点(tarAt)などにスムーズに変化する
    //=========================================================================
    protected void SmoothingMove() {
        if(!m_fSmoothing) return;

        Vector3 vec;
        float   sqrDis;
        Vector3 dir;
        //座標のスムージング
        vec      = m_tarPos - m_Pos;
        sqrDis   = vec.sqrMagnitude;
        dir      = vec.normalized;
        m_Pos   += dir * m_SmoothPosSpeed;

        if(sqrDis <= m_SmoothPosSpeed * m_SmoothPosSpeed) {
            m_Pos = m_tarPos;
        }
        
        //焦点のスムージング
        vec      = m_tarAt - m_At;
        sqrDis   = vec.sqrMagnitude;
        dir      = vec.normalized;
        m_At    += dir * m_SmoothRotSpeed;

        if(sqrDis <= m_SmoothRotSpeed * m_SmoothRotSpeed) {
            m_At = m_tarAt;
        }

        //上方向のスムージング
        vec      = m_tarUp - m_Up;
        sqrDis   = vec.sqrMagnitude;
        dir      = vec.normalized;
        m_Up    += dir * m_SmoothRotSpeed;

        if(sqrDis <= m_SmoothRotSpeed * m_SmoothRotSpeed) {
            m_Up = m_tarUp;
        }

    }

    //値をトランスフォームに適用===============================================
    protected void App() {
        //適応
        if(m_fnSetTransPos != null) {
            m_fnSetTransPos(m_Pos);
        }
        if(m_fnSetTransRot != null) {
            m_fnSetTransRot(Quaternion.LookRotation(m_At,m_Up));
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 10);
    
        
    }
}