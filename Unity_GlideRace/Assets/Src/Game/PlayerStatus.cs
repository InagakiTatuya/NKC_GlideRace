//#############################################################################
//  最終更新者：稲垣達也
//　最終更新日：2015/11/04
//
//　プレイヤーキャラクターのステータスで使うクラスを複数定義する
//
//#############################################################################

//Usuing///////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// ステート ///////////////////////////////////////////////////////////////////
public enum RackState : int {
    READY = 0,
    RACK1 = 1,
    RACK2 = 2,
    GOAL  = 3,
}

// 重力ステータス /////////////////////////////////////////////////////////////
[System.Serializable]
public class GravityStatus {
    //公開静的変数
    public static float STA_GRAVITY = -0.019f;
    public static float STA_FALLMIN = -1.19f;
    //公開変数
    public float seed;      //重力加速の基礎となる値
    public float fall;

    public GravityStatus() {
        seed = 0;
        fall = 0;
    }

    public void Reset() {
        seed = 0;
        fall = 0;
    }

    public void AddSeed() {
        seed++;
        fall = STA_GRAVITY * seed;
        fall = Mathf.Max(fall, STA_FALLMIN);
    }

}

// 速度ステータス /////////////////////////////////////////////////////////////
[System.Serializable]
public class SpeedStatus {
    public float ACC   = 0.1f; //Acceleration
    public float MAX   = 1f;
    public float TURN  = 0.1f;
    public float seed  = 0f;
    public float value = 0f;

    public SpeedStatus(float aACC, float aMAX, float aTURN) {
        ACC   = aACC;
        MAX   = aMAX;
        TURN  = aTURN;
        seed  = 0f;
        value = 0f;
    }

    public void AddSeed(float v) {
        seed  = Mathf.Min(seed + v, 16.0f); // 16 == pow(1 / 0.25, 2)
        value = Mathf.Pow(seed + 1, 0.25f) - 1;
    }

    public void SubSeed(float v) {
        seed  = Mathf.Max(seed - v, 0.0f);
        value = Mathf.Pow(seed + 1, 0.25f) - 1;
    }
}

// グライダーステータス ///////////////////////////////////////////////////////
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

// ヒートステータス ///////////////////////////////////////////////////////////
[System.Serializable]
public class HeatStatus {
    public const  float     MAX          = 100f;
    public static float[]   RANKINGBONUS = { 1.0f, 1.1f, 1.3f, 1.6f };
    
    public float timeCount = 0f;
    public float value     = 0f;
    
    public HeatStatus() {
        timeCount = 0f;
        value     = 0f;
    }
}

// ゴールデータ ///////////////////////////////////////////////////////////////
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

