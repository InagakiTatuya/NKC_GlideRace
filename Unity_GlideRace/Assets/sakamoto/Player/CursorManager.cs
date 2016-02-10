using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public partial class CursorManager : MonoBehaviour {

	private	readonly	Vector3[]	DEFALTPOS	=	new Vector3[4]{
		new Vector3(-45.3f,-7.5f,1.0f),
		new Vector3(-15.7f,-3.0f,1.0f),
		new Vector3(12.8f,1.5f,1.0f),
		new Vector3(41.0f,5.2f,1.0f)};
	private	InputData				input;
	private	float					speed	=	6;
	private	IconManager				icon;
	public	GameObject				obj;
	public	RectTransform			trans;
	private	Image					image;
	private PlayerSelectWindow		window;
	public	int						PlayerNum;
	private	NPCIcon					npcIcon;
	private	bool					controlFlg;

	void Awake(){
		icon			=	transform.GetChild(0).GetComponent<IconManager>();
		icon.No			=	PlayerNum;
		image			=	GetComponent<Image>();
		trans			=	image.rectTransform;
		controlFlg		=	false;
	}

	void Start(){
		window			=	obj.GetComponent<PlayerSelectWindow>();
		trans.localPosition	=	DEFALTPOS[PlayerNum-1];
		input = new InputData();
		InputPad.InputData(ref input, PlayerNum);
	}

	void Update(){
		trans.position += (Vector3)input.axis * speed;
		
		AccelCheck();
		BreakeCheck();
		Control();
		DestroyIcon();
		InputPad.InputDownData(ref input, PlayerNum);
		input.axis		=	InputPad.Axis(PlayerNum);
		window.ID = icon.selectNo;
		PostionLimit();
	}

	void AccelCheck(){
		if(controlFlg)		return;
		if(!input.accel)	return;
		if(!icon.putOn)		return;
		if( icon.decFlg)	return;
		if(!icon.setFlg)	icon.swicthFlg += 1;
	}
	
	void BreakeCheck(){
		if(controlFlg)				return;
		if(!input.brake)			return;
		if(icon.swicthFlg % 2 != 0)	icon.cancelFlg = true;
	}

	void DestroyIcon(){
		if(window.state	!= PlayerSelectWindow.STATUS.CLOSE)	return;
		Destroy(gameObject);
	}
	void PostionLimit(){
		Vector3 pos = trans.position;
		if(pos.x >= Screen.width){
			pos.x = Screen.width;
		}
		else if(pos.x <= 0){
			pos.x	=	0;
		}
		if(pos.y >= Screen.height){
			pos.y = Screen.height;
		}else if(pos.y <= 0){
			pos.y = 0;
		}
		transform.position = pos;
	}

	public bool GetAccel(){
		if(input.accel)	return	true;
		return false;
	}
}