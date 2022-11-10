using UnityEngine;
using System.Collections;

public class Satellite : MonoBehaviour {

    [SerializeField]
    private Transform around_transform;

    [Header( "ROTATION SETTINGS" )]
    [SerializeField]
    [Range( 0f, 10f )]
    private float refresh_time = 0f;

    [SerializeField]
    private AnimationCurve
        rotate_on_x,
        rotate_on_y,
        rotate_on_z;

    [SerializeField, HideInInspector]
    private Transform 
        cached_transform,
        parent_transform,
        satellite_transform;

    [SerializeField, HideInInspector]
    private GameObject satellite;

    [SerializeField, HideInInspector]
    private AnimationRotation animation_rotation;

    [SerializeField, HideInInspector]
    private WaitForSeconds follow_wait_for_seconds;

    // Start ###################################################################################################################################################################
    void Awake() {
#if UNITY_EDITOR
Debug.Log( "Здесь неправильно работает функция вращения спутника !!!" );
#endif
        satellite = new GameObject( transform.name + "_satellite" );

        cached_transform = transform;
        parent_transform = cached_transform.parent;
        satellite_transform = satellite.transform;

        satellite_transform.position = around_transform.position;
        satellite_transform.parent = around_transform.parent;
        cached_transform.parent = satellite_transform;

        cached_transform.GetComponent<Rigidbody>().isKinematic = true;

        animation_rotation = satellite.AddComponent<AnimationRotation>();
        //animation_rotation.SetSpeedOnX( rotate_on_x );
        //animation_rotation.Rotate_on_y.SetCurve( rotate_on_y );
        //animation_rotation.Rotate_on_z.SetCurve( rotate_on_z );
        animation_rotation.SetTransformedRefreshTime( refresh_time );
    }

    // Repeat satellite's activation ###########################################################################################################################################
    void OnEnable() {

        animation_rotation.enabled = true;

        follow_wait_for_seconds = new WaitForSeconds( refresh_time );
        StartCoroutine( FollowParent() );
    }

    // Prepare for repeating using of satellite ################################################################################################################################
    void OnDisable() {

        animation_rotation.enabled = false;
    }

    // Foolow the satellite object with parental object ########################################################################################################################
    IEnumerator FollowParent() {

        while( !Game.Is( GameState.Complete ) ) {

            satellite_transform.position = around_transform.position;

            yield return follow_wait_for_seconds;
        }

        yield break;
    }
}