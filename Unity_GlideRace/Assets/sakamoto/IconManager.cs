using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IconManager : MonoBehaviour {
	//初期位置
	readonly		Vector3			DEFALUTPOS	=	new Vector3(-20.0f, -5.0f, 0.0f);
	//速度
	readonly		float			speed		=	15.0f;
	public			int				swicthFlg;
	public			bool			cancelFlg;
	public			bool			setFlg;
	public			bool			putOn;
	private			Image			image;
	private			RectTransform	trans;
	private			float			timer;
	private	const	float			maxTime	=	0.25f;
	public			bool			decFlg;
	private			Vector3			prevPos;
	public			int				selectNo;
	private			GameObject		parentObj;
	private			IconCount		ic;
	private			bool			send;
	private	const	int				setNo	=	1;
	public			int				No;

	void Start(){
		selectNo			=	4;
		swicthFlg			=	0;
		timer				=	maxTime;
		decFlg				=	false;
		cancelFlg			=	false;
		setFlg				=	false;
		prevPos				=	transform.position;
		image				=	gameObject.GetComponent<Image>();
		trans				=	image.rectTransform;
		parentObj			=	transform.parent.transform.parent.gameObject;
		ic					=	parentObj.GetComponent<IconCount>();
		send				=	false;
		ic.selectNo[No-1]	=	1;
	}
	
	void Update () {
		if (swicthFlg%2 != 0){
			if (!setFlg) prevPos = trans.position;
		    setFlg	=	true;
			decFlg	=	true;
			timer	=	0.0f;
			transform.position = prevPos;
			ic.selectChara[No-1]	=	selectNo;
			if(ic	!=	null){	
				if(!send)	ic.setNum	+= setNo;
				send	=	true;
			}
		}else{
			if(ic	!=	null){	
				if(send)	ic.setNum	-= setNo;
				send	=	false;
			}
			TrackingIcon();
		}
		Cancel();
		timer += Time.deltaTime;
	}

	void Cancel(){
		if (!putOn){
			selectNo				=	4;
			ic.selectChara[No-1]	=	selectNo;
		}
		if(!cancelFlg)	return;
		swicthFlg	+=	1;
		setFlg = false;
		cancelFlg = false;
	}

	void TrackingIcon(){
		float n = Mathf.Min(timer/maxTime,1.0f);
		CursorManager	parent	=	transform.parent.gameObject.GetComponent<CursorManager>();
		Vector3			target	=	parent.trans.position + DEFALUTPOS;
		Vector3			value	=	target * n + prevPos * (1 - n);
		trans.position	=	value;
		if(n == 1.0f)	decFlg	=	false;
	}
}
