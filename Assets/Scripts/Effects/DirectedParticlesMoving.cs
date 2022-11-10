using UnityEngine;
using System.Collections;

public class DirectedParticlesMoving : MonoBehaviour {

    [SerializeField]
    [Range( 0f, 10f )]
    private float direction_refresh_time = 10f;

    [SerializeField]
    [Range( 0f, 10f )]
    private float movement_refresh_time = 0f;

    private Transform tracking_transform;
    public void SetTrackingTransform( Transform tracking_transform ) { this.tracking_transform = tracking_transform; }

    private Transform parent_transform;
    public void SetParentTransform( Transform parent_transform ) { this.parent_transform = parent_transform; }

    private Transform particles_transform;
    public void SetParticlesTransform( Transform particles_transform ) { this.particles_transform = particles_transform; }

    private Transform cached_transform;

    private Rigidbody physics;

    private WaitForSeconds
        direction_wait_for_seconds,
        movement_wait_for_seconds;

    private bool 
        enable_direction = false,
        enable_movement = false;

    public void EnableDirectionControl( float refresh_time, Rigidbody body ) { physics = body; enable_direction = true; direction_refresh_time = refresh_time; StartCoroutine( DirectionControl() ); }
    public void EnableMovementControl( float refresh_time ) { enable_movement = true; movement_refresh_time = refresh_time; StartCoroutine( MovementControl() ); }
    public void DisableDirectionControl() { enable_direction = false; }
    public void DisableMovementControl() { enable_movement = false; }
    
    // Use this for initialization #############################################################################################################################################
	void Start () {

        cached_transform = transform;
	}

	// Change speed of particles depending on tracking object (Sun) ############################################################################################################
	IEnumerator DirectionControl() {

        cached_transform = transform;

        direction_wait_for_seconds = new WaitForSeconds( direction_refresh_time );

        while( enable_direction ) {
	
            cached_transform.LookAt( physics.velocity );

            yield return direction_wait_for_seconds;
        }

        yield break;
    }

	// Change speed of particles depending on tracking object (Sun) ############################################################################################################
	IEnumerator MovementControl() {

        cached_transform = transform;

        movement_wait_for_seconds = new WaitForSeconds( movement_refresh_time );

        while( enable_movement ) {
	
            cached_transform.position = parent_transform.position;

            yield return movement_wait_for_seconds;
        }

        yield break;
    }
}
