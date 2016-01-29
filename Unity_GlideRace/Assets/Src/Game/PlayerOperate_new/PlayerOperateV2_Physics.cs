//#############################################################################
//  ファイル名：PlayerOperate_Physics.cs
//  最終編集者：稲垣達也
//
//  プレイヤーの操作と制御
//    Partial によって分けられたファイルの一つ
//    重力処理、地面・壁判定を行う
//#############################################################################
using UnityEngine;
using System.Collections;


public partial class PlayerOperateV2 : BaseObject {

    //接触^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private int           GROUND_MASK;
    private const float   HOVER           = 0.3f; //地面から浮かせる距離
    private const float   UNDERRAYDIS_MIN = 0.5f; //足元レイの最小の長さ
    private const float   FRONTRAYDIS_MIN = 1.6f; //移動先レイの最小の長さ
    private const float   SLOPE = 45f / 180f * Mathf.PI; //上れる坂の角度
    private Raycaster     m_UnderRay;         //足元レイ
    private Raycaster     m_FrontDownRay;     //移動先レイ
    private Raycaster     m_FrontRay;         //移動方向レイ

    //初期化===================================================================
    private void StartPhysics() {
        //接触
        GROUND_MASK      = 0x1 << LayerMask.NameToLayer(LayerNames.Ground);
        m_UnderRay       = new Raycaster();
        m_FrontDownRay   = new Raycaster();
        m_FrontRay       = new Raycaster();
        m_Gravity        = new GravityStatus();
    }
    
    //レイキャスト処理========================================================
    private void PhysicsReycast() {

        //地面接触判定
        m_UnderRay.Reset();
        m_UnderRay.origin    = traPos;
        m_UnderRay.direction = Vector3.down;
        m_UnderRay.distance  = UNDERRAYDIS_MIN + m_Gravity.fallValue;
        m_UnderRay.layerMask = GROUND_MASK;
        m_UnderRay.Raycast();

        //正面にレイを飛ばす
        m_FrontRay.Reset();
        m_FrontRay.origin    = traPos + Vector3.up * 0.5f;
        m_FrontRay.direction = m_handleDir;
        m_FrontRay.distance  = m_Speed.value;
        m_FrontRay.layerMask = GROUND_MASK;
        m_FrontRay.Raycast();

        //移動先に垂直なレイを飛ばす
        m_FrontDownRay.Reset();
        float sv = Mathf.Max(0.01f, m_Speed.value);
        m_FrontDownRay.origin    = traPos + (m_forward * sv) + (Vector3.up * FRONTRAYDIS_MIN);
        m_FrontDownRay.direction = Vector3.down;
        m_FrontDownRay.distance  = m_Gravity.fallValue + FRONTRAYDIS_MIN + UNDERRAYDIS_MIN;
        m_FrontDownRay.layerMask = GROUND_MASK;
        m_FrontDownRay.Raycast();


    }

    //地面判定=================================================================
    //  レイキャスト情報から地面に接触しているかどうかを見る
    //=========================================================================
    private void PhysicsGroundCheck() {
        //ステート変更
        m_TrgState[STATE_OnGround] = (!m_NowState[STATE_OnGround] && m_UnderRay.hit);
        m_NowState[STATE_OnGround] = m_UnderRay.hit;
        
        //地面接触に対する処理
        if(m_NowState[STATE_OnGround]) {
            //地面から浮かす
            SetTraPosY = Mathf.Min(traPos.y + HOVER, m_UnderRay.hitData.point.y + HOVER);
        }
    }

    //重力処理=================================================================
    //  重力の影響を受けて落下する。
    //  また、地面に接触しているときは、地面から浮かせる
    //=========================================================================
    private void PhysicsGravity() {

        //ステート変更
        //以下の条件では、重力処理を行わない
        //  !( ジャンプ || ブースト中 || 滑空中 )
        bool useGra = !m_NowState.Or(STATE_OnGround, STATE_Jump, STATE_Boost, STATE_Glide);
        m_TrgState[STATE_UseGravity] = (!m_NowState[STATE_UseGravity] && useGra);
        m_NowState[STATE_UseGravity] = useGra;
        
        //重力リセット
        if(m_TrgState.Or(STATE_OnGround, STATE_Glide)) {
            m_Gravity.Reset();
        }

        //重力発生瞬間の処理
        if(m_TrgState[STATE_UseGravity]) {

        }

         //重力処理
        if(m_NowState[STATE_UseGravity]) {
            m_Gravity.AddSeed();
            SetTraPosY = traPos.y - m_Gravity.fallValue;
        }
    }
    
    //壁判定===================================================================
    //  移動先方向にレイキャストを飛ばして、取得した情報から壁があるかどうかを
    //  判定する
    //=========================================================================
    private void PhysicsWallCheck() {
        bool wall = false;

        //正面レイキャスト情報から壁判定を取る
        if(m_FrontRay.hit) {
            float rot  = Mathf.Acos(Vector3.Dot(Vector3.up, m_FrontRay.hitData.normal));
            float y    = m_FrontRay.hitData.normal.y;
            wall = ((y <= 0) || (y > 0 && rot > SLOPE));
        }

        //ステート変更
        m_TrgState[STATE_HitWoll] = (!m_NowState[STATE_HitWoll] && wall);
        m_NowState[STATE_HitWoll] = wall;

        if(m_NowState[STATE_HitWoll]) Debug.Log("壁！");
    }
}