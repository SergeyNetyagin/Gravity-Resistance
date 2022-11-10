using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections;
using System.Collections.Generic;

[Flags]
public enum PlayerState {

    Idle            = 0,           
    On_surface      = 1,
    Landing_zone    = 2,
    Autolanding     = 4,
    Destroyed       = 8,
    Upgraded        = 16
}

[System.Serializable]
public class PlayerData {

    public PlayerData() {

    }

    public void Load( Player player ) {

    }

    public void Save( Player player ) {

    }
}

public class Player : MonoBehaviour {

    [Header( "ДОПОЛНИТЕЛЬНЫЕ НАСТРОЙКИ ТЯГИ ДВИГАТЕЛЕЙ" )]
    [SerializeField]
    [Tooltip( "Величина плавного последовательного изменения вектора тяги при управлении от клавиатуры; по умолчанию = Vector2( 10f, 10f )" )]
    private Vector2 thrust_acceleration = new Vector2( 10f, 10f );
    public Vector2 Thrust_acceleration { get { return thrust_acceleration; } }

    [SerializeField]
    [Tooltip( "Величина плавного последовательного изменения вектора тяги при автопосадке; по умолчанию = Vector2( 60f, 60f )" )]
    private Vector2 safe_acceleration = new Vector2( 60f, 60f );

    [SerializeField]
    [Tooltip( "Безопасная скорость приземления при автопосадке; по умолчанию = Vector2( 0.5f, 0.5f )" )]
    private Vector2 safe_speed_auto = new Vector2( 0.5f, 0.5f );

    [SerializeField]
    [Tooltip( "Безопасная скорость приземления при ручной посадке; по умолчанию = Vector2( 2.0f, 2.0f )" )]
    private Vector2 safe_speed_manual = new Vector2( 2f, 2f );

    [SerializeField]
    [Space( 10 )]
    [Tooltip( "Временный коэффициент тяги двигателей для автопосадки; по умолчанию = Vector2( 0.05f, 0.05f )" )]
    private Vector2 safe_landing_thrust = new Vector2( 0.05f, 0.05f );

    [SerializeField]
    [Tooltip( "Временная масса корабля для обеспечения устойчивого режима автопосадки; по умолчанию = 10.0" )]
    private float safe_landing_mass = 10f;

    [SerializeField]
    [Tooltip( "Временная величина трения для обеспечения устойчивого режима автопосадки; по умолчанию = 2.0" )]
    private float safe_landing_drag = 2f;

    [SerializeField]
    [Tooltip( "Временная величина гравитации для обеспечения устойчивого режима автопосадки; по умолчанию = Vector3( 0f, -10f, 0f )" )]
    private Vector3 safe_landing_gravity = new Vector3( 0f, -10f, 0f );

    [SerializeField]
    [Tooltip( "Максимально доустимое отклонение корабля от оси центра посадочной площадки при автопосадке; по умолчанию = 0.2" )]
    [Range( 0.1f, 0.5f )]
    private float delta_axle_x = 0.2f;

    [Header( "СООБЩЕНИЯ И ЭФФЕКТЫ ДЛЯ КОРАБЛЯ ИГРОКА" )]
    [SerializeField]
    private ComplexMessage win_message;
    public ComplexMessage Win_message { get { return win_message; } }

    [SerializeField]
    private ComplexMessage die_message;
    public ComplexMessage Die_message { get { return die_message; } }

    [SerializeField]
    private ComplexMessage first_leaks_message;
    public ComplexMessage First_leaks_message { get { return first_leaks_message; } }

    [SerializeField]
    private ComplexMessage repeating_leaks_message;
    public ComplexMessage Repeating_leaks_message { get { return repeating_leaks_message; } }

    [SerializeField]
    private ComplexMessage autolanding_message;
    public ComplexMessage Autolanding_message { get { return autolanding_message; } }

    [SerializeField]
    [Space( 10 )]
    private Effect win_effect_prefab;
    public Effect Win_effect_prefab { get { return win_effect_prefab;} }

    [SerializeField] 
    private Effect die_effect_prefab;
    public Effect Die_effect_prefab { get { return die_effect_prefab;} }

    [Header( "НАСТРОЙКИ ЗВУКА ДВИГАТЕЛЕЙ КОРАБЛЯ" )]
    [SerializeField]
    [Tooltip( "Коэффициент затухания звука; по умолчанию = 0.5" )]
    [Range( 0.1f, 0.9f )]
    private float lerp_volume_rate = 0.5f;

    [SerializeField]
    [Tooltip( "Минимальная громкость звука двигателя (при отсутствии тяги); по умолчанию = 0.1" )]
    [Range( 0.1f, 0.5f )]
    private float min_volume = 0.1f;

    [SerializeField]
    [Tooltip( "Максимальная громкость звука двигателя (при максимальной тяге); по умолчанию = 1.0" )]
    [Range( 0.5f, 1f )]
    private float max_volume = 1f;

    private ListStations list_stations;
    private Station starting_station;

    private float 
        smooth_rate = 0f,
        engine_pitch = 1f,
        engine_volume = 1f,
        smooth_pitch = 0.7f,
        smooth_volume = 0.5f;

    private ZoneControl dangerous_zone = null;
    public ZoneControl Dangerous_zone { get { return dangerous_zone; } }
    private bool pause_zone_damages = false;

    public void StartContinuousDamages( ZoneControl zone ) { dangerous_zone = zone; }
    public void StopContinuousDamages( ZoneControl zone ) { if( dangerous_zone == zone ) dangerous_zone = null; }
    public void PauseContinuousDamages() { if( dangerous_zone != null ) { pause_zone_damages = true; dangerous_zone.PauseDamage(); } }
    public void ResumeContinuousDamages() { if( dangerous_zone != null ) { pause_zone_damages = false; dangerous_zone.ResumeDamage(); } }

    private Effect leaks_effect = null;

    public float Leaks_usage { get { return ship.Current_leaks_usage; } }
    public bool Has_fuel_leaks { get { return (ship.Current_leaks_usage > 0f); } }
    public bool Leaks_effect_is_playing { get { return (leaks_effect != null) && (leaks_effect.Particle_system != null) && leaks_effect.Particle_system.isPlaying; } }
    public void RepairFuelLeaks() { ship.Current_leaks_usage = 0f; if( leaks_effect != null) leaks_effect.gameObject.SetActive( false ); ship.Current_leaks_rate = 1f; leaks_effect = null; }
    public void StopLeaksEffect() { if( (leaks_effect != null) && (leaks_effect.Particle_system != null) ) leaks_effect.Particle_system.Stop(); }
    public void PauseLeaksEffect() { if( (leaks_effect != null) && (leaks_effect.Particle_system != null) ) leaks_effect.Particle_system.Pause(); }
    public void ResumeLeaksEffect() { if( (leaks_effect != null) && (leaks_effect.Particle_system != null) ) leaks_effect.Particle_system.Play(); }

    private float 
        ship_fuel_usage = 0f,
        ship_max_speed = 0f,
        thrust_x_to_y_rate = 0f,
        thrust_y_to_x_rate = 0f;

    private Vector2 
        thrust = Vector2.zero,
        thrust_rate = Vector2.zero,
        current_thrust_max = Vector2.zero,
        current_thrust_rate = Vector2.zero,
        ship_thrust_max_upgrade_inversed = Vector2.zero;

    private Vector3 speed = Vector3.zero;
    private Vector3 position = Vector3.zero;

    private const float quaternion_max_w = 0.997f;

    public float Thrust_x { get { return thrust.x; } }
    public float Thrust_y { get { return thrust.y; } }
    public Vector2 Thrust { get { return thrust; } set { thrust = value; } }
    public float Thrust_x_to_y_rate { get { return thrust_x_to_y_rate; } }
    public float Thrust_y_to_x_rate { get { return thrust_y_to_x_rate; } }

    public float Current_thrust_max_x { get { return current_thrust_max.x; } }
    public float Current_thrust_max_y { get { return current_thrust_max.y; } }
    public Vector2 Current_thrust_max { get { return current_thrust_max; } }

    [System.NonSerialized]
    private float ship_thrust_maximum_x_inversed = 0f;
    public float Current_thrust_rate_x { get { return current_thrust_rate.x; } }
    public float Ship_thrust_maximum_x_inversed { get { return ship_thrust_maximum_x_inversed; } }

    [System.NonSerialized]
    private float ship_thrust_maximum_y_inversed = 0f;
    public float Current_thrust_rate_y { get { return current_thrust_rate.y; } }
    public float Ship_thrust_maximum_y_inversed { get { return ship_thrust_maximum_y_inversed; } }

    private Rigidbody physics;
    public Rigidbody Physics { get { return physics; } }

    private PlayerState state = PlayerState.Idle;
    public PlayerState State { get { return state; } }
    public bool Is( PlayerState check_state ) { return (state & check_state) == check_state;  }
    public void AddState( PlayerState new_state ) { state |= new_state;  }
    public void ResetState( PlayerState old_state ) { state &= ~old_state;  }
    public bool Is_idle { get { return (state == PlayerState.Idle); } }
    public bool On_surface { get { return (state & PlayerState.On_surface) == PlayerState.On_surface; } }
    public bool Is_upgraded { get { return (state & PlayerState.Upgraded) == PlayerState.Upgraded; } }
    public bool Is_destroyed { get { return (state & PlayerState.Destroyed) == PlayerState.Destroyed; } }
    public bool Is_autolanding { get { return (state & PlayerState.Autolanding) == PlayerState.Autolanding; } }
    public bool Is_landing_zone { get { return (state & PlayerState.Landing_zone) == PlayerState.Landing_zone; } }

    private Station station = null;
    public Station Station { get { return station; } }
    public bool In_flight { get { return (station == null) && ((state & PlayerState.On_surface) != PlayerState.On_surface); } }
    public bool At_station { get { return (station != null) && ((state & PlayerState.On_surface) == PlayerState.On_surface); } }

    private Ship ship;
    public Ship Ship { get { return ship; } }

    private ShieldControl shield;
    public ShieldControl Shield { get { return shield; } }

    private Transform ship_transform;
    public Transform Ship_transform { get { return ship_transform; } }

    private Transform cached_transform;
    public Transform Cached_transform { get { return cached_transform; } }

    private Transform navigator_point_transform;
    public Transform Navigator_point_transform { get { return navigator_point_transform; } }

    private Transform hold_drop_point_transform;
    public Transform Hold_drop_point_transform { get { return hold_drop_point_transform; } }

    private Transform fuel_leaks_point_transform;
    public Transform Fuel_leaks_point_transform { get { return fuel_leaks_point_transform; } }

    private AudioSource engine_audio_source;

    private Animator animator;
    public Animator Ship_animator { get { return animator; } }

    private WaitForFixedUpdate autolanding_wait_for_fixed_update = new WaitForFixedUpdate();

    public Vector2 Landing_axle { get; set; }

    public void EnableEngineSound() { engine_audio_source.mute = false; }
    public void DisableEngineSound() { engine_audio_source.mute = true; }

    public bool Is_ship_flying { get { return ((physics.velocity.x != 0f) || (physics.velocity.y != 0f) ); } }
    public bool Is_ship_immovable { get { return ((physics.velocity.x == 0f) && (physics.velocity.y == 0f) ); } }
    public bool Is_engine_working { get { return ((thrust.x != 0f) || (thrust.y != 0f) ); } }
    public bool Is_engine_stopped { get { return ((thrust.x == 0f) && (thrust.y == 0f) ); } }

    public Vector2 Velocity { get { return physics.velocity; } }
    public float Velocity_x { get { return physics.velocity.x; } }
    public float Velocity_y { get { return physics.velocity.y; } }
    public float Velocity_max { get { return (Mathf.Abs( physics.velocity.x ) > Mathf.Abs( physics.velocity.y )) ? physics.velocity.x : physics.velocity.y; } }

    public Vector2 Safe_speed { get { return Is_autolanding ? safe_speed_auto : safe_speed_manual; } }
    public float Safe_speed_x { get { return Is_autolanding ? safe_speed_auto.x : safe_speed_manual.x; } }
    public float Safe_speed_y { get { return Is_autolanding ? safe_speed_auto.y : safe_speed_manual.y; } }

    public void Sleep() { if( !physics.IsSleeping() ) physics.Sleep(); thrust = Vector2.zero; CheckEngineThrust(); }
    public void StopThrust() { physics.velocity = Vector3.zero; thrust = Vector2.zero; }

    public void SetShipConstraints() { physics.constraints |= RigidbodyConstraints.FreezeRotation; }
    public void ResetShipConstraints() { physics.constraints &= ~RigidbodyConstraints.FreezeRotation; }
    public bool Has_constraints { get { return (physics.constraints & RigidbodyConstraints.FreezeRotation) == RigidbodyConstraints.FreezeRotation; } }

    // Startng initialization ##################################################################################################################################################
    private void Start() {

        cached_transform = transform;

        // Определяем корабль, который используется на уровне
        ship = cached_transform.GetComponentInChildren<Ship>();
        if( ship == null ) ship = Instantiate( Game.Control.Ship_prefabs[ (int) Game.Playing_ship ] ).GetComponent<Ship>();

        // Получаем ссылки на корабль
        ship_transform = ship.transform;
        ship_transform.parent = cached_transform;
        ship_transform.localPosition = Vector3.zero;

        // Выполняем полную начальную инициализацию корабля и устанавлваем его физический слой
        ship.StartingInitialization( gameObject.layer );

        // Создаём ссылку на тело корабля для эффектов взрыва
        GetComponent<ObstacleControl>().SetSpaceBody( ship.GetComponent<SpaceBody>() );

        // Получаем компоненты эффектов и анимаций
        shield = ship.GetComponent<ShieldControl>();
        animator = ship.GetComponent<Animator>();
        animator.enabled = false;
         
        // Кэширование точек сброса груза, центра колец навигатора, утечек топлива
        navigator_point_transform = ship.Navigator_point_transform;
        hold_drop_point_transform = ship.Hold_drop_point_transform;
        fuel_leaks_point_transform = ship.Fuel_leaks_axle_transform;

        physics = GetComponent<Rigidbody>();
        engine_audio_source = ship.GetComponent<AudioSource>();

        ship_max_speed = ship.Max_speed;

        physics.mass = ship.Total_ship_mass;
        physics.sleepThreshold = 0f;

        thrust_rate = ship.Thrust_rate;
        thrust_x_to_y_rate = thrust_rate.x / thrust_rate.y;
        thrust_y_to_x_rate = thrust_rate.y / thrust_rate.x;

        Vector2 ship_thrust_max = ship.Engine_thrust.Upgrade_max_ship * thrust_rate;
        ship_thrust_max_upgrade_inversed.x = 1f / ship_thrust_max.x;
        ship_thrust_max_upgrade_inversed.y = 1f / ship_thrust_max.y;

        ship_thrust_max = ship.Engine_thrust.Maximum * thrust_rate;
        ship_thrust_maximum_x_inversed = 1f / ship_thrust_max.x;
        ship_thrust_maximum_y_inversed = 1f / ship_thrust_max.y;

        StopThrust();

        // Задаём значения локализованных данных (поскольку текущий метод может опережать другие, где есть подобная инициализация)
        Game.RefreshLocalizedPhrases();
        Game.RefreshLocalizedSeparators();
        Game.RefreshLocalizedUnits();

        // Находим ближайшую станцию, чтобы затем назначить положение корабля в пространстве
        list_stations = FindObjectOfType<ListStations>();

        for( int i = 0; i < list_stations.Count; i++ ) {

            if( list_stations.GetStation( i ).Name_key == Game.Level.Starting_station_key ) {

                starting_station = list_stations.GetStation( i );
                position = starting_station.transform.position;
                break;
            }
        }

        // Если на старте предусмотрена автопосадка, устанавливаем опоры в полётный режим и запускаем режим автопосадки
        if( Game.Level.Starting_autolanding ) {

            position.y += ship.Altitude_autolanding;

            ship.SupportsLandingUp( 1000f );
            animator.enabled = true;

            StartCoroutine( AutoLanding() );
        }

        // Иначе устанавливаем опоры в посадочный режим и ставим корабль на площадку
        else {

            position.y += ship.Altitude_sit;

            ship.SupportsSitDown( 1000f );
            animator.enabled = true;

            LandingShip( starting_station );
        }

        // Устанавливаем положение корабля в пространстве
        cached_transform.position = position;

        // Загружаем параметры игрока
        LoadGame.Load( this );
    }

    // Using FixedUpdate() because next it uses physical calculations ##########################################################################################################
    private void FixedUpdate() {

        // Проверяем, нет ли утечки топлива из баков корабля
        if( (ship.Current_leaks_usage > 0f) && !Game.Is( GameState.Paused ) ) CalculateFuelReserve( Time.fixedDeltaTime, ship.Current_leaks_usage );
        
        // Проверяем, не потерял ли корабль правильную ориентацию в полёте или не упал ли кувырком на посадочную площадку
        if( On_surface && (Mathf.Abs( cached_transform.rotation.w ) < quaternion_max_w) ) DestroyShip( Vector3.zero );
        if( In_flight && (Mathf.Abs( cached_transform.rotation.w ) < quaternion_max_w) ) DestroyShip( Vector3.zero );

        // Рассчитываем текущий коэффициет тяги по отношению к максимальному
        current_thrust_rate.x = thrust.x * ship_thrust_max_upgrade_inversed.x;
        current_thrust_rate.y = thrust.y * ship_thrust_max_upgrade_inversed.y;

        // Рассчитываем максимальную текущую тягу для управляющих устройств (джойстика, клавиатуры и т.д.)
        current_thrust_max.x = ship.Engine_thrust.Available * thrust_rate.x;
        current_thrust_max.y = ship.Engine_thrust.Available * thrust_rate.y;

        CheckDamages();
        CheckEngineThrust();

        Game.Canvas.RefreshThrustIndicator( true );
    }

    // Deactivate a station's service ##########################################################################################################################################
    public void FlightShip() {

        ResetState( PlayerState.On_surface );
        ResetState( PlayerState.Autolanding );

        Game.Canvas.SetStationPanel( false );
        if( station != null ) station.CancelHoldValue();
        if( station != null ) Game.Message.Hide( station.Welcome_message );
        station = null;

        if( Mathf.Abs( cached_transform.rotation.w ) >= quaternion_max_w ) SetShipConstraints();

        physics.isKinematic = true;
        position.Set( cached_transform.position.x, cached_transform.position.y, 0f );
        cached_transform.position = position;
        cached_transform.rotation = Quaternion.identity;
        physics.isKinematic = false;
    }

    // Activate a station's service ############################################################################################################################################
    public void LandingShip( Station new_station ) {

        if( new_station == null ) return;

        AddState( PlayerState.On_surface );
        ResetState( PlayerState.Autolanding );

        station = new_station;
        Game.Canvas.SetStationPanel( true );
        Game.Message.Show( station.Welcome_message );

        ResetShipConstraints();
    }

    // Check for continuous damage zone ########################################################################################################################################
    private void CheckDamages() {

        #if UNITY_EDITOR
        if( Game.Use_immortal_mode ) return;
        #endif

        if( (dangerous_zone != null) && !pause_zone_damages ) {

            for( int i = 0; i < dangerous_zone.Damages.Length; i++ )
                DamageShip( dangerous_zone.Damages[i].Indicator_type, dangerous_zone.Damages[i].Strength * Time.fixedDeltaTime );
        }
    }

    // Change the player health in depend on damage value ######################################################################################################################
    public void DamageShip( IndicatorType type, float damage ) {

        #if UNITY_EDITOR
        if( Game.Use_immortal_mode ) return;
        #endif
        
        // Calculate a new indicator value
        switch( type ) {

            case IndicatorType.Hull_durability:

                ship.Hull_durability.Available -= ship.Hull_durability.Unit_size * damage;
                if( ship.Hull_durability.Available < 0f ) ship.Hull_durability.Available = 0f;
                if( ship.Hull_durability.Available <= ship.Hull_durability.Restart_limit ) DestroyShip( cached_transform.position );
                if( At_station ) Game.Canvas.RefreshServiceHullIndicator( true );
                Game.Canvas.RefreshHullIndicator( true );
                break;

            case IndicatorType.Fuel_capacity:

                ship.Fuel_capacity.Available -= ship.Fuel_capacity.Unit_size * damage;
                if( ship.Fuel_capacity.Available < 0f ) ship.Fuel_capacity.Available = 0f;
                if( !At_station && (ship.Fuel_capacity.Available <= ship.Fuel_capacity.Restart_limit) ) DestroyShip( cached_transform.position );
                if( At_station) Game.Canvas.RefreshServiceFuelIndicator( true );
                Game.Canvas.RefreshFuelIndicator( true );
                break;

            case IndicatorType.Engine_thrust:

                ship.Engine_thrust.Available -= ship.Engine_thrust.Unit_size * damage;
                if( ship.Engine_thrust.Available < 0f ) ship.Engine_thrust.Available = 0f;
                if( ship.Engine_thrust.Available <= ship.Engine_thrust.Restart_limit ) DestroyShip( cached_transform.position );
                if( At_station) Game.Canvas.RefreshServiceEngineIndicator( true );
                Game.Canvas.RefreshEngineIndicator( true );
                break;
        }
    }

    // Вызывается при гибели корабля игрока ####################################################################################################################################
    public void DestroyShip( Vector3 contact_point ) {

        if( Is_destroyed ) return;

        ResetState( PlayerState.Autolanding | PlayerState.Landing_zone | PlayerState.On_surface );
        AddState( PlayerState.Destroyed );

        Game.Canvas.RefreshFlightIndicators( false );
        Game.Canvas.RefreshServiceIndicators( false );

        Game.Radar.Disable();
        Game.Navigator.Disable();
        Game.Canvas.RefreshFlightIndicators( false );

        Game.Message.Show( die_message );

        // Воспроизводим предварительный взрыв в точке столкновения
        if( contact_point == Vector3.zero ) contact_point = ship.Engine_point;
        Game.Effects_control.Show( GetComponent<ObstacleControl>().Destroy_prefab, contact_point, true );

        // Взрываем сам корабль
        Game.Effects_control.Show( die_effect_prefab, cached_transform.position, true );

        // Создаём осколки корабля
        if( ship.GetComponent<SpaceBody>() != null ) ship.GetComponent<SpaceBody>().DestroySpaceBody( true, true );

        // Деактивируем корабль, игрок остаётся активен (подумать: возможно и он уже тоже не нужен)
        ship.gameObject.SetActive( false );
        physics.isKinematic = true;

        // Если игрок погиб, то при следующей загрузке уровня корабль появляется в режиме автопосадки на станцию с минимальным запасом топлива
        Game.Level.SetStartingAutolanding( true );

        Game.AddState( GameState.Restarting );

        Game.Camera_control.StartShaking( ShakeType.Awful );

        // Меняем данные для последующего сохранения
        Game.Level.SetCombo( 1f );
        Game.Level.SetNormalGravity();
        Game.Level.SetStartingAutolanding( true );

        // Удаляем файл апгрэйда корабля
        SaveGame.RemoveShipFile( ship );

        // Очищаем трюм корабля
        ship.ClearHold();

        StartCoroutine( WaitBeforeDestroying( 3f ) );
    }
    
    // Flight control ##########################################################################################################################################################
    private void CheckEngineThrust() {

        // Calculate a fuel before using of thrust and showing of boosters
        if( (thrust.x != 0f) || (thrust.y != 0f ) ) CalculateFuelReserve();

        // If fuel has reached a limit, thrust is always equal to zero
        if( ship.Fuel_capacity.Available == 0f ) thrust = current_thrust_rate = Vector2.zero;

        // Adjust sound in depending on current thrust
        CalculateSoundVolume();

        // Start of the up/down booster excludes work of the down thrust, and on the contrary too
        ship.ControlJetVertical( current_thrust_rate.y );

        // Start of the left/right booster excludes work of the left thrust, and on the contrary too
        ship.ControlJetHorizontal( current_thrust_rate.x );

        // Ограничение максимальной скорости корабля
        //speed = physics.velocity;

        //if( speed.x > ship_max_speed ) speed.x = ship_max_speed;
        //else if( speed.x < -ship_max_speed ) speed.x = -ship_max_speed;

        //if( speed.y > ship_max_speed ) speed.y = ship_max_speed;
        //else if( speed.y < -ship_max_speed ) speed.y = -ship_max_speed;

        // Не ускоряем корабль дополнительно, если он превысил максимальную скорость
        //if( (physics.velocity.x != speed.x) || (physics.velocity.y != speed.y) ) { physics.velocity = speed; return; }

        // Придать ускорение кораблю при помощии вектора тяги
        if( (thrust.x != 0f) || (thrust.y != 0f) ) physics.AddForce( thrust, ForceMode.Impulse );
    }

    // Adjust a player's sound in depend on his speed and using of shield ######################################################################################################
    private void CalculateSoundVolume() {

        engine_volume = (Mathf.Abs( thrust.x ) > Mathf.Abs( thrust.y )) ? Mathf.Abs( current_thrust_rate.x ) : Mathf.Abs( current_thrust_rate.y );

        engine_pitch = (Game.Camera_control.Is_shaking) ? (ship.Max_delta_pitch_shaked * engine_volume) : (ship.Max_delta_pitch_stable * engine_volume);
        engine_pitch += ship.Min_pitch;

        if( engine_volume < min_volume ) engine_volume = min_volume;
        else if( engine_volume > max_volume ) engine_volume = max_volume;

        if( (ship.Fuel_capacity.Available == 0f) || Game.Is( GameState.Paused ) ) engine_volume = 0f;

        smooth_rate = lerp_volume_rate * Time.fixedDeltaTime;
        smooth_volume = Mathf.Lerp( smooth_volume, engine_volume, smooth_rate );
        smooth_pitch = Mathf.Lerp( smooth_pitch, engine_pitch, smooth_rate );

        engine_audio_source.pitch = smooth_pitch;
        engine_audio_source.volume = smooth_volume * Game.Engine_volume;
    }

    // Расчёт расхода топлива, вызванный созданием тяги ########################################################################################################################
    private void CalculateFuelReserve() {

        #if UNITY_EDITOR
        if( Game.Use_immortal_mode ) return;
        #endif

        if( Is_autolanding ) ship_fuel_usage = ship.Fuel_thrust_usage * ship.Fuel_auto_rate * Time.fixedDeltaTime;
        else ship_fuel_usage = ship.Fuel_thrust_usage * Time.fixedDeltaTime;

        if( thrust.x != 0f ) ship.Fuel_capacity.Available -= ship_fuel_usage * Mathf.Abs( current_thrust_rate.x );
        if( thrust.y != 0f ) ship.Fuel_capacity.Available -= ship_fuel_usage * Mathf.Abs( current_thrust_rate.y );
        if( ship.Fuel_capacity.Available < 0f ) ship.Fuel_capacity.Available = 0f;

        Game.Canvas.RefreshFuelIndicator( true );
        if( Game.Player.At_station ) Game.Canvas.RefreshServiceFuelIndicator( true );

        if( !Is( PlayerState.Autolanding ) ) physics.mass = ship.Total_ship_mass;
    }

    // Расчёт расхода топлива, вызванного работой бортовых систем ##############################################################################################################
    public void CalculateFuelReserve( float delta_time, float fuel_usage ) {

        #if UNITY_EDITOR
        if( Game.Use_immortal_mode ) return;
        #endif

        ship.Fuel_capacity.Available -= fuel_usage * delta_time;
        if( ship.Fuel_capacity.Available < 0f ) ship.Fuel_capacity.Available = 0f;

        Game.Canvas.RefreshFuelIndicator( true );
        if( Game.Player.At_station ) Game.Canvas.RefreshServiceFuelIndicator( true );

        if( !Is( PlayerState.Autolanding ) ) physics.mass = ship.Total_ship_mass;
    }
    
    // Allows to put the Ship automaticaly to the station ######################################################################################################################
    public IEnumerator AutoLanding() {

        AddState( PlayerState.Autolanding );

        float saved_drag = physics.drag;
        Vector3 saved_gravity = new Vector3( 0f, UnityEngine.Physics.gravity.y, 0f );

        physics.mass = safe_landing_mass;
        physics.drag = safe_landing_drag;
        UnityEngine.Physics.gravity = safe_landing_gravity;

        thrust = Vector2.zero;
        thrust_rate = safe_landing_thrust;

        Game.Message.Show( autolanding_message );
        Game.Canvas.RefreshAutolandingIndicator( false );

        while( Is( PlayerState.Autolanding ) ) {

            // На случай, если включена кинематическая пауза
            if( !Game.Is( GameState.Paused ) ) {

                // Корректировка вертикальной скорости
                if( physics.velocity.y < (-safe_speed_auto.y) ) { if( thrust.y < Mathf.Abs( current_thrust_max.y ) ) thrust.y += safe_acceleration.y * Time.fixedDeltaTime; }
                else if( physics.velocity.y >= 0f ) { if( thrust.y < Mathf.Abs( current_thrust_max.y ) ) thrust.y -= safe_acceleration.y * Time.fixedDeltaTime; }

                // Корректировка горизонтальной скорости
                thrust.x = 0f;
                if( cached_transform.position.x < (Landing_axle.x - delta_axle_x) ) { thrust.x += safe_acceleration.x * Time.fixedDeltaTime; }
                else if( cached_transform.position.x > (Landing_axle.x + delta_axle_x) ) { thrust.x -= safe_acceleration.x * Time.fixedDeltaTime; }

                // Если топливо закончилось, посадка отменяется
                if( ship.Fuel_capacity.Available == 0f ) break;
            }

            yield return autolanding_wait_for_fixed_update;
        }

        Game.Message.Hide( autolanding_message );
        Game.Canvas.RefreshAutolandingIndicator( false );

        thrust_rate = ship.Thrust_rate;
        thrust = Vector2.zero;

        physics.mass = ship.Total_ship_mass;
        physics.drag = saved_drag;
        UnityEngine.Physics.gravity = saved_gravity;

        yield break;
    }

    // Event: destroying the ship ##############################################################################################################################################
    private IEnumerator WaitBeforeDestroying( float time ) {

        while( time > 0f ) {

            time -= Time.deltaTime;

            yield return null;
        }

        Game.Canvas.ActivateRestartWindow();

        yield break;
    }

    // Создаёт утечку топлива из бака корабля ##################################################################################################################################
    public void CheckFuelLeaks() {

        if( Is( PlayerState.Destroyed ) ) return;

        // Вычисляем вероятность утечки: если недостаточная, то ничего не меняем
        if( UnityEngine.Random.Range( 0f, 1f ) > ship.Leaks_probability ) return;

        // Если утечка уже есть, усиливаем её эффект; также в любом случае выводим соответствующее сообщение
        if( ship.Current_leaks_usage > 0f ) ship.Current_leaks_rate += UnityEngine.Random.Range( 0.1f, 1.0f );
        if( ship.Current_leaks_rate == 1f ) Game.Message.Show( first_leaks_message );
        else Game.Message.Show( repeating_leaks_message );

        // Если утечка усилилась, то меняем тон звучания утечки
        if( leaks_effect != null ) leaks_effect.GetComponent<AudioSource>().pitch = (ship.Current_leaks_rate <= 2f) ? ship.Current_leaks_rate : 2f;

        // Устанавливаем степень утечки
        ship.Current_leaks_usage = ship.Fuel_max_leaks * ship.Current_leaks_rate * ship.Fuel_capacity.Maximum * ship.Fuel_capacity.Upgrade_max_game_inversed;

        // Если утечка уже была ранее, то дальше ничего не делаем (эффект уже был запущен)
        if( ship.Current_leaks_rate > 1f ) return;

        // Если утечка возникла впервые, то запускаем эффект утечки и привязываем его к месту утечки корабля, меняя его положение случайным образом
        leaks_effect = Game.Effects_control.Show( ship.Leaks_effect_prefab, fuel_leaks_point_transform.GetChild( 0 ).position, true );
        leaks_effect.transform.parent = fuel_leaks_point_transform.GetChild( 0 );
        leaks_effect.gameObject.layer = gameObject.layer;

        int angle = UnityEngine.Random.Range( 0, 3 );
        fuel_leaks_point_transform.Rotate( 0f, ((angle == 0) ? 0f : ((angle == 1) ? 90f : -90f)), 0f );
    }

    // Сохраняет параметры игрока перед закрытием приложения или выходом из уровня #############################################################################################
    private void OnDisable() {

        // Сохраняем параметры игрока
        SaveGame.Save( this );
    }
}