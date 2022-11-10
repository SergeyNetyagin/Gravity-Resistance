using UnityEngine;

public class ZoneControl : MonoBehaviour, IDetecting {

    [SerializeField]
    private ZoneType zone_type;
    public ZoneType Zone_type { get { return zone_type; } }

    [Header( "MESSAGES OF THIS ZONE" )]
    [SerializeField]
    private ComplexMessage contact_message_key;

    [SerializeField]
    private ComplexMessage detection_message_key;

    [Header( "ПОВРЕЖДЕНИЯ КОРАБЛЯ В ЭТОЙ ЗОНЕ" )]
    [SerializeField]
    private Damage[] damages;
    public Damage[] Damages { get { return damages; } }
        
    [Header( "ЭФФЕКТЫ ГРАВИТАЦИИ, ВОЗНИКАЮЩИЕ В ЭТОЙ ЗОНЕ" )]
    [SerializeField]
    [Range( 0f, 10f )]
    private float drag = 1f;

    [SerializeField]
    private Vector3 gravity = new Vector3( 0f, -1f, 0f );

    [SerializeField]
    [Space( 10 )]
    private Effect enter_prefab;
    public Effect Enter_prefab { get { return enter_prefab; } }

    [SerializeField]
    private Effect exit_prefab;
    public Effect Exit_prefab { get { return exit_prefab; } }

    private RendererControl renderer_control;

    private Transform cached_transform;
    public Transform Cached_transform { get { return cached_transform; } }

    // Реализация интерфейса для IDetecting ####################################################################################################################################

    private MeshRenderer mesh_renderer;

    private int layer_original;
    public int Layer_original { get { return layer_original; } }

    private float radius = 0.11f;
    public float Radius { get { return radius; } }

    private float diameter = 0.2f;
    public float Diameter { get { return diameter; } }

    public MineralType Mineral_type { get { return MineralType.Unknown; } }
    public ObstacleType Obstacle_type { get { return ObstacleType.Unknown; } }

    public bool Is_visible { get { return renderer_control.Is_visible; } }

    public bool Is_zone { get { return (zone_type != ZoneType.Jet); } }
    public bool Is_mineral { get { return false; } }
    public bool Is_freight { get { return false; } }
    public bool Is_obstacle { get { return false; } }
    public bool Is_station { get { return false; } }
    public bool Is_mission { get { return false; } }

    public float Magnitude { get; set; }
    public float Sqr_magnitude { get; set; }
    public Vector3 Detected_point { get; set; }

    public void CalculateDimensions() {

        mesh_renderer = GetComponent<MeshRenderer>();

        if( mesh_renderer != null ) radius = (mesh_renderer.bounds.extents.x + mesh_renderer.bounds.extents.y) * 0.5f;
        diameter = radius * 2f;
    }

    // Starting initialization #################################################################################################################################################
    void Awake() {

        layer_original = gameObject.layer;

        #if UNITY_EDITOR
        if( GetComponent<ObstacleControl>() != null ) Debug.Log( "Объект " + gameObject.name + " одновременно имеет взаимоисключающие компоненты: ZoneControl и ObstacleControl" );
        #endif

        CalculateDimensions();
    }
    
    // Starting initialization #################################################################################################################################################
    void Start() {

        cached_transform = transform;
        renderer_control = GetComponent<RendererControl>();
    }

    // Collision analysis ######################################################################################################################################################
    void OnTriggerEnter( Collider collider ) {

        if( collider.CompareTag( "Radar" ) ) return;
        if( !gameObject.activeInHierarchy || !collider.gameObject.activeInHierarchy ) return;

        switch( zone_type ) {
                
            // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
 		    case ZoneType.Black_hole:

                break;

            // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
            default:

                Game.Message.Show( contact_message_key );
                Game.Camera_control.StartShaking( ShakeType.Engine );
                Game.Level.SetSpecialGravity( ref gravity, drag );

                Game.Player.StartContinuousDamages( this );

                break;
        }
    }

    // Collision analysis ######################################################################################################################################################
    void OnTriggerExit( Collider collider ) {

        if( !collider.CompareTag( "Ship" ) ) return;

        switch( zone_type ) {

            // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
 		    case ZoneType.Black_hole:

                break;

            // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
            default:

                Game.Message.Hide( contact_message_key );
                Game.Level.SetNormalGravity();
                Game.Camera_control.StopShaking();

                Game.Player.StopContinuousDamages( this );

                break;
        }
    }

    // Makes the visual effect #################################################################################################################################################
    public void ShowEffect( GameObject effect_prefab, Vector3 effect_point, float hide_time, bool is_sound ) {

        if( effect_prefab == null ) return;

        effect_prefab.GetComponent<AudioSource>().playOnAwake = is_sound;
        GameObject effect = Instantiate( effect_prefab, effect_point, Quaternion.identity ) as GameObject;
        Destroy( effect, hide_time );
    }

    // Temporary stop of the damage ############################################################################################################################################
    public void PauseDamage() {

        Game.Message.Hide( contact_message_key );
        Game.Camera_control.StopShaking();
    }

    // Resume of the damage ####################################################################################################################################################
    public void ResumeDamage() {

        Game.Message.Show( contact_message_key );
        Game.Camera_control.StartShaking( ShakeType.Engine );
    }
}