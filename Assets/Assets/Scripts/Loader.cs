using UnityEngine;
using System.Collections;

namespace Completed
{	
	public class Loader : MonoBehaviour 
	{
		public GameObject gameManager;
		public GameObject soundManager;
        public GameObject boardManager;
		GameObject curBoard = null;
		GameObject previousBoard = null;

        public Vector2 lastBoardPos = new Vector2(0, 0);
		
		
		void Awake ()
		{
			if (GameManager.instance == null)
				Instantiate(gameManager);
			
			if (SoundManager.instance == null)
				Instantiate(soundManager);

            if (BoardManager.instance == null)
                Instantiate(boardManager);
		}

		void Update()
		{
			GameObject player = GameObject.Find ("Player");
			int offsetX = (int)(player.transform.position.x) / (13 + 1);
			int offsetY = 0;
			if(player.transform.position.y < 0)
				offsetY = ((int)(player.transform.position.y) / (8 + 1)) - 1;
            //Debug.Log(offsetX + " - " + offsetY);

            gameObject.transform.position = new Vector3((int)(6 + (offsetX * 14)), (int)(3 + (offsetY * 8)), -10);

            if (lastBoardPos.x != offsetX || lastBoardPos.y != offsetY)
            {
                if (lastBoardPos.x > offsetX)// Esquerda
                {
                    player.transform.position = new Vector3(player.transform.position.x - 1, player.transform.position.y, player.transform.position.z);
                }
                else if (lastBoardPos.x < offsetX)// Direita
                {
                    //player.transform.position = new Vector3(player.transform.position.x + 1, player.transform.position.y, player.transform.position.z);
                }

                if (lastBoardPos.y > offsetY)// Baixo
                {
                    player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 1, player.transform.position.z);
                }
                else if (lastBoardPos.y < offsetY)// Cima
                {
                    //player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
                }
                lastBoardPos = new Vector2(offsetX, offsetY);
            }

		}
	}
}