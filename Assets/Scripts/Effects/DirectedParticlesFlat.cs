using UnityEngine;
using System.Collections;

public class DirectedParticlesFlat : MonoBehaviour {

    [SerializeField]
    private Transform tracking_object_transform;

    [SerializeField]
    private Vector3 max_speed = new Vector3( 50.0f, 0.0f, 0.0f );

    [SerializeField]
    [Range( 1f, 100f )]
    private float check_time = 30f;

    private WaitForSeconds speed_wait_for_seconds;

    private ParticleSystem particle_system;
    private ParticleSystem.VelocityOverLifetimeModule velocity_module;

    private ParticleSystem.MinMaxCurve 
        curve_x = new ParticleSystem.MinMaxCurve(),
        curve_y = new ParticleSystem.MinMaxCurve(),
        curve_z = new ParticleSystem.MinMaxCurve();

    private Transform cached_transform;

	// Use this for initialization #############################################################################################################################################
	void Start () {

        cached_transform = transform;
	
        particle_system = GetComponent<ParticleSystem>();

        velocity_module = particle_system.velocityOverLifetime;
        velocity_module.enabled = true;

        curve_x.mode = ParticleSystemCurveMode.Constant;
        curve_x.constantMin = max_speed.x;
        curve_x.constantMax = max_speed.x;

        curve_y.mode = ParticleSystemCurveMode.Constant;
        curve_y.constantMin = max_speed.y;
        curve_y.constantMax = max_speed.y;

        curve_z.mode = ParticleSystemCurveMode.Constant;
        curve_z.constantMin = max_speed.z;
        curve_z.constantMax = max_speed.z;

        velocity_module.x = curve_x;
        velocity_module.y = curve_y;
        velocity_module.z = curve_z;

        speed_wait_for_seconds = new WaitForSeconds( check_time );
        StartCoroutine( OrientationControl() );
	}

	// Change speed of particles depending on tracking object (Sun) ############################################################################################################
	IEnumerator OrientationControl() {

        while( !Game.Is( GameState.Complete ) ) {
	
            if( max_speed.x != 0f ) {

                curve_x.constantMin = - Mathf.Sin( Mathf.Deg2Rad * tracking_object_transform.eulerAngles.y ) * max_speed.x;
                curve_x.constantMax = curve_x.constantMin;
                velocity_module.x = curve_x;
            }

            if( max_speed.y != 0f ) {

                curve_y.constantMin = - Mathf.Sin( Mathf.Deg2Rad * tracking_object_transform.eulerAngles.x ) * max_speed.y;
                curve_y.constantMax = curve_y.constantMin;
                velocity_module.y = curve_y;
            }

            if( max_speed.z != 0f ) {

                curve_z.constantMin = - Mathf.Sin( Mathf.Deg2Rad * tracking_object_transform.eulerAngles.z ) * max_speed.z;
                curve_z.constantMax = curve_z.constantMin;
                velocity_module.z = curve_z;
            }

            yield return speed_wait_for_seconds;
        }

        yield break;
    }
}
