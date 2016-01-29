//#############################################################################
//  作者：稲垣達也
//  
//  行数が多くなる処理を簡略化するために
//  個人的にほしい関数等を宣言する
//#############################################################################
using UnityEngine;
using System.Collections;

public class MyUtility {

    ///////////////////////////////////////////////////////////////////////////
    //  回転処理
    ///////////////////////////////////////////////////////////////////////////
    //回転処理=================================================================
    //  渡されたベクトルを２Ｄ回転させる
    //  第１引数：回転させるベクトル
    //  第２引数：回転させる量
    //=========================================================================
    public static Vector3 Vec3DRotation(Vector2 aVec, float aRot) {
        Vector2 v = aVec;
        float sin_r = Mathf.Sin(aRot);
        float cos_r = Mathf.Cos(aRot);

        v.x = aVec.x * cos_r - aVec.y * sin_r;
        v.y = aVec.x * sin_r + aVec.y * cos_r;
        return v;
    }

    //回転処理=================================================================
    //  渡されたベクトルを３Ｄ回転させる
    //  第１引数：回転させるベクトル
    //  第２引数：回転させる量
    //  第３引数：回転軸
    //=========================================================================
    public static Vector3 Vec3DRotation(Vector3 aVec, float aRot, Vector3 aAxis) {
        float sin_r = Mathf.Sin(aRot / 2f);
        float cos_r = Mathf.Cos(aRot / 2f);

        Quaternion f = new Quaternion(aVec.x, aVec.y, aVec.z, 0.0f);
        Quaternion q = new Quaternion(sin_r *  aAxis.x, sin_r *  aAxis.y, sin_r *  aAxis.z, cos_r);
        Quaternion r = new Quaternion(sin_r * -aAxis.x, sin_r * -aAxis.y, sin_r * -aAxis.z, cos_r);
        Quaternion qr = r * f * q;
        return new Vector3(qr.x, qr.y, qr.z);
    }

    //回転処理=================================================================
    //  渡されたベクトルを二つのベクトルのなす角の量で回転させる
    //  第１引数：回転させるベクトル
    //  第２引数：なす角を作るベクトル１
    //  第３引数：なす角を作るベクトル２
    //  第４引数：回転軸
    //=========================================================================
    public static Vector3 Vec3DRotationEx(Vector3 aVec, Vector3 aV1, Vector3 aV2, Vector3 aAxis) {
        Vector3 vec   = aVec;
        float   angle = Mathf.Acos(Vector3.Dot(aV1, aV2));
        Vector3 acxis = Vector3.Cross(aV1, aV2);
        float   pm    = (acxis.y < 0) ? -1 : +1;
        return Vec3DRotation(vec, angle, aAxis * pm);
    }
}
