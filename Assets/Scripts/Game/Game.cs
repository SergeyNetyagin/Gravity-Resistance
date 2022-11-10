using UnityEngine;
using UnityEngine.UI;

using System;

using SmartLocalization;

[Flags]
public enum GameState {

    Reset       = 0,
    Loading     = 1,
    Paused      = 2,
    Briefing    = 4,
    Restarting  = 8,
    Starting    = 16,
    Playing     = 32,
    Complete    = 64
}

[System.Serializable]
public class GameData {

    public Language Language = Language.Autodetect;

    public float Money = 0f;
    public float Experience = 0f;

    public bool Use_sound_in_vacuum = false;
    public bool Use_vertical_control = true;
    public bool Use_horizontal_control = false;

    public float Sound_volume = 1f;
    public float Music_volume = 1f;
    public float Voice_volume = 1f;
    public float Engine_volume = 1f;

    public GameData() { }

    public void Load() {

        Game.Language = Language;
        Game.LoadMoney( Money );
        Game.LoadExperience( Experience );

        Game.Use_sound_in_vacuum = Use_sound_in_vacuum;
        Game.Use_vertical_control = Use_vertical_control;
        Game.Use_horizontal_control = Use_horizontal_control;

        Game.Sound_volume = Sound_volume;
        Game.Music_volume = Music_volume;
        Game.Voice_volume = Voice_volume;
        Game.Engine_volume = Engine_volume;
    }

    public void Save() {

        Language = Game.Language;
        Money = Game.Money;
        Experience = Game.Experience;
        
        Use_sound_in_vacuum = Game.Use_sound_in_vacuum;
        Use_vertical_control = Game.Use_vertical_control;
        Use_horizontal_control = Game.Use_horizontal_control;

        Sound_volume = Game.Sound_volume;
        Music_volume = Game.Music_volume;
        Voice_volume = Game.Voice_volume;
        Engine_volume = Game.Engine_volume;
    }
}

[System.Serializable]
public class Indicator {

    [Tooltip( "Тип индикатора" )]
    public IndicatorType Type;

    [Tooltip( "Ключ названия индикатора (двигатель, автопосадка, радар и т.п.)" )]
    public string Name_key;

    [Tooltip( "Ключ названия этого же индикатора с маленькой буквы (для апгрэйда)" )]
    public string Upgrade_key;

    [Tooltip( "Ключ названия единицы измерения ресурса" )]
    public string Unit_key;

    [Tooltip( "Ключ фразы для полного восстановления ресурса" )]
    public string Full_restore_key;

    [Tooltip( "Ключ фразы для частичного восстановления ресурса" )]
    public string Partial_restore_key;

    [Tooltip( "Объяснение причины завершения игры: если корабль уничтожен по причине завершения данного ресурса" )]
    public string Cause_restart_key;

    [Tooltip( "Совет игроку, если корабль уничтожен по причине завершения данного ресурса" )]
    public string Advice_restart_key;

    [Tooltip( "Размер одной порции ресурсов, за которую указывается стоимость пополнения или апгрэйда" )]
    [SerializeField]
    private float unit_size = 1f;
    public float Unit_size { get { return unit_size; } }
    private float unit_size_inversed = -1f;
    public float Unit_size_inversed { get { return (unit_size_inversed == -1f) ? (unit_size_inversed = (1f / Unit_size)) : unit_size_inversed; } }

    [Tooltip( "Имеющееся количество в абсолютном выражении в данный момент времени" )]
    [SerializeField]
    private float available = 0f;
    public float Available { get { return available; } set { available = value; available_inversed = -1f; } }
    private float available_inversed = -1f;
    public float Available_inversed { get { return (available_inversed == -1f) ? (available_inversed = (1f / available)) : available_inversed; } }

    [Tooltip( "Максимально допустимое количество в абсолютном выражении в данный момент времени (насколько позволяет текущая модернизация корабля)" )]
    [SerializeField]
    private float maximum = 0f;
    public float Maximum { get { return maximum; } set { maximum = value; maximum_inversed = -1f; } }
    private float maximum_inversed = -1f;
    public float Maximum_inversed { get { return (maximum_inversed == -1f) ? (maximum_inversed = (1f / Maximum)) : maximum_inversed; } }

    [Tooltip( "Максимально возможное количество в абсолютном выражении для данного типа корабля (насколько можно модернизировать корабль)" )]
    [SerializeField]
    private float upgrade_max_ship = 0f;
    public float Upgrade_max_ship { get { return upgrade_max_ship; } }
    private float upgrade_max_ship_inversed = -1f;
    public float Upgrade_max_ship_inversed { get { return (upgrade_max_ship_inversed == -1f) ? (upgrade_max_ship_inversed = (1f / Upgrade_max_ship)) : upgrade_max_ship_inversed; } }

    [Tooltip( "Максимально возможное количество в абсолютном выражении во всей игре (например, для самого мощного корабля)" )]
    [SerializeField]
    private float upgrade_max_game = 0f;
    public float Upgrade_max_game { get { return upgrade_max_game; } }
    private float upgrade_max_game_inversed = -1f;
    public float Upgrade_max_game_inversed { get { return (upgrade_max_game_inversed == -1f) ? (upgrade_max_game_inversed = (1f / Upgrade_max_game)) : upgrade_max_game_inversed; } }

    [Tooltip( "Величина ресурса, после которого игра уже не может продолжаться; если = 0, то игра останавливается после полного исчерпания ресурса" )]
    [SerializeField]
    private float restart_limit = 0f;
    public float Restart_limit { get { return restart_limit; } }

    [Tooltip( "Базовая цена пополнения/восстановления одной единицы (итоговая сумма зависит от торгового коэффициента станции и сложности уровня)" )]
    public float Restore_cost = 0f;

    [Tooltip( "Базовая цена апгрэйда одной единицы (итоговая сумма зависит от торгового коэффициента станции и сложности уровня)" )]
    public float Upgrade_cost = 0f;

    [Tooltip( "Комплексное сообщение на случай частичного повреждения ресурса" )]
    public ComplexMessage Partial_damage_message;

    [Tooltip( "Комплексное сообщение на случай критического повреждения ресурса" )]
    public ComplexMessage Critical_damage_message;
     
    [Tooltip( "Комплексное сообщение на случай полного разрушения ресурса" )]
    public ComplexMessage Total_damage_message;

    [Tooltip( "Комплексное сообщение на случай частичного восстановления ресурса" )]
    public ComplexMessage Partial_restore_message;

    [Tooltip( "Комплексное сообщение на случай полного восстановления ресурса" )]
    public ComplexMessage Total_restore_message;

    [Tooltip( "Комплексное сообщение на случай проведения апгрэйда ресурса" )]
    public ComplexMessage Upgrade_message;

    [Tooltip( "Эффект, воспроизводимый после апгрэйда данного индикатора" )]
    [SerializeField]
    private Effect upgrade_effect;
    public Effect Upgrade_effect { get { return upgrade_effect; } }

    public void CopyFrom( Indicator indicator ) {

        unit_size = indicator.Unit_size;
        available = indicator.Available;
        maximum = indicator.Maximum;

        upgrade_max_ship = indicator.Upgrade_max_ship;
        upgrade_max_game = indicator.Upgrade_max_game;

        Restore_cost = indicator.Restore_cost;
        Upgrade_cost = indicator.Upgrade_cost;
    }
}

// Class for storage of the all key gameplay global parameters
public static class Game {

    public static string Path_values { get { return "Values/"; } }

    public static string Path_levels { get { return Application.persistentDataPath + "/Levels/"; } }
    public static string Path_ships { get { return Application.persistentDataPath + "/Ships/"; } }
    public static string Path_config { get { return Application.persistentDataPath + "/Config/"; } }
    public static string Path_player { get { return Application.persistentDataPath + "/Config/"; } }
    public static string File_config { get { return "Gravity Resistance"; } }
    public static string Extension_config { get { return ".sav"; } }
    public static string Extension_save { get { return ".sav"; } }

    public static string ConfigFileName() { return (Game.Path_config + Game.File_config + Game.Extension_config); }
    public static string PlayerFileName( Player player ) { return (Game.Path_config + Game.Player.name + Game.Extension_config); }
    public static string LevelFileName( Level level ) { return (Game.Path_levels + level.Type.ToString() + Game.Extension_save); }
    public static string ShipFileName( Ship ship ) { return (Game.Path_ships + ship.Type.ToString() + Game.Extension_save); }

    public static Language Language = Language.Autodetect;
    
    // Текущий уровень, соответствующий номеру активной сцены (служит для ввода некоторых ограничений: например, для работы GameControl и предотвращения ненужных сохранений)
    public static LevelType Current_level = LevelType.Level_Introduction;

    // Уровень, который выбран для загрузки (служит входным параметром для CanvasLoding, чтобы затем запустить данный уровень)
    public static LevelType Loading_level = LevelType.Level_Introduction;

    // Корабль, который был выбран для игры на запускаемом уровне (служит для быстрой инициализации корабля игрока после загрузки уровня)
    public static ShipType Playing_ship = ShipType.Ship_00_Midge;

    public static bool 
        Is_genuine = true,
        Is_first_time = true,
        Use_hero = false,
        Use_immortal_mode = false,
        Use_sound_in_vacuum = false,
        Use_vertical_control = true,
        Use_horizontal_control = false;

    public static float
        Sound_volume = 1f,
        Music_volume = 1f,
        Voice_volume = 1f,
        Engine_volume = 1f;

    private static float experience = 0f;
    public static float Experience { get { return experience; } set { experience = value; } }
    public static void LoadExperience( float loaded_experience ) { experience = loaded_experience; }
    private static float max_experience = 20f;
    public static float Max_experience { get { return max_experience; } }
    private static float max_experience_inversed = -1f;
    public static float Max_experience_inversed { get { if( max_experience_inversed == -1f ) max_experience_inversed = 1f / max_experience; return max_experience_inversed; } }
            
    private static float money = 0f, sum_spent = 0f, sum_earned = 0f;
    public static void LoadMoney( float loaded_money ) { money = loaded_money; }
    public static float Money { get { return money; } set { float delta = value - money; money = value; if( delta > 0f ) sum_earned += delta; else sum_spent -= delta; } }
    public static float Sum_spent { get { return sum_spent; } }
    public static float Sum_earned { get { return sum_earned; } }
    public static void ResetSumCalculation() { sum_spent = sum_earned = 0f; }

    public static Vector3 Min_vector = new Vector3( float.MinValue, float.MinValue, float.MinValue );
    public static Vector3 Max_vector = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );

    // Текстовые разделители, подлежащие локализации ###########################################################################################################################

    #region LOCALIZED_SEPARATORS

    public static string Separator_x { get; set; }
    public static string Separator_plus { get; set; }
    public static string Separator_minus { get; set; }
    public static string Separator_zero { get; set; }
    public static string Separator_float { get; set; }
    public static string Separator_slash { get; set; }
    public static string Separator_colon { get; set; }
    public static string Separator_semicolon { get; set; }
    public static string Separator_space { get; set; }
    public static string Separator_quote { get; set; }
    public static string Separator_hyphen { get; set; }
    public static string Separator_triad { get; set; }
    public static string Separator_equals { get; set; }

    public static void RefreshLocalizedSeparators() {

        Separator_x = Localization.GetTextValue( "Separator.x" );
        Separator_plus = Localization.GetTextValue( "Separator.Plus" );
        Separator_minus = Localization.GetTextValue( "Separator.Minus" );
        Separator_zero = Localization.GetTextValue( "Separator.Zero" );
        Separator_float = Localization.GetTextValue( "Separator.Float" );
        Separator_slash = Localization.GetTextValue( "Separator.Slash" );
        Separator_colon = Localization.GetTextValue( "Separator.Colon" );
        Separator_semicolon = Localization.GetTextValue( "Separator.Semicolon" );
        Separator_space = Localization.GetTextValue( "Separator.Space" );
        Separator_quote = Localization.GetTextValue( "Separator.Quote" );
        Separator_hyphen = Localization.GetTextValue( "Separator.Hyphen" );
        Separator_triad = Localization.GetTextValue( "Separator.Triad" );
        Separator_equals = Localization.GetTextValue( "Separator.Equals" );
    }
    #endregion

    #region LOCALIZED_UNITS

    public static string Unit_distance_mm { get; set; }
    public static string Unit_distance_cm { get; set; }
    public static string Unit_distance_m { get; set; }
    public static string Unit_distance_km { get; set; }
    public static string Unit_mass_gm { get; set; }
    public static string Unit_mass_kg { get; set; }
    public static string Unit_mass_t { get; set; }
    public static string Unit_thrust_N { get; set; }
    public static string Unit_thrust_kN { get; set; }
    public static string Unit_pressure_Pa { get; set; }
    public static string Unit_pressure_kPa { get; set; }
    public static string Unit_pressure_MPa { get; set; }
    public static string Unit_power_W { get; set; }
    public static string Unit_power_kW { get; set; }
    public static string Unit_power_MW { get; set; }
    public static string Unit_money_thnd { get; set; }
    public static string Unit_money_mln { get; set; }
    public static string Unit_money_bln { get; set; }
    public static string Unit_time_h { get; set; }
    public static string Unit_time_min { get; set; }
    public static string Unit_time_s { get; set; }
    public static string Unit_percent { get; set; }

    public static void RefreshLocalizedUnits() {
    
        Unit_distance_mm = Localization.GetTextValue( "Unit.Distance.mm" );
        Unit_distance_cm = Localization.GetTextValue( "Unit.Distance.cm" );
        Unit_distance_m = Localization.GetTextValue( "Unit.Distance.m" );
        Unit_distance_km = Localization.GetTextValue( "Unit.Distance.km" );
        Unit_mass_gm = Localization.GetTextValue( "Unit.Mass.gm" );
        Unit_mass_kg = Localization.GetTextValue( "Unit.Mass.kg" );
        Unit_mass_t = Localization.GetTextValue( "Unit.Mass.t" );
        Unit_thrust_N = Localization.GetTextValue( "Unit.Thrust.N" );
        Unit_thrust_kN = Localization.GetTextValue( "Unit.Thrust.kN" );
        Unit_pressure_Pa = Localization.GetTextValue( "Unit.Pressure.Pa" );
        Unit_pressure_kPa = Localization.GetTextValue( "Unit.Pressure.kPa" );
        Unit_pressure_MPa = Localization.GetTextValue( "Unit.Pressure.MPa" );
        Unit_power_W = Localization.GetTextValue( "Unit.Power.W" );
        Unit_power_kW = Localization.GetTextValue( "Unit.Power.kW" );
        Unit_power_MW = Localization.GetTextValue( "Unit.Power.MW" );
        Unit_money_thnd = Localization.GetTextValue( "Unit.Money.thnd" );
        Unit_money_mln = Localization.GetTextValue( "Unit.Money.mln" );
        Unit_money_bln = Localization.GetTextValue( "Unit.Money.bln" );
        Unit_time_h = Localization.GetTextValue( "Unit.Time.h" );
        Unit_time_min = Localization.GetTextValue( "Unit.Time.min" );
        Unit_time_s = Localization.GetTextValue( "Unit.Time.s" );
        Unit_percent = Localization.GetTextValue( "Unit.Percent" );
    }
    #endregion

    #region LOCALIZED_PHRASES

    public static string Phrase_money { get; set; }
    public static string Phrase_station { get; set; }
    public static string Phrase_service_no { get; set; }
    public static string Phrase_service_norm { get; set; }
    public static string Phrase_service_poor { get; set; }
    public static string Phrase_service_trade_no { get; set; }
    public static string Phrase_service_money_no { get; set; }
    public static string Phrase_service_hold_empty { get; set; }
    public static string Phrase_service_restore_full { get; set; }
    public static string Phrase_service_restore_partial { get; set; }
    public static string Phrase_service_leask_repair { get; set; }
    public static string Phrase_service_filling_full { get; set; }
    public static string Phrase_service_filling_partial { get; set; }
    public static string Phrase_hull { get; set; }
    public static string Phrase_fuel { get; set; }
    public static string Phrase_engine { get; set; }
    public static string Phrase_upgrade { get; set; }
    public static string Phrase_service { get; set; }
    public static string Phrase_maximum { get; set; }
    public static string Phrase_purchase { get; set; }
    public static string Phrase_autolanding { get; set; }
    public static string Phrase_buy_price { get; set; }
    public static string Phrase_sell_price { get; set; }
    public static string Phrase_buy_ship { get; set; }
    public static string Phrase_sell_ship { get; set; }
    public static string Phrase_gravity_stable { get; set; }
    public static string Phrase_gravity_unstable { get; set; }
    public static string Phrase_gravity_type { get; set; }
    public static string Phrase_gravity_force { get; set; }
    public static string Phrase_ship_recommended { get; set; }
    public static string Phrase_cost_opening { get; set; }
    public static string Phrase_cost_free { get; set; }
    public static string Phrase_buy_license { get; set; }
    public static string Phrase_ship_money { get; set; }
    public static string Phrase_ship_gift { get; set; }
    public static string Phrase_ship_free { get; set; }
    public static string Pharse_ship_buy_confirm { get; set; }
    public static string Pharse_ship_buy_decline { get; set; }
    public static string Pharse_ship_sell_confirm { get; set; }
    public static string Pharse_ship_sell_decline { get; set; }
    public static string Pharse_level_buy_confirm { get; set; }
    public static string Pharse_level_buy_decline { get; set; }

    public static void RefreshLocalizedPhrases() {

        Phrase_money = Localization.GetTextValue( "Phrase.Money" );
        Phrase_station = Localization.GetTextValue( "Phrase.Station" );
        Phrase_service_no = Localization.GetTextValue( "Station.Service.No" );
        Phrase_service_norm = Localization.GetTextValue( "Station.Service.Norm" );
        Phrase_service_poor = Localization.GetTextValue( "Station.Service.Poor" );
        Phrase_service_money_no = Localization.GetTextValue( "Station.Service.Money.No" );
        Phrase_service_trade_no = Localization.GetTextValue( "Station.Service.Trade.No" );
        Phrase_service_hold_empty = Localization.GetTextValue( "Station.Service.Hold.Empty" );
        Phrase_service_restore_full = Localization.GetTextValue( "Station.Service.Restore.Full" );
        Phrase_service_restore_partial = Localization.GetTextValue( "Station.Service.Restore.Partial" );
        Phrase_service_filling_full = Localization.GetTextValue( "Station.Service.Filling.Full" );
        Phrase_service_filling_partial = Localization.GetTextValue( "Station.Service.Filling.Partial" );
        Phrase_service_leask_repair = Localization.GetTextValue( "Station.Service.Leaks.Repair" );
        Phrase_hull = Localization.GetTextValue( "Phrase.Hull" );
        Phrase_fuel = Localization.GetTextValue( "Phrase.Fuel" );
        Phrase_engine = Localization.GetTextValue( "Phrase.Engine" );
        Phrase_upgrade = Localization.GetTextValue( "Phrase.Upgrade" );
        Phrase_service = Localization.GetTextValue( "Station.Service" );
        Phrase_maximum = Localization.GetTextValue( "Station.Service.Maximum" );
        Phrase_purchase = Localization.GetTextValue( "Station.Service.Purchase" );
        Phrase_autolanding = Localization.GetTextValue( "Indicator.Name.Autolanding_amount" );
        Phrase_buy_price = Localization.GetTextValue( "Phrase.Price.Buy" );
        Phrase_sell_price = Localization.GetTextValue( "Phrase.Price.Sell" );
        Phrase_buy_ship = Localization.GetTextValue( "Phrase.Ship.Buy" );
        Phrase_sell_ship = Localization.GetTextValue( "Phrase.Ship.Sell" );
        Phrase_gravity_stable = Localization.GetTextValue( "Level.Gravity.Stable" );
        Phrase_gravity_unstable = Localization.GetTextValue( "Level.Gravity.Unstable" );
        Phrase_gravity_type = Localization.GetTextValue( "Level.Gravity.Type" );
        Phrase_gravity_force = Localization.GetTextValue( "Level.Gravity.Force" );
        Phrase_ship_recommended = Localization.GetTextValue( "Level.Ship.Recommended" );
        Phrase_cost_opening = Localization.GetTextValue( "Level.Cost.Opening" );
        Phrase_cost_free = Localization.GetTextValue( "Level.Cost.Free" );
        Phrase_buy_license = Localization.GetTextValue( "Level.Buy.License" );
        Phrase_ship_money = Localization.GetTextValue( "Ship.Money" );
        Phrase_ship_gift = Localization.GetTextValue( "Ship.Gift" );
        Phrase_ship_free = Localization.GetTextValue( "Ship.Free" );
        Pharse_ship_buy_confirm = Localization.GetTextValue( "Ship.Buy.Confirm" );
        Pharse_ship_buy_decline = Localization.GetTextValue( "Ship.Buy.Decline" );
        Pharse_ship_sell_confirm = Localization.GetTextValue( "Ship.Sell.Confirm" );
        Pharse_ship_sell_decline = Localization.GetTextValue( "Ship.Sell.Decline" );
        Pharse_level_buy_confirm = Localization.GetTextValue( "Level.Buy.Confirm" );
        Pharse_level_buy_decline = Localization.GetTextValue( "Level.Buy.Decline" );
    }
    #endregion

    // Ссылки для быстрого доступа к ключевым глобальным объектам ##############################################################################################################
    #region QUICK_REFERENCES
    public static Level Level { get; set; }
    public static GameControl Control { get; set; }
    public static CanvasMessage Message { get; set; }
    public static CanvasGame Canvas { get; set; }
    public static Camera Camera { get; set; }
    public static CameraControl Camera_control { get; set; }
    public static Transform Camera_transform { get; set; }
    public static Player Player { get; set; }
    public static Transform Player_transform { get; set; }
    public static Timer Timer { get; set; }
    public static Radar Radar { get; set; }
    public static InventoryTrade Trade { get; set; }
    public static CanvasNavigator Navigator { get; set; }
    public static EffectControl Effects_control { get; set; }
    public static ScenarioControl Scenario_control { get; set; }
    public static ZoomControl Zoom_control { get; set; }
    public static InputControl Input_control { get; set; }
    public static LocalizationControl Localization { get; set; }
    #endregion

    // State of the game #######################################################################################################################################################
    private static GameState state = GameState.Reset;
    public static GameState State { get { return state; } }

    public static void AddState( GameState add_state ) { state |= add_state; }
    public static void SetState( GameState new_state ) { state = new_state; }
    public static void ResetState( GameState old_state ) { state &= ~old_state; }
    public static bool Is( GameState check_state ) { return (state & check_state) != 0; }

    public static void Pause() { Time.timeScale = 0f; Game.AddState( GameState.Paused ); Game.Control.DisabeAudioExceptMusic(); }
    public static void Resume() { Time.timeScale = 1f; Game.ResetState( GameState.Paused ); Game.Control.EnabeAudioAll(); }

    // Returns a maximum closing speed between collised objects ################################################################################################################
    public static float ClosingSpeed( Collision collision ) {

        float max_speed = 0f;
        Vector3 speed = collision.relativeVelocity;

        if( Mathf.Abs( speed.x ) > max_speed ) max_speed = Mathf.Abs( speed.x );
        if( Mathf.Abs( speed.y ) > max_speed ) max_speed = Mathf.Abs( speed.y );
        if( Mathf.Abs( speed.z ) > max_speed ) max_speed = Mathf.Abs( speed.z );

        return max_speed * Game.Level.Complication;
    }

    // Calculates position of collision's effect ###############################################################################################################################
    public static Vector3 CalculateEffectPosition( Transform first_transform, Transform other_transform ) {

        Vector3 operation = first_transform.position;

        if( first_transform.position.x > other_transform.position.x ) operation.x -= (first_transform.position.x - other_transform.position.x) * 0.5f;
        else operation.x += (other_transform.position.x - first_transform.position.x) * 0.5f;

        if( first_transform.position.y > other_transform.position.y ) operation.y -= (first_transform.position.y - other_transform.position.y) * 0.5f;
        else operation.y += (other_transform.position.y - first_transform.position.y) * 0.5f;

        return operation;
    }

    // Показывает индикаторы и соответствующий текст на панели меню ############################################################################################################
    public static void ReportIndicator( Indicator indicator, EffectiveText text, Image bar_available, Image bar_maximum, bool use_units, bool use_float ) {

        text.Rewrite( Game.Localization.GetTextValue( indicator.Name_key ) ).Append( Game.Separator_colon ).Append( Game.Separator_space );
        if( indicator.Type != IndicatorType.Autolanding_amount ) text.Append( Game.Localization.GetTextValue( indicator.Upgrade_key ) ).Append( Game.Separator_space );

        // Определяем текущее значение индикатора
        float available = (indicator.Type == IndicatorType.Hold_capacty) ||
                          (indicator.Type == IndicatorType.Shield_time) ||
                          (indicator.Type == IndicatorType.Charge_time) ? indicator.Maximum : indicator.Available;

        // Определяем максимально возможное значение (без апгрэйда)
        float maximum = indicator.Upgrade_max_ship;

        if( use_float ) text.AppendDottedFloat( available );
        else text.Append( Mathf.FloorToInt( available ) );

        if( use_units ) text.Append( Game.Separator_space ).Append( Game.Localization.GetTextValue( indicator.Unit_key ) ).Append( Game.Separator_space ).Append( Game.Separator_slash ).Append( Game.Separator_space );
        else text.Append( Game.Separator_space ).Append( Game.Separator_slash ).Append( Game.Separator_space );

        if( use_float ) text.AppendDottedFloat( maximum );
        else text.Append( Mathf.FloorToInt( maximum ) );

        if( use_units ) text.Append( Game.Separator_space ).Append( Game.Localization.GetTextValue( indicator.Unit_key ) );

        // Определяем текущее значение индикатора для прогресса: оно может отличаться от того, как соотносятся цифровые значения
        available = (indicator.Type == IndicatorType.Hold_capacty) ? indicator.Maximum : indicator.Available;

        // Определяем максимально возможное значение (без апгрэйда): оно может отличаться от того, как соотносятся цифровые значения
        maximum = (indicator.Type == IndicatorType.Charge_time) ? (indicator.Maximum - indicator.Upgrade_max_ship) : (
                  (indicator.Type == IndicatorType.Autolanding_amount) ? indicator.Available : indicator.Maximum);

        // Определяем делитель для правильного отображения прогресса
        float divider = (indicator.Type == IndicatorType.Charge_time) ? indicator.Maximum_inversed : indicator.Upgrade_max_ship_inversed;
        
        // Отображаем индикаторы прогресса в зависимости от их типа
        bar_available.fillAmount = available * divider;
        bar_maximum.fillAmount = maximum * divider;
    }
}