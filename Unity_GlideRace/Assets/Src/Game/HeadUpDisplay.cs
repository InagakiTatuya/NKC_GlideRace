﻿//#############################################################################
//  最終更新者：稲垣達也
//  
//  情報を表示するGUIを管理する
//  
//#############################################################################

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeadUpDisplay : MonoBehaviour {

    private Canvas m_Cnvs; //キャンバス

    //ゲージ
    private RectTransform   m_GaugeParent;  //ゲージの親オブジェクト
    private Image           m_GaugeGlider;  //グライダーゲージ
    private Image           m_GaugeHeat;    //ヒートゲージ

    //文字
    private Image           m_Goal; //ゴールのイメージ

    //公開プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    
    public Image         GetGoal        { get{ return m_Goal; } }
    public RectTransform GetGoalRectTra { get{ return m_Goal.transform as RectTransform; } }

    ///////////////////////////////////////////////////////////////////////////
    //  公開関数
    ///////////////////////////////////////////////////////////////////////////
    //=========================================================================
    //ゲージ関連
    //=========================================================================
    public void SetGaugeGlider(float v) {
        m_GaugeGlider.fillAmount = v;
    }

    public void SetGaugeHeat(float v) {
        m_GaugeHeat.fillAmount = v;
    }

    ///////////////////////////////////////////////////////////////////////////
    //  非公開関数
    ///////////////////////////////////////////////////////////////////////////
    //初期化===================================================================
    void Awake() {
        //キャンバス取得
        m_Cnvs = transform.GetComponent<Canvas>();

        //ゲージ取得・初期化
        m_GaugeParent = transform.FindChild("Gauge").GetComponent<RectTransform>();
        m_GaugeGlider = m_GaugeParent.FindChild("GliderGauge/Bar").GetComponent<Image>();
        m_GaugeHeat   = m_GaugeParent.FindChild("HeatGauge/Bar"  ).GetComponent<Image>();

        //文字
        m_Goal        = transform.FindChild("Goal").GetComponent<Image>();
        m_Goal.enabled = false;
    }
    //void Start() { }
    //void Update() { }


}

