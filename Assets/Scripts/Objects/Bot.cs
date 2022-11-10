using UnityEngine;
using System.Collections;

public class Bot : MonoBehaviour {

	// Use this for initialization
	void Awake() {

        // Скрипт <Bot> начинает выполняться раньше, чем <Protection> (это уже установлено в настройках)
        // Поэтому нужно здесь, в Awake(), сразу же у кораблей-ботов отключить скрипты <Protection>, чтобы они не создавали поля 
        // В методе <Protection.Awake()> есть проверка: если компонент неактивен, то сразу происхоит возврат из Protection.Awake()
        // Всё это нужно, чтобы у ботов не было щитов - иначе придётся городить огород, чтобы их защита с ними не взаимодействовала, но они взаимодействовали со всем остальным

    }

	// Use this for initialization
	void Start() {
	
	}
	
	// Update is called once per frame
	void Update() {
	
	}
}
