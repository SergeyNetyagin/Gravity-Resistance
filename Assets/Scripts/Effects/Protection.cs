using UnityEngine;
using System.Collections;

public class Protection : MonoBehaviour {

    static int radar_layer = 0;

    [SerializeField]
    [Tooltip( "Работает ли (видима ли) защита постоянно (иначе становится видимой только при приближении опасного тела)" )]
    private bool constant_protection = false;

    [SerializeField]
    [Tooltip( "Реагирует ли защитное поле на подлёт корабля (полезно использовать, например, для станции если не нужно показывать поле при посадке корабля)" )]
    private bool reaction_to_ship = false;

    [SerializeField]
    [Tooltip( "Материал для динамического эффекта. Поле имеет две оболочки: 1) статический эффект, не меняющий прозрачность, и 2) динамический эффект, меняющий прозрачость" )]
    private Material dynamic_material;

    [SerializeField]
    [Tooltip( "Материал для статического эффекта. Поле имеет две оболочки: 1) статический эффект, не меняющий прозрачность, и 2) динамический эффект, меняющий прозрачость" )]
    private Material static_material;

    [Space( 10 )]
    [Tooltip( "Скорость реакции эффекта на взаимодействие со сторонним объектом" )]
    [SerializeField]
    [Range( 0f, 1f )]
    private float reaction_speed = 0.1f;

    [Tooltip( "Скорость затухания эффекта" )]
    [SerializeField]
    [Range( 0f, 2f )]
    private float decay_speed = 2.0f;

    [SerializeField]
    [Range( 0f, 1f )]
    [Tooltip( "Степень прозрачности поля за пределами точки столкновения с объектом; по умолчанию = 0.05" )]
    private float alpha_rate = 0.01f;

    [SerializeField]
    [Range( 0f, 1f )]
    [Tooltip( "Яркость вспышки поля в точке столкновения с объектом; по умолчанию = 0.5" )]
    private float hit_power = 0.5f;

    private Vector3 contact_point = Vector3.zero;

    private Transform cached_transform;

    private Ship ship;

    private SoundEffects sound_effects;
    private ObstacleControl obstacle;

    private bool is_station = false;

    private bool auto_protection = true;
    public void EnableAutoProtection() { auto_protection = true; }
    public void DisableAutoProtection() { auto_protection = false; }

    private GameObject dynamic_field;
    private GameObject static_field;
    
    #if UNTIY_STANDALONE
    private ForceFieldCustomized force_field;
    #else
    private ForceFieldCustomizedMobile force_field;
    #endif
    
    // Awake ###################################################################################################################################################################
    void Awake() {

        if( radar_layer == 0 ) radar_layer = LayerMask.NameToLayer( "Radar" );

        // На случай, если не нужно, чтобы у объекта создавалось защитное поле (тогда скрипт отключается извне, но Awake() всё равно вызывается для неактивных компонентов)
        if( !enabled ) return;

        cached_transform = transform;

        is_station = GetComponentInParent<Station>() != null;

        auto_protection = is_station ? true : false;

        // Сперва создать дочерний объект для статического эффекта
        CreateStaticEffect();
        
        // Затем создать дочерний объект для динамического эффекта
        CreateDynamicEffect();
    }

    // Start ###################################################################################################################################################################
    void Start() {

        sound_effects = GetComponentInParent<SoundEffects>();
    }

    // Initialise components ###################################################################################################################################################
    void OnEnable() {

        if( constant_protection ) Enable();
    }
    
    // Off the protection shield ###############################################################################################################################################
    void OnDisable() {

        Disable();
    }
    
    // Принудительное включение защитного поля #################################################################################################################################
    public void Enable() {

        static_field.gameObject.SetActive( true );
        dynamic_field.gameObject.SetActive( true );
    }

    // Принудительное выключение защитного поля ################################################################################################################################
    public void Disable() {

        static_field.gameObject.SetActive( false );
        dynamic_field.gameObject.SetActive( false );
    }
        
    // Обработка коллизий для случаев, когда коллайдер является триггером (станции, мины и т.п.) ###############################################################################
    void OnTriggerEnter( Collider collider ) {

        if( is_station && !auto_protection ) return;
        if( collider.gameObject.layer == radar_layer ) return;

        contact_point = Game.CalculateEffectPosition( cached_transform, collider.transform );

        // Если это защита станции и на неё заходит корабль, то станция просто берёт его под свою защиту
        if( is_station && (collider.CompareTag( "Ship" ) || collider.CompareTag( "Support" ) || collider.CompareTag( "Landing" )) ) {

            if( ship == null ) ship = collider.GetComponentInParent<Ship>();
            if( ship != null ) ship.MakeAsProtectedCollider( collider );

            if( reaction_to_ship ) ShowLongHitEffect( contact_point, true );
        }

        // Если это струя реактивного двигателя, то просто показать вспышку
        else if( is_station && collider.CompareTag( "Jet" ) ) {

            if( reaction_to_ship ) ShowLongHitEffect( contact_point, true );
        }
        
        // Если это груз или мнерал, то просто показать вспышку
        else if( is_station && collider.GetComponent<ObstacleControl>().Is_value ) {

            if( reaction_to_ship ) ShowLongHitEffect( contact_point, true );
        }

        // Если объект ещё действительно активен и это не осколок корабля, станция защищает себя и объект уничтожается
        else if( is_station && collider.gameObject.activeInHierarchy && !collider.CompareTag( "Wreck" ) ) {

            ShowLongHitEffect( contact_point, (is_station && Game.Player.At_station) ? true : Game.Use_sound_in_vacuum );

            obstacle = collider.GetComponent<ObstacleControl>();
            if( collider.GetComponent<SpaceBody>() != null ) collider.GetComponent<SpaceBody>().DestroySpaceBody( false, (is_station && Game.Player.At_station) ? true : Game.Use_sound_in_vacuum );
            else if( obstacle != null ) obstacle.DestroyObstacle( (is_station && Game.Player.At_station) ? true : Game.Use_sound_in_vacuum );
        }
    }

    // В основном служит для определения того, находится ли корабль под защитой станции ########################################################################################
    void OnTriggerExit( Collider collider ) {

        if( is_station && !auto_protection ) return;

        contact_point = Game.CalculateEffectPosition( cached_transform, collider.transform );

        // Если это защита станции и корабль взлетает с неё, корабль выходит из под защиты станции
        if( is_station && (collider.CompareTag( "Ship" ) || collider.CompareTag( "Support" ) || collider.CompareTag( "Landing" )) ) {

            if( ship != null ) ship.MakeAsUnprotectedCollider( collider );
            if( (ship != null) && !ship.HasAnyProtectedColliders() ) ship = null;

            if( reaction_to_ship ) ShowLongHitEffect( contact_point, true );
        }

        // Иначе, если это струя реактивного двигателя, то просто показать вспышку
        else if( is_station && collider.CompareTag( "Jet" ) ) {

            if( reaction_to_ship ) ShowLongHitEffect( contact_point, true );
        }
        
        // Иначе просто воспроизводится эффект сполохов защитного поля
        else {

            ShowLongHitEffect( contact_point, (is_station && Game.Player.At_station) ? true : Game.Use_sound_in_vacuum );
        }
    }

    // Только показывает эффект удара, но никак не управляет самим полем #######################################################################################################
    public void ShowShortHitEffect( Vector3 point, bool use_sound ) {

        force_field.OnHit( point, hit_power, alpha_rate );

        if( Game.Control.Global_sound_enabled && use_sound && (sound_effects != null) ) sound_effects.PlayEffect( ref point );
    }
    
    // Преварительно включает (если не включено) защитное поле и показывает эффект удара; затем выключает его по истечении времени #############################################
    public void ShowLongHitEffect( Vector3 point, bool use_sound ) {

        if( !static_field.activeInHierarchy || !dynamic_field.activeInHierarchy ) StartCoroutine( ActivateShield( decay_speed ) );

        force_field.OnHit( point, hit_power, alpha_rate );

        if( Game.Control.Global_sound_enabled && use_sound && (sound_effects != null) ) sound_effects.PlayEffect( ref point );
    }
    
    // Delay shield protection by pointed time #################################################################################################################################
    IEnumerator ActivateShield( float time = 0f ) { 

        Enable();

        while( time > 0f ) {

            time -= Time.deltaTime;
            yield return null;
        }

        if( !constant_protection ) Disable();

        yield break;
    }
    
    // Создаёт объект для отображения статического поля ########################################################################################################################
    void CreateStaticEffect() {

        static_field = new GameObject( gameObject.name + "_static_field" );
        static_field.layer = cached_transform.gameObject.layer;
        static_field.transform.parent = cached_transform;
        static_field.transform.localPosition = Vector3.zero;
        static_field.transform.localEulerAngles = Vector3.zero;
        static_field.transform.localScale = Vector3.one;

        static_field.AddComponent<MeshFilter>();
        Mesh mesh = (GetComponent<MeshFilter>() != null ) ? GetComponent<MeshFilter>().mesh : GetComponentInParent<MeshFilter>().mesh;
        static_field.GetComponent<MeshFilter>().sharedMesh = mesh;

        static_field.AddComponent<MeshRenderer>();
        MeshRenderer mesh_renderer = static_field.GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterial = static_material;
        mesh_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mesh_renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mesh_renderer.receiveShadows = false;
        mesh_renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;

        static_field.SetActive( false );
    }

    // Создаёт объект для отображения динамического поля #######################################################################################################################
    void CreateDynamicEffect() {

        dynamic_field = new GameObject( gameObject.name + "_dynamic_field" );
        dynamic_field.layer = cached_transform.gameObject.layer;
        dynamic_field.transform.parent = static_field.transform;
        dynamic_field.transform.localPosition = Vector3.zero;
        dynamic_field.transform.localEulerAngles = Vector3.zero;
        dynamic_field.transform.localScale = Vector3.one;

        dynamic_field.AddComponent<MeshFilter>();
        Mesh mesh = (GetComponent<MeshFilter>() != null ) ? GetComponent<MeshFilter>().mesh : GetComponentInParent<MeshFilter>().mesh;
        dynamic_field.GetComponent<MeshFilter>().sharedMesh = mesh;

        dynamic_field.AddComponent<MeshRenderer>();
        MeshRenderer mesh_renderer = dynamic_field.GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterial = dynamic_material;
        mesh_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mesh_renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mesh_renderer.receiveShadows = false;
        mesh_renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;

        #if UNTIY_STANDALONE
        force_field = dynamic_field.AddComponent<ForceFieldCustomized>();
        #else
        force_field = dynamic_field.AddComponent<ForceFieldCustomizedMobile>();
        #endif

        force_field.SetHitFilter( dynamic_field.GetComponent<MeshFilter>() );
        force_field.SetHitMaterial( mesh_renderer.sharedMaterial );
        force_field.SetReactionSpeed( reaction_speed );
        force_field.SetDecaySpeed( decay_speed );

        dynamic_field.SetActive( false );
    }
}