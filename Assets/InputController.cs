using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class InputController : MonoBehaviour {
	public InputField input;
	public AutoType log;
	public EventSystem manager;
	public ScrollRect scroller;
	public bool in_progress = false;
	// Use this for initialization
	void Start () {
		input = GetComponent<InputField> ();
		input.onValueChange.AddListener (activate);
	}

	void Update() {
	}

	public void reactivate() {
		EventSystem.current.SetSelectedGameObject (input.gameObject);
	}
	
	public void activate(string arg0) {
		if (Input.GetKeyDown (KeyCode.Return)&&!in_progress) {
			in_progress = true;
			input.text = "";
			if (arg0.Length >= 2) {
				log.text.text += log.message;
				log.message = arg0;
				log.autoMode = false;
			}
			in_progress = false;
		}
	}
}