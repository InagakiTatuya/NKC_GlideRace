using UnityEngine;
using System.Collections;

public class InagakiTest : MonoBehaviour {

    [SerializeField]
    private float scale = 1f;

	// Use this for initialization
	void Start () {
	    Vector2 scl = new Vector2(transform.localScale.x, transform.localScale.z) * scale;
        GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", scl);
	}
	
	// Update is called once per frame
	void Update () {


    }
}
