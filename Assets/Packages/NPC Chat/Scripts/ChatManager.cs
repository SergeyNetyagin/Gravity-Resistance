using UnityEngine;
using System.Collections;

namespace TurnTheGameOn.NPCChat{
	
	[System.Serializable]
	public class ChatManager : ScriptableObject {


		public int targetNPC;
		[TextArea(5,5)]	public string noteText;
		public int[] currentDialogue;
		[HideInInspector] public int numberOfSlots;

		void Update(){
			if (numberOfSlots != currentDialogue.Length) {
				numberOfSlots = currentDialogue.Length;
			}
		}

		public void ChangeTarget( int newNPC){
			targetNPC = newNPC;
		}
		
		public void NewDialogue( int newDialogue ){
			currentDialogue[targetNPC] = newDialogue;
		}
			

	}

}