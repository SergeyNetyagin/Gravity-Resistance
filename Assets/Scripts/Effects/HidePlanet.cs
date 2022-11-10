using UnityEngine;
using System.Collections;

public class HidePlanet : MonoBehaviour {

    [SerializeField]
    private float 
        min_hide_angle = 90.0f,
        max_hide_angle = 270.0f;

    private MeshRenderer mesh_renderer;

    private Transform cached_transform;

    private WaitForSeconds hide_satellite_wait_for_seconds = new WaitForSeconds( 0.5f );

	// Use this for initialization #############################################################################################################################################
	void Start () {

	    mesh_renderer = transform.GetChild( 0 ).GetComponent<MeshRenderer>() as MeshRenderer;
        
        cached_transform = gameObject.transform;

        StartCoroutine( CheckHidePlanet() );
	}

    // Do not show the satellite in front of the player ########################################################################################################################
    IEnumerator CheckHidePlanet() {

        while( mesh_renderer != null ) {

            if( (cached_transform.rotation.eulerAngles.y > min_hide_angle) && (cached_transform.rotation.eulerAngles.y < max_hide_angle) ) {

                if( mesh_renderer.enabled ) mesh_renderer.enabled = false;
            }
            
            else {

                if( !mesh_renderer.enabled ) mesh_renderer.enabled = true;
            }

            yield return hide_satellite_wait_for_seconds;
        }

        yield break;
    }
}
