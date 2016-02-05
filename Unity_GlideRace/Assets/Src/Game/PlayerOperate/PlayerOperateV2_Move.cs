//#############################################################################
//  ファイル名：PlayerOperate_Move.cs
//  最終更新者：稲垣達也
//
//  Partial によって分けられたファイルの一つ
//    速度や移動方向に関わるシステム
//
//#############################################################################
using UnityEngine;
using System.Collections;

public partial class PlayerOperateV2 : BaseObject {
    
    //方向^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private Vector3       m_handleDir;    //ＸＺ空間上の移動方向（Ｙは常に０）
    private Vector3       m_angleDir;     //ＸＹ空間上の高さ方向（Ｘは常に０）
    private Vector3       m_forward;      //移動方向
    private Vector3       m_modelFrwrd;   //モデルの正面方向

    //転回・ドリフト^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private const float   ROTSTEPNEXT  = 60.0f; //急カーブする数値
    private const float   ROTSTEP1     =  1.0f; //回転ステップ１
    private const float   ROTSTEP2     =  2.3f; //回転ステップ２
    private float         m_handleSeed;   //基礎の数値
    private int           m_driftDir;     //ドリフト方向

    //ブースト^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private int BOOSTMAXCOUNT = 40;
    private int m_boostCnt;
    
    //プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public Vector3 GetHandleDir    { get { return m_handleDir;   } }
    public Vector3 GetAngleDir     { get { return m_angleDir;    } }
    public Vector3 GetMoveForawrd  { get { return m_forward;     } }
    public Vector3 GetModelForward { get { return m_modelFrwrd;  } }

    //その他の関数/////////////////////////////////////////////////////////////
    //  Start Update 以外の場所で呼ばれる関数
    ///////////////////////////////////////////////////////////////////////////
    
    //方向設定=================================================================
    public void SetPlayerDir(Vector3 aDir) {
        m_handleDir = m_forward = aDir;
    }
    public void SetPlayerModelDir(Vector3 aDir) {
        m_modelFrwrd = aDir;
    }
    //強制ブースト=============================================================
    public void ForcedBoost() {
        m_TrgState[TRGGER_Boost] = true;
    }

    //初期化///////////////////////////////////////////////////////////////////
    //  Startで呼ばれる関数
    ///////////////////////////////////////////////////////////////////////////
    private void SpeedStart() {
        
        //方向
        m_handleDir  = traForward;
        m_angleDir   = Vector3.forward;
        m_forward    = traForward;
        m_modelFrwrd = traForward;

        //旋回
        m_handleSeed = 0f;
        m_driftDir   = 0;

        //ブースト
        m_boostCnt = 0;
    }

    //更新関数/////////////////////////////////////////////////////////////////
    //  Updateで呼ばれる関数
    ///////////////////////////////////////////////////////////////////////////

    //加速・減速処理===========================================================
    //  入力情報やステートにより加速減速の処理を行う
    //=========================================================================
    private void SpeedFunc() {
        float addSeed = 0;
        
        //加速
        bool up = (m_Input.accel);
        if(up) {
            addSeed = m_Speed.ACC;
        }

        //減速
        bool dwon = (!m_Input.accel && m_NowState[STATE_OnGround]);
        if(dwon) {
            addSeed = -m_Speed.ACC * 1.5f;
        }

        //急激な減速（ブレーキ）
        bool hdwn = (m_Input.brake && !m_NowState.Or(STATE_Boost, STATE_Glide));
        if(hdwn) {
            addSeed = -m_Speed.ACC * 3.3f;
        }

        //速度の基礎値適用
        m_Speed.AddSeed(addSeed);

    }

    //ブースト処理=============================================================
    //  ブーストステートに変更と、ブースト処理を行う
    //=========================================================================
    private void ActionBoost() {
        //ステート変更
        bool boost = (m_TrgState[TRGGER_Boost] ||
            (m_boostCnt > 0 && m_boostCnt < BOOSTMAXCOUNT) );
        m_TrgState[STATE_Boost] = (!m_NowState[STATE_Boost] && boost);
        m_NowState[STATE_Boost] = boost;

        //ブースト開始
        if(m_TrgState[STATE_Boost]) {
            Debug.Log("ブースト！！");
            //ブースト制御用
            m_boostCnt = 0;
            //速度の制御
            m_Speed.SetLevel(1f + 1f);
            m_Speed.AddSeed(m_Speed.MAXSEED);
        }

        //ブースト
        if(m_NowState[STATE_Boost]){
            m_boostCnt++;  //カウント
            m_Speed.SubLevel(1f / JumpStatus.MAXCNT);
            
            //終了
            if(m_boostCnt >= BOOSTMAXCOUNT) {
                Debug.Log("ブースト終了");
                m_Speed.SetLevel(1.0f);
            }
        }
    }

    //ハンドル操作=============================================================
    //  左右入力により移動方向を変更する
    //=========================================================================
    private void HandleFunc() {
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        //旋回するときに減速するシステムがいる
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        //if(m_State[STATE_Drift]) return;

        m_handleSeed += m_Input.axis.x;
        if(m_handleSeed > 0.0f && m_Input.axis.x < 0.0f) { m_handleSeed = 0.0f; }
        if(m_handleSeed < 0.0f && m_Input.axis.x > 0.0f) { m_handleSeed = 0.0f; }
        if(m_Input.axis.x > -0.1f && 0.1f > m_Input.axis.x) { m_handleSeed = 0.0f; }

        if((int)m_handleSeed != 0) {
            // Seed が 一定以上になると  急に曲がる
            //  ROTSTEPNEXT 未満  ROTSTEP1倍  ／  ROTSTEPNEXT 以上  ROTSTEP2倍
            float rot = m_Speed.TURN * ((Mathf.Abs(m_handleSeed) < ROTSTEPNEXT) ? ROTSTEP1 : ROTSTEP2);
            Vector3 axis = new Vector3(0, -1, 0) * Mathf.Sign(m_handleSeed);
            m_handleDir = MyUtility.Vec3DRotation(m_handleDir, rot, axis);
        }
    }

    //高さ移動方向修正=========================================================
    //  足元のレイキャスト情報と移動先のレイキャスト情報から
    //  坂の角度を計算して移動先ベクトルを再設定する
    //=========================================================================
    private void SlopeFunc() {

        //高さ方向を水平にする
        if(m_TrgState.Or(STATE_OnGround, STATE_Glide)) {
            m_angleDir = Vector3.forward;
        }

        //---------------------------------------------------------------------
        //ここより先は、地面に接触していないときは処理しない
        if(!m_NowState[STATE_OnGround]) return;
        
        //坂判定フラグ
        bool fSlope = false;

        //移動先の垂直なレイキャスト情報から坂判定フラグを取得
        if(m_FrontDownRay.hit) {
            float rot = Mathf.Acos(Vector3.Dot(Vector3.up, m_FrontDownRay.hitData.normal));
            fSlope = (0f <= rot && rot <= SLOPE);
        } else {
            return;
        }

        //移動方向修正
        if(fSlope) {
            Vector3 vec = (m_FrontDownRay.hitData.point - m_UnderRay.hitData.point).normalized;
            m_angleDir.y = vec.y;
            m_angleDir.z = Mathf.Cos(Mathf.Asin(vec.y));
            m_angleDir.Normalize();
        }

    }

    //壁判定===================================================================
    //  移動先方向にレイキャストを飛ばして、取得した情報から壁があるかどうかを
    //  判定する
    //=========================================================================
    private void WallCheck() {
        //正面にレイを飛ばす
        if(m_FrontRay.hit) {
            float rot = Mathf.Acos(Vector3.Dot(Vector3.up, m_FrontRay.hitData.normal));
            float y = m_FrontRay.hitData.normal.y;
            m_NowState[STATE_HitWoll] = ((y <= 0) || (y > 0 && rot > SLOPE));
        } else {
            m_NowState[STATE_HitWoll] = false;
        }
    }

    //移動方向の適用===========================================================
    //  実際の移動方向とモデル方向を決定する
    //=========================================================================
    private void AppForward() {
        //移動方向
        m_forward = MyUtility.Vec3DRotation(m_angleDir,
            Mathf.Atan2(m_handleDir.z, m_handleDir.x) - Mathf.PI/2f, Vector3.up);

        //Debug.DrawRay(traPos, m_forward, Color.cyan, Mathf.Max(0.1f, m_Speed.value));
        
        //モデル方向
        m_modelFrwrd = m_handleDir;
    }

    //速度値の適用=============================================================
    private void AppSpeed() {
        traPos += m_forward * m_Speed.value;
        traForward = m_modelFrwrd;
    }
}