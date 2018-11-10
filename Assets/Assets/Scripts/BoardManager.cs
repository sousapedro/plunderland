using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace Completed
{
	
	public class BoardManager : MonoBehaviour
	{
        public static BoardManager instance = null;

		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.
			
			
			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		
		
		public int roomColumsNum = 13; 										//Number of columns in our game board.
		public int roomRowsNum = 7;											//Number of rows in our game board.
		public Count wallCount = new Count (5, 9);						//Lower and upper limit for our random number of walls per level.
		public Count foodCount = new Count (1, 5);						//Lower and upper limit for our random number of food items per level.
		public GameObject exit;											//Prefab to spawn for exit.
		public GameObject[] floorTiles;									//Array of floor prefabs.
		public GameObject[] wallTiles;									//Array of wall prefabs.
		public GameObject[] foodTiles;									//Array of food prefabs.
		public GameObject[] enemyTiles;									//Array of enemy prefabs.
		public GameObject[] outerWallTiles;//Array of outer tile prefabs.

        static private int boardLinesNum = 4;
        static private int boardColsNum = 4;

        public BoardPiece[][] boardConfig = new BoardPiece[boardLinesNum][];
		private Vector2 endPos;
		private Vector2 startPos;

		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

		public GameObject tileChoice;

        void Awake()
        {            
            //Check if instance already exists
            if (instance == null)
                instance = this;
            //else if (instance != this)
            //    Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            SetupScene(GameManager.instance.level);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance.SetupScene(GameManager.instance.level);
        }

		//Clears our list gridPositions and prepares it to generate a new board.
		void InitialiseList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();
			
			//Loop through x axis (columns).
			for(int x = 1; x < roomColumsNum-1; x++)
			{
				//Within each column, loop through y axis (rows).
				for(int y = 1; y < roomRowsNum-1; y++)
				{
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
		
		//Sets up the outer walls and floors of each game room.
		void RoomFloorsAndWallsSetup (int indexLine, int indexColumn)
		{
			//Instantiate Board and set boardHolder to its transform.
			boardHolder = new GameObject ("Board" + indexLine + "-"+indexColumn).transform;
			GameObject myBoardGO = GameObject.Find ("Board"+indexLine+"-"+indexColumn);
            myBoardGO.transform.SetParent(this.gameObject.transform);

			//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
			for(int x = -1; x < (roomColumsNum + 1); x++)
			{
				//Loop along y axis, starting from -1 to place floor or outerwall tiles.
				for(int y = -1; y < (roomRowsNum + 1); y++)
				{
                    if (x == roomColumsNum && (indexColumn == (boardColsNum-1)))
                    {
						GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
						GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (boardHolder);
                    }
                    else if (y == -1 && (indexLine == (boardLinesNum - 1)))
                    {
						GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
						GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (boardHolder);
					} else if ((x < roomColumsNum) && (y >= 0)) {
						//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
						GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
                        Debug.Log("QQQ: " + indexLine + " - " + indexColumn);
						if (!boardConfig [indexLine] [indexColumn].left && x == -1)//(indexColumn*10 - 1))//left
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
                        else if (boardConfig[indexLine][indexColumn].left && (x == -1 && y != boardColsNum))
							toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];

						if (!boardConfig [indexLine] [indexColumn].top && y == roomRowsNum)//top
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
                        if (boardConfig[indexLine][indexColumn].top && (y == roomRowsNum && x != boardLinesNum))//top
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
						//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
						GameObject instance =
							Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

						//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
						instance.transform.SetParent (boardHolder);
					}
				}
			}
            //Debug.Log ("Board[" + indexLine + "][" + indexColumn + "]");
            //Debug.Log ("Left: " + boardConfig[indexLine ][indexColumn].left + " - " +
            //    "Top: " + boardConfig[indexLine ][indexColumn].top + " - " + 
            //    "Right: " + boardConfig[indexLine ][indexColumn].right + " - " + 
            //    "Bottom: " + boardConfig[indexLine][indexColumn].bottom);
			boardHolder.position = new Vector3 (indexColumn * (roomColumsNum+1), (-1)*indexLine * (roomRowsNum+1));

			//ENDTEST
		}
		
		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);
			
			//Return the randomly selected Vector3 position.
			return randomPosition;
		}
		
		//boardConfig
		void LayoutObjectsAtBoard (GameObject[] tileArray)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			//int objectCount = Random.Range (minimum, maximum+1);

			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int row = 0; row < boardLinesNum; row++)
				for(int column = 0; column < boardColsNum; column++)
				{
					//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
					//Vector3 randomPosition = RandomPosition();
					int roomType = Random.Range(0,7);

                    for (int j = (row * (-(roomRowsNum + 1))) + 6, obsRow = 0; j > (row * (-(roomRowsNum + 1))) - 1; j--, obsRow++)
                    {
                        for (int i = (column * (roomColumsNum + 1)), obsCol = 0; i < (((column + 1) * (roomColumsNum + 1)) - 1); i++, obsCol++)
                        {

							Vector3 positionToCreate = new Vector3 (i, j);
							string text = System.IO.File.ReadAllText("Assets\\Sprites\\LB\\"+roomType+".txt");
							//Random.Range(0,7)
							//Debug.Log (text [/*(obsRow*(columns-1)) + */obsCol]);

							//Choose a random tile from tileArray and assign it to tileChoice
							//GameObject tileChoice = Resources.Load("Assets\\Completed\\Prefabs\\Floor1") as GameObject;//tileArray[Random.Range (0, tileArray.Length)];

							//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
							int chance = Random.Range(0,10);
							if(((obsRow*(roomColumsNum)) + obsCol) < text.Length)
							if(text [(obsRow*(roomColumsNum)) + obsCol] == '1')//(chance < 1)
								Instantiate(tileChoice, positionToCreate, Quaternion.identity, this.gameObject.transform);
							
						}
					}
				}
		}

		void DefineRoomsAndObjects (GameObject[] tileArray)
		{
            for (int row = 0; row < boardLinesNum; row++)
                for (int column = 0; column < boardColsNum; column++)
				{
					if (boardConfig [row] [column].isStart)
						boardConfig [row] [column].roomType = ERoomType.StartBedroom;
					else if (boardConfig [row] [column].isEnd)
						boardConfig [row] [column].roomType = ERoomType.ParentsBedroom;
					else
						boardConfig[row][column].roomType = (ERoomType) Random.Range(1,(ERoomType.NumberOfTypes.GetHashCode()-1));

                    for (int j = (row * (-(roomRowsNum + 1))) + 6, obsRow = 0; j > (row * (-(roomRowsNum + 1))) - 1; j--, obsRow++)
                    {
                        for (int i = (column * (roomColumsNum + 1)), obsCol = 0; i < (((column + 1) * (roomColumsNum + 1)) - 1); i++, obsCol++)
                        {

							Vector3 positionToCreate = new Vector3 (i, j);

							PrepareRoom (positionToCreate, row, column, obsRow, obsCol);
						}
					}
				}
		}

		void PrepareRoom (Vector3 posToCreate, int row, int column, int obsRow, int obsCol)
		{
			//string text = System.IO.File.ReadAllText ("Assets\\Sprites\\LB\\" + boardConfig [row] [column].roomType.GetHashCode () + ".txt");
			//string text = System.IO.File.ReadAllText ("Assets\\Sprites\\LB\\" + boardConfig [row] [column].roomType.GetHashCode () + ".txt");

			string text = Resources.Load<TextAsset>(boardConfig [row] [column].roomType.GetHashCode ()+"").ToString();


			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
			int chance = Random.Range(0,10);
			if(((obsRow*(roomColumsNum)) + obsCol) < text.Length)
			if(text [(obsRow*(roomColumsNum)) + obsCol] == '1')//(chance < 1)
                Instantiate(tileChoice, posToCreate, Quaternion.identity, this.gameObject.transform);

			/*switch (type) {
			case ERoomType.Kitchen:
				int chance = Random.Range(0,10);
				if(((obsRow*(columns)) + obsCol) < text.Length)
				if(text [(obsRow*(columns)) + obsCol] == '1')
					Instantiate(tileChoice, positionToCreate, Quaternion.identity);
				break;
			default:
				break;
			}*/
		}

   		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				
				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                Instantiate(tileChoice, randomPosition, Quaternion.identity, this.gameObject.transform);
			}
		}
		
		public void SetupBoardPieces ()
		{
            //Instantiate the initial empty board with the number of lines and columns defined
            for (int i = 0; i < boardLinesNum; i++)
            {
                boardConfig[i] = new BoardPiece[boardColsNum];
                for (int j = 0; j < boardColsNum; j++)
                {
                    boardConfig[i][j] = new BoardPiece();
                }
            }			

            // Randomly selects the starting board
			int curCol = Random.Range (0, 3);
            // Always starts on the line 0
			int curLine = 0;
			boardConfig [curLine] [curCol].isStart = true;
			
            Debug.Log ("Starting Board: [" + curLine + "] - [" + curCol + "]");
            // Defines the board piece it starts
			startPos = new Vector2 (curLine, curCol);

            GeneratePathway(curLine, curCol);
		}

        private void GeneratePathway(int curLine, int curCol)
        {
            int next;
            boardConfig[curLine][curCol].mainBoard = true;
            boardConfig[curLine][curCol].visited = true;
            boardConfig[curLine][curCol].left = curCol > 0 ? true : false;
            boardConfig[curLine][curCol].right = curCol < 3 ? true : false;

            bool finish = false;
            while (!finish)
            {
                next = Random.Range(0, 3);
                if (next == 0)
                {// Left
                    if (curCol - 1 >= 0)
                    {// ainda tem lugar a esquerda
                        if (!boardConfig[curLine][curCol - 1].visited)
                        {// nao foi visitado
                            curCol -= 1;
                            boardConfig[curLine][curCol].mainBoard = true;
                            boardConfig[curLine][curCol].visited = true;
                            boardConfig[curLine][curCol].left = curCol > 0 ? true : false;
                            boardConfig[curLine][curCol].right = curCol < 3 ? true : false;
                            Debug.Log(curLine + " - " + curCol);
                        }
                    }
                    else if (curLine == (boardLinesNum - 1))
                    {
                        endPos = new Vector2(curLine, curCol);
                        boardConfig[curLine][curCol].isEnd = true;
                        finish = true;
                    }

                }
                else if (next == 1)
                {// Right
                    if (curCol + 1 <= (boardColsNum - 1))
                    {// ainda tem lugar a direita
                        if (!boardConfig[curLine][curCol + 1].visited)
                        {// nao foi visitado
                            curCol += 1;
                            boardConfig[curLine][curCol].mainBoard = true;
                            boardConfig[curLine][curCol].visited = true;
                            boardConfig[curLine][curCol].left = curCol > 0 ? true : false;
                            boardConfig[curLine][curCol].right = curCol < 3 ? true : false;
                            Debug.Log(curLine + " - " + curCol);
                        }
                    }
                    else if (curLine == (boardLinesNum - 1))
                    {
                        endPos = new Vector2(curLine, curCol);
                        boardConfig[curLine][curCol].isEnd = true;
                        finish = true;
                    }

                }
                else if (next == 2)
                {// Down
                    if (curLine + 1 <= (boardLinesNum - 1))
                    {// ainda tem lugar abaixo
                        boardConfig[curLine][curCol].bottom = true;
                        curLine += 1;
                        boardConfig[curLine][curCol].mainBoard = true;
                        boardConfig[curLine][curCol].visited = true;
                        boardConfig[curLine][curCol].left = curCol > 0 ? true : false;
                        boardConfig[curLine][curCol].right = curCol < 3 ? true : false;
                        boardConfig[curLine][curCol].top = true;
                        Debug.Log(curLine + " - " + curCol);
                    }
                    else if (curLine == (boardLinesNum - 1))
                    {
                        endPos = new Vector2(curLine, curCol);
                        boardConfig[curLine][curCol].isEnd = true;
                        finish = true;
                    }
                }
            }

            for (int i = 0; i < boardLinesNum; i++)
            {
                for (int j = 0; j < boardColsNum; j++)
                {
                    if (!boardConfig[i][j].visited)
                    {
                        boardConfig[curLine][curCol].mainBoard = false;
                        boardConfig[curLine][curCol].visited = true;
                        if (j == 0)
                        {
                            boardConfig[i][j].left = false;

                            if (boardConfig[i][j + 1] != null)//checa se tem alguem a direita
                                boardConfig[i][j].right = boardConfig[i][j + 1].left;
                            else
                                boardConfig[i][j].right = Random.Range(0, 2) > 0 ? true : false;


                        }
                        else if (j > 0)
                        {
                            boardConfig[i][j].left = boardConfig[i][j - 1].right;

                            if (j == (boardColsNum - 1))
                                boardConfig[i][j].right = false;
                            else
                            {
                                if (boardConfig[i][j + 1] != null)
                                    boardConfig[i][j].right = boardConfig[i][j + 1].left;
                                else
                                    boardConfig[i][j].right = Random.Range(0, 2) > 0 ? true : false;
                            }

                        }


                        if (i == 0)
                        {
                            boardConfig[i][j].top = false;

                            if (boardConfig[i + 1][j] != null)
                                boardConfig[i][j].bottom = boardConfig[i + 1][j].top;
                            else
                                boardConfig[i][j].bottom = Random.Range(0, 2) > 0 ? true : false;

                        } if (i > 0)
                        {
                            boardConfig[i][j].top = boardConfig[i - 1][j].bottom;

                            if (i == (boardLinesNum - 1))
                                boardConfig[i][j].bottom = false;
                            else
                            {
                                if (boardConfig[i + 1][j] != null)
                                    boardConfig[i][j].bottom = boardConfig[i + 1][j].top;
                                else
                                    boardConfig[i][j].bottom = Random.Range(0, 2) > 0 ? true : false;
                            }
                        }

                    }
                }
            }
        }

		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level)
		{
			SetupBoardPieces ();
			//Creates the outer walls and floor.
			for (int i = 0; i < boardLinesNum; i++) {
                for (int j = 0; j < boardColsNum; j++)
                {
                    RoomFloorsAndWallsSetup(i, j);
                }
			}
			
			//Reset our list of gridpositions.
			InitialiseList ();
			
			DefineRoomsAndObjects (wallTiles);
								
			//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (enemyTiles, 0, 0);
			
			//Instantiate the exit tile in the bottom right hand corner of our game board
            Instantiate(exit, new Vector3((endPos.y * (roomColumsNum + 1)), ((-1) * endPos.x * (roomRowsNum + 1)), 0f), Quaternion.identity);
			
            //9
			GameObject player = GameObject.Find ("Player");
			player.transform.position = new Vector3 ((startPos.y*(roomColumsNum+1)), 6);
			//9

		}
	}
}
