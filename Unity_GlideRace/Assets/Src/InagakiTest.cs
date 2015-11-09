using UnityEngine;
using System.Collections;

public class InagakiTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        InputData data = null;
        InputPad.InputData(ref data, 1);
        Debug.Log("1"+data.axis);

        InputPad.InputData(ref data, 2);
        Debug.Log("2"+data.axis);
	    

    }
}
