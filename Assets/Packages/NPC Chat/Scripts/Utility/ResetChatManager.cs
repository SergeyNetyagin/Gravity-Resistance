using UnityEngine;
using System.Collections;
using TurnTheGameOn.NPCChat;

public class ResetChatManager : MonoBehaviour {

	void OnEnable(){
		ChatManager chatManager = Resources.Load ("ChatManager") as ChatManager;
		for(int i = 0; i < chatManager.currentDialogue.Length; i++){
			chatManager.currentDialogue [i] = 0;
		}
	}

}