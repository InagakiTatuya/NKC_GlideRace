//#############################################################################
//  最終更新者：稲垣達也
//　最終更新日：2015/11/01
//
//　ゲームパッドの入力をまとめて管理する
//　参照渡しするためにクラスで定義する
//#############################################################################
using UnityEngine;

[System.Serializable]
public class InputData {
    public Vector2 axis;

    public bool accel;
    public bool brake;
    public bool glide;
    public bool drift;
    public bool menu;

    public void Reset() {
        axis = Vector2.zero;
        accel = false;
        brake = false;
        glide = false;
        drift = false;
        menu  = false;
    }
}

