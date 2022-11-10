using UnityEngine;

public class Comet : MonoBehaviour {

    [SerializeField]
    [Range( 0f, 10f )]
    private float tail_refresh_time = 10f;

    [SerializeField]
    [Range( 0f, 10f )]
    private float movement_refresh_time = 0f;

    [SerializeField]
    private Transform tracking_transform;

    [SerializeField]
    private GameObject tail_prefab;
    private GameObject tail_effect;

    [SerializeField, HideInInspector]
    private DirectedParticlesVolume effect_component;

	// Use this for initialization #############################################################################################################################################
	void Start() {

        if( tail_prefab == null ) return;

        tail_effect = Instantiate( tail_prefab, transform.position, Quaternion.identity ) as GameObject;
        tail_effect.name = gameObject.name + "_comet_tail_effect";
        tail_effect.transform.parent = transform.parent;

        effect_component = tail_effect.GetComponent<DirectedParticlesVolume>();
        effect_component.SetTrackingTransform( tracking_transform );
        effect_component.SetParentTransform( transform );
        effect_component.SetParticlesTransform( tail_effect.transform );

        effect_component.EnableDirectionControl( tail_refresh_time );
        effect_component.EnableMovementControl( movement_refresh_time );
	}
}
