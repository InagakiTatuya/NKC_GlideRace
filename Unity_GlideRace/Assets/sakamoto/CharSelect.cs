using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour {

	private	Image			image;
	private	RectTransform	trans;
	private	bool	putOn;
	public	static	bool	fallOnFlg;

	private int selectNum;
	public	int	SelectNum
	{
		get { return selectNum; }
		set { selectNum = value; }
	}

	private	GameObject	icon;

	void Start () {
		image		=	GetComponent<Image>();
		trans		=	image.rectTransform;
		putOn		=	false;
		fallOnFlg	=	false;
		icon = null;
	}
	
	void Update () {
		fallOnFlg	=	false;
	}

	void OnTriggerStay2D(Collider2D other){
		if (other.tag == "PlayerIcon"){

			icon = other.gameObject;
			IconManager obj = icon.GetComponent<IconManager>();
			obj.selectNo = selectNum;
			fallOnFlg = true;
		}
	}
}
