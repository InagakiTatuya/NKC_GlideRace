using UnityEngine;
using System.Collections;

public class selectIcon : MonoBehaviour {
	
	public static int[]		s_selectNo  	=	new int[4];
	public static int[]		s_selectChara	=	new int[4];
	private	int				length		=	4;
	private	GameObject[]	obj;
	private	IconCount		icon;

	void Start () {
		int	objLength	=	transform.childCount;
		obj				=	new GameObject[length];
		for(int i=0;i<objLength;i++){
			if(icon != null)	break;
			icon	=	transform.GetChild(i).GetComponent<IconCount>();
		}
	}
	
	void Update () {
		for(int i = 0;i<length;i++){
			s_selectNo[i]		=	icon.selectNo[i];
			s_selectChara[i]	=	icon.selectChara[i];
		}
	}
}