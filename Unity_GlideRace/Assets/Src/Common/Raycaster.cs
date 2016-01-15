//#############################################################################
//  最終更新者：稲垣達也
//  Physics.Raycast関数とそれに接触したデータを一括して扱うクラス
//
//#############################################################################

using UnityEngine;
using System.Collections;

[System.Serializable]
public class Raycaster {
    public Vector3      origin;      //原点
    public Vector3      direction;   //方向
    public float        distance;    //長さ
    public int          layerMask;   //マスク
    public RaycastHit   hitData;

    public bool Raycast() {
        return Physics.Raycast(origin, direction, out hitData, distance, layerMask);
    }
    public bool Raycast(Vector3 aOri, Vector3 aDir, float aDis, int aMask) {
        origin    = aOri;
        direction = aDir;
        distance  = aDis;
        layerMask = aMask;
        return Physics.Raycast(origin, direction, out hitData, distance, layerMask);
    }

}