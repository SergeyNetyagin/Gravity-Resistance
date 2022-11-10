using UnityEngine;
using System.Collections;

public class WandererControl : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Need to make motion in only one screen's plane (z-position is constant while object is moving)" )]
    private bool is_flat_motion = true;

    [SerializeField]
    [Tooltip( "Need to make better meteoric rain effect" )]
    private bool create_in_frustum = true;

    [SerializeField]
    [Range( 5.0f, 60.0f)]
    private float 
        check_time = 10f;

    [SerializeField]
    [Range( 1f, 500f )]
    private float 
        min_activation_time = 50f,
        max_activation_time = 200f;

    [SerializeField]
    private Vector3
        min_activation_point = new Vector3( -1000f, -1000f, -100f ),
        max_activation_point = new Vector3(  1000f,  1000f, 1000f );

    private Wanderer[] wanderers;

    private float
        speed = 0f,
        acceleration = 0f;

    private Vector3
        position,
        direction,
        min_position,
        max_position;

    private Transform cached_transform;

    private WaitForSeconds check_wait_for_seconds;

	// Use this for initialization #############################################################################################################################################
	void Start() {

        if( min_activation_point.z == max_activation_point.z ) is_flat_motion = true;

        cached_transform = transform;
    
        wanderers = new Wanderer[ cached_transform.childCount ];

        for( int i = 0; i < wanderers.Length; i++ ) {

            wanderers[i] = cached_transform.GetChild( i ).GetComponent<Wanderer>() as Wanderer;
            wanderers[i].Activation_time = Random.Range( min_activation_time, max_activation_time );
        }

        if( min_activation_point.x < Game.Level.Activation_left_position ) min_activation_point.x = Game.Level.Activation_left_position;
        if( max_activation_point.x > Game.Level.Activation_right_position ) max_activation_point.x = Game.Level.Activation_right_position;
        if( min_activation_point.y < Game.Level.Activation_bottom_position ) min_activation_point.y = Game.Level.Activation_bottom_position;
        if( max_activation_point.y > Game.Level.Activation_top_position ) max_activation_point.y = Game.Level.Activation_top_position;
        if( !is_flat_motion ) min_activation_point.z = Game.Level.Activation_near_position;
        if( !is_flat_motion ) max_activation_point.z = Game.Level.Activation_far_position;

        check_wait_for_seconds = new WaitForSeconds( check_time );
        StartCoroutine( CheckAndActivationControl() );
	}

    // Relocate object #########################################################################################################################################################
    IEnumerator CheckAndActivationControl() {

        while( wanderers.Length > 0 ) {

            for( int i = 0; i < wanderers.Length; i++ ) {

                if( wanderers[i].Is_busy ) continue;

                if( wanderers[i].gameObject.activeInHierarchy ) {

                    if( is_flat_motion ) continue;
                    else if( Game.Camera_control.IsInClippingZone( wanderers[i].Cached_transform ) ) continue;
                    else wanderers[i].Sleep( false );
                }

                wanderers[i].Free_time += check_time;

                if( wanderers[i].Free_time >= wanderers[i].Activation_time ) {

                    if( is_flat_motion && Game.Camera_control.IsInFrustum( PreparePosition( wanderers[i] ) ) ) continue;
                    else wanderers[i].WakeUp( null, true );
                }
            }

            yield return check_wait_for_seconds;
        }

        yield break;
    }

    // Prepare new position for next activation ################################################################################################################################
    private Renderer PreparePosition( Wanderer wanderer ) {

        direction.x = wanderer.Animation_movement.Speed_on_x;
        direction.y = wanderer.Animation_movement.Speed_on_y;
        direction.z = wanderer.Animation_movement.Speed_on_z;

        // Positions <x> and <y> generates from edge of the level
        if( is_flat_motion ) {

            position.z = Random.Range( min_activation_point.z, max_activation_point.z );
            Game.Camera_control.MakeFrustumPosition( ref min_position, ref max_position, position.z );

            position.x = create_in_frustum ? 
                Random.Range( min_position.x, max_position.x ) : 
                ((direction.x > 0f) ? Random.Range( min_activation_point.x, min_position.x ) : Random.Range( max_position.x, max_activation_point.x ));

            position.y = create_in_frustum ? 
                Random.Range( min_position.y, max_position.y ) : 
                ((direction.y > 0f) ? Random.Range( min_activation_point.y, min_position.y ) : Random.Range( max_position.y, max_activation_point.y ));
        }

        // Positions <x> and <y> generates from edge of the clipping panels
        else {

            Game.Camera_control.MakeFrustumPosition( ref min_position, ref max_position, (direction.z > 0.0f) ? Game.Player_transform.position.z : max_activation_point.z );

            position.x = create_in_frustum ? 
                Random.Range( min_position.x, max_position.x ) : 
                ((direction.x > 0f) ? Random.Range( min_activation_point.x, min_position.x ) : Random.Range( max_position.x, max_activation_point.x ));

            position.y = create_in_frustum ? 
                Random.Range( min_position.y, max_position.y ) : 
                ((direction.y > 0f) ? Random.Range( min_activation_point.y, min_position.y ) : Random.Range( max_position.y, max_activation_point.y ));

            position.z = (direction.z > 0.0f) ? min_activation_point.z : max_activation_point.z;

            // Correction of Y-orbit
            acceleration = (direction.z > 0f) ? 7f : 1f;
            speed = direction.z * (Game.Camera_transform.position.y - position.y) / (Game.Camera_transform.position.z - max_activation_point.z) * acceleration;
            wanderer.Animation_movement.SetSpeedOnY( speed );

            // Correction of X-orbit
            speed = Mathf.Abs( direction.z ) * (Game.Camera_transform.position.x - position.x) / max_activation_point.z;
            wanderer.Animation_movement.SetSpeedOnX( speed );
        }

        // Set a new position
        wanderer.Cached_transform.position = position;

        return wanderer.Cached_renderer;
    }
}