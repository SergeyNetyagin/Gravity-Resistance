using UnityEngine;

[RequireComponent( typeof( Rigidbody ) )]
public class ObstacleControl : MonoBehaviour, IDetecting {

    private static int animation_ID_complete = Animator.StringToHash( "Complete" );
    private static int animation_ID_ship_sit = Animator.StringToHash( "Ship_sit" );
    private static int animation_ID_ship_landing = Animator.StringToHash( "Ship_landing" );

    [SerializeField]
    private ObstacleType obstacle_type;
    public ObstacleType Obstacle_type { get { return obstacle_type; } }

    [Header( "СООБЩЕНИЯ О КОНТАКТЕ ИЛИ ОБНАРУЖЕНИИ ЭТОГО ПРЕПЯТСТВИЯ" )]
    [SerializeField]
    private ComplexMessage contact_message;

    [SerializeField]
    [Space( 10 )]
    private ComplexMessage detection_message;

    [Header( "ПОВЕЖДЕНИЯ ОТ СТОЛКНОВЕНИЯ С ЭТИМ ПРЕПЯТСТВИЕМ" )]
    [Tooltip( "Если установлен этот флаг, то даже во время кинематической паузы физика составных частей объекта не отключается" )]
    private bool is_non_kinematic = false;
    public bool Is_non_kinematic { get { return is_non_kinematic; } }
    public void MarkAsKinematic() { is_non_kinematic = false; }
    public void MarkAsNonKinematic() { is_non_kinematic = true; }

    [Space( 10 )]
    [SerializeField]
    private Damage[] damages;
    public Damage[] Damages { get { return damages; } }

    [Header( "ЭФФЕКТЫ, СВЯЗАННЫЕ С ЭТИМ КОСМИЧЕСКИМ ТЕЛОМ" )]
    [SerializeField]
    [Tooltip( "Эффект, возникающий при столкновении твёрдого тела с данным объектом" )]
    private Effect contact_prefab;
    public Effect Contact_prefab { get { return contact_prefab; } }

    [SerializeField]
    [Tooltip( "Эффект уничтожения данного объекта" )]
    private Effect destroy_prefab;
    public Effect Destroy_prefab { get { return destroy_prefab; } }

    [SerializeField]
    [Space( 10 )]
    [Tooltip( "Эффект контакта струи двигателя с поверхностью объекта; например, искры (если эффект не задан, он не воспроизводится)" ) ]
    private Effect jet_contact_prefab;
    public Effect Jet_contact_prefab { get { return jet_contact_prefab; } }

    [SerializeField]
    [Tooltip( "Эффект ответной реакции тела или поверхности на струю от двигателя корабля; например, пыль (если эффект не задан, он не воспроизводится)" )]
    private Effect jet_reaction_prefab;
    public Effect Jet_reaction_prefab { get { return jet_reaction_prefab; } }

    [SerializeField]
    [Range( 1, 100 )]
    [Tooltip( "Скорость повторения эффекта столкновения струи двигателя с внешним объектом (раз в секунду): по умолчанию = 10" )]
    private int contact_speed = 10;
    private float contact_delta_time = 0f;
    public float Contact_delta_time { get { return contact_delta_time; } }
    public float Contact_sum_time { get; set; }

    [SerializeField]
    [Range( 1, 50 )]
    [Tooltip( "Скорость повторения эффекта обратной реакции внешнего объекта на попадание струи от двигателя (раз в секунду): по умолчанию = 5" )]
    private int reaction_speed = 5;
    private float reaction_delta_time = 0f;
    public float Reaction_delta_time { get { return reaction_delta_time; } }
    public float Reaction_sum_time { get; set; }

    [System.NonSerialized]
    private bool was_saved = false;

    private Vector3 contact_point;
    private ContactPoint[] contacts;

    private RendererControl renderer_control;
    public RendererControl Renderer_control { get { return renderer_control; } }

    private Rigidbody physics;
    public Rigidbody Physics { get { return physics; } }
    public float Total_mass { get { return physics.mass; } }

    private Transform cached_transform;
    public Transform Cached_transform { get { return cached_transform; } }

    private AnimationMovement moving;
    public bool Is_moving { get { return (moving != null); } }
    public void EnableMovement() { if( moving != null ) moving.enabled = true; }
    public bool DisableMovement() { bool state = false; if( moving != null ) { state = moving.enabled; moving.enabled = false; } return state; }

    private AnimationRotation rotating;
    public bool Is_rotating { get { return (rotating != null); } }
    public void EnableRotation() { if( rotating != null ) rotating.enabled = true; }
    public bool DisableRotation() { bool state = false; if( rotating != null ) { state = rotating.enabled; rotating.enabled = false; } return state; }

    private Station station;
    public Station Station { get { return station; } }
    private bool is_station { get { return (station != null); } }
    public bool Is_station { get { return (station != null); } }

    private Protection protection;
    public Protection Protection { get { return protection; } }
    public bool Is_protection { get { return (protection != null); } }
    public void SetProtection( Protection protection ) { this.protection = protection; }

    private SpaceBody space_body;
    public SpaceBody Space_body { get { return space_body; } }
    public bool Is_space_body { get { return (space_body != null); } }
    public void SetSpaceBody( SpaceBody space_body ) { this.space_body = space_body; }

    private Freight freight;
    public Freight Freight { get { return freight; } set { freight = value; } }
    public bool Is_freight { get { return (freight != null); } }

    private Mineral mineral;
    public Mineral Mineral { get { return mineral; } set { mineral = value; } }
    public bool Is_mineral { get { return (mineral != null); } }

    private Mission mission;
    public Mission Mission { get { return mission; } set { mission = value; } }
    public bool Is_mission { get { return (mission != null); } }

    private Wanderer wanderer;
    public Wanderer Wanderer { get { return wanderer; } set { wanderer = value; } }
    public bool Is_wanderer { get { return (wanderer != null); } }

    public bool Is_value { get { return (Is_freight || Is_mineral); } }

    // Реализация интерфейса для IDetecting ####################################################################################################################################

    private MeshRenderer mesh_renderer;

    private int layer_original;
    public int Layer_original { get { return layer_original; } }

    private float radius = 0.1f;
    public float Radius { get { return radius; } }

    private float diameter = 0.2f;
    public float Diameter { get { return diameter; } }

    public ZoneType Zone_type { get { return ZoneType.Unknown; } }
    public MineralType Mineral_type { get { return (Is_mineral ? mineral.Type : MineralType.Unknown); } }

    public bool Is_visible { get { return renderer_control.Is_visible; } }

    public bool Is_zone { get { return false; } }
    public bool Is_obstacle { get { return (station == null) && (mission == null); } }

    public float Magnitude { get; set; }
    public float Sqr_magnitude { get; set; }
    public Vector3 Detected_point { get; set; }

    public void CalculateDimensions() {

        mesh_renderer = GetComponent<MeshRenderer>();

        if( mesh_renderer != null ) radius = (mesh_renderer.bounds.extents.x + mesh_renderer.bounds.extents.y + mesh_renderer.bounds.extents.z) * 0.333f;
        diameter = radius * 2f;
    }

    // Проинициализировать ссылки нужно именно здесь, чтобы потом другие компоненты могли их изменить по ситуации ##############################################################
    void Awake() {

        layer_original = gameObject.layer;

        #if UNITY_EDITOR
        if( GetComponent<ZoneControl>() != null ) Debug.Log( "Объект " + gameObject.name + " одновременно имеет взаимоисключающие компоненты: ZoneControl и ObstacleControl" );
        #endif

        CalculateDimensions();

        moving = GetComponent<AnimationMovement>();
        rotating = GetComponent<AnimationRotation>();

        station = GetComponent<Station>();
        mineral = GetComponent<Mineral>();
        freight = GetComponent<Freight>();
        mission = GetComponent<Mission>();
        wanderer = GetComponent<Wanderer>();
    }

    // Starting initialization #################################################################################################################################################
    void OnEnable() {

        if( cached_transform != null ) return;

        protection = GetComponent<Protection>();
        space_body = GetComponent<SpaceBody>();

        reaction_delta_time = 1f / reaction_speed;
        Reaction_sum_time = Time.time;

        contact_delta_time = 1f / contact_speed;
        Contact_sum_time = Time.time;

        cached_transform = transform;
        physics = GetComponent<Rigidbody>();
        renderer_control = GetComponent<RendererControl>();
    }

    // Collision analysis ######################################################################################################################################################
    void OnCollisionEnter( Collision collision ) {

        // Если это корабль и он уже к этому моменту уничтожен, то ничего не делаем
        if( ((obstacle_type == ObstacleType.Player) || collision.collider.CompareTag( "Player" )) && Game.Player.Is_destroyed ) return;
        
        // Если объект уже стал невидимым или неактивным, также не анализируем 
        if( (obstacle_type != ObstacleType.Player) && (renderer_control != null) && !renderer_control.Is_visible ) return;
        if( !gameObject.activeInHierarchy || !collision.gameObject.activeInHierarchy ) return;

        // Определяем точку столкновения - она необходима почти во всех типах столкновений
        contacts = collision.contacts;
        contact_point = (contacts.Length > 0) ? contacts[0].point : Game.CalculateEffectPosition( cached_transform, collision.transform );

        switch( obstacle_type ) {

            case ObstacleType.Player:       CollisionWithPlayer( collision ); break;
            case ObstacleType.Station:      CollisionWithStation( collision ); break;
            //case ObstacleType.Shell:        CollisionWithShell( collision ); break;
		    //case ObstacleType.Mine:         CollisionWithMines( collision ); break;
 		    //case ObstacleType.Comet:        CollisionWithAsteroid( collision ); break;
            //case ObstacleType.Asteroid:     CollisionWithAsteroid( collision ); break;
            //case ObstacleType.Meteor:       CollisionWithMeteorite( collision ); break;
            //case ObstacleType.Meteorite:    CollisionWithMeteorite( collision ); break;
    		//case ObstacleType.Debris:       CollisionWithSatellite( collision ); break;
            //case ObstacleType.Satellite:    CollisionWithSatellite( collision ); break;
            //case ObstacleType.Weapon:       CollisionWithStructure( collision ); break;
            //case ObstacleType.Structure:    CollisionWithStructure( collision ); break;*/
        }
    }

    // Служит для определения столкновения игрока с конкретным коллайдером станции #############################################################################################
    void CollisionWithPlayer( Collision collision ) {

        if( collision.collider.CompareTag( "Stabilizer" ) ) {

            Game.Level.SetCombo( 1f );
            Game.Player.FlightShip();
            Game.Player.Ship.Hull_durability.Available = 0f;
            Game.Canvas.RefreshPauseIndicator( true );
            Game.Player.DestroyShip( contact_point );
        }
    }

    // Анализ столкновений корабля со станцией #################################################################################################################################
    void CollisionWithStation( Collision collision ) {

        // Если корабль заходит на посадку
        if( collision.collider.CompareTag( "Support" ) && Game.Player.Is_landing_zone ) {

            // Если корабль при старте сразу появился на площадке
            if( Game.Player.On_surface ) {

                Game.Player.LandingShip( station );
            }

            // В режиме автопосадки кораблю не причиняется никакого вреда
            else if( Game.Player.Is_autolanding ) {
                       
                Game.Player.LandingShip( station );
                Game.Player.Ship.SupportsSitDown();
            }

            // Если это не автопосадка, проверяем на предмет повреждений
            else if( !Game.Player.On_surface ) {

                Game.Player.LandingShip( station );
                Game.Player.Ship.SupportsSitDown();

                float closing_speed = Game.ClosingSpeed( collision );

                // Если совершена жёсткая посадка, подсчитываем повреждения
                if( closing_speed > Game.Player.Safe_speed_y ) {

                    Game.Level.SetCombo( 1f );
                    Game.Canvas.RefreshPauseIndicator( true );
                    DamageShip( closing_speed, Game.Player.Ship.Hull_durability.Partial_damage_message );
                    Game.Message.Show( Game.Control.Landing_hard_message );
                }

                // Если корабль благополучно приземлился, набираем очки опыта, комбо-коэффициент и поздравления со станции
                else {
                        
                    //Game.Combo += Game.Control.ac
                    //Game.Experience += 
                    Game.Canvas.RefreshPauseIndicator( true );
                    Game.Message.Show( Game.Control.Landing_soft_message );
                }
            }

            if( Game.Player.At_station && Game.Player.Station.Is_trading ) {

                Game.Canvas.RefreshAutolandingIndicator( false );
                Game.Canvas.ShowAutolandingButton();
            }

            // Запоминаем чекпоинт для записи данных при выходе из уровня
            Game.Level.SetStartingStationKey( Game.Player.Station.Name_key );
            Game.Level.SetStartingAutolanding( false );
            
            // проверка
            /* if( (Game.Scenario_control.Current_mission != null) && (station != null) && (Game.Scenario_control.Destination_transform == station.Cached_transform) ) {

                Game.Player.UnloadFromHold( Game.Scenario_control.Current_mission.GetComponent<Value>(), false );
                Game.Canvas_game.RefreshHoldIndicator();
            }*/

            return;
        }

        // Если корабль сталкивается со станцией в произвольном месте (не в режиме соприкосновения опор с посадочной площадкой)
        else if( collision.collider.CompareTag( "Ship" ) || collision.collider.CompareTag( "Support" ) ) {
              
            float closing_speed = Game.ClosingSpeed( collision );
                        
            // Если корабль приземлилися на неподготовленные опоры
            if( Game.Player.At_station && (animation_ID_ship_landing != 0) && (animation_ID_ship_sit != 1) ) {

                Game.Level.SetCombo( 1f );
                Game.Canvas.RefreshPauseIndicator( true );
                DamageShip( closing_speed, Game.Control.Landing_wrong_message );
            }

            // Если корабль столкнулся со станцией в произвольном месте
            else {

                Game.Level.SetCombo( 1f );
                Game.Canvas.RefreshPauseIndicator( true );
                DamageShip( closing_speed, Game.Control.Landing_wrong_message );
            }
        }
    }

    // Анализ столкновений с минами ############################################################################################################################################
    void CollisionWithMines( Collision collision ) {

        if( collision.collider.CompareTag( "Ship" ) || collision.collider.CompareTag( "Support" ) ) {

            Sleep();
            //game_message.ShowMessage( Contact_message );
            //radar.RemoveAsTarget( cached_transform );
            //ShowDestroyEffect( cached_transform.position, 5.0f, true );
            //TotalDamageShip( Game.Complication );
        }
    }

    // Анализ столкновений со снарядами ########################################################################################################################################
    void CollisionWithShell( Collision collision ) {

        if( collision.collider.CompareTag( "Ship" ) || collision.collider.CompareTag( "Support" ) ) {

            Sleep();
            //ShowContactEffect( Game.CalculateEffectPosition( cached_transform, collision.transform ), 1.0f, true );
            //DamageShip( null );
        }

        else if( collision.collider.CompareTag( "Shield" ) ) {
                    
            Sleep();
            //ShowContactEffect( Game.CalculateEffectPosition( cached_transform, collision.transform ), 1.0f, true );
            //collision.transform.GetComponent<ShieldEffect>();
        }

        else if( collision.transform.GetComponent<ObstacleControl>() != null ) {
                    
            Sleep();
            //ShowContactEffect( Game.CalculateEffectPosition( cached_transform, collision.transform ), 1.0f, false );
            //ShowDestroyEffect( Game.CalculateEffectPosition( cached_transform, collision.transform ), 5.0f, false );

            if( collision.transform.GetComponent<Wanderer>() == null ) collision.transform.GetComponent<ObstacleControl>().Sleep();
            else collision.transform.GetComponent<Wanderer>().Sleep( false );
        }

        else if( collision.transform.GetComponent<Freight>() != null ) {

            if( collision.transform.GetComponent<Wanderer>() == null ) collision.transform.GetComponent<ObstacleControl>().Sleep();
            else collision.transform.GetComponent<Wanderer>().Sleep( false );
        }
    }
    
    // Анализ столкновений со спутниками и космическим мусором #################################################################################################################
    void CollisionWithSatellite( Collision collision ) {

        if( collision.collider.CompareTag( "Ship" ) || collision.collider.CompareTag( "Support" ) ) {

            //game_message.ShowMessage( Contact_message );
            //ShowContactEffect( Game.CalculateEffectPosition( cached_transform, collision.transform ), 1.0f, true );
            //TotalDamageShip( ClosingSpeed( collision ) );
        }

        gameObject.SetActive( false );
        //radar.RemoveAsTarget( collision.transform );
        //ShowDestroyEffect( Game.CalculateEffectPosition( cached_transform, collision.transform ), 5.0f, true );
    }

    // Анализ столкновений с метеорами и метеоритами ###########################################################################################################################
    void CollisionWithMeteorite( Collision collision ) {

        if( collision.collider.CompareTag( "Ship" ) || collision.collider.CompareTag( "Support" ) ) {

            //game_message.ShowMessage( Contact_message );
            //ShowContactEffect( Game.CalculateEffectPosition( cached_transform, collision.transform ), 1.0f, true );
            //TotalDamageShip( ClosingSpeed( collision ) );
        }

        // здесь был момент, когда метеорит попал на платформу станции, а у платформы нет риджитбади - нужно учесть подобные моменты
        /*if( Full_mass < collision.gameObject.GetComponent<Rigidbody>().mass ) {

            gameObject.SetActive( false );
            radar.RemoveAsTarget( collision.transform );
            ShowDestroyEffect( Game.CalculateEffectPosition( cached_transform, collision.transform ), 5.0f, true );
        }*/
    }

    // Анализ столкновений с астероидами и кометами ############################################################################################################################
    void CollisionWithAsteroid( Collision collision ) {

        if( collision.collider.CompareTag( "Ship" ) || collision.collider.CompareTag( "Support" ) ) {

            //game_message.ShowMessage( Contact_message );
            //ShowContactEffect( Game.CalculateEffectPosition( cached_transform, collision.transform ), 1.0f, true );
            //SingleDamageShip( IndicatorName.Hull, ClosingSpeed( collision ) );
        }
    }

    // Анализ столкновений с различными конструкциями ##########################################################################################################################
    void CollisionWithStructure( Collision collision ) {

        if( collision.collider.CompareTag( "Ship" ) || collision.collider.CompareTag( "Support" ) ) {
                    
            //game_message.ShowMessage( Contact_message );
            //ShowContactEffect( Game.CalculateEffectPosition( cached_transform, collision.transform ), 1.0f, true );
            //SingleDamageShip( IndicatorName.Hull, Game.ClosingSpeed( collision ) );
        }
    }

    // #########################################################################################################################################################################
    void OnCollisionExit( Collision collision ) {

        // Если это корабль и он уже к этому моменту уничтожен, то ничего не делаем
        if( (obstacle_type == ObstacleType.Player) && Game.Player.Is_destroyed ) return;

        switch( obstacle_type ) {

        case ObstacleType.Station:

            if( collision.collider.CompareTag( "Support" ) && Game.Player.Is_landing_zone ) {

                if( Game.Player.On_surface && (Game.Player.Thrust_y > 0f) ) {
                    
                    if( Game.Player.At_station && Game.Player.Station.Is_trading ) {

                        Game.Canvas.RefreshAutolandingIndicator( false );
                        Game.Canvas.HideAutolandingButton();
                    }

                    Game.Player.FlightShip();
                    Game.Player.Ship.SupportsSitUp();
                } 
            }

            break;
        }
    }
    
    // Damages the ship by any ship's parameters ###############################################################################################################################
    void DamageShip( float closing_speed, ComplexMessage complex_message ) {

        Game.Message.Show( complex_message );
        Game.Effects_control.Show( contact_prefab, contact_point, true );

        float damage_sum = 0f;
        float damage_strength = 0f;

        for( int i = 0; i < damages.Length; i++ ) {

            damage_strength = damages[i].Strength * Game.Player.Ship.GetIndicator( damages[i].Indicator_type ).Unit_size * closing_speed;

            damage_sum += damage_strength;
            Game.Player.DamageShip( damages[i].Indicator_type, damage_strength );
        }

        // Запустить эффект вибрации камеры от столкновения в зависимости от степени повреждений
        if( damage_sum > Game.Camera_control.Shake_max_damage_awful ) Game.Camera_control.StartShaking( ShakeType.Awful );
        else if( damage_sum > Game.Camera_control.Shake_max_damage_hard ) Game.Camera_control.StartShaking( ShakeType.Hard );
        else if( damage_sum > Game.Camera_control.Shake_max_damage_soft ) Game.Camera_control.StartShaking( ShakeType.Soft );

        // Если было очень сильное столкновение, запускаем проверку на утечки
        if( damage_sum > Game.Camera_control.Shake_max_damage_leaks ) Game.Player.CheckFuelLeaks();
    }

    // Уничтожает данный объект ################################################################################################################################################
    public void DestroyObstacle( bool use_sound ) {

        if( destroy_prefab != null ) Game.Effects_control.Show( destroy_prefab, cached_transform.position, use_sound );

        Game.Radar.RemoveAsTarget( cached_transform );

        Destroy( gameObject );
    }

    // Sleep the object ########################################################################################################################################################
    public void Sleep() {

        // Ссылки могут быть не проинициализированы, например, когда мы используем загрузку из GameTest()
        if( cached_transform != null ) {

            Game.Radar.RemoveAsTarget( cached_transform );

            physics.Sleep();
            physics.isKinematic = true;
        }

        // Или, например, если объект создан во время выполнения, у него также могут не успеть проинициализироваться кэш-ссылки
        else {

            GetComponent<Rigidbody>().Sleep();
            GetComponent<Rigidbody>().isKinematic = true;
        }

        gameObject.SetActive( false );
    }

    // Awake the object ########################################################################################################################################################
    public void WakeUp() {

        gameObject.SetActive( true );
        physics.isKinematic = false;
        physics.WakeUp();
    }
    
    // Awake the object ########################################################################################################################################################
    public void WakeUp( Vector3 activation_point ) {

        cached_transform.position = activation_point;

        gameObject.SetActive( true );
        physics.isKinematic = false;
        physics.WakeUp();
    }

    // Awake the object ########################################################################################################################################################
    public void WakeUp( Transform object_transform ) {

        cached_transform.position = object_transform.position;

        gameObject.SetActive( true );
        physics.isKinematic = false;
        physics.WakeUp();
    }

    /*
        // Collision analysis ######################################################################################################################################################
        void OnCollisionEnter( Collision collision ) {

            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            if( current_mission.Delivery_time != 0f ) Game.Player_timer.StartTimer( current_mission.Delivery_time );
            //////////////////////////////////////////////////////////////////////////////////////////////////////////

            if( (renderer_control != null) && !renderer_control.Is_visible ) return;
            if( !gameObject.activeInHierarchy || !collision.gameObject.activeInHierarchy ) return;

            if( collision.collider.CompareTag( "Shield" ) && (collision.collider.GetComponentInParent<Player>() == null) ) return;
            else if( !collision.collider.CompareTag( "Ship" ) && !collision.collider.CompareTag( "Support" ) ) return;

            if( player.HasIntoHold( this ) ) return;
            if( FreightDestroyed( collision ) ) return;

            // Load the freight on board
            if( player.Ship.Hold.Available >= Total_mass_in_tons ) {

                ShowTakingEffect( Game.CalculateEffectPosition( cached_transform, player_transform ), 3.0f, true );

                player.LoadFreightIntoHold( this );
                canvas_game.SetControlButtons( true, false );
                canvas_game.RefreshHoldIndicator();

                // If freight shoot to the ship while ship staying at station
                if( player.Station != null ) {

                    player.Station.AssignBoughtFreight( player.GetFreightTransform() );
                    canvas_game.SetStationService( true );
                }
            }

            // If freight is not possible to load into the hold
            else game_message.ShowMessage( Mass_limit_message );
        }

        // Check of safe loading of freight ########################################################################################################################################
        bool FreightDestroyed( Collision collision ) {

            ShowContactEffect( Game.CalculateEffectPosition( cached_transform, player_transform ), 1.0f, true );

            // Check for repeating contact with the flying mission freight
            if( (mission != null) && mission.Is_active && mission.Was_dropped ) return false;

            if( Game.ClosingSpeed( collision ) > safe_speed ) {

                ShowDestroyEffect( cached_transform.position, 5.0f, true );

                TotalDamageShip();
                player.Detector.RemoveAsTarget( cached_transform );
                game_message.ShowMessage( Contact_message );

                if( (mission != null) && (scenario.Current_mission == mission) ) scenario.CancelMission();
                else if( wanderer == null ) Sleep();
                else wanderer.Sleep( false );

                return true;
            }

            return false;
    }*/

    // Сохранение состояния объекта перед выходом из уровня или из игры ############################################################################################################
    private void OnApplicationQuit() {

        if( was_saved ) return;
        else was_saved = true;
    }
        
    // Сохранение состояния объекта перед выходом из уровня или из игры ############################################################################################################
    private void OnDisable() {

        if( was_saved ) return;
        else was_saved = true;
    }

    // Сохранение состояния объекта перед уничтожением #############################################################################################################################
    private void OnDestroy() {

        if( was_saved ) return;
        else was_saved = true;
    }
}