using UnityEngine;

public class GroupRotation : MonoBehaviour {

    [SerializeField]
    private Vector3
        min_speed = new Vector3( -1f, -1f, -1f ),
        max_speed = new Vector3( 1f, 1f, 1f );

    private Transform cached_transform;

	// Use this for initialization #############################################################################################################################################
	void Start() {

        if( cached_transform == null ) AssignNewRotation();
    }

    // It need to outstrip AnimatonRotation ####################################################################################################################################
    void OnEnable() {

        if( cached_transform == null ) AssignNewRotation();
    }

    // Assing a new color for material #########################################################################################################################################
    [ContextMenu( "CUSTOM: Assign random rotation values" )]
    void AssignNewRotation() {

        cached_transform = transform;

        if( max_speed.x < min_speed.x ) max_speed.x = min_speed.x;
        if( max_speed.y < min_speed.y ) max_speed.y = min_speed.y;
        if( max_speed.z < min_speed.z ) max_speed.z = min_speed.z;

        for( int i = 0; i < cached_transform.childCount; i++ ) {

            AnimationRotation animation_rotation = cached_transform.GetChild( i ).GetComponent<AnimationRotation>();

            if( animation_rotation != null ) {

                animation_rotation.SetSpeedOnX( Random.Range( min_speed.x, max_speed.x ) );
                animation_rotation.SetSpeedOnY( Random.Range( min_speed.y, max_speed.y ) );
                animation_rotation.SetSpeedOnZ( Random.Range( min_speed.z, max_speed.z ) );
            }
        }

        #if UNITY_EDITOR
        if( !Application.isPlaying ) Debug.Log( "The child objects' rotaions of the <" + gameObject.name + "> is assigned" );
        #endif
    }
}
