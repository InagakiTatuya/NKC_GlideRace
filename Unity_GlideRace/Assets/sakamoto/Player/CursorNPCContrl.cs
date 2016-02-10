using UnityEngine;
using System.Collections;

public partial class CursorManager : MonoBehaviour {
	
	void OnTriggerStay2D(Collider2D other){
		if(controlFlg)				return;
		if(other.tag != "NPCIcon")	return;
		npcIcon	=	other.gameObject.GetComponent<NPCIcon>();
		if(npcIcon == null)			return;
		if(!input.accel)			return;
		controlFlg		=	true;
		npcIcon.PCursor	=	this;
		npcIcon.state	=	NPCIcon.STATUS.Control;
	}

	void Control(){
		if(!controlFlg)		return;
		if(npcIcon == null)	return;
		if(npcIcon.state != NPCIcon.STATUS.Decision)	return;
		controlFlg	=	false;
		npcIcon		=	null;
	}
}
