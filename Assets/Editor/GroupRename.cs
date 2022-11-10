using UnityEngine;
using UnityEditor;

public class GroupRename : EditorWindow {

    public string 
        old_name = "",
        new_name = "";
    
    // Open the dialog window ##################################################################################################################################################
    [MenuItem( "Custom/Rename group" )]
    static void ShowWindow () { 

       GroupRename window = (GroupRename) EditorWindow.GetWindow( typeof( GroupRename ) ); 
       window.Show (); 
    } 

    // Show the dialog window ##################################################################################################################################################
    void OnGUI () { 

        GUILayout.Label( "Rename:", EditorStyles.boldLabel ); 
        GUILayout.BeginHorizontal(); 

        old_name = GUILayout.TextField( old_name, GUILayout.Width( 100 ) );
        GUILayout.Label( "to..." );
        new_name = GUILayout.TextField( new_name, GUILayout.Width( 100 ) );

        GUILayout.Label( "" );
        if( GUILayout.Button( "Rename" ) ) Rename();

        GUILayout.EndHorizontal(); 
    } 

    // Rename selected game objects ############################################################################################################################################
    void Rename() {

        if( Selection.activeGameObject == null ) {

            if( Application.isEditor ) Debug.Log( "No selected objects found" );
            return;
        }

        if( (old_name.Length == 0) || (new_name.Length == 0) ) {

            if( Application.isEditor ) Debug.Log( "Error of the entered name" );
            return;
        }

        Object[] game_objects = Selection.GetFiltered( typeof( GameObject ), SelectionMode.Editable );

        if( game_objects == null ) {

            if( Application.isEditor ) Debug.Log( "No selected objects found" );
            return;
        }

        string work_name = string.Empty;

        for( int i = 0; i < game_objects.Length; i++ ) {

            work_name = game_objects[i].name.Replace( old_name, new_name );
            game_objects[i].name = work_name;
        }
    } 
}
