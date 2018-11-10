using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
    public class Player : MonoBehaviour
	{
		public float restartLevelDelay = 0.1f;		//Delay time in seconds to restart level.
		public int pointsPerFood = 10;				//Number of points to add to player food points when picking up a food object.
		public int pointsPerSoda = 20;				//Number of points to add to player food points when picking up a soda object.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
		public Text foodText;						//UI Text to display current player food total.
		public Text auxText;
		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a food object.
		public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a food object.
		public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.
		
		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int food;                           //Used to store player food points total during level.
		private int torch;		
		
		//Start overrides the Start function of MovingObject
		void Start ()
		{
			//Get a component reference to the Player's animator component
			animator = GetComponent<Animator>();
			
			torch = GameManager.instance.playerLightPoints;
			
			foodText.text = "Food: " + food;
		}
				
		private void FixedUpdate ()
		{
            MovementUpdate();

            //if(horizontal != 0 || vertical != 0)
            //    SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);

            //Light light = GetComponentInChildren<Light> ();
            //float torchMax = GameManager.instance.playerLightPoints;
            //light.intensity = (7f + 1f * Mathf.Sin (Time.time))*(torch/torchMax);
            //light.transform.position = new Vector3 (light.transform.position.x, light.transform.position.y, (-4f +(-4f +(-0.2f * Mathf.Sin (Time.time)))*(torch/torchMax)));
		}

        private void MovementUpdate()
        {
            int horizontal = 0;  	//Used to store the horizontal move direction.
            int vertical = 0;		//Used to store the vertical move direction.

            horizontal = (int)(Input.GetAxisRaw("Horizontal"));
            vertical = (int)(Input.GetAxisRaw("Vertical"));

            //GetComponent<Rigidbody2D>().AddForce(new Vector2(horizontal, vertical) * 5f, ForceMode2D.Force);

            transform.position += ((new Vector3(horizontal, vertical, 0)) * 5f) * Time.deltaTime;

            //Vector2 speed = new Vector2(horizontal, vertical) * 5f;
            //GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + speed * Time.deltaTime);

            FlipSprite(horizontal);
        }

        private void FlipSprite(int horizontal)
        {
            if (horizontal < 0)
                GetComponent<SpriteRenderer>().flipX = true;
            else if (horizontal > 0)
                GetComponent<SpriteRenderer>().flipX = false;
        }

		private void OnTriggerEnter2D (Collider2D other)
		{
			if(other.tag == "Exit")
			{
                //Invoke ("Restart", restartLevelDelay);
                Restart();

				enabled = false;
			}
			
            //else if(other.tag == "Food")
            //{
            //    //Add pointsPerFood to the players current food total.
            //    food += pointsPerFood;
				
            //    //Update foodText to represent current total and notify player that they gained points
            //    foodText.text = "+" + pointsPerFood + " Food: " + food;
				
            //    //Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
            //    SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				
            //    //Disable the food object the player collided with.
            //    other.gameObject.SetActive (false);
            //}
		}
		
		
		//Restart reloads the scene when called.
		private void Restart ()
		{
            GameObject GO = GameObject.Find("BoardManager(Clone)");
            foreach (Transform child in GO.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		
		
		public void LoseFood (int loss)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
			animator.SetTrigger ("playerHit");
			
			//Subtract lost food points from the players total.
			food -= loss;
			
			//Update the food display with the new total.
			foodText.text = "-"+ loss + " Food: " + food;
			
			//Check to see if game has ended.
			CheckIfGameOver ();
		}
		
		private void CheckIfGameOver ()
		{
			//Check if food point total is less than or equal to zero.
			if (food <= 0) 
			{
				//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
				SoundManager.instance.PlaySingle (gameOverSound);
				
				//Stop the background music.
				SoundManager.instance.musicSource.Stop();
				
				//Call the GameOver function of GameManager.
				GameManager.instance.GameOver ();
			}
		}
	}
}

