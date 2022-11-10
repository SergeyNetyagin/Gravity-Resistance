using UnityEngine;

public class StationLight : MonoBehaviour {

	// Use this for initialization #############################################################################################################################################
	void Start() {
	
        GetComponent<Light>().color = GetComponentInParent<Station>().Color;
	}
}
