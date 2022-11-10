using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEditor;

namespace TurnTheGameOn.NPCChat{



	[CustomEditor(typeof(NPCChat))]
	public class Editor_NPCChatUpdate : Editor {

		[MenuItem("Assets/Create/ChatManager")]
		public static void CreateChatManager(){
			ChatManager asset = ScriptableObject.CreateInstance<ChatManager>();
			AssetDatabase.CreateAsset (asset, "Assets/ChatManager.asset");
			AssetDatabase.SaveAssets ();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
		}
		
		[MenuItem("GameObject/UI/NPC Chat/NPC Chat Object")]
		public static void CreateNPCChat(){
			GameObject NPC = Instantiate(Resources.Load("NPC Chat")) as GameObject;
			NPC.name = "NPC Chat";
			Selection.activeObject = NPC;
			Debug.Log("NPC Chat Game Object added to scene.");
		}

		[MenuItem("GameObject/UI/NPC Chat/Default Chat Box")]
		static void Create(){
			GameObject canvasCheck = Selection.activeGameObject;
			if (canvasCheck == null) {
				Canvas findCanvas = GameObject.FindObjectOfType<Canvas> ();
				if (findCanvas != null) {
					canvasCheck = findCanvas.gameObject;
				} else {
					canvasCheck = new GameObject ("Canvas", typeof(Canvas));
					canvasCheck.GetComponent<Canvas> ().renderMode = RenderMode.ScreenSpaceOverlay;
					canvasCheck.AddComponent <GraphicRaycaster>();
				}
				GameObject instance = Instantiate (Resources.Load ("Default Chat Box", typeof(GameObject))) as GameObject;
				instance.transform.SetParent(canvasCheck.transform);
				instance.name = "Default Chat Box";
				instance.transform.localPosition = new Vector3(0,0,0);
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = instance;
				canvasCheck = null;
				canvasCheck = GameObject.Find ("Event System");
				if(canvasCheck == null){
					canvasCheck = new GameObject ("Event System", typeof(EventSystem));
					canvasCheck.AddComponent<StandaloneInputModule> ();
				}
				canvasCheck = null;
				instance = null;		
			} else {
				Canvas findCanvas = GameObject.FindObjectOfType<Canvas> ();
				if (findCanvas != null && canvasCheck.GetComponent<Canvas>() == null) {
					canvasCheck = findCanvas.gameObject;
				}
				GameObject instance = Instantiate (Resources.Load ("Default Chat Box", typeof(GameObject))) as GameObject;
				instance.name = "Default Chat Box";
				instance.transform.SetParent(canvasCheck.transform);
				instance.transform.localPosition = new Vector3(0,0,0);
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = instance;
				canvasCheck = null;
				canvasCheck = GameObject.Find ("Event System");
				if(canvasCheck == null){
					canvasCheck = new GameObject ("Event System", typeof(EventSystem));
					canvasCheck.AddComponent<StandaloneInputModule> ();
				}
				canvasCheck = null;
				instance = null;
			}
			Debug.Log("NPC Chat Box Object added to scene.");
		}

		int editorConversation;
		int editorPage;
		static bool showConversationSettings;
		static bool showGeneralSettings;
		static bool showButtonSettings;
		static bool showPageEventSettings;
		static bool showChatEvents;
		static bool showPageDialogue;
		static bool showHelp;

		public override void OnInspectorGUI(){
			GUISkin editorSkin = Resources.Load("EditorSkin") as GUISkin;



			NPCChat chatComponent = (NPCChat)target;
			chatComponent.chatManager = Resources.Load ("ChatManager") as ChatManager;





			EditorGUILayout.BeginVertical("box");
			GUI.skin = editorSkin;

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
			EditorGUILayout.LabelField ("Chat Manager Index", EditorStyles.miniBoldLabel);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (8);
			EditorGUILayout.BeginVertical ("box");
			///NPC Number
			SerializedProperty nPCNumber = serializedObject.FindProperty ("nPCNumber");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (nPCNumber, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Chat Manager NPC Conversation Index", editorSkin.customStyles [4]);

			EditorGUILayout.BeginHorizontal ();



			SerializedProperty conversations = serializedObject.FindProperty ("conversations");
			if (conversations.intValue == 0)
				conversations.intValue = 1;
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (conversations, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if (GUILayout.Button ("Update")) {
				editorConversation = 0;
				editorPage = 0;
				chatComponent.UpdateConversations ();
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
			}
			EditorGUILayout.EndHorizontal ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Set the number of conversations for this NPC component", editorSkin.customStyles [4]);

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();






			EditorGUILayout.Space ();
			EditorGUILayout.LabelField ("Conversations", EditorStyles.miniBoldLabel);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (8);
			EditorGUILayout.BeginVertical ("box");

				EditorGUILayout.BeginHorizontal ();
				
				if (editorConversation >= 1) {
					if (GUILayout.Button ("-", GUILayout.MaxWidth (40))) {
						editorConversation -= 1;
						editorPage = 0;
						chatComponent.tempConv = editorConversation;
						chatComponent.tempInt = chatComponent._NPCDialogue [editorConversation].pagesOfChat;
						EditorGUI.FocusTextInControl (null);
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
					}
				} else {
					GUILayout.Box ("", GUILayout.MaxWidth (40));
				}
				if (editorConversation < chatComponent._NPCDialogue.Length - 1) {
					if (GUILayout.Button ("+", GUILayout.MaxWidth (40))) {
						editorConversation += 1;
						editorPage = 0;
						chatComponent.tempConv = editorConversation;
						chatComponent.tempInt = chatComponent._NPCDialogue [editorConversation].pagesOfChat;
						EditorGUI.FocusTextInControl (null);
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
					}
				} else {
					GUILayout.Box ("", GUILayout.MaxWidth (40));
				}
				EditorGUILayout.LabelField ("Conversation " + editorConversation.ToString ());
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.Space ();
				if(showHelp)
					EditorGUILayout.LabelField ( "Current conversation being edited", editorSkin.customStyles [4]);

				EditorGUILayout.BeginHorizontal ();
				chatComponent.tempInt = EditorGUILayout.IntField ("Chat Pages", chatComponent.tempInt);
				if (chatComponent.tempInt < 1)
					chatComponent.tempInt = 1;
				if (GUILayout.Button ("Update")) {				
					chatComponent.tempConv = editorConversation;
					chatComponent.canUpdatePages = true;
					chatComponent.UpdateConversations ();
					editorPage = 0;
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;
				}
				EditorGUILayout.EndHorizontal ();
				if(showHelp)
					EditorGUILayout.LabelField ( "Dialogue pages in this conversation", editorSkin.customStyles [4]);

			///Set Next Conversation
			SerializedProperty useNextDialogue = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("useNextDialogue");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (useNextDialogue, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			 
			//chatComponent._NPCDialogue [editorConversation].useNextDialogue = EditorGUILayout.Toggle ("Set Next Conversation", chatComponent._NPCDialogue [editorConversation].useNextDialogue);
			if(showHelp)
				EditorGUILayout.LabelField ( "Set a new conversation in Chat Manager after this one is completed", editorSkin.customStyles [4]);
			chatComponent.CalculateArrays ();
			if (chatComponent._NPCDialogue [editorConversation].useNextDialogue == true) {


				//chatComponent._NPCDialogue [editorConversation].nextDialogue = EditorGUILayout.IntSlider ("Next", chatComponent._NPCDialogue [editorConversation].nextDialogue, 0, (chatComponent._NPCDialogue.Length - 1));
				SerializedProperty nextDialogue = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("nextDialogue");
			EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (nextDialogue, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();

				if(showHelp)
					EditorGUILayout.LabelField ( "Conversation set in Chat Manager after this conversation is completed", editorSkin.customStyles [4]);
			}

		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();






			EditorGUILayout.Space ();
			EditorGUILayout.LabelField ("Conversation Pages", EditorStyles.miniBoldLabel);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (8);
			EditorGUILayout.BeginVertical ("box");
				EditorGUILayout.BeginHorizontal ();
				

				if (editorPage >= 1) {
					if (GUILayout.Button ("-", GUILayout.MaxWidth (40))) {
						editorPage -= 1;
						EditorGUI.FocusTextInControl (null);
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
					}
				} else {
					GUILayout.Box ("", GUILayout.MaxWidth (40));
				}
				if (editorPage < chatComponent._NPCDialogue [editorConversation].pagesOfChat - 1) {
					if (GUILayout.Button ("+", GUILayout.MaxWidth (40))) {
						editorPage += 1;
						EditorGUI.FocusTextInControl (null);
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
					}
				} else {
					GUILayout.Box ("", GUILayout.MaxWidth (40));
				}
			EditorGUILayout.LabelField ("Conversation " + editorConversation.ToString () + " - Page " + editorPage.ToString ());
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.Space ();
				if(showHelp)
					EditorGUILayout.LabelField ( "Current page being edited", editorSkin.customStyles [4]);


			EditorGUILayout.Space ();EditorGUILayout.Space ();
			GUI.skin = editorSkin;
			EditorGUILayout.BeginVertical ("Box");
			GUI.skin = null;
				chatComponent._NPCDialogue [editorConversation].chatBoxes [editorPage] = 
					(GameObject)EditorGUILayout.ObjectField ("Chat Box", chatComponent._NPCDialogue [editorConversation].chatBoxes [editorPage], typeof(GameObject), true) as GameObject;
				if(showHelp)
					EditorGUILayout.LabelField ( "Assigned Chat Box for the page", editorSkin.customStyles [4]);
			EditorGUILayout.EndVertical ();

			GUI.skin = editorSkin;
			EditorGUILayout.BeginVertical ("Box");
			GUI.skin = null;
				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Chat Box Header Text");
				EditorGUILayout.EndHorizontal ();

				chatComponent._NPCDialogue [editorConversation].NPCName [editorPage] = EditorGUILayout.TextField (chatComponent._NPCDialogue [editorConversation].NPCName [editorPage]);
				EditorGUILayout.Space ();
				if(showHelp)
					EditorGUILayout.LabelField ( "Header field text, typically the NPC's name", editorSkin.customStyles [4]);
			EditorGUILayout.EndVertical ();


			GUI.skin = editorSkin;
			EditorGUILayout.BeginVertical ("Box");
			GUI.skin = null;
				EditorStyles.textField.wordWrap = true;
				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Chat Box Text");
				EditorGUILayout.EndHorizontal ();

				chatComponent._NPCDialogue [editorConversation].chatPages [editorPage] = EditorGUILayout.TextArea (chatComponent._NPCDialogue [editorConversation].chatPages [editorPage]);
				if(showHelp)
					EditorGUILayout.LabelField ( "Dialogue or message for the Chat Box", editorSkin.customStyles [4]);
			EditorGUILayout.EndVertical ();


			/// Page Audio
			/// 


			GUI.skin = editorSkin;
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			//GUI.skin = null;
			EditorGUILayout.LabelField ("Page Audio", GUILayout.MaxWidth(90));
			SerializedProperty pageAudio = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("pageAudio").GetArrayElementAtIndex(editorPage);
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (pageAudio, GUIContent.none, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndHorizontal ();			

			if(showHelp)
				EditorGUILayout.LabelField ( "AudioClip played when page starts", editorSkin.customStyles [4]);
			EditorGUILayout.EndVertical ();

			if (chatComponent._NPCDialogue [editorConversation].pageAudio [editorPage] != null) {
				EditorGUILayout.BeginHorizontal ();
				//GUI.skin = null;
				EditorGUILayout.LabelField ("Loop AudioClip", GUILayout.MaxWidth(90));
				SerializedProperty loopAudio = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("loopAudio").GetArrayElementAtIndex(editorPage);
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (loopAudio, GUIContent.none, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				EditorGUILayout.EndHorizontal ();
				//chatComponent._NPCDialogue [editorConversation].loopAudio [editorPage] = EditorGUILayout.Toggle ("Loop Clip", chatComponent._NPCDialogue [editorConversation].loopAudio [editorPage]);
				if (showHelp)
					EditorGUILayout.LabelField ("Loop this page's AudioCllip", editorSkin.customStyles [4]);
			}

			/// Page Events

			 
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space ();
			GUI.skin = editorSkin;
			EditorGUILayout.BeginVertical ("Box");
			showPageEventSettings = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showPageEventSettings, "  Page Events", true);
			EditorGUILayout.EndVertical ();
			GUI.skin = null;
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Space ();

			if (showPageEventSettings) {
				EditorGUILayout.LabelField ("On Page Start Event");
				SerializedProperty OnPageStartEvent = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("OnPageStartEvent").GetArrayElementAtIndex (editorPage);

				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (OnPageStartEvent, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();


				EditorGUILayout.LabelField ("On Page End Event");
				SerializedProperty OnPageEndEvent = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("OnPageEndEvent").GetArrayElementAtIndex (editorPage);

				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (OnPageEndEvent, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
			}

			///
			/// Page Buttons
			///

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space ();
			GUI.skin = editorSkin;
			EditorGUILayout.BeginVertical ("Box");
			showButtonSettings = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showButtonSettings, "  Page Buttons", true);
			EditorGUILayout.EndVertical ();
			GUI.skin = null;
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Space ();

			if (showButtonSettings) {

				EditorGUILayout.Space ();
				chatComponent._NPCDialogue [editorConversation].enableButtonAfterDialogue [editorPage] = EditorGUILayout.Toggle ("Enable After Dialogue", chatComponent._NPCDialogue [editorConversation].enableButtonAfterDialogue [editorPage]);
				if(showHelp)
					EditorGUILayout.LabelField ( "Enable Page Buttons after dialogue finishes scrolling", editorSkin.customStyles [4]);
				EditorGUILayout.Space ();

				for (int i = 0; i < 6; i++) {

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.Space ();
					SerializedProperty enableButton = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("NPCButtons").GetArrayElementAtIndex (editorPage).FindPropertyRelative ("buttonComponent").GetArrayElementAtIndex (i);
					EditorGUI.BeginChangeCheck ();
					EditorGUILayout.PropertyField (enableButton, true);
					if (EditorGUI.EndChangeCheck ())
						serializedObject.ApplyModifiedProperties ();

					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.Space ();
					if(showHelp)
						EditorGUILayout.LabelField ( "Enable or Disable this button", editorSkin.customStyles [4]);

					if (chatComponent._NPCDialogue[editorConversation].NPCButtons[editorPage].buttonComponent[i] == NPCDialogueButtons.ItemType.enableButton) {
						SerializedProperty buttonString = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("NPCButtons").GetArrayElementAtIndex (editorPage).FindPropertyRelative ("buttonString").GetArrayElementAtIndex (i);
						EditorGUI.BeginChangeCheck ();
						EditorGUILayout.PropertyField (buttonString, true);
						if (EditorGUI.EndChangeCheck ())
							serializedObject.ApplyModifiedProperties ();
						if (showHelp)
							EditorGUILayout.LabelField ("Button text", editorSkin.customStyles [4]);

						SerializedProperty NPCClick = serializedObject.FindProperty ("_NPCDialogue").GetArrayElementAtIndex (editorConversation).FindPropertyRelative ("NPCButtons").GetArrayElementAtIndex (editorPage).FindPropertyRelative ("NPCClick").GetArrayElementAtIndex (i);

						EditorGUI.BeginChangeCheck ();
						EditorGUILayout.PropertyField (NPCClick, true);
						if (EditorGUI.EndChangeCheck ()) {
							serializedObject.ApplyModifiedProperties ();
						}
						if (showHelp)
							EditorGUILayout.LabelField ("Assign a ButtonClickedEvent for this button", editorSkin.customStyles [4]);

						EditorGUILayout.Space ();
					}
				}
			}
			/// 
			/// 
			/// 
			/// 
			/// 
			/// 
			/// 
			/// 
			/// 
			/// 
	//		chatComponent._NPCDialogue [editorConversation].setActiveAfter [editorPage] =
	//			(GameObject)EditorGUILayout.ObjectField ("Set Active After", chatComponent._NPCDialogue [editorConversation].setActiveAfter [editorPage], typeof(GameObject), true) as GameObject;
	//		if(showHelp)
	//			EditorGUILayout.LabelField ( "Calls SetActive(true) on assigned GameObject when page completes", editorSkin.customStyles [4]);

	//		chatComponent._NPCDialogue [editorConversation].disableAfter [editorPage] =
	//			(GameObject)EditorGUILayout.ObjectField ("Disable After", chatComponent._NPCDialogue [editorConversation].disableAfter [editorPage], typeof(GameObject), true) as GameObject;
	//		if(showHelp)
	//			EditorGUILayout.LabelField ( "Calls SetActive(false) on assigned GameObject when page completes", editorSkin.customStyles [4]);

	//		chatComponent._NPCDialogue [editorConversation].destroyAfter [editorPage] =
	//			(GameObject)EditorGUILayout.ObjectField ("Destroy After", chatComponent._NPCDialogue [editorConversation].destroyAfter [editorPage], typeof(GameObject), true) as GameObject;
	//		if(showHelp)
	//			EditorGUILayout.LabelField ( "Calls DestroyGameObject() on assigned GameObject when page completes", editorSkin.customStyles [4]);




			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();








			GUI.skin = null;









			EditorGUILayout.Space ();
			EditorGUILayout.LabelField ("Options", EditorStyles.miniBoldLabel);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (8);
			EditorGUILayout.BeginVertical ("box");






			SerializedProperty chatOnCollision = serializedObject.FindProperty ("chatOnCollision");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (chatOnCollision, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			///Chat On Collision

			if(showHelp)
				EditorGUILayout.LabelField ( "Trigger chat on player collision", editorSkin.customStyles [4]);











			///Close Chat On Trigger Exit
			SerializedProperty closeOnTriggerExit = serializedObject.FindProperty ("closeOnTriggerExit");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (closeOnTriggerExit, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Closes chat on player collision exit", editorSkin.customStyles [4]);
			///Close Chat On Mouse Or Key Up
			SerializedProperty closeOnMouseOrKeyUp = serializedObject.FindProperty ("closeOnMouseOrKeyUp");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (closeOnMouseOrKeyUp, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Closes chat on player collision exit", editorSkin.customStyles [4]);
			///Chat On Mouse Up
			SerializedProperty chatOnMouseUp = serializedObject.FindProperty ("chatOnMouseUp");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (chatOnMouseUp, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Trigger chat on mouse-up-over-collider if in range", editorSkin.customStyles [4]);
			///Chat On Key Up
			SerializedProperty chatOnKeyUp = serializedObject.FindProperty ("chatOnKeyUp");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (chatOnKeyUp, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Trigger chat on key-up if in range", editorSkin.customStyles [4]);
			///Chat On Key Down
			SerializedProperty chatOnKeyDown = serializedObject.FindProperty ("chatOnKeyDown");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (chatOnKeyDown, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Trigger chat on key-down if in range", editorSkin.customStyles [4]);


			///Disable On Chat
			SerializedProperty disableOnChat = serializedObject.FindProperty ("disableOnChat");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (disableOnChat, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Disable behaviours when chat is triggered and enable when chat closes", editorSkin.customStyles [4]);
			showChatEvents = EditorGUI.Foldout (EditorGUILayout.GetControlRect (), showChatEvents, "OnChat Events", true);
			if (showChatEvents) {
				///OnChatEvent
				SerializedProperty OnChatEvent = serializedObject.FindProperty ("OnChatEvent");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (OnChatEvent, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if(showHelp)
					EditorGUILayout.LabelField ( "Unity Event called when chat is triggered", editorSkin.customStyles [4]);
				///OnStopChatEvent
				SerializedProperty OnStopChatEvent = serializedObject.FindProperty ("OnStopChatEvent");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (OnStopChatEvent, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if(showHelp)
					EditorGUILayout.LabelField ( "Unity Event called when chat closes", editorSkin.customStyles [4]);
			}
			if(showHelp && !showChatEvents)
				EditorGUILayout.LabelField ( "Unity Events that are called when chat is triggered or closes", editorSkin.customStyles [4]);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();



			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (8);
			EditorGUILayout.BeginVertical ("box");
			///Use Distance Check
			SerializedProperty useDistanceCheck = serializedObject.FindProperty ("useDistanceCheck");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (useDistanceCheck, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Enable distance to chat check", editorSkin.customStyles [4]);

			if (chatComponent.useDistanceCheck) {			
				///Distance To Chat
				SerializedProperty distanceToChat = serializedObject.FindProperty ("distanceToChat");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (distanceToChat, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("NPC radius, must be in range to chat, chat will close if the player leaves this area", editorSkin.customStyles [4]);
				///Chat Radius Offset
				SerializedProperty distanceCheckOffset = serializedObject.FindProperty ("distanceCheckOffset");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (distanceCheckOffset, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("Adjust the position offset of the distance to chat radius", editorSkin.customStyles [4]);
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();




			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (8);
			EditorGUILayout.BeginVertical ("box");
			///Enable Text Scrolling
			SerializedProperty scrollPageText = serializedObject.FindProperty ("scrollPageText");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (scrollPageText, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if(showHelp)
				EditorGUILayout.LabelField ( "Enable Text Scrolling", editorSkin.customStyles [4]);
			if(chatComponent.scrollPageText){
				///Text Scroll Speed
				SerializedProperty textScrollSpeed = serializedObject.FindProperty ("textScrollSpeed");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (textScrollSpeed, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if(showHelp)
					EditorGUILayout.LabelField ( "0.0001 is fastest, 20 is slowest", editorSkin.customStyles [4]);
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();



			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (8);
			EditorGUILayout.BeginVertical ("box");
			///NPCOutline Toggle
			SerializedProperty enableOutline = serializedObject.FindProperty ("enableOutline");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (enableOutline, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if (showHelp)
				EditorGUILayout.LabelField ("Requires Use Distance Check, Enable or disable the NPC Outline Shader", editorSkin.customStyles [4]);
			if (!chatComponent.useDistanceCheck && chatComponent.enableOutline) {
				EditorGUILayout.LabelField ("Requires Use Distance Check", editorSkin.customStyles [4]);
			}
			if (chatComponent.enableOutline && chatComponent.useDistanceCheck) {
				///NPCOutline
				SerializedProperty NPCOutline = serializedObject.FindProperty ("NPCOutline");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (NPCOutline, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("NPC Outline Material Reference", editorSkin.customStyles [4]);
				///inRangeColor
				SerializedProperty inRangeColor = serializedObject.FindProperty ("inRangeColor");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (inRangeColor, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("NPC Outline color when player is able to talk", editorSkin.customStyles [4]);
				///outOfRangeColor
				SerializedProperty outOfRangeColor = serializedObject.FindProperty ("outOfRangeColor");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (outOfRangeColor, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("NPC Outline color when player is out of range", editorSkin.customStyles [4]);
				///dialogueColor
				SerializedProperty dialogueColor = serializedObject.FindProperty ("dialogueColor");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (dialogueColor, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("NPC Outline color when player is engaged in dialogue with this NPC", editorSkin.customStyles [4]);
				///mouseOverColor
				SerializedProperty mouseOverColor = serializedObject.FindProperty ("mouseOverColor");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (mouseOverColor, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("NPC Outline color when player able to talk an mouse is over collider", editorSkin.customStyles [4]);
				
				///inRangeSize
				SerializedProperty inRangeSize = serializedObject.FindProperty ("inRangeSize");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (inRangeSize, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("NPC Outline size when player is able to talk", editorSkin.customStyles [4]);				
				///outOfRangeSize
				SerializedProperty outOfRangeSize = serializedObject.FindProperty ("outOfRangeSize");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (outOfRangeSize, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("NPC Outline size when player is out of range", editorSkin.customStyles [4]);
				///mouseOverSize
				SerializedProperty mouseOverSize = serializedObject.FindProperty ("mouseOverSize");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (mouseOverSize, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("NPC Outline size when player is able to talk an mouse is over collider", editorSkin.customStyles [4]);
				///dialogueSize
				SerializedProperty dialogueSize = serializedObject.FindProperty ("dialogueSize");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (dialogueSize, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("NPC Outline size when player is engaged in dialogue with this NPC", editorSkin.customStyles [4]);
				///Mesh Renderer Objects
				SerializedProperty meshRend = serializedObject.FindProperty ("meshRend");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (meshRend, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("Mesh Renderer Objects that NPC Outline will be applied to", editorSkin.customStyles [4]);
				///Skinned Mesh Renderer Objects
				SerializedProperty skinnedMeshRend = serializedObject.FindProperty ("skinnedMeshRend");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (skinnedMeshRend, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("Skinned Mesh Renderer Objects that NPC Outline will be applied to", editorSkin.customStyles [4]);
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();




			EditorGUILayout.Space ();
			EditorGUILayout.LabelField ("References", EditorStyles.miniBoldLabel);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (8);
			EditorGUILayout.BeginVertical ("box");
			///Chat Manager
			SerializedProperty chatManager = serializedObject.FindProperty ("chatManager");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (chatManager, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if (showHelp)
				EditorGUILayout.LabelField ("Scriptable Object that manages the current conversation index of NPC Chat", editorSkin.customStyles [4]);
			///Player Transform
			SerializedProperty player = serializedObject.FindProperty ("player");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (player, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if (showHelp)
				EditorGUILayout.LabelField ("Transform used for distance and collision checking, Tag must be 'Player'", editorSkin.customStyles [4]);
			///Auto Find Player
			SerializedProperty autoFindPlayer = serializedObject.FindProperty ("autoFindPlayer");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (autoFindPlayer, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			if (showHelp)
				EditorGUILayout.LabelField ("Search the scene for the named player Transform until it is found", editorSkin.customStyles [4]);
			if (chatComponent.autoFindPlayer == NPCChat.AutoFindPlayer.byName) {
				///Player's Name to Find
				SerializedProperty playerName = serializedObject.FindProperty ("playerName");
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.PropertyField (playerName, true);
				if (EditorGUI.EndChangeCheck ())
					serializedObject.ApplyModifiedProperties ();
				if (showHelp)
					EditorGUILayout.LabelField ("Name of the Transform to be searched and assigned as Player", editorSkin.customStyles [4]);
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();






			EditorGUILayout.EndVertical ();

		}
	}

}