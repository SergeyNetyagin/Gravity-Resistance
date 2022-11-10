using UnityEngine;

public class RendererControl : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Автоматически назначить управление видимостью родительского коллайдера этого объекта" )]
    private bool control_collider = false;

    [SerializeField]
    [Tooltip( "Автоматически формировать список видимости для всех дочерних объектов" )]
    private bool control_childs = false;

    [SerializeField]
    [Tooltip( "Автоматически формировать список видимости для всех циклических скриптов: ForceField, AnimationRotation, AnimationMovement, AnimationColor, AnimationColorAplha, AnimationLight" )]
    private bool control_calculations = false;

    [SerializeField]
    [Tooltip( "Автоматически назначить управление видимостью системы частиц этого объекта" )]
    private bool control_particles = false;

    [SerializeField]
    [Tooltip( "Включать/выключать указанные объекты" )]
    private GameObject[] objects;

    [SerializeField]
    [Tooltip( "Включать/выключать указанные скрипты" )]
    private MonoBehaviour[] scripts;

    [SerializeField]
    [Tooltip( "Включать/выключать указанные коллайдеры" )]
    private Collider[] colliders;

    private ParticleSystem particles;
    private ParticleSystem.EmissionModule emission;

    private bool is_visible = false;
    public bool Is_visible { get { return is_visible; } }

    // Starting initialization #################################################################################################################################################
    void Awake() {

        particles = control_particles ? GetComponent<ParticleSystem>() : null;
        if( particles != null ) emission = particles.emission;
    }

    // Starting initialization #################################################################################################################################################
    void Start() {

        #if UNITY_EDITOR
        if( GetComponent<Renderer>() == null ) Debug.Log( "RendererControl требует, чтобы у объекта был компонент <Renderer>: объект <" + gameObject.name + ">" );
        #endif

        // Если объект используется в рекламе, он всегда видим (иначе, видимость определяется по ряду критериев)
        if( Game.Current_level > LevelType.Level_Menu ) is_visible = gameObject.activeInHierarchy && Game.Camera_control.IsInFrustum( GetComponent<Renderer>() );
        else is_visible = true;

        // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Подготовка очерних объектов: если одновременно назначены конкретые объекты и выбран флажок "дочерние объекты", приоритет за дочерними объектами
        // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if( control_childs ) {

            objects = new GameObject[ transform.childCount ];

            for( int i = 0; i < transform.childCount; i++ ) objects[i] = transform.GetChild( i ).gameObject;
        }

        for( int i = 0; i < objects.Length; i++ ) objects[i].SetActive( is_visible );

        // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Подготовка скриптовых компонентов: приоритет отдаётся флажку (однако при необходимости можно будет переписат так, чтобы объединить обе опции)
        // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        int scripts_count = 0;

        bool is_color = (control_calculations && (GetComponent<AnimationColor>() != null)) ? true : false;
        bool is_light = (control_calculations && (GetComponent<AnimationLight>() != null)) ? true : false;
        bool is_alpha = (control_calculations && (GetComponent<AnimationColorAlpha>() != null)) ? true : false;
        bool is_rotation = (control_calculations && (GetComponent<AnimationRotation>() != null)) ? true : false;
        bool is_movement = (control_calculations && (GetComponent<AnimationMovement>() != null)) ? true : false;
        bool is_force_field = (control_calculations && (GetComponent<Forcefield>() != null)) ? true : false;
        bool is_force_field_customized = (control_calculations && (GetComponent<ForceFieldCustomized>() != null)) ? true : false;
        bool is_force_field_customized_mobile = (control_calculations && (GetComponent<ForceFieldCustomizedMobile>() != null)) ? true : false;

        if( control_calculations ) {

            scripts_count += is_color ? 1 : 0;
            scripts_count += is_light ? 1 : 0;
            scripts_count += is_alpha ? 1 : 0;
            scripts_count += is_rotation ? 1 : 0;
            scripts_count += is_movement ? 1 : 0;

            scripts_count += is_force_field ? 1 : 0;
            scripts_count += is_force_field_customized ? 1 : 0;
            scripts_count += is_force_field_customized_mobile ? 1 : 0;

            scripts = new MonoBehaviour[ scripts_count ];

            int i = 0;

            if( is_color ) scripts[i++] = GetComponent<AnimationColor>() as MonoBehaviour;
            if( is_light ) scripts[i++] = GetComponent<AnimationLight>() as MonoBehaviour;
            if( is_alpha ) scripts[i++] = GetComponent<AnimationColorAlpha>() as MonoBehaviour;
            if( is_rotation ) scripts[i++] = GetComponent<AnimationRotation>() as MonoBehaviour;
            if( is_movement ) scripts[i++] = GetComponent<AnimationMovement>() as MonoBehaviour;

            if( is_force_field ) scripts[i++] = GetComponent<Forcefield>() as MonoBehaviour;
            if( is_force_field_customized ) scripts[i++] = GetComponent<ForceFieldCustomized>() as MonoBehaviour;
            if( is_force_field_customized_mobile ) scripts[i++] = GetComponent<ForceFieldCustomizedMobile>() as MonoBehaviour;
        }

        for( int i = 0; i < scripts.Length; i++ ) if( scripts[i].enabled != is_visible ) scripts[i].enabled = is_visible;

        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Подготовка коллайдеров: если указан коллайдер родительского объекта, остальные коллайдеры не контролируются (можно переписать скрипт, чтобы их объединить)
        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if( control_collider && (GetComponent<Collider>() != null) ) {

            colliders = new Collider[1];

            colliders[0] = GetComponent<Collider>();
        }

        for( int i = 0; i < colliders.Length; i++ ) colliders[i].enabled = is_visible;
        
        // Если объект является странствующим, то при старте он отключается безусловно
        if( gameObject.GetComponent<Wanderer>() != null ) gameObject.SetActive( false );

        // Если у родительского объекта есть частицы, они контролируются в зависимости от видимости
        if( particles != null ) emission.enabled = is_visible;
    }
    
    // It make an object as visible in the scene ###############################################################################################################################
    void OnBecameVisible() {

        is_visible = true;

        for( int i = 0; i < objects.Length; i++ ) objects[i].SetActive( is_visible );
        for( int i = 0; i < scripts.Length; i++ ) scripts[i].enabled = is_visible;
        for( int i = 0; i < colliders.Length; i++ ) colliders[i].enabled = is_visible;

        if( particles != null ) emission.enabled = is_visible;
    }

    // It does object invisible in the scene ###################################################################################################################################
    void OnBecameInvisible() {

        is_visible = false;

        for( int i = 0; i < objects.Length; i++ ) objects[i].SetActive( is_visible );
        for( int i = 0; i < scripts.Length; i++ ) scripts[i].enabled = is_visible;
        for( int i = 0; i < colliders.Length; i++ ) colliders[i].enabled = is_visible;

        if( particles != null ) emission.enabled = is_visible;
    }
}