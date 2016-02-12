using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPCWindow : MonoBehaviour {

	public	NPCButton	npcButton;
	public	int			selectNo;
	private	GameObject	childObj;
	private	Image		childImage;

	void Start () {
		npcButton	=	transform.parent.GetComponent<NPCButton>();
		selectNo	=	4;
		childObj	=	transform.GetChild(0).gameObject;
		childImage	=	childObj.GetComponent<Image>();
	}
	
	void Update () {
		childImage.sprite	=	npcButton.NPCwm.sprite[selectNo];
	}

	void OnTriggerStay2D(Collider2D other){
		CursorManager	cm	=	other.gameObject.GetComponent<CursorManager>();
		if(cm == null)	return;
		if(!cm.GetAccel())	return;
		Debug.Log(npcButton.state);
		npcButton.state	=	NPCButton.STATUS.CLOSE;
	}

}
