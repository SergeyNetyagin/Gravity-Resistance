using UnityEngine;

public class GroupColorMaterial : MonoBehaviour {

    [SerializeField]
    private Color albedo_color = Color.clear;

	// Use this for initialization #############################################################################################################################################
	void Start() {

        #if UNITY_EDITOR
        AssignNewMaterial();
        #endif
    }

    // Assing a new color for material #########################################################################################################################################
    [ContextMenu( "CUSTOM: Assign materials' color" )]
    private void AssignNewMaterial() {

        if( albedo_color == Color.clear ) return;

        for( int i = 0; i < transform.childCount; i++ ) {

            if( transform.GetChild( i ).GetComponent<MeshRenderer>() == null ) continue;

            Material body_material = transform.GetChild( i ).GetComponent<MeshRenderer>().material;
            if( body_material != null ) body_material.color = albedo_color;
        }

        #if UNITY_EDITOR
        if( !Application.isPlaying ) Debug.Log( "The child objects' color of materials of the <" + gameObject.name + "> is assigned" );
        #endif
    }
}
