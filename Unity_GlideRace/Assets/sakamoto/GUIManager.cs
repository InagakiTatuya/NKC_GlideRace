using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GUIManager : MonoBehaviour {

	//固定変数-------------------------------
    const float RADIUS = 2.0f;
    const float FLATTEN_RATE = 0.2f;
	//---------------------------------------

	//先行計算しておいた移動方向をインスペクタで代入する
	[SerializeField]
	private Vector3 moveVec;
	//先行計算しておいた移動方距離をインスペクタで代入する
	[SerializeField]
	private float moveLength;

	private float distance;

	private Transform targetObj;


	private Renderer tesRender;
    GameObject choicObj;
    public GameObject[] choice;
	//回転フラグ
	private bool rotflg;
	//回転角度
	private float privangle;
	private float delta = 0;
	//選択されているID
	public int selectID = 0;
	private string arrow;
	float alpha = 1;
	bool[] endflg;
	enum state{
		initial,
		move,
		dispersion,
		arrange,
	}
	private int stateNo;
	UnityAction[] Func;
	UnityAction[] Child;
	Vector3[] dis = new Vector3[] { 
		new Vector3(-0.4f,  0.0f,  8.8f), 
		new Vector3( 0.2f, -1.73f, 9.4f),
		new Vector3( 0.2f,  1.73f, 9.4f) };
	Vector3[] disSc = new Vector3[] { 
		new Vector3(1.5f, 1.5f, 0.0f), 
		new Vector3(0.75f, 0.75f, 9.4f), 
		new Vector3(0.75f, 0.75f, 9.4f) };

	void SetChildCount(int Count){
		//子オブジェクト文の配列数に変形
		choice = new GameObject[Count];
		endflg = new bool[Count];

	 }
	//初期化
	void Start(){
		targetObj = transform.GetChild(0);
		rotflg = false;
		SetChildCount(ChildNum());
		//選択IDは0に
		selectID = 0;
		Func = new UnityAction[]{
			Initial,
			Move,
			Dispersion,
			Arrange,
		};
	}

	//更新
	void Update(){
		//Debug.Log(stateNo);
		Func[stateNo]();

	}

	void Initial(){
		//子オブジェクトのデータを配列内に格納
		for (int i = 0; i < ChildNum(); i++)
		{
			endflg[i] = false;
			Vector3 prevPos = Camera.main.ScreenToWorldPoint(new Vector3(-(Screen.width), (Screen.height / 2), 10));
			transform.GetChild(i).transform.position = prevPos;
			transform.GetChild(i).transform.localScale = new Vector3(1.0f, 1.0f, 0);
			choice[i] = transform.GetChild(i).gameObject;
		}
		stateNo = (int)state.move;
	}

	void Move(){
		for (int i = 0; i < ChildNum(); i++){
			Vector3 pos = choice[i].transform.position;
			choicObj	= choice[i];
			pos.x += 0.18f;
			if (dis[0].x <= pos.x){
				pos.x = dis[0].x;
				stateNo = (int)state.dispersion;
			}
			choicObj.transform.position = pos;
		}
	}

	void Dispersion(){
		for (int i = 0; i < ChildNum(); i++){
			dispersion(transform.GetChild(i),i);
		}
		if (IsComplite()) stateNo = (int)state.arrange;
	}

	void Arrange(){
		//回転角度は常に0に
		privangle = 0.0f;
		//カルーセル
		arrange();
		//方向キー↑を入力した時
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			//現在、回転中でなければ
			if (!GetRotation())
			{
				SetRotation(true);
				SetArrow("Up");
			}
		}//方向キー↓を入力した時
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			//現在、回転中でなければ
			if (!GetRotation())
			{
				SetRotation(true);
				SetArrow("Down");
			}
		}
		ControlRotate();
		privangle += delta;
		arrange();
	}

	bool IsComplite()
	{
		for (int i = 0; i < endflg.Length; i++){
			if (!endflg[i])	return false;
		}
		return true;
	}

	//カルーセル
	void dispersion(Transform childTarget, int ID){

		float speed = 0.2f;
		Vector3 scale = childTarget.localScale;
		Vector3 disS = Vector3.Normalize(disSc[ID] - scale);
		childTarget.localScale += disS * speed;

		if (ID == 0){
			if (childTarget.localScale.x >= disSc[ID].x) childTarget.localScale = disSc[ID];
			endflg[ID] = true;
		}else{
			if (childTarget.localScale.x <= disSc[ID].x) childTarget.localScale = disSc[ID];
		}

		if (ID == 0) return;
		alpha -= 0.05f;
		if (alpha <= 0.4) alpha = 0.4f;
		Color c = childTarget.GetComponent<Renderer>().material.color;
		childTarget.GetComponent<Renderer>().material.color = new Color(c.r	,c.b ,c.g, alpha);
		Vector3 pos = childTarget.position;
		tesRender = childTarget.gameObject.GetComponent<Renderer>();
		Vector3 disP	= dis[ID]	- pos;
		pos += Vector3.Normalize(disP) * 0.5f;

		if(ID==1){
			if (pos.x > dis[ID].x)	pos.x = dis[ID].x;
			if (pos.y < dis[ID].y)	pos.y = dis[ID].y;
			endflg[ID] = matchPos(dis[ID], pos);
		}else if(ID==2){
			if (pos.x > dis[ID].x)	pos.x = dis[ID].x;
			if (pos.y > dis[ID].y) pos.y = dis[ID].y;
			endflg[ID] = matchPos(dis[ID], pos);
		}
		childTarget.position = pos;
	}

	bool matchPos(Vector3 target,Vector3 pos){
		if (target.x == pos.x && target.y == pos.y) return true;
		return false;
	}

	//カルーセル
    void arrange()
    {
		float theta = 360.0f / ChildNum();
		float baseAngle = privangle + 180.0f;
		for (int i = 0; i < ChildNum(); i++){
			choicObj = choice[i].gameObject;
            tesRender = choicObj.GetComponent<Renderer>();
            Color tesColor = tesRender.material.color;
            // 270 度の位置が正面にくるように
			float angle = theta * i + baseAngle;
            float radians = angle * Mathf.PI / 180.0f;
            float x = RADIUS * Mathf.Cos(radians) * FLATTEN_RATE;
            float y = RADIUS * Mathf.Sin(radians);
            float radiusX = RADIUS * FLATTEN_RATE;
            float diameterX = radiusX * 2;
            float scale = (diameterX - x) / diameterX;
            Color opacity = new Color(tesColor.r, tesColor.g, tesColor.b, Mathf.Abs(1 - (x + radiusX)));
			Transform choicetrans = choicObj.transform;
			choicetrans.position = new Vector3(x + transform.position.x, y + transform.position.y, -(diameterX - x));
			choicetrans.localScale = new Vector3(scale, scale, 0);
			tesRender.material.color = opacity;
        }
    }

	//子オブジェクトを回転
	void ControlRotate(){
		//回転中ならば
		if(!GetRotation())	return;
		//string変数がUpならば
		if(GetArrow() == "Up")			StartCoroutine("UpRotate");
		//string変数がDownならば
		else if(GetArrow() == "Down")	StartCoroutine("DownRotate");
	}

	//上回転
	IEnumerator UpRotate(){
		delta += 5;
		//deltaが120で割り切れる時
		if (delta % 120 != 0) yield break;
		//回転終了
		SetRotation(false);
		//選択IDを1増やす
		selectID++;
		//選択IDが子オブジェクトの数以上の時
		if (selectID >= transform.childCount) selectID = 0;
    }

	//下回転
	IEnumerator DownRotate(){
		delta -= 5;
		//deltaが120で割り切れる時
		if (delta % 120 != 0) yield break;
		//回転終了
		SetRotation(false);
		//選択IDを１減らす
		selectID--;
		//選択IDが0以下の時
		if (selectID < 0) selectID = transform.childCount - 1;
	}

	//子オブジェクト数を計算
    int ChildNum(){
        return  transform.childCount;
	}

    bool GetRotation(){
		return rotflg;
	}

	void SetRotation(bool flg){
		rotflg = flg;
	}

	string GetArrow()
	{
		return arrow;
	}

	void SetArrow(string a)
	{
		arrow = a;
	}
}