using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IconManager : MonoBehaviour {

	readonly Vector3 DEFALUTPOS = new Vector3(16.4f, 14.3f, 0.0f);
	readonly float speed = 15.0f;
	public		int swicthFlg;
	public		float rot = 5.0f;
	public		bool activFlg;
	public		bool cancelFlg;
	public		bool setFlg;
	private		bool trackingFlg;
	private		Image	image;
	private		RectTransform trans;
	private		float	timer;
	private	const	float	maxTime	=	0.25f;
	public		bool	decFlg;
	private Vector3 prevPos;
	public int selectNo;

	void Start(){
		selectNo	=	0;
		swicthFlg	=	0;
		timer		=	maxTime;
		decFlg		=	false;
		activFlg	=	true;
		cancelFlg	=	false;
		setFlg		=	false;
		prevPos		=	transform.position;
		image		=	gameObject.GetComponent<Image>();
		trans		=	image.rectTransform;
	}
	
	void Update () {
		if (swicthFlg%2 != 0){
			if (!setFlg) prevPos = trans.position;
		    setFlg	=	true;
			decFlg	=	true;
			timer	=	0.0f;
			transform.position = prevPos;
		}else{
			TrackingIcon();
		}
		Cancel();
		timer += Time.deltaTime;
	}

	void Cancel(){
		if(!cancelFlg)	return;
		swicthFlg	+=	1;
		setFlg		=	false;
		cancelFlg	=	false;
		trackingFlg	=	true;
	}

	void TrackingIcon(){
		selectNo	=	5;
		float n = Mathf.Min(timer/maxTime,1.0f);
		CursorManager	parent	=	transform.parent.gameObject.GetComponent<CursorManager>();
		Vector3 target	=	parent.trans.position + DEFALUTPOS;
		Vector3	value	=	target * n + prevPos * (1 - n);
		trans.position	=	value;
		if(n == 1.0f)	decFlg	=	false;
	}
}
