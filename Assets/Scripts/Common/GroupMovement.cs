using UnityEngine;

public class GroupMovement : MonoBehaviour {

    [SerializeField]
    private Vector3
        min_speed = new Vector3( 0.1f, 0.1f, 0.1f ),
        max_speed = new Vector3( 10.0f, 10.0f, 10.0f );

    private Transform cached_transform;

	// Use this for initialization #############################################################################################################################################
	void Start() {

        if( cached_transform == null ) AssignNewMovement();
    }

    // It need to outstrip AnimatonRotation ####################################################################################################################################
    void OnEnable() {

        if( cached_transform == null ) AssignNewMovement();
    }

    // Assing a new color for material #########################################################################################################################################
    [ContextMenu( "CUSTOM: Assign random movement values" )]
    void AssignNewMovement() {

        cached_transform = transform;

        if( max_speed.x < min_speed.x ) max_speed.x = min_speed.x;
        if( max_speed.y < min_speed.y ) max_speed.y = min_speed.y;
        if( max_speed.z < min_speed.z ) max_speed.z = min_speed.z;

        for( int i = 0; i < cached_transform.childCount; i++ ) {

            AnimationMovement animation_movement = cached_transform.GetChild( i ).GetComponent<AnimationMovement>();

            if( animation_movement != null ) {

                animation_movement.SetSpeedOnX( Random.Range( min_speed.x, max_speed.x ) );
                animation_movement.SetSpeedOnY( Random.Range( min_speed.y, max_speed.y ) );
                animation_movement.SetSpeedOnZ( Random.Range( min_speed.z, max_speed.z ) );
            }
        }

        #if UNITY_EDITOR
        if( !Application.isPlaying ) Debug.Log( "The child objects' movements of the <" + gameObject.name + "> is assigned" );
        #endif
	}
}
