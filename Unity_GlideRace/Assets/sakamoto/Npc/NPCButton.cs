using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPCButton : MonoBehaviour {

	public	enum STATUS{OPEN,NORMAL,CLOSE,NOTURN,NONE,}
	public	STATUS				state;
	public	int					No;
	private	float				timer;
	private	float				Counter;
	private	float				Reduction;
	private	Vector3				transSize;
	private	Image				childImage;
	private	RectTransform		childTrans;
	private	Image				image;
	private	RectTransform		trans;
	private	Vector2				DEFALUTSIZE;
	private	Vector2				MAXSIZE;
	private	bool				putOnFlg;
	private	GameObject			prefabObj;
	public	GameObject			ManagerObj;
	public	NPCWindowManager	NPCwm;

	void Start () {
		NPCwm	=	gameObject.transform.parent.GetComponent<NPCWindowManager>();
		putOnFlg	=	false;
		state	=	STATUS.NONE;
		GameObject	obj			=	transform.GetChild(0).gameObject;
		image					=	GetComponent<Image>();
		trans					=	image.rectTransform;
		DEFALUTSIZE				=	trans.sizeDelta;
		MAXSIZE					=	DEFALUTSIZE * 1.1f;
		childImage				=	obj.GetComponent<Image>();
		childTrans				=	childImage.rectTransform;
		childTrans.localScale	=	Vector3.zero;
		Counter = 0.0f;
		prefabObj				=	NPCwm.obj;
	}
	
	void Update () {
		if(No == 0)	Debug.Log(state);
		if(!putOnFlg){
			Reduction	+= 0.1f;
			float	t = Mathf.Pow(Reduction,3.0f);
			Vector2	size;
			size.x	=	t;
			size.y	=	t;
			if(DEFALUTSIZE.x <= trans.sizeDelta.x &&
				DEFALUTSIZE.y	<= trans.sizeDelta.y)	trans.sizeDelta	-=	size;
			else{ Reduction = 0.0f;}
		}
		switch(state){
			case STATUS.OPEN:	Open();		break;
			case STATUS.CLOSE:	Close();	break;
			case STATUS.NORMAL:	Normal();	break;
			case STATUS.NOTURN:	Noturn();	return;
			case STATUS.NONE:	None();		return;
		}
	}
	void OnTriggerStay2D(Collider2D other){
		putOnFlg	=	true;
		if(state != STATUS.NONE) return;
		CursorManager	cm	=	other.gameObject.GetComponent<CursorManager>();
		if(cm == null)	return;
		Counter += 0.1f;
		if(ComparisonSize(trans.sizeDelta,MAXSIZE))	trans.sizeDelta	+=	SetSize(Mathf.Pow(Counter,3));
		else{ trans.sizeDelta	=	MAXSIZE; }
		if(!cm.GetAccel())	return;
		state = STATUS.OPEN;
	}

	void OnTriggerExit2D(Collider2D other){
		Counter = 0.0f;
		putOnFlg=false;
	}

	void Open(){
		timer += 0.1f;
		float t = timer;
		t = Mathf.Pow(t, 3);
		transSize.x = t;
		transSize.y = t;
		childTrans.localScale = transSize;
		if (timer < 1.0f)	return;
		timer = 1.0f;
		childTrans.localScale	=	Vector3.one;
		trans.sizeDelta			=	Vector2.zero;
		state					=	STATUS.NORMAL;

		GameObject		obj		=	Instantiate(prefabObj);
		obj.transform.parent	=	NPCwm.ManagerObj.transform;
		obj.transform.position	=	NPCwm.ManagerObj.transform.position;
		NPCIcon		npcIcon		=	obj.GetComponent<NPCIcon>();
		npcIcon.No				=	No;
		npcIcon.windowNo		=	transform.GetChild(0).gameObject.GetComponent<NPCWindow>();
	}

	void Close(){
		timer -= 0.1f;
		float t = timer;
		t = Mathf.Pow(t, 3);
		transSize.x = t;
		transSize.y = t;
		childTrans.localScale	=	transSize;
		if (timer >= 0.0f)	return;
		timer = 0.0f;
		childTrans.localScale	=	Vector3.zero;
		trans.sizeDelta			=	DEFALUTSIZE;
		state					=	STATUS.NONE;
	}

	void Normal(){

	}

	void Noturn(){
		trans.localScale		=	Vector3.zero;
		childTrans.localScale	=	Vector3.zero;
	}

	void None(){
		if(trans.localScale	!=	Vector3.zero)	return;
		trans.localScale	=	Vector3.one;
		trans.sizeDelta		=	DEFALUTSIZE;
	}

	bool ComparisonSize(Vector2 a,Vector2 b){
		if(a.x < b.x && a.y < b.y)	return true;
		return false;
	}
	Vector2 SetSize(float n){
		Vector2 size;
		size.x	=	n;
		size.y	=	n;
		return	size;
	}
}
