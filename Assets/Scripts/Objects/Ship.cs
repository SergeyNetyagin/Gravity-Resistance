using UnityEngine;
using System.Collections;

[System.Serializable]
public class ShipData {

    [System.Serializable]
    private class CompartmentItem {

        public float mass;
        public string prefab;
    }

    [System.NonSerialized]
    public bool Is_available = false;

    [System.NonSerialized]
    public bool Is_active = false;

    public float Operation_time = 0f;
    public float Current_leaks_usage = 0f;
    public float Current_leaks_rate = 0f;

    public Indicator Hull_durability;
    public Indicator Fuel_capacity;
    public Indicator Engine_thrust;
    public Indicator Hold_capacity;
    public Indicator Shield_time;
    public Indicator Shield_power;
    public Indicator Charge_time;
    public Indicator Radar_range;
    public Indicator Radar_power;
    public Indicator Autolanding_amount;

    [SerializeField, HideInInspector]
    private CompartmentItem[] items;

    [System.NonSerialized]
    private GameObject loaded_asset;

    [System.NonSerialized]
    private Value loaded_value;

    public ShipData( Ship ship ) {

        Hull_durability = new Indicator();
        Fuel_capacity = new Indicator();
        Engine_thrust = new Indicator();
        Hold_capacity = new Indicator();
        Shield_time = new Indicator();
        Shield_power = new Indicator();
        Charge_time = new Indicator();
        Radar_range = new Indicator();
        Radar_power = new Indicator();
        Autolanding_amount = new Indicator();

        items = new CompartmentItem[ ship.Compartments_maximum ];
    }

    public void Load( Ship ship ) {

        ship.SetOperationTime( Operation_time );
        ship.SetCurrentLeaksUsage( Current_leaks_usage );
        ship.SetCurrentLeaksRate( Current_leaks_rate );

        ship.Hull_durability.CopyFrom( Hull_durability );
        ship.Fuel_capacity.CopyFrom( Fuel_capacity );
        ship.Engine_thrust.CopyFrom( Engine_thrust );
        ship.Hold_capacity.CopyFrom( Hold_capacity );
        ship.Shield_time.CopyFrom( Shield_time );
        ship.Shield_power.CopyFrom( Shield_power );
        ship.Charge_time.CopyFrom( Charge_time );
        ship.Radar_range.CopyFrom( Radar_range );
        ship.Radar_power.CopyFrom( Radar_power );
        ship.Autolanding_amount.CopyFrom( Autolanding_amount );
        
        ship.SetAvailable( Is_available );
        ship.SetActive( Is_active );

        // Предварительно создаём пустые отсеки
        ship.CreateCompartments();
        
        // Восстанавливливаем содержимое трюма
        for( int i = 0; i < ship.Compartments_available; i++ ) {

            if( items[i] != null ) {

                loaded_asset = Resources.Load<GameObject>( Game.Path_values + items[i].prefab );

                if( loaded_asset != null ) {

                    loaded_value = GameObject.Instantiate<GameObject>( loaded_asset ).GetComponent<Value>();

                    if( loaded_value != null ) {

                        loaded_value.name = string.Copy( loaded_asset.name );

                        items[i].mass = Mathf.Floor( items[i].mass * 100f ) * 0.01f;
                        loaded_value.GetComponent<Rigidbody>().mass = items[i].mass;
                        loaded_value.FullMassAndCostCalculation();
                        ship.LoadToHold( loaded_value, i );
                    }
                }
            }
        }
    }

    public void Save( Ship ship ) {

        Operation_time = ship.Operation_time;
        Current_leaks_usage = ship.Current_leaks_usage;
        Current_leaks_rate = ship.Current_leaks_rate;

        Hull_durability.CopyFrom( ship.Hull_durability );
        Fuel_capacity.CopyFrom( ship.Fuel_capacity );
        Engine_thrust.CopyFrom( ship.Engine_thrust );
        Hold_capacity.CopyFrom( ship.Hold_capacity );
        Shield_time.CopyFrom( ship.Shield_time );
        Shield_power.CopyFrom( ship.Shield_power );
        Charge_time.CopyFrom( ship.Charge_time );
        Radar_range.CopyFrom( ship.Radar_range );
        Radar_power.CopyFrom( ship.Radar_power );
        Autolanding_amount.CopyFrom( ship.Autolanding_amount );
        
        Is_available = ship.Is_available;
        Is_active = ship.Is_active;

        // Сохраняем содержимое трюма
        for( int i = 0; i < ship.Compartments_available; i++ ) {

            items[i] = new CompartmentItem();

            items[i].mass = ship.GetCompartmentMass( i );
            items[i].prefab = ship.GetCompartmentPrefab( i );
        }
    }
}

public class Ship : MonoBehaviour {

    [System.Serializable]
    private class Booster {

        public ParticleSystem 
            jet_particle_system,
            glow_particle_system;

        [Range( 0.5f, 2f )]
        [Tooltip( "Значение <Start Size> из <Particle System> для главной струи двигателя: по умолчанию = 1f" )]
        public float jet_max_size = 1f;

        [Range( 1f, 10f )]
        [Tooltip( "Значение <Start Size> из <Particle System> для эффекта свечения от двигателя: по умолчанию = 5f" )]
        public float glow_max_size = 5f;

        [Range( 1f, 2f )]
        [Tooltip( "Минимальная длина коллайдера двигателя: по умолчанию = 1.85f" )]
        public float max_center_z_collider = 1.85f;

        [Range( 4f, 5f )]
        [Tooltip( "Макимальная длина коллайдера двигателя: по умолчанию = 4.55f" )]
        public float max_size_z_collider = 4.55f;

        [HideInInspector]
        public float 
            smooth_jet_size = 0f,
            current_jet_size = 0f,
            smooth_glow_size = 0f,
            current_glow_size = 0f;

        [HideInInspector]
        public ParticleSystem.EmissionModule 
            jet_emission_module,
            glow_emission_module;

        [HideInInspector]
        public BoxCollider jet_collider;

        [HideInInspector]
        public Vector3 
            center_box,
            size_box;
    }

    [System.Serializable]
    private class Support {

        public Collider support_collider;

        [HideInInspector]
        public bool on_surface = false;
    }

    private class Compartment {

        public Value value;
        public float mass;
        public string prefab;
    }

    #region SHIP_MAIN_PARAMETERS
    [Header( "ИДЕНТИФИКАТОРЫ КОРАБЛЯ" )]
    [Tooltip( "Уникальный идентификатор типа корабля (стартовый тип = 0): тип каждого более совершенного судна больше предыдущего на 1" )]
    [SerializeField]
    private ShipType type;
    public ShipType Type { get { return type; } }

    [SerializeField]
    [Tooltip( "Рисунок данного корабля, который будет затем отображаться в меню (необходимы размеры 128 x 128 пикселей)" )]
    private Sprite picture;
    public Sprite Picture { get { return picture; } }

    [Tooltip( "Ключ класса корабля (основное предназначение), пишется с большой буквы (например: ИССЛЕДОВАТЕЛЬСКИЙ КОРАБЛЬ, ГРУЗОВОЙ КОРАБЛЬ, РАЗВЕДЫВАТЕЛЬНЫЙ КОРАБЛЬ)" )]
    [SerializeField]
    private string grade_key;
    public string Grade_key { get { return grade_key; } }

    [Tooltip( "Ключ названия (или типа) корабля, пишется с большой буквы" )]
    [SerializeField]
    private string type_key;
    public string Type_key { get { return type_key; } }

    [Tooltip( "Ключ описания корабля данного класса" )]
    [SerializeField]
    private string description_key;
    public string Description_key { get { return description_key; } }

    [Header( "ТЕХНИЧЕСКИЕ ПАРАМЕТРЫ КОРАБЛЯ" )]
    [Tooltip( "Коэффициент тяги двигателей корабля для более реалистичного движения; по умолчанию для корабля тип 0 = Vector2( 0.04f, 0.04f )" )]
    [SerializeField]
    private Vector2 thrust_rate = new Vector2( 0.04f, 0.04f );
    public Vector2 Thrust_rate { get { return thrust_rate; } }
            
    [Range( 5f, 50f )]
    [Tooltip( "Масса пустого корабля без топлива и груза, тонн; по умолчанию масса корабля тип 1 равна 5 тонн" )]
    public float Empty_mass = 5f;

    [Range( 0.05f, 10f )]
    [Tooltip( "Расход топлива для пустого корабля при максимально возможной тяге, тонн в секунду; по умолчанию для корабля тип 1 равен 0.3 тонн в секунду" )]
    public float Fuel_thrust_usage = 0.3f;

    [Range( 0.05f, 1f )]
    [Tooltip( "Дополнительный расход топлива во время зарядки защитного поля при его максимальной мощности, тонн в секунду; по умолчанию для корабля тип 1 равен 0.2 тонны в секунду" )]
    public float Fuel_charge_usage = 0.2f;

    [Range( 0.05f, 1f )]
    [Tooltip( "Дополнительный расход топлива во время работы радара при его максимальной мощности, тонн в секунду; по умолчанию = 0.1 тонны в секунду" )]
    public float Fuel_radar_usage = 0.1f;

    [Range( 0.05f, 0.5f )]
    [Tooltip( "Максимально возможные утечки топлива корабля в случае повреждения бака, тонн в секунду; по умолчанию = 0.2 тонны в секунду" )]
    [SerializeField]
    private float fuel_max_leaks = 0.2f;
    public float Fuel_max_leaks { get { return fuel_max_leaks; } }

    [SerializeField]
    [Tooltip( "Какова вероятность возникновения утечки топлива после сильных столкновений; по умолчанию = 0.2f" )]
    [Range( 0f, 1f )]
    private float leaks_probability = 0.2f;
    public float Leaks_probability { get { return leaks_probability; } }

    [SerializeField] 
    [Tooltip( "Эффект утечки топлива из бака корабля" )]
    private Effect leaks_effect_prefab;
    public Effect Leaks_effect_prefab { get { return leaks_effect_prefab;} }

    [SerializeField]
    [Tooltip( "Ось (у которой имеется дочерняя точка) для позиции появления утечки после столкновения (ось вращается случайным образом так, чтобы утечка могла появится слева, справа и позади корабля)" )]
    private Transform fuel_leaks_axle_transform;
    public Transform Fuel_leaks_axle_transform { get { return fuel_leaks_axle_transform; } }

    private float current_leaks_usage = 0f;
    public float Current_leaks_usage { get { return current_leaks_usage; } set { if( value > fuel_max_leaks ) value = fuel_max_leaks; current_leaks_usage = value; } }
    public void SetCurrentLeaksUsage( float value ) { if( value > fuel_max_leaks ) value = fuel_max_leaks; current_leaks_usage = value; }

    private float current_leaks_rate = 0f;
    public float Current_leaks_rate { get { return current_leaks_rate; } set { current_leaks_rate = value; } }
    public void SetCurrentLeaksRate( float value ) { current_leaks_rate = value; }

    [Range( 0.1f, 1.0f )]
    [Tooltip( "Коэффициент расхода топлива по отношению к максимальной тяге во время автопосадки; по умолчанию = 0.3" )]
    public float Fuel_auto_rate = 0.3f;

    [Tooltip( "Максимальная скорость корабля (предотвращает бесконечный рост скорости при ускорении), км/с: по умолчанию = 0.5" )]
    [Range( 0.1f, 1.0f )]
    public float Max_speed = 0.5f;

    [Header( "ЭКОНОМИЧЕСКИЕ ПАРАМЕТРЫ КОРАБЛЯ" )]
    [Tooltip( "Стоимость покупки судна (цена продажи зависит от кривой торговой амортизации судна PriceSellCurve); по стоимость корабля тип 1 равна 100 000 (класс 0 - бесплатно)" )]
    [SerializeField]
    private float price_buy;
    public float Price_buy { get { return price_buy; } }

    [SerializeField]
    [Tooltip( "Кривая падения стоимости корабля в зависимости от игрового времени владения им; график строится в квадрате 1.0 x 1.0" )]
    private AnimationCurve price_sell_rate;

    [Range( 1f, 100f )]
    [Tooltip( "Максимальное число игровых часов, за которое цена продажи корабля достигает минимального значения кривой амортизации; по умолчанию = 10" )]
    private float max_sell_hours = 10f;
    private float max_sell_hours_inversed = -1f;
    public float Max_sell_hours_inversed { get { return (max_sell_hours_inversed == -1f) ? (max_sell_hours_inversed = (1f / max_sell_hours)) : max_sell_hours_inversed; } }

    [HideInInspector]
    private float operation_time = 0f;
    public  float Operation_time { get { return operation_time; } }
    public void SetOperationTime( float time ) { operation_time = time; }
    public float Operation_hours { get { return operation_time * hour_inversed; } }

    public float Price_sell { get {

        float price_sell = 0f;

        if( Operation_hours >= max_sell_hours ) price_sell = Mathf.Floor( Min_sell_price * Operability_rate * 0.01f ) * 100f;
        else price_sell = Mathf.Floor( Price_buy * price_sell_rate.Evaluate( Operation_hours * Max_sell_hours_inversed ) * Operability_rate * 0.01f ) * 100f;

        return price_sell;
    } }

    private float max_sell_price = -1f;
    public float Max_sell_price { get { return (max_sell_price == -1f) ? (max_sell_price = Price_buy * price_sell_rate.keys[0].value) : max_sell_price; } }

    private float min_sell_price = -1f;
    public float Min_sell_price { get { return (min_sell_price == -1f) ? (min_sell_price = Price_buy * price_sell_rate.keys[price_sell_rate.length-1].value) : min_sell_price; } }

    public float Operability_rate { get {

        float operability = 1f;

        operability *= Hull_durability.Available * Hull_durability.Maximum_inversed;
        operability *= Engine_thrust.Available * Engine_thrust.Maximum_inversed;
        operability *= 1f - Current_leaks_usage;

        return operability;
    } }

    private const float hour_inversed = 1f / 3600f;
    #endregion

    #region SHIP_GEOMETRY_PARAMETERS
    [Header( "НАСТРОЙКИ, ЗАВИСЯЩИЕ ОТ РАЗМЕРОВ КОРАБЛЯ" )]
    [SerializeField]
    [Tooltip( "Высота корабля над платформой станции перед посадкой, если он на старте должен появиться сразу на платформе; по умолчанию = 1" )]
    [Range( 0f, 2f )]
    private float altitude_sit = 1f;
    public float Altitude_sit { get { return altitude_sit; } }

    [SerializeField]
    [Tooltip( "Высота корабля над станцией посадки, если он на старте должен сесть на её платформу в режиме автопосадки; по умолчанию = 7" )]
    [Range( 1f, 10f )]
    private float altitude_autolanding = 7f;
    public float Altitude_autolanding { get { return altitude_autolanding; } }

    [SerializeField]
    [Tooltip( "Центральная точка, которая будет использована в качестве центра для отображения колец навигатора / радара" )]
    private Transform navigator_point_transform;
    public Transform Navigator_point_transform { get { return navigator_point_transform; } }

    [SerializeField]
    [Tooltip( "Точка сброса грузов из трюма - место возле корабля, где появляется выброшенный по инициативе игрока груз" )]
    private Transform hold_drop_point_transform;
    public Transform Hold_drop_point_transform { get { return hold_drop_point_transform; } }

    [Header( "НАСТРОЙКИ РАЗМЕРОВ ЭЛЕМЕНТОВ НАВИГАТОРА" )]
    [SerializeField]
    [Tooltip( "Визуальный радиус главного кольца навигатора вокруг корабля в пикселях; по умолчанию = 165" )]
    [Range( 100, 300f )]
    private float main_ring_radius = 165f;

    [SerializeField]
    [Tooltip( "Визуальный радиус сегментов главного кольца навигатора вокруг корабля в пикселях; по умолчанию = 175" )]
    [Range( 100, 300f )]
    private float main_ring_childs_radius = 175f;

    [SerializeField]
    [Tooltip( "Визуальный радиус кросс-кольца навигатора вокруг корабля в пикселях; по умолчанию = 145" )]
    [Range( 100, 300f )]
    private float cross_ring_radius = 145f;

    [SerializeField]
    [Tooltip( "Визуальный радиус сегментов кросс-кольца навигатора вокруг корабля в пикселях; по умолчанию = 155" )]
    [Range( 100, 300f )]
    private float cross_ring_childs_radius = 155f;

    [SerializeField]
    [Tooltip( "Визуальное расстояние от центра навигатора до указателей дистанции в пикселях; по умолчанию = 115" )]
    [Range( 100, 300f )]
    private float arrows_radius = 115f;
    #endregion

    #region SHIP_INDICATORS
    [Header( "БАЗОВЫЕ ХАРАКТЕРИСТИКИ СИСТЕМ КОРАБЛЯ" )]
    [Tooltip( "Прочность корпуса, МПа: минимум = 100; максимум = 2000" )]
    [Space( 10 )]
    public Indicator Hull_durability;

    [Tooltip( "Объём топливных баков, тонн: минимум = 10; максимум = 100 (обычно отношение массы топлива составляет 3/1; в игре принято примерно 2/1 или 3/1)" )]
    [Space( 10 )]
    public Indicator Fuel_capacity;

    [Tooltip( "Тяга двигателя, кН: минимум = 10; максимум = 200 (как правило, тяга в кН должна быть в 5 раз больше полной массы в тоннах)" )]
    [Space( 10 )]
    public Indicator Engine_thrust;

    [Tooltip( "Объём трюма (не подлежит апгрэйду), тонн: минимум = 1; максимум = 20" )]
    [Space( 10 )]
    public Indicator Hold_capacity;

    [Tooltip( "Время работы защитного экрана, сек: минимум = 2; максимум = 20" )]
    [Space( 10 )]
    public Indicator Shield_time;

    [Tooltip( "Мощность защитного экрана, МВт: минимум = 1; максимум = 10" )]
    [Space( 10 )]
    public Indicator Shield_power;

    [Tooltip( "Время зарядки защитного экрана, сек: минимум = 20; максимум = 200" )]
    [Space( 10 )]
    public Indicator Charge_time;

    [Tooltip( "Дистанция обнаружения радаром каких-либо объектов, км: минимум = 0; максимум = 5" )]
    [Space( 10 )]
    public Indicator Radar_range;

    [Tooltip( "Мощность радара (влияет на число обнаруживаемых типов минералов), кВт: минимум = 5 кВт, максимум = 100 кВт" )]
    [Space( 10 )]
    public Indicator Radar_power;

    [Tooltip( "Количество лицензий на автопосадку для данного корабля, посадок: минимум = 0; максимум = 10" )]
    [Space( 10 )]
    public Indicator Autolanding_amount;
    #endregion

    #region SHIP_BOOSTERS
    [Header( "НАСТРОЙКИ ЭФФЕКТОВ И ЭМИССИИ ЧАСТИЦ ДВИГАТЕЛЕЙ" )]
    [Space( 10 )]
    [SerializeField]
    private Booster Booster_up;
    public Vector3 Engine_point { get { return Booster_up.jet_particle_system.transform.position; } }

    [Space( 10 )]
    [SerializeField]
    private Booster Booster_down;

    [Space( 10 )]
    [SerializeField]
    private Booster Booster_left;

    [Space( 10 )]
    [SerializeField]
    private Booster Booster_right;

    [SerializeField]
    [Space( 10 )]
    private Light pointlight;

    [SerializeField]
    [Range( 5f, 15f )]
    private float pointlight_max_range = 10f;

    [SerializeField]
    [Range( 1.5f, 10f )]
    private float pointlight_acceleration = 2f;
    #endregion

    #region SHIP_ADDITIONAL_PROPERTIES
    [Header( "НАСТРОЙКИ ОПОР КОРАБЛЯ" )]
    [SerializeField]
    private Support[] supports;
    public int Supports_amount { get { return supports.Length; } }

    public void AttachSupport( Collider collider ) { for( int i = 0; i < supports.Length; i++ ) if( supports[i].support_collider == collider ) { supports[i].on_surface = true; break; } }
    public void DettachSupport( Collider collider ) { for( int i = 0; i < supports.Length; i++ ) if( supports[i].support_collider == collider ) { supports[i].on_surface = false; break; } }
    public bool All_supports_attached { get { int amount = 0; for( int i = 0; i < supports.Length; i++ ) if( supports[i].on_surface ) amount++; return (amount == supports.Length) ? true : false; } }
    public bool All_supports_dettached { get { int amount = 0; for( int i = 0; i < supports.Length; i++ ) if( !supports[i].on_surface ) amount++; return (amount == supports.Length) ? true : false; } }
    public void DettachAllSupports() { for( int i = 0; i < supports.Length; i++ ) supports[i].on_surface = false; }

    [Header( "НАСТРОЙКИ ЗВУЧАНИЯ ДВИГАТЕЛЯ" )]
    [SerializeField]
    [Space( 10 )]
    [Tooltip( "Минимальная высота звука двигателя (при нулевой тяге); по умолчанию = 0.1" )]
    [Range( 0.1f, 2f )]
    private float min_pitch = 0.1f;
    public float Min_pitch { get { return min_pitch; } }

    [SerializeField]
    [Tooltip( "Максимальная разность тонов двигателя от нулевой до максимальной тяги в обычном состоянии; по умолчанию = 1.0" )]
    [Range( 0.1f, 1f )]
    private float max_delta_pitch_stable = 1.0f;
    public float Max_delta_pitch_stable { get { return max_delta_pitch_stable; } }

    [SerializeField]
    [Tooltip( "Максимальная разность тонов двигателя от нулевой до максимальной тяги в условиях аномалий; по умолчанию = 1.5" )]
    [Range( 0.2f, 2f )]
    private float max_delta_pitch_shaked = 1.5f;
    public float Max_delta_pitch_shaked { get { return max_delta_pitch_shaked; } }

    [HideInInspector]
    private bool protected_by_station = false;
    public bool Protected_by_station { get { return protected_by_station; } }
    public bool Protected_by_shield { get { return shield_control.Is_active; } }

    private bool is_player = false;
    public bool Is_player { get { return is_player; } }

    [System.NonSerialized]
    private float
        booster_max_power = 0f,

        glow_to_booster_rate = 0f,
        center_to_booster_rate = 0f,
        size_to_booster_rate = 0f,

        pointlight_range_to_booster_rate = 0f,

        spotlight_range_to_booster_rate = 0f,
        spotlight_intencity_to_booster_rate = 0f;

    private const float
        jet_off_size = 0.05f,
        lerp_jet_on_rate = 0.5f,
        lerp_jet_off_rate = 2f,
        collider_min_size = 0.05f,
        collider_min_center = 0.1f;

    private ShieldControl shield_control;

    private Transform cached_transform;

    private Collider[] ship_colliders;
    private bool[] protected_colliders;

    private Animator animator;
    public bool Is_animation_mode { get { return !animator.GetBool( animation_ID_complete ); } }

    private int animation_ID_complete;
    public int Animation_ID_complete { get { return animation_ID_complete; } }

    private int animation_ID_ship_sit;
    public int Animation_ID_ship_sit { get { return animation_ID_ship_sit; } }

    private int animation_ID_ship_landing;
    public int Animation_ID_ship_landing { get { return animation_ID_ship_landing; } }
    
    public void SupportsSitUp( float speed = 1f ) {

        Game.Player.Ship_animator.speed = speed;

        animator.SetInteger( animation_ID_ship_sit, -1 );
        animator.SetInteger( animation_ID_ship_landing, 0 );
    }

    public void SupportsSitDown( float speed = 1f ) {

        Game.Player.Ship_animator.speed = speed;

        animator.SetInteger( animation_ID_ship_sit, 1 );
        animator.SetInteger( animation_ID_ship_landing, 0 );
    }

    public void SupportsLandingUp( float speed = 1f ) {

        Game.Player.Ship_animator.speed = speed;

        animator.SetInteger( animation_ID_ship_sit, 0 );
        animator.SetInteger( animation_ID_ship_landing, 0 );
    }

    public void SupportsLandingDown( float speed = 1f ) {

        Game.Player.Ship_animator.speed = speed;

        animator.SetInteger( animation_ID_ship_sit, 0 );
        animator.SetInteger( animation_ID_ship_landing, 1 );
    }

    [System.NonSerialized]
    private float width = 1.5f;
    public float Width { get { return width; } }
    
    [System.NonSerialized]
    private float height = 2.0f;
    public float Height { get { return height; } }

    [System.NonSerialized]
    private float length = 1.5f;
    public float Length { get { return length; } }

    [System.NonSerialized]
    private float radius = 1.4f;
    public float Radius { get { return radius; } }

    private WaitForSeconds operation_time_wait_for_seconds = new WaitForSeconds( 1f );
    #endregion

    #region SHIP_HOLD_COMPARTMENTS
    // Содержимое трюма корабля
    [System.NonSerialized]
    private Compartment[] compartments;

    [System.NonSerialized]
    private int compartments_loaded = 0;
    public int Compartments_loaded { get { return compartments_loaded; } }

    [System.NonSerialized]
    private int compartments_available = 0;
    public int Compartments_available { get { return compartments_available; } }
    [System.NonSerialized]
    private float compartments_available_inversed = -1f;
    public float Compartments_available_inversed { get { return (compartments_available_inversed == -1f) ? (1f / compartments_available) : compartments_available_inversed; } }

    [System.NonSerialized]
    private int compartments_maximum = 0;
    public int Compartments_maximum { get { return compartments_maximum; } }
    [System.NonSerialized]
    private float compartments_maximum_inversed = -1f;
    public float Compartments_maximum_inversed { get { return (compartments_maximum_inversed == -1f) ? (1f / compartments_maximum) : compartments_maximum_inversed; } }

    public float Total_ship_mass { get { return (Empty_mass + Fuel_capacity.Available + Total_loads_mass); } }

    public bool IsEmptyCompartment( int i ) { return (compartments[i].value == null); }
    public bool IsAbsentCompartment( int i ) { return (i >= compartments_available); }

    public float GetCompartmentMass( int i ) { return compartments[i].mass; }
    public string GetCompartmentPrefab( int i ) { return compartments[i].prefab; }
    public Value GetCompartmentValue( int i ) { return compartments[i].value; }
    public Sprite GetCompartmentPicture( int i ) { return (compartments[i].value == null) ? null : compartments[i].value.Picture; }

    // Служит для создания отсеков (например, необходим перед чтением данных из файла) #############################################################################################
    public void CreateCompartments() {

        compartments_available = Mathf.FloorToInt( Hold_capacity.Maximum * Hold_capacity.Unit_size_inversed );
        compartments_maximum = Mathf.FloorToInt( Hold_capacity.Upgrade_max_ship * Hold_capacity.Unit_size_inversed );

        compartments = new Compartment[ compartments_maximum ];

        for( int i = 0; i < compartments.Length; i++ ) {

            compartments[i] = new Compartment();
            compartments[i].value = null;
            compartments[i].mass = 0f;
            compartments[i].prefab = string.Empty;
        }
    }

    // Полностью очищает трюм и выбрасывает грузы за борт (если они там есть) ######################################################################################################
    public void ClearHold() {

        if( (compartments == null) || (compartments.Length == 0) ) return;

        for( int i = 0; i < compartments.Length; i++ ) {

            if( compartments[i].value != null ) compartments[i].value.WakeUp( hold_drop_point_transform );

            compartments[i].value = null;
            compartments[i].mass = 0f;
            compartments[i].prefab = string.Empty;
        }

        compartments_loaded = 0;
    }

    // Перераспределяет грузы в трюме в соответствии с расположением в инвентаре ###################################################################################################
    public void RefreshHold( InventoryPanel[] panels ) {

        compartments_loaded = 0;

        int length = (panels.Length < compartments_maximum) ? panels.Length : compartments_maximum;

        for( int i = 0; i < length; i++ ) {

            compartments[i].value = panels[i].Item.Value;
            compartments[i].mass = (compartments[i].value != null) ? panels[i].Item.Value.Total_mass_in_tons : 0f;
            compartments[i].prefab = (compartments[i].value != null) ? string.Copy( panels[i].Item.Value.gameObject.name ) : string.Empty;

            if( compartments[i].value != null ) compartments_loaded++;
        }
    }

    // Возвращает суммарную стоимость всех грузов в трюме ##########################################################################################################################
    public float Total_loads_cost { get {

        float cost = 0f;

        for( int i = 0; i < compartments_available; i++ ) if( compartments[i].value != null ) cost += compartments[i].value.Total_cost;

        return cost;
    } }

    // Возвращает суммарную массу всех грузов в трюме ##############################################################################################################################
    public float Total_loads_mass { get {

        float mass = 0f;

        for( int i = 0; i < compartments_available; i++ ) if( compartments[i].value != null ) mass += compartments[i].value.Total_mass_in_tons;

        return mass;
    } }
    
    // Есть ли в трюме миссионный груз? ############################################################################################################################################          
    public bool Has_mission_load { get {

        for( int i = 0; i < compartments_available; i++ ) if( (compartments[i].value != null) && compartments[i].value.Is_mission ) return true;

        return false;
    } }
    
    // Есть ли в трюме скоропортящийся груз? #######################################################################################################################################
    public bool Has_perishable_load { get {

        for( int i = 0; i < compartments_available; i++ ) if( (compartments[i].value != null) && Game.Control.IsPerishable( compartments[i].value.Subject_type ) ) return true;

        return false;
    } }
    
    // Есть ли в трюме взрывоопасный груз? #########################################################################################################################################
    public bool Has_explosive_load { get {
        
        for( int i = 0; i < compartments_available; i++ ) if( (compartments[i].value != null) && Game.Control.IsExplosive( compartments[i].value.Subject_type ) ) return true;

        return false;
    } }

    // Загружен ли уже именно этот груз в трюм или нет? ############################################################################################################################
    public bool HasIntoHold( Value value ) {

        for( int i = 0; i < compartments_available; i++ ) if( (compartments[i].value != null) && (compartments[i].value == value) ) return true;

        return false;
    }
    
    // Поместить указанный груз в трюм #############################################################################################################################################
    public bool LoadToHold( Value value, int concrete_index = 0 ) {

        if( value == null ) return false;
        if( HasIntoHold( value ) ) return false;

        // Если в трюме больше нет свободных отсеков
        if( compartments_loaded == compartments_available ) {

            Game.Message.Show( value.Hold_limit_message_key );
            return false;
        }

        // Если масса груза больше, чем максимальная вместимость отсека
        if( value.Total_mass_in_tons > Hold_capacity.Unit_size ) {

            Game.Message.Show( value.Compartment_limit_message_key );
            return false;
        }

        // Если груз является миссионным, активируем ноую точку миссии; иначе просто сообщаем об успешной загрузке
        if( value.Is_mission ) Game.Scenario_control.ActivateMissionDestination();
        else Game.Message.Show( value.Loading_message_key ); 

        // Деактивируем объект на время нахождения внутри трюма
        value.Sleep( true );

        // Помещаем груз в пустой отсек трюма
        for( int i = concrete_index; i < compartments_available; i++ ) {

            if( compartments[i].value == null ) {

                compartments[i].value = value;
                compartments[i].mass = value.Total_mass_in_tons;
                compartments[i].prefab = string.Copy( value.gameObject.name );

                compartments_loaded++;
                Hold_capacity.Available = Hold_capacity.Unit_size * ((float) compartments_loaded);
                break;
            }
        }

        // Освобождаем радар от данного груза в качестве цели
        Game.Radar.RemoveAsTarget( value.Cached_transform );

        // Сообщаем об успешной загрузке объекта в трюм
        return true;
    }
    
    // Выгрузить указанный груз из трюма (можно просто выпросить или же это выгрузка в процессе его продажи) #######################################################################
    public bool UnloadFromHold( Value value, bool is_dropped ) {

        if( value == null ) return false;
        if( !HasIntoHold( value ) ) return false;

        // Если груз был выброшен из трюма
        if( is_dropped ) {

            if( value.Is_mission ) Game.Scenario_control.DropMissionLoad( value );

            else {

                Game.Message.Show( value.Drop_message_key );
                value.WakeUp( hold_drop_point_transform );
            }
        }

        // Если груз был продан на станции или миссия была успешно завершена
        else {

            if( value.Is_mission ) Game.Scenario_control.MissionAccomplished();

            else {

                Game.Message.Show( value.Sell_message_key );
                value.Sleep( false );
            }
        }

        // Удаляем груз из отсека трюма
        for( int i = 0; i < compartments_available; i++ ) {

            if( compartments[i].value == value ) {

                compartments[i].value = null;
                compartments[i].mass = 0f;
                compartments[i].prefab = string.Empty;

                compartments_loaded--;
                Hold_capacity.Available = Hold_capacity.Unit_size * ((float) compartments_loaded);
                break;
            }
        }

        // Сообщаем об успешной выгрузке объекта из трюма
        return true;
    }
    #endregion

    // Истина, если корабль имеется в наличии у игрока
    [System.NonSerialized]
    private bool is_available = false;
    public bool Is_available { get { return (type == ShipType.Ship_00_Midge) ? true : is_available; } }
    public void SetAvailable( bool state ) { is_available = state; was_changed = true; }
    
    // Истина, если последний раз именно он использовался в игре или был выбран в главном меню
    [System.NonSerialized]
    private bool is_active = false;
    public bool Is_active { get { return is_active; } }
    public void SetActive( bool state ) { is_active = state; was_changed = true; }
    
    // Служит для предотвращения большого объёма записей кораблей в главном меню (если в главном меню корабль не изменялся, его не нужно снова перезаписывать)
    [System.NonSerialized]
    private bool was_changed = false;
    public bool Was_changed { get { return was_changed; } }

    [System.NonSerialized]
    private bool was_saved = false;

    // Starting initialization #####################################################################################################################################################
    void Start() {

        if( cached_transform == null ) StartingInitialization( gameObject.layer );

        if( Game.Current_level > LevelType.Level_Menu ) StartCoroutine( UpdateCoroutine() );
    }
    
    // Starting initialization #####################################################################################################################################################
    public void StartingInitialization( int layer ) {

        cached_transform = transform;

        animator = GetComponent<Animator>();

        shield_control = GetComponent<ShieldControl>();
        is_player = (GetComponentInParent<Player>() == null) ? false : true;

        // Устанавливаем физический слой
        gameObject.layer = layer;
        Transform[] childs = GetComponentsInChildren<Transform>( true );
        for( int i = 0; i < childs.Length; i++ ) if( !childs[i].CompareTag( "Jet" ) ) childs[i].gameObject.layer = layer;

        // Если потребуется (то есть если будут возникать ненужные взаимодействия между кораблём и двигателем), 
        // можно будет в настройках частиц каждого двигателя отключать коллизию между этим двигателем и кораблём

        pointlight_range_to_booster_rate = pointlight_max_range / Booster_up.jet_max_size;

        glow_to_booster_rate = Booster_up.glow_max_size / Booster_up.jet_max_size;
        center_to_booster_rate = Booster_up.max_center_z_collider / Booster_up.jet_max_size;
        size_to_booster_rate = Booster_up.max_size_z_collider / Booster_up.jet_max_size;

        animation_ID_complete = Animator.StringToHash( "Complete" );
        animation_ID_ship_sit = Animator.StringToHash( "Ship_sit" );
        animation_ID_ship_landing = Animator.StringToHash( "Ship_landing" );
        
        // Initialize emission modules
        Booster_up.jet_emission_module = Booster_up.jet_particle_system.emission;
        Booster_down.jet_emission_module = Booster_down.jet_particle_system.emission;
        Booster_left.jet_emission_module = Booster_left.jet_particle_system.emission;
        Booster_right.jet_emission_module = Booster_right.jet_particle_system.emission;

        Booster_up.glow_emission_module = Booster_up.glow_particle_system.emission;
        Booster_down.glow_emission_module = Booster_down.glow_particle_system.emission;
        Booster_left.glow_emission_module = Booster_left.glow_particle_system.emission;
        Booster_right.glow_emission_module = Booster_right.glow_particle_system.emission;

        Booster_up.jet_emission_module.enabled = false;
        Booster_down.jet_emission_module.enabled = false;
        Booster_left.jet_emission_module.enabled = false;
        Booster_right.jet_emission_module.enabled = false;

        Booster_up.glow_emission_module.enabled = false;
        Booster_down.glow_emission_module.enabled = false;
        Booster_left.glow_emission_module.enabled = false;
        Booster_right.glow_emission_module.enabled = false;

        // Initialize jet colliders
        Booster_up.jet_collider = Booster_up.jet_particle_system.GetComponent<BoxCollider>();
        Booster_down.jet_collider = Booster_down.jet_particle_system.GetComponent<BoxCollider>();
        Booster_left.jet_collider = Booster_left.jet_particle_system.GetComponent<BoxCollider>();
        Booster_right.jet_collider = Booster_right.jet_particle_system.GetComponent<BoxCollider>();

        Booster_up.center_box = Booster_up.jet_collider.center;
        Booster_up.size_box = Booster_up.jet_collider.size;
        Booster_down.center_box = Booster_down.jet_collider.center;
        Booster_down.size_box = Booster_down.jet_collider.size;

        Booster_left.center_box = Booster_left.jet_collider.center;
        Booster_left.size_box = Booster_left.jet_collider.size;
        Booster_right.center_box = Booster_right.jet_collider.center;
        Booster_right.size_box = Booster_right.jet_collider.size;

        // Initialize colliders
        CollectInternalColliders();
        IgnoreInternalCollisions();

        // Меняем визуальный радиус колец навигатора, если это корабль игрока (а не бот, или корабль может просто использоваться в рекламных целях)
        if( is_player ) Game.Navigator.SetRingRadius( main_ring_radius, main_ring_childs_radius, cross_ring_radius, cross_ring_childs_radius, arrows_radius );

        // Загружаем данные о текущем состоянии корабля
        if( Game.Current_level > LevelType.Level_Menu ) LoadGame.Load( this );

        // Если в трюм ничего не загружено из файла, то создаём пустые отсеки
        if( (compartments == null) || (compartments.Length == 0) ) CreateCompartments();
    }

    // Метод в основном необходим для того, чтобы считать время эксплуатации корабля ###############################################################################################
    IEnumerator UpdateCoroutine() {

        float refresh_time = Time.time;

        while( Game.Current_level > LevelType.Level_Menu ) {

            // Перерасчёт времени эксплуатации корабля (влияет на его цену продажи)
            refresh_time = Time.time - refresh_time;
            operation_time += refresh_time;
            refresh_time = Time.time;

            yield return operation_time_wait_for_seconds;
        }

        yield break;
    }

    // Ignore the internal ship's collisions #######################################################################################################################################
    void CollectInternalColliders() {

        // Временно отключаем объекты с двигателями, чтобы НЕ собирать их коллайдеры
        Booster_up.jet_collider.gameObject.SetActive( false );
        Booster_down.jet_collider.gameObject.SetActive( false );
        Booster_left.jet_collider.gameObject.SetActive( false );
        Booster_right.jet_collider.gameObject.SetActive( false );

        ship_colliders = gameObject.GetComponentsInChildren<Collider>();

        protected_colliders = new bool[ ship_colliders.Length ]; 
        for( int i = 0; i < ship_colliders.Length; i++ ) protected_colliders[i] = false;

        // Определяем габариты корабля (это необходимо для правильной работы радара; может ещё где-то понадобиться)
        Vector3 min_point = Game.Max_vector;
        Vector3 max_point = Game.Min_vector;
        Vector3 current_point;

        for( int i = 0; i < ship_colliders.Length; i++ ) {

            current_point = ship_colliders[i].bounds.center - ship_colliders[i].bounds.extents;

            if( current_point.x < min_point.x ) min_point.x = current_point.x;
            if( current_point.y < min_point.y ) min_point.y = current_point.y;
            if( current_point.z < min_point.z ) min_point.z = current_point.z;

            current_point = ship_colliders[i].bounds.center + ship_colliders[i].bounds.extents;

            if( current_point.x > max_point.x ) max_point.x = current_point.x;
            if( current_point.y > max_point.y ) max_point.y = current_point.y;
            if( current_point.z > max_point.z ) max_point.z = current_point.z;
        }

        width = max_point.x - min_point.x;
        height = max_point.y - min_point.y;
        length = max_point.z - min_point.z;
        radius = (width + height) * 0.3f;

        // Включаем объекты с двигателями, поскольку мы ранее их отключили
        Booster_up.jet_collider.gameObject.SetActive( true );
        Booster_down.jet_collider.gameObject.SetActive( true );
        Booster_left.jet_collider.gameObject.SetActive( true );
        Booster_right.jet_collider.gameObject.SetActive( true );
    }

    // Ignore the internal ship's collisions #######################################################################################################################################
    void IgnoreInternalCollisions() {

        // Это пустой метод на случай, если придётся дополнительно исключать какие-то коллизии
        // Пока в нём нет необходимости, поскольку все вопросы пока разрешаются при помощи настроек физических слоёв
    }

    // Make this collider as protected collider ####################################################################################################################################
    public void MakeAsProtectedCollider( Collider collider ) {

        protected_by_station = true;

        for( int i = 0; i < ship_colliders.Length; i++ ) {

            if( ship_colliders[i] == collider ) protected_colliders[i] = true;
            else if( protected_colliders[i] == false ) protected_by_station = false;
        }
    }

    // Make this collider as unprotected collider ##################################################################################################################################
    public void MakeAsUnprotectedCollider( Collider collider ) {

        protected_by_station = true;

        for( int i = 0; i < ship_colliders.Length; i++ ) {

            if( ship_colliders[i] == collider ) protected_colliders[i] = protected_by_station = false;
            else if( protected_colliders[i] == false ) protected_by_station = false;
        }
    }

    // Return true, if the ship has any protection colliders #######################################################################################################################
    public bool HasAnyProtectedColliders() {

        bool has_protection_collider = false;

        for( int i = 0; i < ship_colliders.Length; i++ ) {

            if( protected_colliders[i] == true ) { has_protection_collider = true; break; }
        }

        return has_protection_collider;
    }

    // Ignore the external ship's collisions #######################################################################################################################################
    public void IgnoreExternalCollision( Collider collider ) {

        for( int i = 0; i < ship_colliders.Length; i++ ) Physics.IgnoreCollision( ship_colliders[i], collider );
    }
    
    // Ignore the external ship's collisions #######################################################################################################################################
    public void ConsiderExternalCollider( Collider collider ) {

        for( int i = 0; i < ship_colliders.Length; i++ ) Physics.IgnoreCollision( ship_colliders[i], collider, false );
    }
    
    // Returns a specified indicator ###############################################################################################################################################
    public Indicator GetIndicator( IndicatorType indicator_type ) {

        switch( indicator_type ) {

            case IndicatorType.Hull_durability: return Hull_durability;
            case IndicatorType.Fuel_capacity: return Fuel_capacity;
            case IndicatorType.Engine_thrust: return Engine_thrust;
            case IndicatorType.Hold_capacty: return Hold_capacity;
            case IndicatorType.Shield_time: return Shield_time;
            case IndicatorType.Shield_power: return Shield_power;
            case IndicatorType.Charge_time: return Charge_time;
            case IndicatorType.Radar_range: return Radar_range;
            case IndicatorType.Radar_power: return Radar_power;
            case IndicatorType.Autolanding_amount: return Autolanding_amount;
        }

        return null;
    }

    // Show the vertical jet #######################################################################################################################################################
    public void ControlJetVertical( float thrust_rate ) {

        ControlEngineLight();

        // If the thrust is great than zero
        if( thrust_rate > 0f ) {

            ControlBoosterDown();

            Booster_up.current_jet_size =  Mathf.Abs( thrust_rate ) * Booster_up.jet_max_size;
            Booster_up.smooth_jet_size = Mathf.Lerp( Booster_up.smooth_jet_size, Booster_up.current_jet_size, lerp_jet_on_rate * Time.fixedDeltaTime );
            Booster_up.jet_particle_system.startSize = Booster_up.smooth_jet_size;

            Booster_up.center_box.z = Booster_up.smooth_jet_size * center_to_booster_rate;
            Booster_up.jet_collider.center = Booster_up.center_box;
            Booster_up.size_box.z = Booster_up.smooth_jet_size * size_to_booster_rate;
            Booster_up.jet_collider.size = Booster_up.size_box;

            if( !Booster_up.jet_emission_module.enabled ) Booster_up.jet_emission_module.enabled = true;
            Booster_up.glow_particle_system.startSize = Booster_up.smooth_jet_size * glow_to_booster_rate;
            if( !Booster_up.glow_emission_module.enabled ) Booster_up.glow_emission_module.enabled = true;
        }

        // If the thrust is less than zero
        else if( thrust_rate < 0f ) {

            ControlBoosterUp();

            Booster_down.current_jet_size =  Mathf.Abs( thrust_rate ) * Booster_down.jet_max_size;
            Booster_down.smooth_jet_size = Mathf.Lerp( Booster_down.smooth_jet_size, Booster_down.current_jet_size, lerp_jet_on_rate * Time.fixedDeltaTime );
            Booster_down.jet_particle_system.startSize = Booster_down.smooth_jet_size;

            Booster_down.center_box.z = Booster_down.smooth_jet_size * center_to_booster_rate;
            Booster_down.jet_collider.center = Booster_down.center_box;
            Booster_down.size_box.z = Booster_down.smooth_jet_size * size_to_booster_rate;
            Booster_down.jet_collider.size = Booster_down.size_box;

            if( !Booster_down.jet_emission_module.enabled ) Booster_down.jet_emission_module.enabled = true;
            Booster_down.glow_particle_system.startSize = Booster_down.smooth_jet_size * glow_to_booster_rate;
            if( !Booster_down.glow_emission_module.enabled ) Booster_down.glow_emission_module.enabled = true;
        }

        // If the thrust is zero and need to disable jet smoothly
        else {

            ControlBoosterUp();
            ControlBoosterDown();
        }
    }

    // Control booster up ##########################################################################################################################################################
    void ControlBoosterUp() {

        if( Booster_up.jet_emission_module.enabled ) {

            if( Booster_up.smooth_jet_size > jet_off_size ) {

                Booster_up.smooth_jet_size = Mathf.Lerp( Booster_up.smooth_jet_size, 0f, lerp_jet_off_rate * Time.fixedDeltaTime );
                Booster_up.jet_particle_system.startSize = Booster_up.smooth_jet_size;

                Booster_up.center_box.z = Booster_up.smooth_jet_size * center_to_booster_rate;
                Booster_up.jet_collider.center = Booster_up.center_box;
                Booster_up.size_box.z = Booster_up.smooth_jet_size * size_to_booster_rate;
                Booster_up.jet_collider.size = Booster_up.size_box;
                Booster_up.glow_particle_system.startSize = Booster_up.smooth_jet_size * glow_to_booster_rate;
            }

            else {

                Booster_up.jet_particle_system.startSize = 0f;
                Booster_up.jet_emission_module.enabled = false;
                Booster_up.glow_emission_module.enabled = false;
            }
        }

        else {

            Booster_up.size_box.z = collider_min_size;
            Booster_up.center_box.z = collider_min_center;
            Booster_up.jet_collider.size = Booster_up.size_box;
            Booster_up.jet_collider.center = Booster_up.center_box;
        }
    }
    
    // Control booster down ########################################################################################################################################################
    void ControlBoosterDown() {

        if( Booster_down.jet_emission_module.enabled ) {

            if( Booster_down.smooth_jet_size > jet_off_size ) {

                Booster_down.smooth_jet_size = Mathf.Lerp( Booster_down.smooth_jet_size, 0f, lerp_jet_off_rate * Time.fixedDeltaTime );
                Booster_down.jet_particle_system.startSize = Booster_down.smooth_jet_size;

                Booster_down.center_box.z = Booster_down.smooth_jet_size * center_to_booster_rate;
                Booster_down.jet_collider.center = Booster_down.center_box;
                Booster_down.size_box.z = Booster_down.smooth_jet_size * size_to_booster_rate;
                Booster_down.jet_collider.size = Booster_down.size_box;
                Booster_down.glow_particle_system.startSize = Booster_down.smooth_jet_size * glow_to_booster_rate;
            }

            else {

                Booster_down.jet_particle_system.startSize = 0f;
                Booster_down.jet_emission_module.enabled = false;
                Booster_down.glow_emission_module.enabled = false;
            }
        }

        else {

            Booster_down.size_box.z = collider_min_size;
            Booster_down.center_box.z = collider_min_center;
            Booster_down.jet_collider.size = Booster_down.size_box;
            Booster_down.jet_collider.center = Booster_down.center_box;
        }
    }

    // Show the horizontal jet #####################################################################################################################################################
    public void ControlJetHorizontal( float thrust_rate ) {

        ControlEngineLight();

        // If the thrust is great than zero
        if( thrust_rate < 0f ) {

            ControlBoosterRight();

            Booster_left.current_jet_size =  Mathf.Abs( thrust_rate ) * Booster_left.jet_max_size;
            Booster_left.smooth_jet_size = Mathf.Lerp( Booster_left.smooth_jet_size, Booster_left.current_jet_size, lerp_jet_on_rate * Time.fixedDeltaTime );
            Booster_left.jet_particle_system.startSize = Booster_left.smooth_jet_size;

            Booster_left.center_box.z = Booster_left.smooth_jet_size * center_to_booster_rate;
            Booster_left.jet_collider.center = Booster_left.center_box;
            Booster_left.size_box.z = Booster_left.smooth_jet_size * size_to_booster_rate;
            Booster_left.jet_collider.size = Booster_left.size_box;

            if( !Booster_left.jet_emission_module.enabled ) Booster_left.jet_emission_module.enabled = true;
            Booster_left.glow_particle_system.startSize = Booster_left.smooth_jet_size * glow_to_booster_rate;
            if( !Booster_left.glow_emission_module.enabled ) Booster_left.glow_emission_module.enabled = true;
        }

        // If the thrust is less than zero
        else if( thrust_rate > 0f ) {

            ControlBoosterLeft();

            Booster_right.current_jet_size =  Mathf.Abs( thrust_rate ) * Booster_right.jet_max_size;
            Booster_right.smooth_jet_size = Mathf.Lerp( Booster_right.smooth_jet_size, Booster_right.current_jet_size, lerp_jet_on_rate * Time.fixedDeltaTime );
            Booster_right.jet_particle_system.startSize = Booster_right.smooth_jet_size;

            Booster_right.center_box.z = Booster_right.smooth_jet_size * center_to_booster_rate;
            Booster_right.jet_collider.center = Booster_right.center_box;
            Booster_right.size_box.z = Booster_right.smooth_jet_size * size_to_booster_rate;
            Booster_right.jet_collider.size = Booster_right.size_box;

            if( !Booster_right.jet_emission_module.enabled ) Booster_right.jet_emission_module.enabled = true;
            Booster_right.glow_particle_system.startSize = Booster_right.smooth_jet_size * glow_to_booster_rate;
            if( !Booster_right.glow_emission_module.enabled ) Booster_right.glow_emission_module.enabled = true;
        }

        // If the thrust is zero and need to disable jet smoothly
        else {

            ControlBoosterLeft();
            ControlBoosterRight();
        }
    }

    // Control booster left ########################################################################################################################################################
    void ControlBoosterLeft() {

        if( Booster_left.jet_emission_module.enabled ) {

            if( Booster_left.smooth_jet_size > jet_off_size ) {

                Booster_left.smooth_jet_size = Mathf.Lerp( Booster_left.smooth_jet_size, 0f, lerp_jet_off_rate * Time.fixedDeltaTime );
                Booster_left.jet_particle_system.startSize = Booster_left.smooth_jet_size;

                Booster_left.center_box.z = Booster_left.smooth_jet_size * center_to_booster_rate;
                Booster_left.jet_collider.center = Booster_left.center_box;
                Booster_left.size_box.z = Booster_left.smooth_jet_size * size_to_booster_rate;
                Booster_left.jet_collider.size = Booster_left.size_box;
                Booster_left.glow_particle_system.startSize = Booster_left.smooth_jet_size * glow_to_booster_rate;
            }

            else {

                Booster_left.jet_particle_system.startSize = 0f;
                Booster_left.jet_emission_module.enabled = false;
                Booster_left.glow_emission_module.enabled = false;
            }
        }

        else {

            Booster_left.size_box.z = collider_min_size;
            Booster_left.center_box.z = collider_min_center;
            Booster_left.jet_collider.size = Booster_left.size_box;
            Booster_left.jet_collider.center = Booster_left.center_box;
        }
    }
    
    // Control booster right #######################################################################################################################################################
    void ControlBoosterRight() {

        if( Booster_right.jet_emission_module.enabled ) {

            if( Booster_right.smooth_jet_size > jet_off_size ) {

                Booster_right.smooth_jet_size = Mathf.Lerp( Booster_right.smooth_jet_size, 0f, lerp_jet_off_rate * Time.fixedDeltaTime );
                Booster_right.jet_particle_system.startSize = Booster_right.smooth_jet_size;

                Booster_right.center_box.z = Booster_right.smooth_jet_size * center_to_booster_rate;
                Booster_right.jet_collider.center = Booster_right.center_box;
                Booster_right.size_box.z = Booster_right.smooth_jet_size * size_to_booster_rate;
                Booster_right.jet_collider.size = Booster_right.size_box;
                Booster_right.glow_particle_system.startSize = Booster_right.smooth_jet_size * glow_to_booster_rate;
            }

            else {

                Booster_right.jet_particle_system.startSize = 0f;
                Booster_right.jet_emission_module.enabled = false;
                Booster_right.glow_emission_module.enabled = false;
            }
        }

        else {

            Booster_right.size_box.z = collider_min_size;
            Booster_right.center_box.z = collider_min_center;
            Booster_right.jet_collider.size = Booster_right.size_box;
            Booster_right.jet_collider.center = Booster_right.center_box;
        }
    }

    // Control the pointlight power from the all jets ##############################################################################################################################
    void ControlEngineLight() {

        booster_max_power = 0f;

        if( Booster_up.jet_emission_module.enabled ) booster_max_power += Booster_up.jet_particle_system.startSize;
        if( Booster_down.jet_emission_module.enabled ) booster_max_power += Booster_down.jet_particle_system.startSize;
        if( Booster_left.jet_emission_module.enabled ) booster_max_power += Booster_left.jet_particle_system.startSize;
        if( Booster_right.jet_emission_module.enabled ) booster_max_power += Booster_right.jet_particle_system.startSize;

        if( booster_max_power > 0f ) {

            pointlight.range = booster_max_power * pointlight_range_to_booster_rate * pointlight_acceleration;
            if( !pointlight.enabled ) pointlight.enabled = true;
        }

        else {

            if( pointlight.enabled ) pointlight.enabled = false;
        }
    }

    // Event: animation started ####################################################################################################################################################
    public void EventAnimationStarted() {

        animator.SetBool( animation_ID_complete, false );
    }

    // Event: animation started ####################################################################################################################################################
    public void EventAnimationCompleted() {

        animator.SetBool( animation_ID_complete, true );
    }

    // Сохранение состояния корабля перед выходом из уровня или из игры ############################################################################################################
    private void OnApplicationQuit() {

        if( was_saved ) return;
        else was_saved = true;

        if( Game.Current_level > LevelType.Level_Menu ) SaveGame.Save( this );
    }
    
    // Сохранение состояния корабля перед выходом из уровня или из игры ############################################################################################################
    private void OnDisable() {

        if( was_saved ) return;
        else was_saved = true;

        if( Game.Current_level > LevelType.Level_Menu ) SaveGame.Save( this );
    }
}