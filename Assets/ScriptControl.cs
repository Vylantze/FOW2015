using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScriptControl : MonoBehaviour {
	public TextAsset script_holder;
	public TextAsset first;
	public TextAsset second;
	public TextAsset first_name;
	public TextAsset second_name;
	string[] script = { "" };
	public string[] password = {"emily rose", "olly olly oxen free"};
	public string[] status_messages = {
		"[Clearance has been approved. Priority Level 1 files are now accessible]\n",
		"[<OLLY OLLY OXEN FREE> TRIGGERED]\n"
	};
	public int index = 0;
	private int[] list_length = {1,1};
	public string[][] puzzle = new string[2][];
	public string[][] names = new string[2][];
	public int section = 0; // section index, to indicate which mail or password
	public Text final_num;
	public AutoType auto;
	System.Random generator = new System.Random();
	float time = 0f;

	// interface screws
	public AudioSource[] bgm;
	public AudioSource final;
	public AudioSource scare;9
	public AudioClip walking;
	public AudioClip laughter01;
	public AudioClip laughter02;
	public Animator _static;
	public Animator _ghosts;
	public Font font;
	public SpriteRenderer splatter;

	// Use this for initialization
	void Start () {
		script [0] = System.Environment.NewLine + System.Environment.NewLine;
		puzzle [0] = first.text.Split (script,System.StringSplitOptions.RemoveEmptyEntries);
		puzzle[1] = second.text.Split(script,System.StringSplitOptions.RemoveEmptyEntries); // 0 is received, 1 is sent
		script [0] = System.Environment.NewLine;
		names[0] = first_name.text.Split (script,System.StringSplitOptions.RemoveEmptyEntries);
		names[1] = second_name.text.Split(script,System.StringSplitOptions.RemoveEmptyEntries);
		list_length [0] = names [0].Length;
		list_length [1] = names [1].Length;

		script = script_holder.text.Split (script,System.StringSplitOptions.RemoveEmptyEntries);
		bgm [0].Play ();
	}

	public string getNextLine() {
		// music related
		if (index < script.Length) {
			return script[index++] + "\n"; 
		}
		return "";//else return empty
	}

	public string open(string filename) {
		for (int i=0; i<names[section].Length; i++) {
			if (names[section][i]==filename) {
				string temp = "\n" + puzzle[section][i] + "\n\n";/*
				if (auto.location[i]!=-1) {
					auto.text.text = auto.text.text.Remove(auto.location[i], temp.Length);
				}
				
				auto.location[i] = auto.text.text.Length;//*/
				return temp;
			}
		}
		return "\"" + filename + "\" cannot be found\n";
	}

	public string getScript(int num) {
		return script[num];
	}

	public string getPuzzle(int num) {
		return puzzle[section] [num];
	}

	public string getPassword() {
		return password [section];
	}

	public void changeMusic() {
		fadeOut (bgm[section]);
		fadeIn (bgm[section+1]);
	}

	void fadeOut(AudioSource audio){
		if (audio!=null&&audio.isPlaying) { 
			audio.Stop ();
		}
	}

	void fadeIn(AudioSource audio) {
		if (audio != null) {
			audio.Play ();
		}
	}

	public void sectionChange() {
		if (section == 0) {
			StartCoroutine(activateEffect());
		}
		changeMusic ();
		auto.text.text += status_messages [section++];

		if (section == 2) {
			splatter.enabled = true;
			finalTurn (); // activate last thingy
		} else {
			time = Time.time + 2f;
		}
	}

	public void finalTurn() {
		time = Time.time + 4f;
	}

	IEnumerator activateEffect() {
		int type = 0;
		float wait;
		while (true) {
			switch(type) {
			case 0: triggerStatic (); break;
			case 1: triggerGhostLeft(); break;
			case 2: triggerGhostRight(); break;
			case 3: triggerJump(); break;
			case 4: triggerWalk(); break;
			case 5: triggerLaugh(1); break;
			case 6: triggerLaugh(2); break;
			default: break;
			}
			type = generator.Next(6);
			if (section==2) {
				wait = 5f;
			}
			else {
				wait = 30f+generator.Next(30);
			}
			yield return 0;
			yield return new WaitForSeconds (wait);
		}
	}

	void triggerStatic() {
		_static.SetTrigger ("activate");
	}

	void triggerJump() {
		_static.SetTrigger ("jump");
	}

	void triggerGhostLeft() {
		_ghosts.SetTrigger ("left");
	}

	void triggerGhostRight() {
		_ghosts.SetTrigger ("right");
	}

	void triggerWalk() {
		if (!scare.isPlaying) {
			scare.Stop();
			scare.clip = walking;
			triggerScare();
		}
	}

	void triggerLaugh(int num) {
		if (!scare.isPlaying) {
			scare.Stop();
			if (num==1) {
				scare.clip = laughter01;
			}
			else {
				scare.clip = laughter02;
			}
			triggerScare();
		}
	}

	void triggerScare() {
		scare.Play ();
	}

	public int listLength() {
		if (section < list_length.Length) {
			return list_length [section];
		} else {
		}
		return 0;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time > time && section == 1) {
			time = float.MaxValue;
			auto.message = script[1] + "\n";
		}

		if (Time.time>time&&section==2) {
			if (!final.isPlaying) {
				time = Time.time+4f;
				final.Play ();
				triggerStatic();
				StopCoroutine(activateEffect());
			}
			else { // final is playing
				time = float.MaxValue;
				auto.message = script[2];
				auto.letterPause = 0.03f;
				auto.text.text = "";
				auto.text.font = font;
				auto.text.fontSize = 30;
				final_num.text = "79";
				StartCoroutine(activateEffect());
			}
		}
	}
}
