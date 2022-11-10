using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

namespace TurnTheGameOn.NPCChat{

	[CustomEditor(typeof(ChatManager))]
	public class Editor_Chatmanager : Editor {

		ChatManager chatManager;

		static bool showNPCIndexes;
		static bool showHelp;
		static int conversationIndexes;

		public override void OnInspectorGUI(){
			chatManager = Resources.Load ("ChatManager") as ChatManager;
			if (conversationIndexes == 0) {
				conversationIndexes = chatManager.currentDialogue.Length;
			}
			GUISkin editorSkin = Resources.Load("EditorSkin") as GUISkin;

			EditorGUILayout.BeginVertical("Box");

			EditorGUILayout.BeginHorizontal ();
			Texture NPCChatTexture = Resources.Load("NPCChatTexture") as Texture;
			GUILayout.Label(NPCChatTexture,editorSkin.customStyles [0]);
			GUI.skin = null;
			if (GUILayout.Button ("Show Hints", GUILayout.Width(90))) {
				if (showHelp) {
					showHelp = false;
				} else {
					showHelp = true;
				}
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			SerializedProperty noteText = serializedObject.FindProperty ("noteText");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (noteText, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Use this text area to keep notes about NPC Conversation Indexes", editorSkin.customStyles [4]);

			SerializedProperty targetNPC = serializedObject.FindProperty ("targetNPC");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (targetNPC, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "The NPC Conversation Index changed when calling NewDialogue(int)", editorSkin.customStyles [4]);

			EditorGUILayout.BeginHorizontal ();
			SerializedProperty conversationIndex = serializedObject.FindProperty ("currentDialogue");
			conversationIndexes = EditorGUILayout.IntField ("NPC Indexes", conversationIndexes);			
			if (GUILayout.Button ("Update")) {
				conversationIndex.arraySize = conversationIndexes;
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
				serializedObject.ApplyModifiedProperties ();
			}
			EditorGUILayout.EndHorizontal ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Set the number of NPC Conversation Indexes", editorSkin.customStyles [4]);

			EditorGUILayout.BeginVertical("Box");
			showNPCIndexes = EditorGUI.Foldout (EditorGUILayout.GetControlRect(), showNPCIndexes, "   NPC Conversation Indexes", true);
			EditorGUILayout.EndVertical();
			if(showNPCIndexes){
				int size = conversationIndex.arraySize;
				for(int i = 0; i < size; i++){
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("NPC Index " + i, GUILayout.MaxWidth (90));
						SerializedProperty currentDialogue = serializedObject.FindProperty ("currentDialogue").GetArrayElementAtIndex (i);
						EditorGUI.BeginChangeCheck ();
						EditorGUILayout.PropertyField (currentDialogue, GUIContent.none, true);
						if (EditorGUI.EndChangeCheck ())
							serializedObject.ApplyModifiedProperties ();
						EditorGUILayout.EndHorizontal ();
				}
			}
			if(showHelp)
				EditorGUILayout.LabelField ( "Determines which conversation the assigned NPC will use", editorSkin.customStyles [4]);


			EditorGUILayout.EndVertical ();
		}
	}
}