using UnityEngine;

public class Tile : MonoBehaviour {

	public TileTypes TileType{get;set;}

	SpriteRenderer _spriteRenderer;

	public Sprite _empty;
	public Sprite _wall;
	public Sprite _fillerTile;
	public Sprite _activeSprite;

	void Awake(){
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
		if(TileType == TileTypes.empty){
			_activeSprite = _empty;
		}
		if(TileType == TileTypes.wall){
			_activeSprite = _wall;
		}
		if(TileType == TileTypes.fillerWall){
			_activeSprite = _fillerTile;
		}
		_spriteRenderer.sprite = _activeSprite;
	}
}
