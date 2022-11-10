// Типы уровней соответствуют индексам сцен Юнити
public enum LevelType {

    Level_Quit                          = -1,
    Level_Introduction                  = 0,
    Level_Loading                       = 1,
    Level_Hero                          = 2,
    Level_Menu                          = 3,
    Level_00_Earth_Low_Orbit            = 4,
    Level_01_Earth_Radiation_Belt       = 5,
    Level_02_Venus_Orbit                = 6
}

// Типы кораблей соответствуют индексам в массиве префабов этих кораблей
public enum ShipType {

    Ship_unknown        = -1,
    Ship_00_Midge       = 0,
    Ship_01_Bee         = 1,
    Ship_02_Spider      = 2,
    Ship_03_Bumblebee   = 3,
    Ship_04_Swallow     = 4,
    Ship_05_Seeker      = 5,
    Ship_06_Hunter      = 6,
    Ship_07_Rider       = 7,
    Ship_08_Wanderer    = 8,
    Ship_09_Ace         = 9
}

// Типы инвентарей: у корабля, во время торговли, у станции
public enum InventoryType {

    Ship_inventory_only,
    Ship_inventory_trade,
    Station_inventory_trade,
    Station_inventory_only,
    Trade_only
}

public enum IndicatorType {

    Unknown             = -1,
    Hull_durability     = 0,
    Fuel_capacity       = 1,
    Engine_thrust       = 2,
    Hold_capacty        = 3,
    Shield_time         = 4,
    Shield_power        = 5,
    Charge_time         = 6,
    Radar_range         = 7,
    Radar_power         = 8,
    Autolanding_amount  = 9
}

public enum UpgradeType {

    Hull_durability     = 0,
    Fuel_capacity       = 1,
    Engine_thrust       = 2,
    Hold_capacty        = 3,
    Shield_time         = 4,
    Shield_power        = 5,
    Charge_time         = 6,
    Radar_range         = 7,
    Radar_power         = 8
}

public enum GravityType {

    Unknown  = 0,
    Stable   = 1,
    Unstable = 2
}

public enum EdgeType {

    Top = 1,
    Bottom = 2,
    Left = 3,
    Right = 4,
    Near = 5,
    Far = 6
}

public enum ShakeType {

    Engine  = 0,
    Soft    = 1,
    Hard    = 2,
    Awful   = 3
}

public enum AudioType {

    Unknown = 0,
    Engine = 1,
    Voice = 2,
    Sound = 3,
    Music = 4
}

public enum SubjectType {

    Unknown = 0,
    Standard = 1,
    Exposive = 2,
    Perishable = 3,
    Combined = 4
}

public enum ActType {

    Unknown = 0,
    Split_meteorite = 1,
    Destroy_meteorite = 2,
    Find_new_mineral = 3,
    Find_new_element = 4
}

public enum MissionType {

    Unknown            = 0,
    Deliver_freight    = 1,
    Deliver_mineral    = 2,
    Find_freight       = 3,
    Find_mineral       = 4,
    Find_station       = 5,
    Rescue_operation   = 6,
    Research_operation = 7
}

public enum ConstructionType {

    Unknown   = 0,
    Ship      = 1,
    Suit      = 2,
    Mine      = 3,
    Weapon    = 4,
    Station   = 5,
    Satellite = 6
}

public enum ObstacleType {

    Unknown = 0,
    Mine = 1,
    Weapon = 2,
    Shell = 3,
    Comet = 4,
    Meteor = 5,
    Debris = 6,
    Asteroid = 7,
    Satellite = 8,
    Meteorite = 9,
    People = 10,
    Station = 11,
    Ship = 12,
    Player = 13,
    Structure = 14
}

public enum ZoneType {

    Unknown = 0,
    Jet = 1,
    Gas = 2,
    Heat = 3,
    Dust = 4,
    Wind = 5,
    Space = 6,
    Flame = 7,
    Comet = 8,
    Liquid = 9,
    Protons = 10,
    Electrons = 11,
    Lightning = 12,
    Black_hole = 13,
    Xray_radiation = 14,
    Gamma_radiation = 15
}

public enum ValueType {

    Unknown       = -1,
    Level         = 0,
    Ship          = 1,
    Hyperjump     = 2,
    Autolanding   = 3,
    Mineral       = 4,
    Freight       = 5
}

public enum FreightType {

    Unknown      = -1,
    Food         = 0,
    Fuel         = 1,
    Metal        = 2,
    Water        = 3,
    Medicine     = 4,
    Explosive    = 5,
    Equipment    = 6,
    Plastic      = 7
}

public enum MineralType {

    Unknown = -1,
    Methane = 1 ,
    Carbon = 2,
    Iron = 3,
    Sulfur = 4,
    Carbon_dioxide = 5,
    Ammonia = 6,
    Oxygen = 7,
    Aluminium = 8,
    Plumbous = 9,
    Zinc = 10,
    Hydrogen = 11,
    Silicon = 12,
    Sodium = 13,
    Magnesium = 14,
    Water = 15, 
    Manganese = 16,
    Cerium = 17,
    Copper = 18,
    Nickel = 19,
    Stannous = 20,
    Lanthanum = 21,
    Tungsten = 22,
    Molybdenum = 23,
    Lithium = 24,
    Mercury = 25,
    Cobalt = 26,
    Uranium = 27,
    Chromous = 28,
    Zirconium = 29,
    Selenium = 30,
    Niobium = 31,
    Tantalum = 32,
    Indium = 33, 
    Gallium = 34,
    Dysprosium = 35,
    Titanium = 36,
    Beryllium = 37,
    Silver = 38,
    Germanium = 39,
    Ruthenium = 40,
    Hafnium = 41,
    Deuterium = 42,
    Lutecium = 43,
    Scandium = 44,
    Rhenium = 45,
    Iridium = 46,
    Palladium = 47,
    Osmium = 48,
    Gold = 49,
    Rhodium = 50,
    Platinum = 51,
    Olivine = 52,
    Helium_3 = 53,
    Thallium = 54,
    Plutonium = 55,
    Blue_pomegranate = 56,
    Black_opal = 57,
    Osmium_187 = 58,
    Painite = 59,
    Demantoid = 60,
    Musgravite = 61,
    Taaffeite = 62,
    Benitoite = 63,
    Tritium = 64, 
    White_diamond = 65,
    Bixbyite = 66,
    Rubin = 67,
    Jadeite = 68,
    Grandidierite = 69,
    Alabamine = 70,
    Californium_252 = 71,
    Red_diamond = 72,
    Labilium = 73,
    Adversarium = 74,
    Lucensium = 75,
    Dark_matter = 76,
    Dark_energy = 77,
    Dark_radiation = 78
}