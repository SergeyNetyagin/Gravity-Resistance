using UnityEngine;

// Миссия - это то, что дано в задании; миссия может быть минералом или грузом, а также какой-то точкой в пространстве, которую нужно достичь
[RequireComponent( typeof( ObstacleControl ) )]
public class Mission : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Ключ названия типа данного объекта (например, <Миссия>); в базе локализаций пишется с большой буквы" )]
    private string kind_name_key;
    public string Kind_name_key { get { return kind_name_key; } }
    public string Kind_name { get { return Game.Localization.GetTextValue( kind_name_key ); } }

    [SerializeField]
    [Range( 0f, 1000000f )]
    [Tooltip( "Reward for success of this mission" )]
    private float reward = 1000f;
    public float Reward { get { return reward; } }

    [SerializeField]
    [Range( 0f, 1f )]
    [Tooltip( "Penalty if mission will be broken (percent from reward)" )]
    private float penalty = 0.5f;
    public float Penalty { get { return (reward * penalty); } }

    [SerializeField]
    private float altitude_over_station = 1f;
    public float Altitude_over_station { get { return altitude_over_station; } }

    [Header( "MESSAGES FOR TASKS; SUCCESS OR FAILURE OF MISSION" )]
    [SerializeField]
    private ComplexMessage source_message_key;
    public ComplexMessage Source_message_key { get { return source_message_key; } }

    [SerializeField]
    private ComplexMessage destination_message_key;
    public ComplexMessage Destination_message_key { get { return destination_message_key; } }

    [SerializeField]
    private ComplexMessage cancelled_message_key;
    public ComplexMessage Cancelled_message_key { get { return cancelled_message_key; } }

    [SerializeField]
    private ComplexMessage failed_message_key;
    public ComplexMessage Failed_message_key { get { return failed_message_key; } }

    [SerializeField]
    private ComplexMessage accomplished_message_key;
    public ComplexMessage Accomplished_message_key { get { return accomplished_message_key; } }

    [Header( "MISSION'S EFFECTS" )]
    [SerializeField]
    [Tooltip( "Материал для данного типа миссии" )]
    private Material mission_material;

    [SerializeField]
    [Tooltip( "Префаб для создания дополнительных эффектов вокруг объекта миссии: вспышки, свет и т.п." )]
    private GameObject effect_prefab;
    private GameObject additional_effect;

    [SerializeField]
    private Effect success_prefab;
    public Effect Success_prefab { get { return success_prefab; } }

    private Effect failure_prefab;
    public Effect Failure_prefab { get { return failure_prefab; } }

    private bool is_active = false;
    public bool Is_active { get { return is_active; } set { is_active = value; } }

    private bool was_dropped = false;
    public bool Was_dropped { get { return was_dropped; } set { was_dropped = value; } }

    private float
        sqr_magnitude_passed,
        sqr_magnitude_distance;

    private ObstacleControl obstacle_control;

    private Transform cached_transform;
    public Transform Cached_transform { get { return cached_transform; } }

    public float Sqr_magnitude_distance { get { return sqr_magnitude_distance; } }
    public float Sqr_magnitude_passed { get {

        Vector3 operation;

        //operation.x = cached_transform.position.x - source_transform.position.x;
        //operation.y = cached_transform.position.y - source_transform.position.y;
        //operation.z = cached_transform.position.z - source_transform.position.z;

        //sqr_magnitude_passed = operation.sqrMagnitude;

        return sqr_magnitude_passed;
    } }
        
    public void Sleep() { obstacle_control.Sleep(); }
    public void WakeUp() { obstacle_control.WakeUp(); }

    // Awake ###################################################################################################################################################################
    void Awake() {

        if( mission_material != null ) {

            MeshRenderer mesh = GetComponent<MeshRenderer>();
            if( mesh != null ) mesh.material = mission_material;
        }

        if( effect_prefab != null ) {

            additional_effect = Instantiate( effect_prefab, Vector3.zero, Quaternion.identity ) as GameObject;
            additional_effect.transform.parent = transform;
        }
    }
    
    // Starting initialization #################################################################################################################################################
    void Start() {

        cached_transform = transform;

        obstacle_control = GetComponent<ObstacleControl>();
    }
}