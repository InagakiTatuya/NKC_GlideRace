using UnityEngine;
using System.Collections;

public class TestDot : MonoBehaviour {

    //接触^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private int             GROUND_MASK;
    
    private float           HOVER = 0.3f;   //地面から浮かせる距離
    private float           RAYDISMIN = 0.5f;   //レイキャストの最小距離
    private float           m_OriginRayDis;
    private RaycastHit      m_OriginRayHit;

    private float           FORWARDRAYDIS_ADD = 1.6f; //
    private float           m_ForwardRayDis;
    private RaycastHit      m_ForwardRayHit;
    
    private GravityStatus   m_Gravity;
    private bool            m_fOnGround;

    private Vector3         m_forward;      //移動方向
    private Vector3         m_modeFrwrd;    //モデルの正面

    private float speedValue  = 1f;

    private Vector3 nextVec;
    
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

    //初期化======================================================================
    void Start() {


        //接触
        GROUND_MASK = 0x1 << LayerMask.NameToLayer(LayerNames.Ground);
        m_OriginRayDis    = RAYDISMIN;
        m_OriginRayHit    = new RaycastHit();
        m_ForwardRayDis   = RAYDISMIN + FORWARDRAYDIS_ADD;
        m_ForwardRayHit   = new RaycastHit();
        m_Gravity         = new GravityStatus();

        m_forward    = Forward;
        m_modeFrwrd  = Forward;
    }

    //更新=======================================================================
    void Update() {
        m_OriginRayDis = RAYDISMIN + (-1 * m_Gravity.fall);
        bool orginhit = Physics.Raycast(Pos, Vector3.down, out m_OriginRayHit, m_OriginRayDis, GROUND_MASK);

        m_ForwardRayDis = RAYDISMIN + FORWARDRAYDIS_ADD + (-1 * m_Gravity.fall);
        Vector3 ori = Pos + (m_forward * speedValue) + (Vector3.up * FORWARDRAYDIS_ADD);
        bool forwardhit = Physics.Raycast(ori, Vector3.down, out m_ForwardRayHit, m_ForwardRayDis, GROUND_MASK);

        Vector3 vec = m_forward;
        if(orginhit && forwardhit) {
            vec = (m_ForwardRayHit.point - m_OriginRayHit.point).normalized;
        }
        Debug.DrawRay(m_OriginRayHit.point, vec);
        nextVec = m_forward = vec * speedValue;

        if(Input.GetKeyDown(KeyCode.Space)) {
            Pos += nextVec;
        }

    }

    //重力処理=================================================================
    private void GravityFunc() {
        
        //地面接触判定
        m_OriginRayDis = RAYDISMIN + (-1 * m_Gravity.fall);
        if(Physics.Raycast(Pos, Vector3.down, out m_OriginRayHit, m_OriginRayDis, GROUND_MASK)) {
            
            m_fOnGround = true;
            m_Gravity.Reset();
        } else {
            m_fOnGround = false;
        }
        
        //地面接触に対する処理
        if(m_fOnGround){
            //地面から浮かす
            SetPosY = Mathf.Min(Pos.y + HOVER, m_OriginRayHit.point.y + HOVER);
        
        } else {
            //重力処理
            m_Gravity.AddSeed();
            //SetPosY = Pos.y + m_Gravity.fall;
        }
    }

    //速度値の適用=============================================================
    private void AppSpeedFunc() {
        Vector3 vec = m_forward;
        
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        //◇地面接地している
        //  □地面の角度から加算するベクトルを算出
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        if(m_fOnGround) {
            m_ForwardRayDis = RAYDISMIN + FORWARDRAYDIS_ADD + (-1 * m_Gravity.fall);
            Vector3 ori = Pos + (m_forward * speedValue) + (Vector3.up * FORWARDRAYDIS_ADD);
            bool hit = Physics.Raycast(ori, Vector3.down, out m_ForwardRayHit, m_ForwardRayDis, GROUND_MASK);
            if(!hit) return;

            vec = (m_ForwardRayHit.point - m_OriginRayHit.point).normalized;
        }

        nextVec = vec * speedValue;
        //Pos += vec * speedValue;
        //transform.forward = m_modeFrwrd;
        m_forward = Forward;

    }

    //回転=====================================================================
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
        //点
        Gizmos.color = new Color(0.0f, 0.0f, 0.0f);
        Gizmos.DrawCube(Pos, Vector3.one * 0.02f);

        //重力
        Gizmos.color = new Color(0.8f, 0.8f, 0.8f);
        Gizmos.DrawRay(Pos, Vector3.down * m_OriginRayDis);
        Vector3 ori = Pos + (m_forward * speedValue) + (Vector3.up * FORWARDRAYDIS_ADD);
        Gizmos.DrawRay(ori, Vector3.down * m_ForwardRayDis);

        //正面
        Gizmos.color = new Color(0.0f, 0.0f, 1.0f);
        Gizmos.DrawRay(Pos, m_forward.normalized);
        
        //移動先
        Gizmos.color = new Color(1.0f, 1.0f, 1.0f);
        Gizmos.DrawRay(Pos, nextVec);
    
    }
}
