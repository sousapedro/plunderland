using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
		public int playerLightPoints = 500;
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.	
		
		private Text levelText;									//Text to display current level number.
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.

		public int level = 1;									//Current level number, expressed in game as "Day 1".
		private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.
		
		
		//Awake is always called before any Start functions
		void Awake()
		{
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);	
			
			DontDestroyOnLoad(gameObject);
									
			//Call the InitGame function to initialize the first level 
			InitGame();
		}

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance.level++;
            instance.InitGame();
        }

		
		//Initializes the game for each level.
		void InitGame()
		{
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;
			
			levelImage = GameObject.Find("LevelImage");
			levelText = GameObject.Find("LevelText").GetComponent<Text>();
		    levelText.text = "Level " + level;
			levelImage.SetActive(true);
			Invoke("HideLevelImage", levelStartDelay);		
		}
		
		
		//Hides black image used between levels
		void HideLevelImage()
		{
			levelImage.SetActive(false);
			doingSetup = false;
		}
		
		//Update is called every frame.
		void Update()
		{
		}
			
		
		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{
			levelText.text = "After " + level + " days, you starved.";
			levelImage.SetActive(true);
			enabled = false;
		}
	}
}

