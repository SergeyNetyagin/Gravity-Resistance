using UnityEngine;

public class AutoMass : MonoBehaviour {

    [SerializeField]
    [Tooltip( "If to use for calculating of mass of the object also his child colliders" )]
    private bool with_child_colliders = false;

    [SerializeField]
    [Range( 0.01f, 100000.0f)]
    [Tooltip( "Rate of mass that multiplied to calculated mass: usualy is 1.0f" )]
    private float mass_rate = 1f;

    [SerializeField]
    [Tooltip( "If need to variate of standard mass calculation" )]
    [Range( 0.0f, 1.0f )]
    private float random_deviation = 0.1f;

    private float
        mass = 0f,
        scale = 1f;

    private Rigidbody physics;

    private BoxCollider box_collider;
    private MeshCollider mesh_collider;
    private SphereCollider sphere_collider;
    private CapsuleCollider capsule_collider;

    private Transform cached_transform;

    public float Total_mass { get { return physics.mass; } }

	// Use this for initialization #############################################################################################################################################
	void Start() {

        CalculateMass();
    }

    // Recalculate mass of this object #########################################################################################################################################
    [ContextMenu( "CUSTOM: Calculate mass of the object" )]
    public void CalculateMass() {

        cached_transform = transform;

        mass = 0f;
        scale = (cached_transform.localScale.x + cached_transform.localScale.y + cached_transform.localScale.z) / 3.0f;
                            
        physics = GetComponent<Rigidbody>();

        // If need to calculate mass by this collider
        SumRigidbodyMass( cached_transform );

        // If need to calculate mass child colliders also
        if( with_child_colliders ) for( int i = 0; i < cached_transform.childCount; i++ ) SumRigidbodyMass( cached_transform.GetChild( i ) );
 
        physics.mass = mass;
        if( random_deviation != 0f ) physics.mass *= Random.Range( 1.0f - random_deviation, 1.0f + random_deviation );

        if( GetComponent<Value>() != null ) GetComponent<Value>().FullMassAndCostCalculation();
    }

    // Add mass of collider to the total mass of the object ####################################################################################################################
    void SumRigidbodyMass( Transform object_transform ) {

        box_collider = object_transform.GetComponent<BoxCollider>();
        mesh_collider = object_transform.GetComponent<MeshCollider>();
        sphere_collider = object_transform.GetComponent<SphereCollider>();
        capsule_collider = object_transform.GetComponent<CapsuleCollider>();

        if( (box_collider != null) && !box_collider.isTrigger ) mass += 
            box_collider.size.x * scale *
            box_collider.size.y * scale *
            box_collider.size.z * scale * 
            mass_rate;

        else if( (mesh_collider != null) && !mesh_collider.isTrigger ) mass += 
            mesh_collider.bounds.extents.x * scale * 
            mesh_collider.bounds.extents.y * scale * 
            mesh_collider.bounds.extents.z * scale * 
            mass_rate;

        else if( (capsule_collider != null) && !capsule_collider.isTrigger ) mass += 
            capsule_collider.height * scale * 
            capsule_collider.radius * 4f * scale *
            mass_rate;

        else if( (sphere_collider != null) && !sphere_collider.isTrigger ) mass += 
            sphere_collider.radius * 4.5f * scale *
            mass_rate;
    }
}
