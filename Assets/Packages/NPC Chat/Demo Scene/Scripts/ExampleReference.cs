using UnityEngine;
using System.Collections;

public class ExampleReference : MonoBehaviour {

	public TurnTheGameOn.NPCChat.ChatManager chatManager;

	public void ChangeTarget(int targetNPC){
		chatManager.ChangeTarget (targetNPC);
	}

	public void ChangeConversation(int newConversation){
		chatManager.NewDialogue (newConversation);
	}

}
