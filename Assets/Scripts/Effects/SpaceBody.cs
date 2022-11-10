using UnityEngine;
using System.Collections.Generic;

public class SpaceBody : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Создаёт ли данный компонент эффект нагревания, либо используется просто как разрушаемая коллекция частей объекта; по умолчанию = true" )]
    private bool create_heating_effect = true;

    [SerializeField]
    [Tooltip( "Материал для эффекта нагревания тела" )]
    private Material heating_material;
    public void AssignMaterial( Material material ) { heating_material = material; }

    [Space( 10 )]
    [Tooltip( "Скорость нагревания тела при взаимодействии с частицами; по умолчанию = 0.1" )]
    [SerializeField]
    [Range( 0f, 1f )]
    private float reaction_speed = 0.1f;

    [Tooltip( "Скорость остывания тела; по умолчанию = 0.02" )]
    [SerializeField]
    [Range( 0f, 2f )]
    private float decay_speed = 0.02f;

    [SerializeField]
    [Range( 0f, 1f )]
    [Tooltip( "Степень прозрачности нагретого тела за пределами точки нагрева; по умолчанию = 0.1" )]
    private float alpha_rate = 0.1f;

    [SerializeField]
    [Range( 0f, 1f )]
    [Tooltip( "Степень нагревания тела в точке соприкосновения с частицами; по умолчанию = 0.1" )]
    private float hit_power = 0.1f;

    [SerializeField]
    [Range( 0f, 10000f )]
    [Tooltip( "Температура, при которой тело может взорваться (за исключением астероидов и комет - они не взрываются, а оставляют раскалённые пятна в месте контакта); по умолчанию = 2000" )]
    private float critical_temperature = 2000f;

    [Header( "КАК ВЕДЁТ СЕБЯ ТЕЛО ПОСЛЕ ЕГО ВЗРЫВА" )]
    [Tooltip( "Сопротивление, регулирующее поведение кусков после взрыва (-1 или другое отрицательное значение показывает, что у объекта останется его оригинальное значение)" )]
    [SerializeField]
    [Range( 0f, 1000f )]
    private float drag = -1f;

    [Tooltip( "Если объекту не будут назначены его составные части, он просто исчезает после взрыва" )]
    [SerializeField]
    private GameObject[] pieces_prefabs;

    private Transform cached_transform;

    private GameObject heating_effect;

    #if UNTIY_STANDALONE
    private ForceFieldCustomized force_field;
    #else
    private ForceFieldCustomizedMobile force_field;
    #endif

    // Start ###################################################################################################################################################################
    void Start() {

        cached_transform = transform;
    }

    // Processing of jet collision #############################################################################################################################################
    public void OnHitCollision( Vector3 point, bool is_caused_by_ship ) {

        if( !create_heating_effect ) return;
        if( heating_effect == null ) CreateHeatingBody();

        // Make collision effects
        force_field.OnHit( point, hit_power, alpha_rate );

        // Check the body temperature and destroy object if the temperature is explosion-critical
        if( !CompareTag( "Ship" ) && !CompareTag( "Comet" ) && !CompareTag( "Asteroid" ) ) {

            if( force_field.Body_temperature >= critical_temperature ) DestroySpaceBody( false, is_caused_by_ship ? true : Game.Use_sound_in_vacuum );
        }
    }

    // Destroys this space body and creates small pieces if they exist #########################################################################################################
    public void DestroySpaceBody( bool non_kinematic_pieces, bool use_sound ) {

        GameObject explosion_piece;

        ObstacleControl obstacle = GetComponent<ObstacleControl>();

        bool is_bot = (GetComponentInParent<Bot>() != null);
        bool is_ship = (GetComponentInParent<Ship>() != null);
        bool is_player = is_ship && GetComponentInParent<Ship>().Is_player;
        bool is_obstacle = (obstacle == null) ? false : true;
        bool is_mission = is_obstacle ? obstacle.Is_mission : false;
        bool is_wanderer = is_obstacle ? obstacle.Is_wanderer : false;

        // Запускаем эффект уничтожения (взрыв и т.п.)
        if( is_obstacle ) Game.Effects_control.Show( obstacle.Destroy_prefab, cached_transform.position, use_sound );

        // Удаляем данный объект как цель радара, но только если это не корабль самого игрока
        if( !is_player ) Game.Radar.RemoveAsTarget( cached_transform );

        // Создаём куски от взрыва объекта
        for( int i = 0; (pieces_prefabs != null) && (i < pieces_prefabs.Length); i++ ) {

            // Создаём объект осколка, меняем ему тег, чтобы он соответствовал тегу объекта, и помещаем в нужную иерархию
            explosion_piece = Instantiate( pieces_prefabs[i], cached_transform.position, Quaternion.identity ) as GameObject;
            if( cached_transform.parent.gameObject.activeInHierarchy ) explosion_piece.transform.parent = cached_transform.parent;

            // Если это обломки корабля, то назначаем им тег "Обломок", на который не будет реагировать станция и другие объекты
            // Иначе происходят дополнительные многократные повреждения корабля игрока, и отчёт о причинах гибели получается неверный
            if( is_player ) explosion_piece.tag = "Wreck";
            else explosion_piece.tag = gameObject.tag;

            // Если дана команда не отключать объект во время паузы, помечаем его как некинематический
            if( non_kinematic_pieces ) explosion_piece.GetComponent<ObstacleControl>().MarkAsNonKinematic();
            if( drag >= 0f ) explosion_piece.GetComponent<ObstacleControl>().Physics.drag = drag;

            // Активизируем куски (если они вдруг дективизированы в префабе)
            explosion_piece.SetActive( true );
        }

        // Объект уничтожается или деактивируется ТОЛЬКО ПОСЛЕ создания кусков от взрыва (если это корабль, то он уничтожается/отключается сам)
        if( !is_bot && !is_player && !is_mission && !is_wanderer ) { Destroy( gameObject ); return; }
        else if( is_wanderer && is_obstacle ) { obstacle.Wanderer.Sleep( false ); return; }
        else if( !is_bot && !is_player && is_obstacle ) { obstacle.Sleep(); return; }
    }

    // Creates the heating body for this object, if it was not created #########################################################################################################
    void CreateHeatingBody() {

        heating_effect = new GameObject( gameObject.name + "_heating_effect" );
        heating_effect.layer = cached_transform.gameObject.layer;
        heating_effect.transform.parent = cached_transform;
        heating_effect.transform.localPosition = Vector3.zero;
        heating_effect.transform.localEulerAngles = Vector3.zero;
        heating_effect.transform.localScale = Vector3.one;

        heating_effect.AddComponent<MeshFilter>();
        Mesh mesh = (GetComponent<MeshFilter>() != null ) ? GetComponent<MeshFilter>().mesh : GetComponentInParent<MeshFilter>().mesh;
        heating_effect.GetComponent<MeshFilter>().sharedMesh = mesh;

        heating_effect.AddComponent<MeshRenderer>();
        MeshRenderer mesh_renderer = heating_effect.GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterial = heating_material;
        mesh_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mesh_renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mesh_renderer.receiveShadows = false;
        mesh_renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;

        #if UNITY_EDITOR || UNTIY_STANDALONE
//        force_field = heating_effect.AddComponent<ForceFieldCustomized>();
        #else
        force_field = heating_effect.AddComponent<ForceFieldCustomizedMobile>();
        #endif

        force_field.SetHitFilter( heating_effect.GetComponent<MeshFilter>() );
        force_field.SetHitMaterial( mesh_renderer.material );
        force_field.SetReactionSpeed( reaction_speed );
        force_field.SetDecaySpeed( decay_speed );
//        force_field.Initialization();

        heating_effect.SetActive( true );
    }
}