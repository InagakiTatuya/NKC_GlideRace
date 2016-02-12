using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Noize : MonoBehaviour {

	private Color c;
	private Material m;
	public float timer;
	private float alpha;
	private	Vector2	offset;

	void Start () {
		offset	=	Vector2.zero;
		m = GetComponent<Image>().material;
		if(m == null){
			this.enabled = false;
			return;
		}
		CalcAlpha();
		timer=0;
		//x方向速度、y方向速度、アニメーションループフラグ(0以外でON)、未使用
		c = new Color(0.0f,0.2f,1.0f,0.0f);
		//テクチャアニメーションパラメータ送信
		m.SetColor("_TransData",c);
		//合成アルファ値
		m.SetFloat("_Alpha",alpha);
	}
	
	void Update () {
		if(m == null){
			this.enabled = false;
			return;
		}
		CalcAlpha();
		c.b = timer;
		
		offset.y	= timer / 10.0f;
		m.SetTextureOffset("_BrendTex",offset);

		m.SetColor("_TransData",c);
		m.SetFloat("_Alpha",alpha);
		timer+=Time.deltaTime;
	}

	//1/fゆらぎを表現するalpha値算出用
	void CalcAlpha()
    {
        float r = Random.Range (0.0f, 1.0f);
 
        if(r <= 0.01f)		alpha = r + 0.02f;
        else if(r < 0.5f)	alpha = r + 2.0f * r * r;
        else if(r >= 0.99f)	alpha = r - 0.01f;
        else				alpha = r - 2.0f * (1.0f - r) * (1.0f - r);
 
        alpha = Mathf.Max(alpha, 0.075f);
        alpha = Mathf.Min(alpha, 0.375f);
    }
}
