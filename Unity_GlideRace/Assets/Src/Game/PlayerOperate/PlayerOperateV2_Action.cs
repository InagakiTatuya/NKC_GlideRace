//#############################################################################
//  ファイル名：PlayerOperate_Action.cs
//  最終更新者：稲垣達也
//
//  Partial によって分けられたファイルの一つ
//    アクションやリアクションを定義する
//
//#############################################################################
using UnityEngine;
using System.Collections;

public partial class PlayerOperateV2 : BaseObject {

    //グライダー処理===========================================================
    //  滑空処理を定義する
    //=========================================================================
    private void ActionGlider() {
        //グラインドゲージ回復
        if(m_NowState[STATE_OnGround]) {
            m_Glider.AddValue(GliderStatus.ADDVALUE);
        }

        //以下の条件化では、滑空処理を行わない
        bool notGlide = (m_Glider.value <= 0f) || 
            m_NowState.Or(STATE_OnGround, STATE_Boost, STATE_Jump);

        //ステート変更
        bool glide = (m_Input.glide && !notGlide);
        m_TrgState[STATE_Glide] = (!m_NowState[STATE_Glide] && glide);
        m_NowState[STATE_Glide] = glide;

        //滑空開始
        if(m_TrgState[STATE_Glide]) {
            Debug.Log("グライド開始！ plyNum " + m_plyNo);
            //[グライダーエフェクト発生処理をここに書く]
        }

        //滑空処理
        if(m_NowState[STATE_Glide]) {
            //グラインドゲージ減少
            m_Glider.SubValue(GliderStatus.SUDVALUE);
        }

    }

        
    //ジャンプ処理=============================================================
    //  ジャンプ処理を行う
    //=========================================================================
    private const int JUMPMAXCOUNT = 40;
    private int m_jumpCnt = 0;

    private void ActionJump() {
        //ステート変更
        bool jump = (m_TrgState[TRGGER_Jump] ||
            (m_jumpCnt > 0 && m_jumpCnt < JUMPMAXCOUNT) );
        
        m_TrgState[STATE_Jump] = (!m_NowState[STATE_Jump] && jump);
        m_NowState[STATE_Jump] = jump;


    }

}