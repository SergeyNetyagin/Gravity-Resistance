using UnityEngine;

public class Station : MonoBehaviour {

    [System.Serializable]
    private class StationService {

        [Tooltip( "Оказывает ли станция данный вид услуг" )]
        public bool is_used = true;

        [HideInInspector, System.NonSerialized]
        public bool is_partial = false;

        [HideInInspector, System.NonSerialized]
        public float units = 0f;

        [HideInInspector, System.NonSerialized]
        public float full_price = 0f;
    }

    [SerializeField]
    [Tooltip( "Ключ названия данной станции" )]
    private string name_key;
    public string Name_key { get {  return name_key; } }

    [SerializeField]
    [Tooltip( "Использует ли данная станция защитное поле, или она работает вообще без защиты" )]
    private bool use_protection = true;
    public bool Use_protection { get { return use_protection; } }

    [SerializeField]
    [Tooltip( "Использовать ли данную станцию в миссионных заданиях" )]
    private bool use_in_missions = true;
    public bool Use_in_missions { get { return use_in_missions; } }

    [SerializeField]
    [Range( 0.5f, 2f )]
    [Tooltip( "Торговый коэффициент данной станции по отношению к общепринятым расценкам" )]
    private float trade_rate = 1f;
    public float Trade_rate { get { return trade_rate; } }

    [SerializeField]
    [Tooltip( "Каким цветом будет подсвечиваться посадочная площадка станции" )]
    private Color color;
    public Color Color { get { return color; } }

    [SerializeField]
    [Tooltip( "Специфическое приветствие данной станции" )]
    private ComplexMessage welcome_message;
    public ComplexMessage Welcome_message { get { return welcome_message; } }

    [Header( "НАСТРОЙКИ СЕРВИСА ДАННОЙ СТАНЦИИ" )]
    [SerializeField]
	private StationService
        service_hull; [SerializeField] private StationService
        service_fuel,
        service_engine;

    [System.NonSerialized] 
    private StationService
        service_upgrade,
        service_trade;

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Какой виды апгрэйдов предлагает станция (автопосадка НЕ относится к апгрэйдам - это товар, лицензия); если список пустой, то станция НЕ делает никаких апгрэйдов" )]
    private UpgradeType[] upgrades;
    public UpgradeType[] Upgrades { get { return upgrades; } }
    public bool Is_upgrading { get { return (upgrades.Length > 0); } }

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Какие виды товаров и лицензий предлагает станция; если список пустой, станция не продаёт ничего (при этом, возможно, некоторые станции могут покупать)" )]
    private ValueType[] values;
    public ValueType[] Values { get { return values; } }
    public bool Is_trading { get { return (values.Length > 0); } }

    private Transform 
        hold_unit_transform = null,
        mission_unit_transform = null;

    private Transform cached_transform;
    public Transform Cached_transform { get { return cached_transform; } }

    private Material material;

    // Стоимость полного или частичного ремонта обшивки и устранения утечки ####################################################################################################
    public bool Repair_hull { get { return service_hull.is_used; } }
    public bool Hull_partial_repair { get { return service_hull.is_partial; } }
    public float Hull_units { get { return service_hull.units; } }
    public float Hull_price { get {

        service_hull.is_partial = false;
        service_hull.units = (Game.Player.Ship.Hull_durability.Maximum - Game.Player.Ship.Hull_durability.Available) * Game.Player.Ship.Hull_durability.Unit_size_inversed;
        service_hull.full_price = Game.Player.Ship.Hull_durability.Restore_cost * service_hull.units * trade_rate * Game.Level.Complication;

        service_hull.units = Game.Player.Has_fuel_leaks ? Game.Player.Leaks_usage : 0f;
        service_hull.full_price += Game.Player.Ship.Fuel_capacity.Restore_cost * service_hull.units * trade_rate * Game.Level.Complication;

        service_hull.full_price = Mathf.Floor( service_hull.full_price * 0.1f ) * 10f;

        if( (Game.Money > 0f) && (Game.Money < service_hull.full_price) ) {
            
            service_hull.is_partial = true;
            service_hull.units *= Game.Money / service_hull.full_price;
            service_hull.full_price = Game.Money;
        }
        
        return service_hull.full_price;
    } }

    // Стоимость полной или частичной заправки топливных баков #################################################################################################################
    public bool Sells_fuel { get { return service_fuel.is_used; } }
    public bool Fuel_partial_filling { get { return service_fuel.is_partial; } }
    public float Fuel_units { get { return service_fuel.units; } }
    public float Fuel_price { get {

        service_fuel.is_partial = false;
        service_fuel.units = (Game.Player.Ship.Fuel_capacity.Maximum - Game.Player.Ship.Fuel_capacity.Available) * Game.Player.Ship.Fuel_capacity.Unit_size_inversed;
        service_fuel.full_price = Game.Control.Fuel_price_per_ton * service_fuel.units * trade_rate * Game.Level.Complication;
        service_fuel.full_price = Mathf.Floor( service_fuel.full_price * 0.1f ) * 10f;

        if( (Game.Money > 0f) && (Game.Money < service_fuel.full_price) ) {
            
            service_fuel.is_partial = true;
            service_fuel.units *= Game.Money / service_fuel.full_price;
            service_fuel.full_price = Game.Money;
        }

        return service_fuel.full_price;
    } }
    
    // Стоимость полного  или частичного ремонта двигателя #####################################################################################################################
    public bool Repairs_engine { get { return service_engine.is_used; } }
    public bool Engine_partial_repair { get { return service_engine.is_partial; } }
    public float Engine_units { get { return service_engine.units; } }
    public float Engine_price { get {

        service_engine.is_partial = false;
        service_engine.units = (Game.Player.Ship.Engine_thrust.Maximum - Game.Player.Ship.Engine_thrust.Available) * Game.Player.Ship.Engine_thrust.Unit_size_inversed;
        service_engine.full_price = Game.Player.Ship.Engine_thrust.Restore_cost * service_engine.units * trade_rate * Game.Level.Complication;
        service_engine.full_price = Mathf.Floor( service_engine.full_price * 0.1f ) * 10f;

        if( (Game.Money > 0f) && (Game.Money < service_engine.full_price) ) {
            
            service_engine.is_partial = true;
            service_engine.units *= Game.Money / service_engine.full_price;
            service_engine.full_price = Game.Money;
        }

        return service_engine.full_price;
    } }

    // Не помню, для чего это свойство...
    public bool Has_hold_value { get { return (hold_unit_transform != null); } }
    public void AssignHoldValue( Transform value_transfrom ) { hold_unit_transform = value_transfrom; }
    public void CancelHoldValue() { hold_unit_transform = null; }

    // Показывает, есть ли на станции миссионный груз
    public bool Has_mission_value { get { return (mission_unit_transform != null); } }
    public void AssignMissionValue( Transform value_transform ) { mission_unit_transform = value_transform; }
    public void CancelMissionValue() { mission_unit_transform = null; }

    // Starting initialization #################################################################################################################################################
    void Start() {

        cached_transform = transform;

        if( use_protection ) GetComponentInChildren<Protection>().EnableAutoProtection();
        else GetComponentInChildren<Protection>().DisableAutoProtection();
    }
}