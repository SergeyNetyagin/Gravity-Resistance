using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveGame : MonoBehaviour {

    // Сохранение последнего игрового состояния ################################################################################################################################
    public static void Save( GameData game_data ) {

        string config_file_name = Game.ConfigFileName();

        // Копирование глобальных игровых данных
        game_data.Save();

        if( !Directory.Exists( Game.Path_config ) ) Directory.CreateDirectory( Game.Path_config );

        // Если директория конфигурации найдена, сохраняем глобальные данные в неё
        if( Directory.Exists( Game.Path_config ) ) {

            if( File.Exists( config_file_name ) ) File.Delete( config_file_name );

            BinaryFormatter binary_formatter = new BinaryFormatter();
            FileStream config_file = File.Create( config_file_name );
            binary_formatter.Serialize( config_file, game_data );
            config_file.Close();
        }
    }

    // Save a data for this level ##############################################################################################################################################
    public static void Save( Level level ) {

        string level_file_name = Game.LevelFileName( level );

        LevelData level_data = new LevelData();

        // Копирование данных уровня
        level_data.Save( level );

        if( !Directory.Exists( Game.Path_levels ) ) Directory.CreateDirectory( Game.Path_levels );

        // Если директория конфигурации найдена, сохраняем глобальные данные в неё
        if( Directory.Exists( Game.Path_levels ) ) {

            if( File.Exists( level_file_name ) ) File.Delete( level_file_name );

            BinaryFormatter binary_formatter = new BinaryFormatter();
            FileStream level_file = File.Create( level_file_name );
            binary_formatter.Serialize( level_file, level_data );
            level_file.Close();
        }
    }

    // Save a data for this ship ###############################################################################################################################################
    public static void Save( Ship ship ) {

        string ship_file_name = Game.ShipFileName( ship );

        ShipData ship_data = new ShipData( ship );

        // Копирование данных корабля
        ship_data.Save( ship );

        if( !Directory.Exists( Game.Path_ships ) ) Directory.CreateDirectory( Game.Path_ships );

        // Если директория конфигурации найдена, сохраняем глобальные данные в неё
        if( Directory.Exists( Game.Path_ships ) ) {

            if( File.Exists( ship_file_name ) ) File.Delete( ship_file_name );

            BinaryFormatter binary_formatter = new BinaryFormatter();
            FileStream ship_file = File.Create( ship_file_name );
            binary_formatter.Serialize( ship_file, ship_data );
            ship_file.Close();
        }
    }

    // Сохраняет данные игрока #################################################################################################################################################
    public static void Save( Player player ) {

        string player_file_name = Game.PlayerFileName( player );

        PlayerData player_data = new PlayerData();

        // Копирование данных игрока
        player_data.Save( player );

        if( !Directory.Exists( Game.Path_player ) ) Directory.CreateDirectory( Game.Path_player );

        // Если директория найдена, сохраняем данные игрока в неё
        if( Directory.Exists( Game.Path_player ) ) {

            if( File.Exists( player_file_name ) ) File.Delete( player_file_name );

            BinaryFormatter binary_formatter = new BinaryFormatter();
            FileStream player_file = File.Create( player_file_name );
            binary_formatter.Serialize( player_file, player_data );
            player_file.Close();
        }
    }

    // Remove ship's data for this ship ########################################################################################################################################
    public static void RemoveShipFile( Ship ship ) {

        string ship_file_name = Game.ShipFileName( ship );

        if( File.Exists( ship_file_name ) ) File.Delete( ship_file_name );
    }
}