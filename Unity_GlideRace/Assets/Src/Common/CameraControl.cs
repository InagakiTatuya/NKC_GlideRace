//#############################################################################
//  最終更新者：稲垣達也
//  プレイヤーに追従するカメラを管理する
//
//#############################################################################
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour {

    //変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private Camera m_Cam;
    
    private Vector3 m_Pos;
    private Vector3 m_At;
    private Vector3 m_Up;
    
    private bool    m_isLocalPos;
    private bool    m_isLocalRot;
    private UnityAction<Vector3>    m_fnSetTransPos; //座標に代入する
    private UnityAction<Quaternion> m_fnSetTransRot; //回転値に代入する

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

    public bool    isLocalPos  { get { return m_isLocalPos;  } }
    public bool    isLocalRot  { get { return m_isLocalRot;  } }

    public float smoothPosSpeed { get { return m_SmoothPosSpeed;  } 
                                  set { m_SmoothPosSpeed = value; } }
    public float smoothRotSpeed { get { return m_SmoothRotSpeed;  } 
                                  set { m_SmoothRotSpeed = value; } }

    //公開関数/////////////////////////////////////////////////////////////////
    //セッター=================================================================
    public void SeVecteor(Vector3 aPos, Vector3 aAt, Vector3 aUp) {
        m_Pos = aPos;    m_At = aAt;    m_Up = aUp;
    }
    
    //ローカル・ワールド切り替え===============================================
    public void DoWantLocalPos(bool aValue) {
        m_isLocalPos = aValue;
        if(m_isLocalPos) {
            m_fnSetTransPos = ((a) => { transform.localPosition = a; });
        } else {
            m_fnSetTransPos = ((a) => { transform.position = a; });
        }
    }

    public void DoWantLocalRot(bool aValue) {
        m_isLocalRot = aValue;
        if(m_isLocalRot) {
            m_fnSetTransRot = ((a) => { transform.localRotation = a; });
        } else {
            m_fnSetTransRot = ((a) => { transform.rotation = a; });
        }
    }

    //非公開関数///////////////////////////////////////////////////////////////
    //初期化===================================================================
    void Awake() {
        m_Cam = GetComponent<Camera>();

        DoWantLocalPos(false);
        DoWantLocalRot(false);
    }
    
    void Start() { }
	
    //更新=====================================================================
    void Update() { }
    void FixedUpdate() {
        //座標のスムージング
        Vector3 vec      = m_tarPos - m_Pos;
        float   sqrDis   = vec.sqrMagnitude;
        Vector3 dir      = vec.normalized;
        m_Pos += dir * m_SmoothPosSpeed;

        if(sqrDis <= m_SmoothPosSpeed * m_SmoothPosSpeed) {
            m_Pos = m_tarPos;
        }

        //正規化
        m_At.Normalize();
        if(m_At == Vector3.zero) { m_At = Vector3.forward; }

        m_Up.Normalize();
        if(m_Up == Vector3.zero) { m_Up = Vector3.up; }

        //適応
        if(m_fnSetTransPos != null) {
            m_fnSetTransPos(m_Pos);
        }
        if(m_fnSetTransRot != null) {
            m_fnSetTransRot(Quaternion.LookRotation(m_At, m_Up));
        }
    }

}
