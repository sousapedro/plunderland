using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace Completed
{
	public enum ERoomType
	{
		StartBedroom = 0,
		Kitchen = 1,
		Bathroom = 2,
		LivingRoom = 3,
		DiningRoom = 4,
		ParentsBedroom,
		NumberOfTypes
	}
	
	public class BoardPiece : MonoBehaviour
	{
		public bool isStart = false;
		public bool isEnd = false;

		public bool left = false;
		public bool top = false;
		public bool right = false;
		public bool bottom = false;


		public bool mainBoard = false;
		public bool visited = false;

		public ERoomType roomType;

		public Vector2 posTopLeft;
	}
}
