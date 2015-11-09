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
public enum PlayerState : int {
    READY = 0,
    RACK1 = 1,
    RACK2 = 2,
    GOAL  = 3,
}

// 速度ステータス /////////////////////////////////////////////////////////////
[System.Serializable]
public class SpeedStatus {
    public float ACCELERATION = 0.1f;
    public float MAX   = 1f;
    public float TURN  = 0.1f;
    public float seed  = 0f;
    public float value = 0f;

    public SpeedStatus(float aACC, float aMAX, float aTURN) {
        ACCELERATION = aACC;
        MAX          = aMAX;
        TURN         = aTURN;
        seed  = 0f;
        value = 0f;
    }

    public void AddSeed() {
        seed += ACCELERATION;
        Mathf.Max(seed, 16.0f); // 16 == pow(1/0.25,2)
        value = Mathf.Pow(seed, 0.25f);
    }
}

// グライダーステータス ///////////////////////////////////////////////////////
[System.Serializable]
public class GliderStatus {
    public const float MAX = 100f;
    public float value;
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

