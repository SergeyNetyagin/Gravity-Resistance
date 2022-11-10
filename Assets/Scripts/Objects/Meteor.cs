using UnityEngine;
using System.Collections;

public class Meteor : MonoBehaviour {

    [SerializeField]
    [Range( 0.1f, 10f )]
    private float hit_refresh_time = 10f;

    [SerializeField]
    [Range( 0f, 1f )]
    private float tail_refresh_time = 1f;

    [SerializeField]
    [Range( 0f, 10f )]
    private float movement_refresh_time = 0f;

    [SerializeField]
    private GameObject tail_prefab;
    private GameObject tail_effect;

    [SerializeField, HideInInspector]
    private Transform cached_transform;

    [SerializeField, HideInInspector]
    private DirectedParticlesMoving effect_component;

    [SerializeField, HideInInspector]
    private SpaceBody space_body;

    [SerializeField, HideInInspector]
    private Rigidbody physics;

    [SerializeField, HideInInspector]
    private WaitForSeconds hit_wait_for_seconds;

	// Use this for initialization #############################################################################################################################################
	void Awake() {

        if( tail_prefab == null ) return;

        cached_transform = transform;
        physics = GetComponent<Rigidbody>();
        space_body = GetComponent<SpaceBody>();

        tail_effect = Instantiate( tail_prefab, transform.position, Quaternion.identity ) as GameObject;
        tail_effect.name = gameObject.name + "_meteor_tail_effect";
        tail_effect.transform.parent = transform.parent;

        effect_component = tail_effect.GetComponent<DirectedParticlesMoving>();
        effect_component.SetTrackingTransform( cached_transform );
        effect_component.SetParentTransform( cached_transform );
        effect_component.SetParticlesTransform( tail_effect.transform );

        hit_wait_for_seconds = new WaitForSeconds( hit_refresh_time );
	}

    // Repeat meteor's activation ##############################################################################################################################################
    void OnEnable() {

        if( tail_effect != null ) tail_effect.SetActive( true );

        effect_component.EnableDirectionControl( tail_refresh_time, physics );
        effect_component.EnableMovementControl( movement_refresh_time );

        StartCoroutine( HitControl() );
    }

    // Prepare meteor for next activation ######################################################################################################################################
    void OnDisable() {

        effect_component.DisableDirectionControl();
        effect_component.DisableMovementControl();

        if( tail_effect != null ) tail_effect.SetActive( false );
    }

    // Hit to meteor for heating visual effect #################################################################################################################################
    IEnumerator HitControl() {

        while( !Game.Is( GameState.Complete ) ) {

            space_body.OnHitCollision( cached_transform.position, false );

            yield return hit_wait_for_seconds;
        }

        yield break;
    }
}
