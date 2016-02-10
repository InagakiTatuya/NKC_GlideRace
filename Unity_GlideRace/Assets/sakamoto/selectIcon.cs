using UnityEngine;
using System.Collections;

public class selectIcon : MonoBehaviour {
	
	public	int[]			selectNo	=	new int[4];
	public	int[]			selectChara	=	new int[4];
	private	int				length		=	4;
	private	GameObject[]	obj;
	private	IconCount		icon;

	void Start () {
		int	objLength	=	transform.childCount;
		obj			=	new GameObject[length];
		for(int i=0;i<objLength;i++){
			if(icon != null)	break;
			icon	=	transform.GetChild(i).GetComponent<IconCount>();
		}
	}
	
	void Update () {
		for(int i = 0;i<length;i++){
			selectNo[i]		=	icon.selectNo[i];
			selectChara[i]	=	icon.selectChara[i];
		}
	}
}