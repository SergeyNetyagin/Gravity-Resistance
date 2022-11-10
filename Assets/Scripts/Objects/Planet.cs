using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {

    [System.Serializable]
    private class PlanetRotationControl {

        [HideInInspector]
        public Transform transform;
            
        [HideInInspector]
        public Quaternion 
            start_rotation,
            current_rotation;
    }

    [SerializeField]
    private Transform central_planet_transform;

    private PlanetRotationControl
        mesh,
        planet,
        atmosphere;

	// Use this for initialization #############################################################################################################################################
	void Start () {
	
        //planet_transform = GetComponent<Transform>() as Transform;
        //mesh_transform = planet_transform.GetChild( 0 ).GetComponent<Transform>() as Transform;
        //atmosphere_transform = (mesh_transform.childCount > 0) ? mesh_transform.GetChild( 0 ).GetComponent<Transform>() : null;

        //start_rotation = cached_transform.localRotation;
		//current_rotation = new Quaternion( 0.0f, 0.0f, 0.0f, 1.0f );

	}
}
