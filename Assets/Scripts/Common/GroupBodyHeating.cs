using UnityEngine;

public class GroupBodyHeating : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Материал для эффекта нагревания тела" )]
    private Material heating_material;

    private Transform cached_transform;

	// Use this for initialization
	void Start () {

        cached_transform = transform;
    }

    // Assign the correct bodies' values #########################################################################################################################################
    [ContextMenu( "CUSTOM: Assign correct collective values" )]
    private void AssignCorrectValues() {

        cached_transform = transform;

        for( int i = 0; i < cached_transform.childCount; i++ ) {

            SpaceBody space_body = cached_transform.GetChild( i ).GetComponent<SpaceBody>();

            if( space_body != null ) space_body.AssignMaterial( heating_material );
        }

        #if UNITY_EDITOR
        if( !Application.isPlaying ) Debug.Log( "The child objects' SpaceBodySingle of the <" + gameObject.name + "> is became new values" );
        #endif
    }
}
