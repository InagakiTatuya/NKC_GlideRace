using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPCIcon : MonoBehaviour {
	
	private	readonly	Vector3[]	DEFALTPOS	=	new Vector3[3]{
		new Vector3(-50.0f,0.0f,1.0f),
		new Vector3(-10.0f,0.0f,1.0f),
		new Vector3(30.0f,0.0f,1.0f)};
	public			enum			STATUS{Control,Decision,Cancel,None}
	public			STATUS			state;
	public			CursorManager	PCursor;
	public			NPCWindow		windowNo;
	public			bool			putOn;
	private			Image			image;
	private			RectTransform	trans;
	public			int				selectNo;
	private			float			timer;
	private	const	float			maxTime	=	0.25f;
	private			Vector3			PREVPOS;
	public			int				No;
	private			GameObject		parentObj;
	private			IconCount		ic;
	private			selectIcon		si;
	private			bool			send;
	private	const	int				setNo	=	1;

	void Start () {
		PCursor				=	null;
		state				=	STATUS.None;
		selectNo			=	4;
	    image				=	GetComponent<Image>();
	    trans				=	image.rectTransform;
		trans.localScale	=	Vector3.one;
		timer				=	0.0f;
		trans.localPosition	=	DEFALTPOS[No];
		PREVPOS				=	transform.position;
		parentObj			=	transform.parent.gameObject;
		ic					=	parentObj.GetComponent<IconCount>();
		ic.selectNo[No+1]	=	2;
		send				=	false;
	}
	
	void Update () {
		switch(state){
			case STATUS.Control:	control();	break;
			case STATUS.Decision:	decision();	break;
			case STATUS.Cancel:		cancel();	break;
			case STATUS.None:					break;
		}
		if(windowNo.npcButton.state	== NPCButton.STATUS.CLOSE)	state = STATUS.Cancel;
	}

	void control(){
		if(send)	ic.setNum	-=	setNo;
		send	=	false;
		if(PCursor == null){
			state	=	STATUS.None;
			return;
		}
		//アイコンをマウスの後ろに移動
		timer		+=	Time.deltaTime;
		float	n	=	Mathf.Min(timer/maxTime,1.0f);
		Vector3	adPos;
		adPos.x		=	PCursor.transform.position.x - (PCursor.trans.sizeDelta.x * 0.6f);
		adPos.y		=	PCursor.transform.position.y - (PCursor.trans.sizeDelta.y * 0.1f); 
		adPos.z		=	PCursor.transform.position.z;
		Vector3		value	=	adPos * n + PREVPOS * (1 - n);
		transform.position	=	value;
		if(n != 1.0f)	return;	
		//選択されているキャラをウィンドウに表示
		windowNo.selectNo		=	selectNo;
		ic.selectChara[No+1]	=	selectNo;
		if(!putOn){
			//セレクトウィンドウ以外の時、画像を表示しない
			selectNo				=	4;
			ic.selectChara[No+1]	=	selectNo;
			return;
		}	
		if(!PCursor.GetAccel())	return;
		timer	=	0;
		state	=	STATUS.Decision;
	}
	
	void decision(){
		PREVPOS	=	transform.position;
		if(ic	==	null)	return;
		if(!send)	ic.setNum	+=	setNo;
		send		=	true;
	}

	void cancel(){//ウィンドウが閉じた時
		if(send)	ic.setNum	-=	setNo;
		send	=	false;
		windowNo.selectNo		=	4;
		ic.selectChara[No+1]	=	4;
		ic.selectNo[No+1]		=	0;
		Destroy(gameObject);
	}
}
