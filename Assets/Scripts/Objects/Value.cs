using UnityEngine;

[System.Serializable]
[RequireComponent( typeof( ObstacleControl ) )]
public class Value : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Тип груза или иной ценности (груза)" )]
    private ValueType type = ValueType.Unknown;
    public ValueType Type { get { return type; } }

    [SerializeField]
    [Tooltip( "Рисунок данного объекта, который можно отображать в инвентаре, меню и т.п. (необходимы размеры 128 x 128 пикселей)" )]
    private Sprite picture;
    public Sprite Picture { get { return picture; } }

    [Header( "ДОПОЛНИТЕЛЬНЫЕ ЭФФЕКТЫ" )]
    [SerializeField]
    [Tooltip( "Префаб для эффекта, когда данный груз взят на борт корабля" )]
    private GameObject taking_prefab;
    public GameObject Taking_prefab { get { return taking_prefab; } }

    [SerializeField]
    [Tooltip( "Префаб для эффекта, когда данный груз выброшен за борт корабля" )]
    private GameObject dropping_prefab;
    public GameObject Dropping_prefab { get { return dropping_prefab; } }

    [SerializeField]
    [Tooltip( "Префаб для создания дополнительных эффектов вокруг объекта: вспышки, свет и т.п." )]
    private GameObject effect_prefab;
    private GameObject additional_effect;

    [Header( "ПАРАМЕТРЫ МАССЫ И СТОИМОСТИ ДЛЯ ТОРГОВЛИ" )]
    [SerializeField]
    [Range( 0f, 1f )]
    [Tooltip( "Коэффициент значимой массы, используемой для расчёта цены объекта, по отношению к полной массе объекта (минерал обычно является лишь небольшой составной частью метеорита, а груз находится в контейнере, который также имеет массу)" )]
    private float valuable_mass_rate = 1f;
    public float Valuable_mass_rate { get { return valuable_mass_rate; } }

    [SerializeField]
    [Range( 0.1f, 0.3f )]
    [Tooltip( "Коэффициент случайнго отклонения массы (для того, чтобы у всех объектов всегда была разная масса и, как следствие, стоимость)" )]
    private float mass_deviation_rate = 0.2f;

    [SerializeField] // В общем случае эта переменная рассчитывается автоматически; отображается только для визуального контроля
    //[Tooltip( "Полная масса объекта в килограммах: в точности равна <Rigidbody.mass * 1000>" )]
    //[System.NonSerialized]
    private float total_mass_in_kilos;
    public float Total_mass_in_kilos { get { return total_mass_in_kilos; } }
    public float Total_mass_in_tons { get { return Total_mass_in_kilos * 0.001f; } }

    [SerializeField] // В общем случае эта переменная рассчитывается автоматически; отображается только для визуального контроля
    //[Tooltip( "Значимая масса объекта в килограммах (вычисляется автоматически; можно пересчитать через меню скрипта)" )]
    //[System.NonSerialized]
    private float valuable_mass_in_kilos = 0f;
    public float Valuable_mass_in_kilos { get { return valuable_mass_in_kilos; } }
    public float Valuable_mass_in_tons { get { return Valuable_mass_in_kilos * 0.001f; } }

    [SerializeField] // В общем случае эта переменная рассчитывается автоматически; отображается только для визуального контроля
    //[Tooltip( "Цена за килограмм значимой массы объекта (вычисляется автоматически; можно пересчитать через меню скрипта)" )]
    //[System.NonSerialized]
    private float price_per_kilo = 0f;
    public float Price_per_kilo { get { return price_per_kilo; } }

    [SerializeField] // В общем случае эта переменная рассчитывается автоматически; отображается только для визуального контроля
    //[Tooltip( "Итоговая цена объекта (вычисляется автоматически; можно пересчитать через меню скрипта)" )]
    //[System.NonSerialized]
    private float total_cost = 0f;
    public float Total_cost { get { return total_cost; } }

    [Header( "СООБЩЕНИЯ, СВЯЗАННЫЕ С ОБЪЕКТОМ (ГРУЗОМ)" )]
    [SerializeField]
    [Tooltip( "После продажи предмета" )]
    private ComplexMessage sell_message_key;
    public ComplexMessage Sell_message_key { get { return sell_message_key; } }

    [SerializeField]
    [Tooltip( "После сброса предмета из трюма" )]
    private ComplexMessage drop_message_key;
    public ComplexMessage Drop_message_key { get { return drop_message_key; } }

    [SerializeField]
    [Tooltip( "После загрузки предмета в трюм" )]
    private ComplexMessage loading_message_key;
    public ComplexMessage Loading_message_key { get { return loading_message_key; } }

    [SerializeField]
    [Tooltip( "Когда в трюме не осталось свободных отсеков" )]
    private ComplexMessage hold_limit_message_key;
    public ComplexMessage Hold_limit_message_key { get { return hold_limit_message_key; } }

    [SerializeField]
    [Tooltip( "Когда масса груза превышает допустимую вместимость отсека" )]
    private ComplexMessage compartment_limit_message_key;
    public ComplexMessage Compartment_limit_message_key { get { return compartment_limit_message_key; } }

    private Transform cached_transform;
    public Transform Cached_transform { get { return cached_transform; } }

    private Rigidbody physics;
    private ObstacleControl obstacle;
    public ObstacleControl Obstacle { get { return obstacle; } }

    public bool Is_mineral { get { return (obstacle.Mineral != null); } }
    public bool Is_freight { get { return (obstacle.Freight != null); } }
    public bool Is_mission { get { return (obstacle.Mission != null); } }
    public bool Is_wanderer { get { return (obstacle.Wanderer != null); } }

    public void Sleep( bool state ) {

        if( Is_mission ) obstacle.Mission.Sleep();
        else if( Is_wanderer ) obstacle.Wanderer.Sleep( state );
        else obstacle.Sleep();
    }

    public void WakeUp( Transform object_transform ) {

        if( Is_wanderer ) obstacle.Wanderer.WakeUp( object_transform, true );
        obstacle.WakeUp( object_transform );
    }

    public SubjectType Subject_type { get {

        if( obstacle.Is_freight ) return Game.Control.FreightSubjectType( obstacle.Freight.Type );
        else if( obstacle.Is_mineral ) return Game.Control.MineralSubjectType( obstacle.Mineral.Type );

        return SubjectType.Unknown;
    } }

    // Awake ###################################################################################################################################################################
    private void Awake() {

        cached_transform = transform;

        physics = GetComponent<Rigidbody>();
        obstacle = GetComponent<ObstacleControl>();

        RandomizeMass();
        FullMassAndCostCalculation();

        if( effect_prefab != null ) {

            additional_effect = Instantiate( effect_prefab, Vector3.zero, Quaternion.identity ) as GameObject;
            additional_effect.transform.parent = transform;
        }
    }
    
    // Starting initialization #################################################################################################################################################
    private void Start() {

    }
    
    // Делает массу объекта заранее непредсказуемой (чтобы похожие объекты имели разную стоимость) #############################################################################
    private void RandomizeMass() {

        if( physics == null ) physics = GetComponent<Rigidbody>();

        physics.mass += Mathf.Floor( Random.Range( -mass_deviation_rate * physics.mass, mass_deviation_rate * physics.mass ) * 100f) * 0.01f;

        total_mass_in_kilos = physics.mass * 1000f;
    }

    // Set the total mass in dependng on physical mass #########################################################################################################################
    public void CalculateMassInKilos() {

        if( physics == null ) physics = GetComponent<Rigidbody>();
        
        total_mass_in_kilos = physics.mass * 1000f;
        valuable_mass_in_kilos = total_mass_in_kilos * valuable_mass_rate;

        if( valuable_mass_in_kilos > total_mass_in_kilos ) valuable_mass_in_kilos = total_mass_in_kilos;
    }
    
    // Set the price of the object per kilo ####################################################################################################################################
    public void CalculatePricePerKilo() {

        price_per_kilo = 0f;

        if( obstacle == null ) obstacle = GetComponent<ObstacleControl>();
        if( obstacle.Mineral == null ) obstacle.Mineral = GetComponent<Mineral>();
        if( obstacle.Freight == null ) obstacle.Freight = GetComponent<Freight>();

        if( Is_freight ) {

            if( Game.Control == null ) price_per_kilo = GameObject.Find( "Control" ).GetComponent<GameControl>().GetFreightPricePerKilo( obstacle.Freight.Type );
            else price_per_kilo = Game.Control.GetFreightPricePerKilo( obstacle.Freight.Type );
        }

        else if( Is_mineral ) {

            if( Game.Control == null ) price_per_kilo = GameObject.Find( "Control" ).GetComponent<GameControl>().GetMineralPricePerKilo( obstacle.Mineral.Type );
            else price_per_kilo = Game.Control.GetMineralPricePerKilo( obstacle.Mineral.Type );
        }
    }

    // Calculate income of the freight #########################################################################################################################################
    public void CalculateTotalCost() {

        total_cost = Mathf.Floor( price_per_kilo * valuable_mass_in_kilos * 0.1f ) * 10f;
    }

    // Full calculation of the object ##########################################################################################################################################
    [ContextMenu( "МЕНЮ: Рассчитать массу и стоимость объекта" )]
    public void FullMassAndCostCalculation() {

        CalculateMassInKilos();
        CalculatePricePerKilo();
        CalculateTotalCost();
    }
}