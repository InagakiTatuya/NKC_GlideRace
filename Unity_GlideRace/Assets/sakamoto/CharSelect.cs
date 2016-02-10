using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour {

	private	Image			image;
	private	RectTransform	trans;

	private int selectNum;
	public	int	SelectNum
	{
		get { return selectNum; }
		set { selectNum = value; }
	}

	private static string	commonTagName = "PlayerIcon_P";
	private	static string[]	tagName	= null;
	private	void	createTagName(){
		if(tagName != null)	return;
		tagName	= new string[4];
		for(int i = 0;i < tagName.Length;i ++){
			tagName[i]	= commonTagName + (i + 1);
		}
	}

	void Start () {
		image		=	GetComponent<Image>();
		trans		=	image.rectTransform;
		createTagName();
	}
	
	void OnTriggerStay2D(Collider2D other){
		GameObject icon = other.gameObject;
		IconManager im = icon.GetComponent<IconManager>();
		if(im != null){
			for(int i=0;i<tagName.Length;i++){
				if (other.tag != tagName[i])	continue;
				im.selectNo			=	selectNum;
				im.putOn			=	true;
				break;
			}
		}
		else{
			NPCIcon	npcIcon	=	other.gameObject.GetComponent<NPCIcon>();
			if(npcIcon == null)	return;
			npcIcon.selectNo	=	selectNum;
			npcIcon.putOn		=	true;
		}
	}
	void OnTriggerExit2D(Collider2D other){
		GameObject obj = other.gameObject;
		IconManager im = obj.GetComponent<IconManager>();
		if(im != null){
			for (int i = 0; i < tagName.Length; i++){
				if (other.tag != tagName[i]) continue;
				im.putOn = false;
				break;
			}
		}
		else{
			NPCIcon	npcIcon	=	other.gameObject.GetComponent<NPCIcon>();
			if(npcIcon==null)	return;
			npcIcon.putOn	=	false;
		}
	}
}
