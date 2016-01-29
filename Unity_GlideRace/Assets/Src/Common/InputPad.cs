//#############################################################################
//  最終更新者：稲垣達也
//  最終更新日：2015/11/01
//
//  ゲームパッドの入力をまとめて管理する
//
//#############################################################################
using UnityEngine;
using System.Collections;

public static class InputPad {

    //非公開定数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private const string HOR = "Horizontal";    //横軸
    private const string VER = "Vertical";      //縦軸
    private const string ACC = "Accelerator";   //アクセル
    private const string BRA = "Brake";         //ブレーキ
    private const string GLI = "Glider";        //グライド
    private const string DRI = "Drift";         //ドリフト
    private const string MEN = "Menu";          //メニュー

    //公開関数/////////////////////////////////////////////////////////////////
    //軸情報===================================================================
    public static Vector2 AllAxis() {
        return new Vector2(Input.GetAxisRaw(HOR), Input.GetAxisRaw(VER));
    }
    //public static Vector2 AllAxisUp()   { } //開発中
    //public static Vector2 AllAxisDown() { } //Updateでまわさないと難しい

	public static Vector2 Axis(int aNum = 1)
	{
        string str = "P" + aNum;
        return new Vector2(Input.GetAxis(str + HOR), -Input.GetAxis(str + VER));
    }

    //決定・アクセル================================================================================
    public static bool AllAccel()     { return Input.GetButton(ACC);     }
    public static bool AllAccelUp()   { return Input.GetButtonUp(ACC);   }
    public static bool AllAccelDown() { return Input.GetButtonDown(ACC); }

    public static bool Accel(int aNum)      { return Input.GetButton("P" + aNum + ACC); }
    public static bool AccelUp(int aNum)    { return Input.GetButtonUp("P" + aNum + ACC); }
    public static bool AccelDown(int aNum)  { return Input.GetButtonDown("P" + aNum + ACC); }

    //取消・ブレーキ===============================================================================
    public static bool AllBrake()       { return Input.GetButton(BRA); }
    public static bool AllBrakeUp()     { return Input.GetButtonUp(BRA); }
    public static bool AllBrakeDown()   { return Input.GetButtonDown(BRA); }
    public static bool Brake(int aNum)      { return Input.GetButton("P" + aNum + BRA); }
    public static bool BrakeUp(int aNum)    { return Input.GetButtonUp("P" + aNum + BRA); }
    public static bool BrakeDown(int aNum)  { return Input.GetButtonDown("P" + aNum + BRA); }

    //グライド=====================================================================================
    public static bool AllGlide()       { return Input.GetButton(GLI); }
    public static bool AllGlideUp()     { return Input.GetButtonUp(GLI); }
    public static bool AllGlideDown()   { return Input.GetButtonDown(GLI); }
    public static bool Glide(int aNum)      { return Input.GetButton("P" + aNum + GLI); }
    public static bool GlideUp(int aNum)    { return Input.GetButtonUp("P" + aNum + GLI); }
    public static bool GlideDown(int aNum)  { return Input.GetButtonDown("P" + aNum + GLI); }

    //ドリフト=====================================================================================
    public static bool AllDrift()       { return Input.GetButton(DRI); }
    public static bool AllDriftUp()     { return Input.GetButtonUp(DRI); }
    public static bool AllDriftDown()   { return Input.GetButtonDown(DRI); }
    public static bool Drift(int aNum)      { return Input.GetButton("P" + aNum + DRI); }
    public static bool DriftUp(int aNum)    { return Input.GetButtonUp("P" + aNum + DRI); }
    public static bool DriftDown(int aNum)  { return Input.GetButtonDown("P" + aNum + DRI); }

    //メニュー=====================================================================================
    public static bool AllMenu()        { return Input.GetButton(MEN); }
    public static bool AllMenuUp()      { return Input.GetButtonUp(MEN); }
    public static bool AllMenuDown()    { return Input.GetButtonDown(MEN); }
    public static bool Menu(int aNum)       { return Input.GetButton("P" + aNum + MEN); }
    public static bool MenuUp(int aNum)     { return Input.GetButtonUp("P" + aNum + MEN); }
    public static bool MenuDown(int aNum)   { return Input.GetButtonDown("P" + aNum + MEN); }

    //データを入力する=============================================================================
    // 全て -------------------------------------------------------------------
    public static void InputAllData(ref InputData outData) {
        if(outData == null) outData = new InputData();
        outData.axis     = InputPad.AllAxis();
        outData.accel    = InputPad.AllAccel();
        outData.brake    = InputPad.AllBrake();
        outData.glide    = InputPad.AllGlide();
        outData.drift    = InputPad.AllDrift();
        outData.menu     = InputPad.AllMenu();
    }
    public static void InputAllUpData(ref InputData outData) {
        if(outData == null) outData = new InputData();
        outData.axis     = Vector2.zero;
        outData.accel    = InputPad.AllAccelUp();
        outData.brake    = InputPad.AllBrakeUp();
        outData.glide    = InputPad.AllGlideUp();
        outData.drift    = InputPad.AllDriftUp();
        outData.menu     = InputPad.AllMenuUp();
    }
    public static void InputAllDownData(ref InputData outData) {
        if(outData == null) outData = new InputData();
        outData.axis     = Vector2.zero;
        outData.accel    = InputPad.AllAccelDown();
        outData.brake    = InputPad.AllBrakeDown();
        outData.glide    = InputPad.AllGlideDown();
        outData.drift    = InputPad.AllDriftDown();
        outData.menu     = InputPad.AllMenuDown();
    }

    // プレイヤー指定 ---------------------------------------------------------
    public static void InputData(ref InputData outData, int aNum = 1) {
        if(outData == null) outData = new InputData();
        outData.axis     = InputPad.Axis(aNum);
        outData.accel    = InputPad.Accel(aNum);
        outData.brake    = InputPad.Brake(aNum);
        outData.glide    = InputPad.Glide(aNum);
        outData.drift    = InputPad.Drift(aNum);
        outData.menu     = InputPad.Menu(aNum);
    }

	public static void InputUpData(ref InputData outData, int aNum = 1)
	{
        if(outData == null) outData = new InputData();
        outData.axis     = Vector2.zero;
        outData.accel    = InputPad.AccelUp(aNum);
        outData.brake    = InputPad.BrakeUp(aNum);
        outData.glide    = InputPad.GlideUp(aNum);
        outData.drift    = InputPad.DriftUp(aNum);
        outData.menu     = InputPad.MenuUp(aNum);
    }

	public static void InputDownData(ref InputData outData, int aNum = 1)
	{
        if(outData == null) outData = new InputData();
        outData.axis     = Vector2.zero;
        outData.accel    = InputPad.AccelDown(aNum);
        outData.brake    = InputPad.BrakeDown(aNum);
        outData.glide    = InputPad.GlideDown(aNum);
        outData.drift    = InputPad.DriftDown(aNum);
        outData.menu     = InputPad.MenuDown(aNum);
    }
}
