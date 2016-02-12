//#############################################################################
//  最終更新者：稲垣達也
//  GameSceneManager内で使用するために作成
//  Timerを表示するための処理等を定義する
//
//#############################################################################

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class TimeSprite {

    private NumberSprite[]  m_TimeSpriteArr; //時間描画
    private const int UNITY_MAXMUN = 8; //管理する数

    private const float ONE_UNIT_SIZE = 60;

    //初期化===================================================================
    public void Initialize(Transform aCanvasTra, string aPath) {
        m_TimeSpriteArr = aCanvasTra.FindChild(aPath).GetComponentsInChildren<NumberSprite>();
        
        for(int i=0; i < m_TimeSpriteArr.Length; i++) {
            m_TimeSpriteArr[i].transform.localPosition = new Vector3(ONE_UNIT_SIZE * (i + 1), 0, 0);
        }
        
    }

    //タイム描画===============================================================
    //  タイマーをスプライトに適応する
    //=========================================================================
    public void DrawTimer(float aTime) {
        int m = (int)aTime / 60;
        int s = (int)aTime - (60 * m);
        int f = (int)((aTime - (60 * m + s)) * 100f);

        m_TimeSpriteArr[0].SetNumber(m / 10);
        m_TimeSpriteArr[1].SetNumber(m % 10);
        m_TimeSpriteArr[2].SetNumber(10); 
        m_TimeSpriteArr[3].SetNumber(s / 10);
        m_TimeSpriteArr[4].SetNumber(s % 10);
        m_TimeSpriteArr[5].SetNumber(10); 
        m_TimeSpriteArr[6].SetNumber(f / 10);
        m_TimeSpriteArr[7].SetNumber(f % 10);
        

    }
}


