using UnityEngine;
using UnityEngine.SceneManagement;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LoadGame : MonoBehaviour {

    //[Tooltip( "Вынужденная мера, поскольку иначе Юнити не находит этот компонент на объекте (явный баг Юнити)" )]
    //[SerializeField]
    //private EffectControl effect_control;

	// Use this for initialization #############################################################################################################################################
	void Awake() {

        // Если необходимо, удаляем сохранения
        #region TESTING_MODE_CLEAR SAVES
        #if UNITY_EDITOR

        GameTest game_test = GameObject.FindObjectOfType<GameTest>();

        if( (game_test != null) && game_test.Clear_saves ) {

            if( Directory.Exists( Game.Path_config ) ) Directory.Delete( Game.Path_config, true );
            if( Directory.Exists( Game.Path_levels ) ) Directory.Delete( Game.Path_levels, true );
            if( Directory.Exists( Game.Path_ships ) ) Directory.Delete( Game.Path_ships, true );
        }

        if( game_test.Use_immortal_mode ) Game.Use_immortal_mode = true;

        #endif
        #endregion

        // Инициализируем генератор случайных чисел для текущего уровня случайным "зерном"
        UnityEngine.Random.InitState( System.DateTime.Now.Second * System.DateTime.Now.Millisecond );
        
        // Если загружается игровой уровень
        if( (LevelType) SceneManager.GetActiveScene().buildIndex > LevelType.Level_Menu ) {
          
            Game.SetState( GameState.Loading );

            // Здесь приходится пользоваться более медленным методом по причине того, что у объектов разных уровней разные имена
            Game.Level = GameObject.FindObjectOfType<Level>();
            Game.Effects_control = GameObject.FindObjectOfType<EffectControl>();
            
            // Вынужденная мера, поскольку иначе Юнити не находит этот компонент на объекте (явный баг Юнити)
            //Game.Effects_control = effect_control;
            //Game.Effects_control = GameObject.Find( "Effects" ).GetComponent<EffectControl>();
                        
            Game.Scenario_control = GameObject.Find( "Scenario" ).GetComponent<ScenarioControl>();

            Game.Canvas = GameObject.Find( "Canvas" ).GetComponent<CanvasGame>();
            Game.Message = Game.Canvas.GetComponent<CanvasMessage>();
            Game.Navigator = Game.Canvas.GetComponent<CanvasNavigator>();
            Game.Trade = Game.Canvas.GetComponentInChildren<InventoryTrade>();

            Game.Camera = GameObject.Find( "Camera" ).GetComponent<Camera>();
            Game.Camera_control = Game.Camera.GetComponent<CameraControl>();
            Game.Camera_transform = Game.Camera.GetComponent<Transform>();

            Game.Player = GameObject.Find( "Player" ).GetComponent<Player>();
            Game.Player_transform = Game.Player.GetComponent<Transform>();
            Game.Timer = Game.Player.GetComponentInChildren<Timer>();
            Game.Radar = Game.Player.GetComponentInChildren<Radar>();

            Game.Input_control = GameObject.Find( "Event_system" ).GetComponent<InputControl>();
            Game.Zoom_control = Game.Input_control.GetComponent<ZoomControl>();

            Game.Control = GameObject.Find( "Control" ).GetComponent<GameControl>();
            Game.Localization = Game.Control.GetComponent<LocalizationControl>();
        }

        // Если это уровень главного меню
        else if( (LevelType) SceneManager.GetActiveScene().buildIndex == LevelType.Level_Menu ) {

            Game.Localization = GameObject.FindObjectOfType<LocalizationControl>();
            Game.Control = GameObject.FindObjectOfType<GameControl>();
        }

        // Если это уровень выбора игрового персонажа
        else if( (LevelType) SceneManager.GetActiveScene().buildIndex == LevelType.Level_Hero ) {

            Game.Localization = GameObject.FindObjectOfType<LocalizationControl>();
        }

        // На тот случай, когда уровень стартовал не из меню, а непосредственно из редактора
        Game.Current_level = (LevelType) SceneManager.GetActiveScene().buildIndex;
    }
  
    // Загрузка последнего игрового состояния ##################################################################################################################################
    public static void Load( GameData game_data ) {

        string config_file_name = Game.ConfigFileName();

        Game.Is_first_time = true;
            
        // Если файл конфигурации найден, глобальные данные состояния инициалиируются из этого файла
        if( File.Exists( config_file_name ) ) {

            Game.Is_first_time = false;

            BinaryFormatter binary_formatter = new BinaryFormatter();
            FileStream config_file = File.Open( config_file_name, FileMode.Open );
            GameData loaded_data = (GameData) binary_formatter.Deserialize( config_file );
            config_file.Close();

            if( loaded_data != null ) game_data = loaded_data;

            game_data.Load();
        }

        // Устанавливаем некоторые тестовые значения, если они включены
        #if UNITY_EDITOR
        GameTest game_test = GameObject.FindObjectOfType<GameTest>();
        if( (game_test != null) && game_test.Use_game_testing_values ) game_test.Load();
        #endif
    }

    // Load a data for this level ##############################################################################################################################################
    public static void Load( Level level ) {

        string level_file_name = Game.LevelFileName( level );

        // Если файл конфигурации найден, данные уровня инициалиируются из этого файла
        if( File.Exists( level_file_name ) ) {

            BinaryFormatter binary_formatter = new BinaryFormatter();
            FileStream level_file = File.Open( level_file_name, FileMode.Open );
            LevelData level_data = (LevelData) binary_formatter.Deserialize( level_file );
            level_file.Close();

            if( level_data != null ) level_data.Load( level );
        }

        // Устанавливаем некоторые тестовые значения, если они включены
        #if UNITY_EDITOR
        GameTest game_test = GameObject.FindObjectOfType<GameTest>();
        if( (game_test != null) && game_test.Use_level_testing_values ) game_test.Load( level );
        #endif
    }

    // Load a data for this ship ###############################################################################################################################################
    public static void Load( Ship ship ) {

        string ship_file_name = Game.ShipFileName( ship );

        // Если файл конфигурации найден, данные уровня инициалиируются из этого файла
        if( File.Exists( ship_file_name ) ) {

            BinaryFormatter binary_formatter = new BinaryFormatter();
            FileStream ship_file = File.Open( ship_file_name, FileMode.Open );
            ShipData ship_data = (ShipData) binary_formatter.Deserialize( ship_file );
            ship_file.Close();

            if( ship_data != null ) ship_data.Load( ship );
        }

        // Устанавливаем некоторые тестовые значения, если они включены
        #if UNITY_EDITOR
        GameTest game_test = GameObject.FindObjectOfType<GameTest>();
        if( (game_test != null) && game_test.Use_ship_testing_values ) game_test.Load( ship );
        #endif
    }

    // Загружает данные игрока #################################################################################################################################################
    public static void Load( Player player ) {

        string player_file_name = Game.PlayerFileName( player );

        // Если файл конфигурации найден, данные игрока инициалиируются из этого файла
        if( File.Exists( player_file_name ) ) {

            BinaryFormatter binary_formatter = new BinaryFormatter();
            FileStream player_file = File.Open( player_file_name, FileMode.Open );
            PlayerData player_data = (PlayerData) binary_formatter.Deserialize( player_file );
            player_file.Close();

            if( player_data != null ) player_data.Load( player );
        }
    }
}