using UnityEngine;
using System.Collections;

public class ToggleEffects : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGazeEnter()
    {
        SendMessageUpwards("ToggleSpecialEffects");
    }
}
