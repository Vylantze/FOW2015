using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class AutoType : MonoBehaviour {
	
	public float letterPause = 0.005f;
	private bool jump = false;

	public string message;
	public Text text;
	public EventSystem manager;
	public InputController input;
	public bool autoMode = true;
	public bool cursorVisible = false;
	public ScrollRect scroller;
	public CursorLockMode lockMode = CursorLockMode.Locked;
	public Vector2 max = new Vector2(0f,0f);

	// for the game itself
	public ScriptControl script;
	public bool unlocked = true; // true means password has been given. Clear to go
	public string player_name = "";
	public string help = "\nCommands:\n" +
		"\tlist - Lists all files accessible\n" +
			"\topen <filename> - Opens a file of that name\n" +
			"\t<password> - Unlocks the computer if it is the correct password\n\n";
	public string[] list = new string[2];
	private int list_length = 0;
	//public int[] location = {-1,-1,-1,-1,-1};

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		message = text.text;
		text.text = "";
		StartCoroutine(TypeText());
	}

	void Update() {
		Cursor.visible = cursorVisible;
		if (script.section == 2) {
			input.gameObject.SetActive(false);
		}

		//if (Input.anyKeyDown) {
		if ((Input.GetKey (KeyCode.RightAlt) || Input.GetKey (KeyCode.LeftAlt)) &&
			(Input.GetKey (KeyCode.RightControl) || Input.GetKey (KeyCode.LeftControl)) &&
			Input.GetKey (KeyCode.R)) {
			Application.LoadLevel (Application.loadedLevel);
		} else if (Input.anyKeyDown||Input.GetKey (KeyCode.Mouse0)) {
			manager.SetSelectedGameObject (input.gameObject);
		}

		if (jump) {
			jump = false;
			scroller.normalizedPosition = max;
		}

		if (script.section >= 2) { // once hit the third section, disable input controller
			if (input.gameObject.activeSelf) {
				input.gameObject.SetActive(false);
			}
		}

		if (list_length != script.listLength ()) {
			list_length = script.listLength();
			int section = script.section;
			list[section] = "\n";
			for (int i=0;i<list_length;i++) {
				list[section] += "\t" + script.names[section][i] + "\n";
			}
			list[section] += "\n";
		}
	}
	
	IEnumerator TypeText () {
		while(true) {
			//foreach (char letter in message.ToCharArray()) {

			if (message.Length>0&&autoMode) {
				text.text += message[0];
				message = message.Substring(1);
				jump = true;
			}
			else if (message.Length>0) { // if not auto mode
				// post the text regardless of what happens
				text.text += ">" + message;
				message = message.Substring(0,message.Length-1);
				jump = true;
				toDo ();

				autoMode = true;
			}
			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}      
	}


	void toDo() {
		if (player_name=="") { //if name is still zero, store it then get next line of the script
			player_name = message.Substring(0,1).ToUpper() + message.Substring(1,message.Length-1);
			script.index = 0;
			script.section = 0;
			message = script.getNextLine();
			message = message.Substring(0,message.Length-1) + player_name + "!\n";
			unlocked = false;
		}
		else if (message=="help") {
			text.text += help;
			message = "";
		}
		else if (message=="list") {
			text.text += list[script.section];
			message = "";
		}
		else if (message.Length>5&&message.Substring(0,5)=="open ") {
			string temp = script.open(message.Substring(5));
			text.text = text.text.Replace(temp,"");
			text.text += temp;
			message = "";
		}
		else {
			string temp = script.getPassword().Replace(" ","");
			if (!unlocked&&(script.getPassword()==message.ToLower()||
			                temp==message.ToLower())) { // if not unlocked
				unlocked = true;
				message = "";
				script.sectionChange();
				list_length = 0;
				unlocked = false;
			}
			else {
				text.text += "Invalid command\n";
				message = "";
			}
		}
	}
}