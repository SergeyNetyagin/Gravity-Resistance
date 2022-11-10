using UnityEngine;
using System.Collections;

namespace TurnTheGaneOn{

	public class UI_SlideIn : MonoBehaviour {

		public enum SlideType{ FromBottom, FromTop, FromLeft, FromRight }
		public SlideType slideType;
		RectTransform rectObject;
		Vector2 anchoredPosition;
		Vector2 startingPosition;
		public float smooth;
		Vector2 tempPosition;
		float timer;
		public float activeFor = 0.5f;

		void Awake(){
			rectObject = GetComponent<RectTransform> ();
			startingPosition.x = rectObject.anchoredPosition.x;
			startingPosition.y = rectObject.anchoredPosition.y;
			anchoredPosition = rectObject.anchoredPosition;
		}

		// Use this for initialization
		void OnEnable () {		
			OpenWindow ();
		}

		public void OpenWindow(){
			timer = 0f;
			if (slideType == SlideType.FromBottom) {
				startingPosition.y = -Screen.height + (-startingPosition.y);

			}
			else if (slideType == SlideType.FromTop) {
				startingPosition.y = Screen.height + startingPosition.y;

			}
			else if (slideType == SlideType.FromLeft) {
				startingPosition.x = -Screen.width + (-startingPosition.x);

			}
			else if (slideType == SlideType.FromRight) {
				startingPosition.x = Screen.width + (startingPosition.x);
			}
			rectObject.anchoredPosition = startingPosition;
		}

		public void CloseWindow(){
			anchoredPosition.y = Screen.height * 1.25f;
		}

		// Update is called once per frame
		void Update () {
			if(timer < activeFor){
				timer += 1 * Time.deltaTime;
				//Debug.Log ("ACTIVE");
				if (slideType == SlideType.FromBottom || slideType == SlideType.FromTop) {
					startingPosition.y = Mathf.SmoothStep (startingPosition.y, anchoredPosition.y, smooth);
				}
				else if(slideType == SlideType.FromLeft || slideType == SlideType.FromRight){
					startingPosition.x = Mathf.SmoothStep (startingPosition.x, anchoredPosition.x, smooth);
				}
				rectObject.anchoredPosition = startingPosition;
			}
		}

	}
}