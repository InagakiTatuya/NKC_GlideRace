﻿
//#############################################################################
//  最終更新者：稲垣達也
//
//  InputManagerに自動でデータを入れるためのメニューバーとその処理を定義
//
//#############################################################################

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
 
/// <summary>
/// InputManagerを自動的に設定してくれるクラス
/// </summary>
public class MenuItemInputManagerSetter {

    // インプットマネージャーをクリアします。
    [MenuItem("InputManager/Clear axes")]
    public static void Clear() {
        Debug.Log("インプットマネージャーの設定を開始します。");
        InputManagerGenerator inputManagerGenerator = new InputManagerGenerator();

        Debug.Log("設定を全てクリアします。");
        inputManagerGenerator.Clear();
    }


    [MenuItem("InputManager/Reset axes")]
    public static void Reset() {
        Debug.Log("インプットマネージャーの設定を開始します。");
        InputManagerGenerator inputMgr = new InputManagerGenerator();

        inputMgr.Clear();

        //共通の入力設定
        inputMgr.AddAxis(AxisData.CreateKeyAxis("Horizontal"  , "left", "right"));//横軸
        inputMgr.AddAxis(AxisData.CreateKeyAxis("Vertical"    , "up"  , "down" ));//縦軸
        inputMgr.AddAxis(AxisData.CreatePadAxis("Horizontal"  , 1, 0));//横軸
        inputMgr.AddAxis(AxisData.CreatePadAxis("Vertical"    , 2, 0));//縦軸
        inputMgr.AddAxis(AxisData.CreateButton ("Accelerator" , "space"      , "joystick button 0" ));//適用・アクセル
        inputMgr.AddAxis(AxisData.CreateButton ("Brake"       , "backspace"  , "joystick button 1" ));//取消 ・ブレーキ
        inputMgr.AddAxis(AxisData.CreateButton ("Glider"      , "left shift" , "joystick button 2" ));//滑空
        inputMgr.AddAxis(AxisData.CreateButton ("Drift"       , "left ctrl"  , "joystick button 6" ));//ドリフト
        inputMgr.AddAxis(AxisData.CreateButton ("Menu"        , "escape"     , "joystick button 9" ));//メニュー
        

        //プレイヤーごとの入力設定
        for(int i=1; i <= 4; i++) {
            inputMgr.AddAxis(AxisData.CreatePadAxis("P" + i + "Horizontal"  , 1, i)); //横軸
            inputMgr.AddAxis(AxisData.CreatePadAxis("P" + i + "Vertical"    , 2, i)); //縦軸
            inputMgr.AddAxis(AxisData.CreateButton ("P" + i + "Accelerator" , "joystick " + i + " button 0" ));//適用・アクセル
            inputMgr.AddAxis(AxisData.CreateButton ("P" + i + "Brake"       , "joystick " + i + " button 1" ));//取消 ・ブレーキ
            inputMgr.AddAxis(AxisData.CreateButton ("P" + i + "Glider"      , "joystick " + i + " button 2" ));//滑空
            inputMgr.AddAxis(AxisData.CreateButton ("P" + i + "Drift"       , "joystick " + i + " button 6" ));//ドリフト
            inputMgr.AddAxis(AxisData.CreateButton ("P" + i + "Menu"        , "joystick " + i + " button 9" ));//メニュー
        }
        
        Debug.Log("設定を再設定しました。");
    }

}

#endif