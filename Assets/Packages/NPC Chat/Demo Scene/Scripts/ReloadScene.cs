using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class ReloadScene : MonoBehaviour {

	public TurnTheGameOn.NPCChat.ChatManager chatManager;

	public void ResetLevel(){
		chatManager.currentDialogue [1] = 0;
		#if !UNITY_5_3_OR_NEWER
		Application.LoadLevel (Application.loadedLevel);
		#endif
		#if UNITY_5_3_OR_NEWER
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		#endif
	}

}