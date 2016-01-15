using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour {

	private	InputData	input;
	private	float		speed = 4;
	private	float		dis;
	public	IconManager icon;
	public	GameObject	IgameObject;
	private	Image		image;
	public	RectTransform	trans;
	public	Vector2			pivot;

	void Awake(){
		icon = IgameObject.GetComponent<IconManager>();
		image	=	GetComponent<Image>();
		trans	=	image.rectTransform;
	}
	void Start(){
		input = new InputData();
		InputPad.InputData(ref input);
	}
	void Update(){
		trans.position += (Vector3)input.axis * speed;

		AccelCheck();
		if (input.brake){
			if(icon.swicthFlg%2 != 0)icon.cancelFlg = true;
		}
		InputPad.InputDownData(ref input);
		input.axis = InputPad.Axis();
		PostionLimit();

	}
	void AccelCheck(){
		if(!input.accel)			return;
		if(!CharSelect.fallOnFlg)	return;
		if( icon.decFlg)			return;
		if(!icon.setFlg) icon.swicthFlg += 1;
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
}
