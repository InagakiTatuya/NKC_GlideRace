using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSelectWindow : MonoBehaviour {
	
	public	enum STATUS{INIT,NORMAL,CLOSE,NONE};
	public	STATUS			state;
	public	int				No;
	public	bool			activFlg;
	private	Sprite			sprite;
	private	PlayerWindow	playerWindow;
	public	int				ID;
	public	Image			image;
	public	Image			childimage;
	private RectTransform	trans;
	private	Vector3			transSize;
	private	Vector2			MaxSize;
	private	float			timer;
	public	CursorManager	cursorObj;
	public	NPCButton		npcButton;
	
	void Start () {
		ID					=	5;
		playerWindow		=	transform.parent.GetComponent<PlayerWindow>();
		childimage			=	gameObject.transform.GetChild(0).GetComponent<Image>();
		trans				=	GetComponent<RectTransform>();
		MaxSize				=	trans.sizeDelta;
		trans.localScale	=	Vector3.zero;
		transSize			=	Vector3.zero;
		timer				=	0;
		state	=	(No	==	1)?STATUS.INIT:STATUS.NONE;
	}
	
	void Update () {
		switch(state){
			case STATUS.INIT:	init();		break;
			case STATUS.NORMAL:				break;
			case STATUS.CLOSE:	close();	break;
			case STATUS.NONE:				break;

		}
		SpriteImageInit();
	}

	void init(){
		timer += 0.1f;
		float t = timer;
		t = Mathf.Pow(t, 3);
		transSize.x = t;
		transSize.y = t;
		trans.localScale = transSize;
		if (timer < 1.0f)	return;
		timer = 1.0f;
		trans.localScale = Vector3.one;
		state	=	STATUS.NORMAL;
		if(No != 1)	npcButton.state	=	NPCButton.STATUS.NOTURN;
	}

	void normal(){
	}

	void close(){
		timer -= 0.1f;
		float t = timer;
		t = Mathf.Pow(t, 3);
		transSize.x = t;
		transSize.y = t;
		trans.localScale = transSize;
		activFlg	=	false;
		if(No != 1)	npcButton.state	=	NPCButton.STATUS.NONE;
		if (timer > 0.0f)	return;
		timer = 0.0f;
		trans.localScale = Vector3.zero;
		state		=	STATUS.NONE;
	}
	void SpriteImageInit(){
		if (ID > 4)	childimage.sprite = null;
		else{
			childimage.sprite = playerWindow.rankSprite[ID];
		}
	}

	void OnTriggerStay2D(Collider2D other){
		if(state != STATUS.NORMAL)	return;
		CursorManager	cm	=	other.gameObject.GetComponent<CursorManager>();
		if(cm == null)	return;
		if(!cm.GetAccel())	return;
		state	=	STATUS.CLOSE;
	}
}
