using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardService{
	static BoardService instance;
	public static BoardService Instance {
		get {
			if (instance == null)
				instance = new BoardService();
			return instance;
		}
	}
	





    public Tilemap currentBoard;
    
}
