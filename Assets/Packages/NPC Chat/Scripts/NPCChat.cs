using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace TurnTheGameOn.NPCChat{

	[System.Serializable]
	public class NPCDialogue{
		[Range(1,1000)]
		public int pagesOfChat = 1;
		public bool useNextDialogue;
		public int nextDialogue;
		public bool[] enableButtonAfterDialogue;
		public GameObject[] chatBoxes;
		[TextArea(3,5)] public string[] chatPages;
		public string[] NPCName;
		public NPCDialogueButtons[] NPCButtons;
		public AudioClip[] pageAudio;
		public bool[] loopAudio;
		public UnityEvent[] OnPageStartEvent;
		public UnityEvent[] OnPageEndEvent;
	  //  public GameObject[] setActiveAfter;
	  //  public GameObject[] disableAfter;
	  //  public GameObject[] destroyAfter;
	}

	[System.Serializable]
	public class NPCDialogueButtons{
		public enum ItemType { none, enableButton }
		public ItemType[] buttonComponent;
		public Button.ButtonClickedEvent[] NPCClick;
		public string[] buttonString;
	}

	[ExecuteInEditMode]
	public class NPCChat : MonoBehaviour {
		public enum AutoFindPlayer { off, byName }
		
		[Tooltip("A reference to the chat manager scriptable object. NPC Chat checks the index of the array 'Current Dialogue' for its 'NPC Number' for structured dialogue.")]
		public ChatManager chatManager;
		[Tooltip("A reference to the player object, used for distance check. NOTE: This object should also have a tag set to Player.")]
		public Transform player;
		[Tooltip("Used by the chat manager scriptable object to set the curent conversation for the NPC Chat object.")]
		public int nPCNumber;
		[Tooltip("Enable / Disable triggering chat on player collision.")]
		public bool chatOnCollision;
		[Tooltip("Enable / Disable triggering chat on mouse-up when hovering over NPC Chat collider.")]
		public bool chatOnMouseUp;
		[Tooltip("Set a KeyCode to be used as a chat button")]
		public KeyCode chatOnKeyUp;
		[Tooltip("Set a KeyCode to be used as a chat button")]
		public KeyCode chatOnKeyDown;
		[Tooltip("The radius around NPC Chat used to determine if the player is close enough to trigger chat, used mainly to prevent mouse clicks from triggering chat. NPC Chat will close if the player leaves this radius.")]
		[Range (0.01f,100)] public float distanceToChat = 5;
		[Tooltip("Lower this setting to display text faster.")]
		[Range (0.0001f,20)] public float textScrollSpeed = 10;
		[Tooltip("Set the total number of conversations this NPC will have. Use the chat manager 'Current Dialogue' array with the NPC Number as an index to set a new conversation.")]
		public int conversations;
		public NPCDialogue[] _NPCDialogue;
		public bool talking;
		private bool textIsScrolling;
		private int currentPage;
		private bool canChat;
		private bool buttonPage;
		//private Material instanceMat;
		private Text chatText;
		private Text speakerNameText;
		private GameObject tempClip;
	    private GameObject tempSetActiveObject;
	    private GameObject tempDisableObject;
	    private GameObject tempDestroyObject;
		public bool startConversation;
		public Behaviour[] disableOnChat;
		public UnityEvent OnChatEvent;
		public UnityEvent OnStopChatEvent;
		public int tempInt;
		public int tempConv;
		public bool canUpdatePages;
		public AutoFindPlayer autoFindPlayer;
		public string playerName;
		public Vector3 distanceCheckOffset;
		Vector3 offsetValue;
		public Color gizmoColor;
		public bool useDistanceCheck;
		public bool closeOnTriggerExit;
		public bool closeOnMouseOrKeyUp;
		public bool scrollPageText;
		public bool enableOutline;
		public Material NPCOutline;

		public Color inRangeColor;
		public Color outOfRangeColor;
		public Color mouseOverColor;

		public float inRangeSize;
		public float outOfRangeSize;
		public float mouseOverSize;
		public float dialogueSize;

		public Color dialogueColor;
		public MeshRenderer[] meshRend;
		public SkinnedMeshRenderer[] skinnedMeshRend;

		bool mouseOver;
		bool keyDown;

	    public void Awake(){
			if(enableOutline && Application.isPlaying){
				NPCOutline = new Material(NPCOutline);
				for(int i = 0; i < meshRend.Length; i++){
					Material[] matArray = new Material[meshRend [i].sharedMaterials.Length + 1];
					for(int ii = 0; ii < matArray.Length - 1; ii++){
						matArray [ii] = meshRend [i].sharedMaterials[ii];
					}
					matArray [matArray.Length - 1] = NPCOutline;
					meshRend [i].materials = matArray;
				}
				for(int i = 0; i < skinnedMeshRend.Length; i++){
					Material[] matArray = new Material[skinnedMeshRend [i].sharedMaterials.Length + 1];
					for(int ii = 0; ii < matArray.Length - 1; ii++){
						matArray [ii] = skinnedMeshRend [i].sharedMaterials[ii];
					}
					matArray [matArray.Length - 1] = NPCOutline;
					skinnedMeshRend [i].materials = matArray;
				}
				NPCOutline.SetColor ("_OutlineColor", outOfRangeColor);
				NPCOutline.SetFloat ("_OutlineSize", outOfRangeSize);
			}

			canChat = true;

			if (Application.isPlaying){
				for (int i = 0; i < _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes.Length; i++) {
					if(_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[i] == null){
						int temp = i + 1;	Debug.Log("NPC Chat: You need to assign a Chat Box for page " + temp.ToString() );
					}
					_NPCDialogue[ chatManager.currentDialogue[ nPCNumber ] ].chatBoxes[i].SetActive(false);
				}
			}
		}

		public void Start(){
			conversations = _NPCDialogue.Length;
		}
		
		void OnDrawGizmosSelected() {
			if (useDistanceCheck) {
				Gizmos.color = gizmoColor;
				Gizmos.DrawSphere (offsetValue, distanceToChat);
			}
		}
		
		public void CalculateArrays(){
			
			for(int i = 0; i < _NPCDialogue.Length; i++){
				if(_NPCDialogue[i] != null){
					if(canUpdatePages && tempConv == i){
						_NPCDialogue [i].pagesOfChat = tempInt;
						canUpdatePages = false;
					}System.Array.Resize (ref _NPCDialogue[i].enableButtonAfterDialogue, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].chatBoxes, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].chatPages, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].NPCName, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].NPCButtons, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].pageAudio, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].loopAudio, _NPCDialogue[i].pagesOfChat);

					System.Array.Resize (ref _NPCDialogue[i].OnPageStartEvent, _NPCDialogue[i].pagesOfChat);
					System.Array.Resize (ref _NPCDialogue[i].OnPageEndEvent, _NPCDialogue[i].pagesOfChat);
	     //           System.Array.Resize(ref _NPCDialogue[i].setActiveAfter, _NPCDialogue[i].pagesOfChat);
	     //           System.Array.Resize(ref _NPCDialogue[i].disableAfter, _NPCDialogue[i].pagesOfChat);
	     //           System.Array.Resize(ref _NPCDialogue[i].destroyAfter, _NPCDialogue[i].pagesOfChat);
	                for (int ia = 0; ia < _NPCDialogue[i].pagesOfChat; ia++){
						if(_NPCDialogue[i].NPCButtons[ia] != null){
							System.Array.Resize (ref _NPCDialogue[i].NPCButtons[ia].buttonComponent, 6);
							System.Array.Resize (ref _NPCDialogue[i].NPCButtons[ia].NPCClick, 6);
							System.Array.Resize (ref _NPCDialogue[i].NPCButtons[ia].buttonString, 6);
						}
					}
				}
			}
		}

		public void UpdateConversations(){
			if (conversations == 0)
				conversations = 1;
			System.Array.Resize (ref _NPCDialogue, conversations);				CalculateArrays();
		}

		void Update(){		
			if(player == null){
				if (autoFindPlayer == AutoFindPlayer.byName) {
					GameObject playerObj = GameObject.Find (playerName);
					if (playerObj != null) {
						player = playerObj.transform;
					}
				}
			}
			if(Input.GetKeyUp(chatOnKeyUp) || Input.GetKeyDown(chatOnKeyDown)){
				OnChatKeyUp ();
			}
			if (Input.GetKeyDown (chatOnKeyDown)) {
				OnChatKeyDown ();
				keyDown = true;
			} else {
				keyDown = false;
			}
			if(startConversation){
				StartConversation ();
			}
			if (textIsScrolling) {
				if(chatText.text == _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatPages[currentPage]){
					textIsScrolling = false;
					if (buttonPage && _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].enableButtonAfterDialogue[currentPage]   ) {
						for(int i = 0; i < (_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].NPCButtons[currentPage].buttonComponent.Length); i++){
							if(_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].NPCButtons[currentPage].buttonComponent[i] == NPCDialogueButtons.ItemType.enableButton){
								_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[currentPage].GetComponent<ChatBox>().chatBoxComponents.buttonComponents[i].buttons.gameObject.SetActive(true);
								Button tempButton = _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[currentPage].GetComponent<ChatBox>().chatBoxComponents.buttonComponents[i].buttons;
								tempButton.onClick = _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].NPCButtons[currentPage].NPCClick[i];
								Text tempButtonText = tempButton.GetComponentInChildren<Text>();
								tempButtonText.text = _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].NPCButtons[currentPage].buttonString[i];
								tempButton = null;
								tempButtonText = null;
							}
						}
						_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].OnPageEndEvent[currentPage].Invoke();
					}
						
				}
			}
			if(player){
				if (useDistanceCheck) {
					offsetValue = new Vector3 (transform.position.x - distanceCheckOffset.x, transform.position.y - distanceCheckOffset.y, transform.position.z - distanceCheckOffset.z);
					var dist = Vector3.Distance (player.position, offsetValue);
					if (dist <= distanceToChat) {
						canChat = true;
						if (enableOutline) {
							if (talking) {
								NPCOutline.SetColor ("_OutlineColor", dialogueColor);
								NPCOutline.SetFloat ("_OutlineSize", dialogueSize);
							} else {
								if (!mouseOver) {
									NPCOutline.SetColor ("_OutlineColor", inRangeColor);
									NPCOutline.SetFloat ("_OutlineSize", inRangeSize);
								} else {
									NPCOutline.SetColor ("_OutlineColor", mouseOverColor);
									NPCOutline.SetFloat ("_OutlineSize", mouseOverSize);
								}
							}
						}
					} else {
						canChat = false;
					}
				}
				if (canChat == false) {
					if (enableOutline) {
						NPCOutline.SetColor ("_OutlineColor", outOfRangeColor);
						NPCOutline.SetFloat ("_OutlineSize", outOfRangeSize);
					}
					if(talking){
						if(tempClip)
							Destroy (tempClip);
						chatText.text = "";
						talking = false;
						textIsScrolling = false;
						currentPage = 0;
						for(int i =0; i < disableOnChat.Length; i++){
							disableOnChat[i].enabled = true;
						}
						OnStopChatEvent.Invoke ();
						for(var i = 0; i < _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes.Length; i++){
							_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[i].SetActive(false);
						}
					}
				}
			}
			if( (talking && Input.GetMouseButtonUp(0)) || (talking && Input.GetKeyUp(chatOnKeyUp)) || (talking && Input.GetKeyUp(chatOnKeyDown)) ){
				if (closeOnMouseOrKeyUp) {
					if (textIsScrolling) {
						textIsScrolling = false;
						chatText.text = _NPCDialogue [chatManager.currentDialogue [nPCNumber]].chatPages [currentPage];
						if (buttonPage && _NPCDialogue [chatManager.currentDialogue [nPCNumber]].enableButtonAfterDialogue [currentPage]) {
							for (int i = 0; i < (_NPCDialogue [chatManager.currentDialogue [nPCNumber]].NPCButtons [currentPage].buttonComponent.Length); i++) {
								if (_NPCDialogue [chatManager.currentDialogue [nPCNumber]].NPCButtons [currentPage].buttonComponent [i] == NPCDialogueButtons.ItemType.enableButton) {
									_NPCDialogue [chatManager.currentDialogue [nPCNumber]].chatBoxes [currentPage].GetComponent<ChatBox> ().chatBoxComponents.buttonComponents [i].buttons.gameObject.SetActive (true);
									Button tempButton = _NPCDialogue [chatManager.currentDialogue [nPCNumber]].chatBoxes [currentPage].GetComponent<ChatBox> ().chatBoxComponents.buttonComponents [i].buttons;
									tempButton.onClick = _NPCDialogue [chatManager.currentDialogue [nPCNumber]].NPCButtons [currentPage].NPCClick [i];
									Text tempButtonText = tempButton.GetComponentInChildren<Text> ();
									tempButtonText.text = _NPCDialogue [chatManager.currentDialogue [nPCNumber]].NPCButtons [currentPage].buttonString [i];
									tempButton = null;
									tempButtonText = null;
								}
							}
							_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].OnPageEndEvent[currentPage].Invoke();
						}
					} else {
						if (currentPage < _NPCDialogue [chatManager.currentDialogue [nPCNumber]].chatPages.Length - 1) {
						//	if (_NPCDialogue [chatManager.currentDialogue [nPCNumber]].setActiveAfter [currentPage])
						//		_NPCDialogue [chatManager.currentDialogue [nPCNumber]].setActiveAfter [currentPage].SetActive (true);
						//	if (_NPCDialogue [chatManager.currentDialogue [nPCNumber]].disableAfter [currentPage])
						//		_NPCDialogue [chatManager.currentDialogue [nPCNumber]].disableAfter [currentPage].SetActive (false);
						//	if (_NPCDialogue [chatManager.currentDialogue [nPCNumber]].destroyAfter [currentPage])
						//		Destroy (_NPCDialogue [chatManager.currentDialogue [nPCNumber]].destroyAfter [currentPage]);

							currentPage++;
							NPCChatUpdate ();
							StartCoroutine (StartScrolling ());
						} else {
							if (buttonPage) {
								
							} else {
							//	if (_NPCDialogue [chatManager.currentDialogue [nPCNumber]].setActiveAfter [currentPage])
							//		_NPCDialogue [chatManager.currentDialogue [nPCNumber]].setActiveAfter [currentPage].SetActive (true);
							//	if (_NPCDialogue [chatManager.currentDialogue [nPCNumber]].disableAfter [currentPage])
							//		_NPCDialogue [chatManager.currentDialogue [nPCNumber]].disableAfter [currentPage].SetActive (false);
							//	if (_NPCDialogue [chatManager.currentDialogue [nPCNumber]].destroyAfter [currentPage])
							//		Destroy (_NPCDialogue [chatManager.currentDialogue [nPCNumber]].destroyAfter [currentPage]);

								CloseChat ();
							}
						}
					}
				}
			}
		}
		
		public void CloseChat(){
			for(int i =0; i < disableOnChat.Length; i++){
				disableOnChat[i].enabled = true;
			}
			OnStopChatEvent.Invoke ();
			if(tempClip)
				Destroy (tempClip);

	 //       if (_NPCDialogue[chatManager.currentDialogue[nPCNumber]].setActiveAfter[currentPage])
	 //           _NPCDialogue[chatManager.currentDialogue[nPCNumber]].setActiveAfter[currentPage].SetActive(true);
	 //       if (_NPCDialogue[chatManager.currentDialogue[nPCNumber]].disableAfter[currentPage])
	 //           _NPCDialogue[chatManager.currentDialogue[nPCNumber]].disableAfter[currentPage].SetActive(false);
	 //       if (_NPCDialogue[chatManager.currentDialogue[nPCNumber]].destroyAfter[currentPage])
	 //           Destroy(_NPCDialogue[chatManager.currentDialogue[nPCNumber]].destroyAfter[currentPage]);

	        currentPage = 0;
			chatText.text = _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatPages[currentPage];
	//		Invoke ("NotTalking", 0.1f);
			talking = false;
			for(var i = 0; i < _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes.Length; i++){
				_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[i].SetActive(false);
				_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[i].GetComponent<ChatBox>().chatBoxComponents.renderTextureCamera.gameObject.SetActive(false);
			}
			for(int ia = 0; ia < (_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].NPCButtons[currentPage].buttonComponent.Length); ia++){
				_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[currentPage].GetComponent<ChatBox>().chatBoxComponents.buttonComponents[ia].buttons.gameObject.SetActive(false);
			}
			if(_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].useNextDialogue && !buttonPage)			 chatManager.currentDialogue[nPCNumber] = _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].nextDialogue;
		}
		
		void NPCChatUpdate(){
			buttonPage = false;
			//set the current "chat box speaker name - Text component" to the assigned local value
			speakerNameText = _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[currentPage].GetComponent<ChatBox>().chatBoxComponents.headerText;
			//set the text for the "chat box speaker name - Text component" to the assigned local value
			speakerNameText.text = _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].NPCName[currentPage];

			_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].OnPageStartEvent[currentPage].Invoke();

			if (tempClip != null) {
				Destroy(tempClip);
			}
			if(_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].pageAudio[currentPage] != null){
				tempClip = new GameObject ();
				tempClip.transform.parent = transform;
				tempClip.AddComponent<AudioSource> ();
				tempClip.name = "NPC Page Audio";
				if(_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].loopAudio[currentPage])
					tempClip.GetComponent<AudioSource>().loop = true;
				tempClip.AddComponent<AudioDestroy>();
				tempClip.GetComponent<AudioSource>().clip = _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].pageAudio[currentPage];
				tempClip.GetComponent<AudioSource> ().Play ();
			}

			if (currentPage > 0) {
				_NPCDialogue [ chatManager.currentDialogue[nPCNumber] ].chatBoxes [currentPage - 1].SetActive (false);
				_NPCDialogue [ chatManager.currentDialogue[nPCNumber] ].chatBoxes [currentPage - 1].GetComponent<ChatBox> ().chatBoxComponents.renderTextureCamera.gameObject.SetActive (false);
			}
			for(int ia = 0; ia < (_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].NPCButtons[currentPage].buttonComponent.Length); ia++){
				_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[currentPage].GetComponent<ChatBox>().chatBoxComponents.buttonComponents[ia].buttons.gameObject.SetActive(false);
			}
			_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[currentPage].GetComponent<ChatBox>().chatBoxComponents.text.text = "";
			chatText = _NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[currentPage].GetComponent<ChatBox>().chatBoxComponents.text;
			_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[currentPage].SetActive(true);
			_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].chatBoxes[currentPage].GetComponent<ChatBox>().chatBoxComponents.renderTextureCamera.gameObject.SetActive(true);
			for(int i = 0; i < (_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].NPCButtons[currentPage].buttonComponent.Length); i++){
				if(_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].NPCButtons[currentPage].buttonComponent[i] == NPCDialogueButtons.ItemType.enableButton){
					buttonPage = true;
					if (!_NPCDialogue[ chatManager.currentDialogue[nPCNumber] ].enableButtonAfterDialogue[currentPage] ) {
						_NPCDialogue [chatManager.currentDialogue [nPCNumber]].chatBoxes [currentPage].GetComponent<ChatBox> ().chatBoxComponents.buttonComponents [i].buttons.gameObject.SetActive (true);
						Button tempButton = _NPCDialogue [chatManager.currentDialogue [nPCNumber]].chatBoxes [currentPage].GetComponent<ChatBox> ().chatBoxComponents.buttonComponents [i].buttons;
						tempButton.onClick = _NPCDialogue [chatManager.currentDialogue [nPCNumber]].NPCButtons [currentPage].NPCClick [i];
						Text tempButtonText = tempButton.GetComponentInChildren<Text> ();
						tempButtonText.text = _NPCDialogue [chatManager.currentDialogue [nPCNumber]].NPCButtons [currentPage].buttonString [i];
						tempButton = null;
						tempButtonText = null;
					}
				}
			}
		}

	//	public void NotTalking(){
	//		talking = false;
	//	}
			
		void OnMouseEnter(){
			if(!talking && canChat){
				mouseOver = true;
			}
		}

		void OnMouseExit(){
			mouseOver = false;
		}

		void OnTriggerEnter(Collider col) {
			if(chatOnCollision && !talking && canChat){
				if(col.tag == "Player" ){
					StartConversation ();
				}
			}
		}

		void OnTriggerExit(Collider col) {
			if(closeOnTriggerExit && col.tag == "Player"){
				CloseChat ();
			}
		}

		void OnChatKeyUp(){
			if (canChat && talking == false) {
				StartConversation ();
			}
		}

		void OnChatKeyDown(){
			if (canChat && talking == false && keyDown == false) {
				StartConversation ();
			}
		}

		void OnMouseUp(){
			if (chatOnMouseUp) {
				if (canChat && talking == false) {
					Invoke("StartConversation", 0.1f);
				}
			}
		}

		public void StartConversation(){
			for(int i =0; i < disableOnChat.Length; i++){
				disableOnChat[i].enabled = false;
			}
			OnChatEvent.Invoke ();
			startConversation = false;
			talking = true;
			currentPage = 0;
			NPCChatUpdate();
			StartCoroutine (StartScrolling());
		}
		
		IEnumerator StartScrolling(){
			textIsScrolling = true;
			int startLine = currentPage;
			string displayText = "";
			if (scrollPageText) {
				for (int i = 0; i < _NPCDialogue [chatManager.currentDialogue [nPCNumber]].chatPages [currentPage].Length; i++) {
					if (talking && textIsScrolling && currentPage == startLine) {
						displayText += _NPCDialogue [chatManager.currentDialogue [nPCNumber]].chatPages [currentPage] [i];
						chatText.text = displayText;
						yield return new WaitForSeconds (textScrollSpeed / 100f);
					}
				}
			} else {
				displayText = _NPCDialogue [chatManager.currentDialogue [nPCNumber]].chatPages [currentPage];
				chatText.text = displayText;
			}
		}
		
	}
}