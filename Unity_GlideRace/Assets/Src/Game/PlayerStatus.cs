//#############################################################################
//  最終更新者：稲垣達也
//  最終更新日：2015/11/04
//
//  プレイヤーキャラクターのステータスで使うクラスを複数定義する
//
//#############################################################################

//Usuing///////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;


///////////////////////////////////////////////////////////////////////////////
// 個別ステート
//   キャラの個性を設定するステート
///////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public struct PlayerCharStateData {
    public int   modelId;   //モデルID
    public float wait;      //重さ
    public float accel;     //加速
    public float turn;
    public float maxSpeed;
    
    public PlayerCharStateData(int aModelId , float aWait, float aAccel, float aTurn, float aMaxSpeed) {
        modelId  = aModelId;
        wait     = aWait;
        accel    = aAccel;
        turn     = aTurn;
        maxSpeed = aMaxSpeed;
    }
}


///////////////////////////////////////////////////////////////////////////////
// ラック
///////////////////////////////////////////////////////////////////////////////
public enum RackState : int {
    READY = 0,
    RACK1 = 1,
    RACK2 = 2,
    GOAL  = 3,
}

///////////////////////////////////////////////////////////////////////////////
// 重力ステータス 
///////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class GravityStatus {
    //公開静的変数
    public static float STA_GRAVITY = 0.019f;
    public static float STA_FALLMAX = 1.19f;
    //公開変数
    public float seed;      //重力加速の基礎となる値
    public float fallValue;

    public GravityStatus() {
        seed = 0;
        fallValue = 0;
    }

    public void Reset() {
        seed = 0;
        fallValue = 0;
    }

    public void AddSeed() {
        seed++;
        fallValue = STA_GRAVITY * seed;
        fallValue = Mathf.Min(fallValue, STA_FALLMAX);
    }

}

///////////////////////////////////////////////////////////////////////////////
// 速度ステータス 
///////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class SpeedStatus {
    public const float MAXLEVEL  = 4;
    public const float MATHINDEX = 2f; //指数

    public float ACC        = 0.1f; //Acceleration
    public float TURN       = 0.1f;  
    public float MAXSEED    = 1f;
    public float MASXVALUE  = 1f;
    public float MINADDSEED = 3f;   //加算時にこの値以下ならこの値になる
    private float m_seed  = 0f;    //基準となる数値
    private float m_level = 1f;
    private float m_value = 0f;    //実際の数値

    public float seed     { get{ return m_seed;  } }
    public float level    { get{ return m_level; } }
    public float value    { get{ return m_value; } }
    public float sqrValue { get{ return m_value * m_value; } }

    public SpeedStatus(float aACC, float aTURN, float aMASXVALUE, float aMAXSEED = 16f) {
        ACC       = aACC;
        TURN      = aTURN;
        MAXSEED   = aMAXSEED;
        MASXVALUE = aMASXVALUE;
        m_seed   = 0f;
        m_level  = 1f;
        m_value  = 0f;
    }

    public void Reset() {
        m_seed   = 0f;
        m_level  = 1f;
        m_value  = 0f;
    }

    //レベル===================================================================
    public void AddLevel(float v) {
        m_level = Mathf.Min(m_level + v, MAXLEVEL);
    }
    public void SubLevel(float v) {
        m_level = Mathf.Max(m_level - v, 1.0f);
    }
    public void SetLevel(float v) {
        m_level = Mathf.Max(Mathf.Min(v, MAXLEVEL), 1.0f);
    }
    //シード===================================================================
    public void AddSeed(float v) {
        m_seed  = Mathf.Max(0.0f, Mathf.Min(seed + v, MAXSEED));
        m_value = m_level * MASXVALUE * Mathf.Pow(seed * (1f / MAXSEED), MATHINDEX);
    }

    public void SubSeed(float v) {
        m_seed  = Mathf.Max(0.0f, Mathf.Min(seed - v, MAXSEED));
        m_value = m_level * MASXVALUE * Mathf.Pow(seed * (1f / MAXSEED), MATHINDEX);
    }

    //デバック用\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
    public string DebugDrawString() {
        string s = "";
        s += "seed  = " + m_seed + "\nLevel = " + m_level +
             "\nVaue = " + m_value;
        return s;
    }

}

///////////////////////////////////////////////////////////////////////////////
// ジャンプステータス 
///////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class JumpStatus {
    public const float   ADDSPEEDSEED = 4f;
    public const int     MAXCNT = 60;
    public int frameCnt;

    public JumpStatus() {
        frameCnt = 0;
    }

    public void Reset() {
        frameCnt = 0;
    }
}

///////////////////////////////////////////////////////////////////////////////
// グライダーステータス 
///////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class GliderStatus {
    public const float MAX = 100f;
    public const float ADDVALUE = 10f;
    public const float SUDVALUE = 1f;
    public float value;

    public GliderStatus() {
        value = MAX;
    }

    public void AddValue(float v) {
        value += v;
        if(value > MAX) value = MAX;
    }

    public void SubValue(float v) {
        value -= v;
        if(value < 0) value = 0;
    }

}

///////////////////////////////////////////////////////////////////////////////
// ヒートステータス 
///////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class HeatStatus {
    public const  float     MAX          = 100f;
    public static float[]   RANKINGBONUS = { 1.0f, 1.1f, 1.3f, 1.6f };
    
    public float timeCount = 0f;
    public float value     = 0f;
    
    public HeatStatus() {
        timeCount = 0f;
        value     = MAX;
    }
}



///////////////////////////////////////////////////////////////////////////////
// ゴールデータ 
///////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class GoalData {
    public int   ranking;
    public float time;
    public int   score;

    public GoalData() {
        ranking = 0;
        time    = 0f;
        score   = 0;
    }
}

