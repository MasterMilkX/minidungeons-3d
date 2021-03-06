using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelView : MonoBehaviour
{
    [HideInInspector]
    public StatusBar sbH, sbT, sbM;
    public SimLevel _level;
    public GameObject healthBar, treasureBar, monsterBar;
    public GameObject compass;

    public GameObject damageFXScreen;
    int lastHealth;
    float damageTimer = 0.6f;

    public GameObject[,] _map;
    public GameObject[] _gameCharacters;
    public GameObject[] _gameCharacters3D;

    public Transform Player;

    public Dictionary<SimPoint, GameObject> _javelins;
    public Dictionary<SimPoint, GameObject> _javelins3D;
    int counter = 0;

    bool[] _lefties;
    bool[] _lefties3D;

    public GameObject _tilePrefab2D;
    public Vector3 _tileSize;

    public GameObject _floorPrefab3D;
    public GameObject _ceilingPrefab3D;
    public GameObject _wallPrefab3D;
    public Transform _levelTransform;

    public RenderTexture targetTexture3D;
    public Transform fullPlayer3DView;

    public bool Animating;
    public bool Replaying = false;
    public float MoveSpeed = 0.01f;

    public bool Initialized { get; set; }
    public Audio_Manager audio_Manager;
    public void Initialize(SimLevel level)
    {
        if (_javelins != null && _javelins.Count > 0)
        {
            var javelins = _javelins.Values;
            foreach (GameObject javelin in javelins)
            {
                Destroy(javelin);
            }
        }

        _level = level;
        _map = new GameObject[level.BaseMap.GetLength(0), level.BaseMap.GetLength(1)];
        
        // 2d and 3d rendering of the level
        LayoutView(level);
        Build3DMap(level);

        _level.SetActionCaching(true);
        Initialized = true;
        _okForInput = true;
        _busyAnimating = 0;

        sbH = (StatusBar)healthBar.GetComponent(typeof(StatusBar));
        sbH.CreateFullBar(10, false);
        lastHealth = 10;
        sbT = (StatusBar)treasureBar.GetComponent(typeof(StatusBar));
        sbT.CreateFullBar(level.Treasures.Count, true);
        sbM = (StatusBar)monsterBar.GetComponent(typeof(StatusBar));
        sbM.CreateFullBar(level.Monsters.Count, true);
        Debug.Log("Initialized");
        for (int i = 0; i < level.Characters.Length; i++)
        {

            GameObject viewChar = _gameCharacters[i];
            if (viewChar.GetComponent<Animator>() != null)
            {
                viewChar.GetComponent<Animator>().SetFloat("RandomOffset", UnityEngine.Random.Range(0.5f, 1.0f));
                viewChar.GetComponent<Animator>().SetBool("Alive", true);
            }

        }
        StartCoroutine(Animate(level));
    }

    void LayoutView(SimLevel level)
    {
        Debug.Log(level.BaseMap.GetLength(1));
        for (int x = 0; x < level.BaseMap.GetLength(0); x++)
        {
            for (int y = 0; y < level.BaseMap.GetLength(1); y++)
            {
                Vector3 newPosition = transform.position + new Vector3(_tilePrefab2D.GetComponent<Renderer>().bounds.size.x * x, _tilePrefab2D.GetComponent<Renderer>().bounds.size.y * y * -1, 0);
                GameObject newTile = (GameObject)Instantiate(_tilePrefab2D, newPosition, Quaternion.identity);


                //Debug.Log(newTile + " " + (++counter).ToString());
                //Debug.Log(level + " " + (counter).ToString());


                try
                {
                    newTile.GetComponent<Tile>().TileType = level.BaseMap[x, y].TileType;

                }
                catch (Exception ex)
                {
                    Debug.Log(level.BaseMap[x, y] + " :: " + x + " : " + y + " :: ");
                    Debug.Log("New tile crashed");
                    Debug.Log(ex.ToString());
                }

                newTile.transform.parent = transform;
                _map[x, y] = newTile;
            }
        }
        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                if (_map[x, y].GetComponent<Tile>().TileType == TileTypes.wall || _map[x, y].GetComponent<Tile>().TileType == TileTypes.fillerWall)
                {
                    if (y + 1 < level.BaseMap.GetLength(1))
                    {
                        if (_map[x, y + 1].GetComponent<Tile>().TileType == TileTypes.wall || _map[x, y + 1].GetComponent<Tile>().TileType == TileTypes.fillerWall)
                        {
                            _map[x, y].GetComponent<Tile>().TileType = TileTypes.fillerWall;
                        }
                    }
                }
            }
        }

        _tileSize = _map[0, 0].GetComponent<Renderer>().bounds.size;
        _gameCharacters = new GameObject[level.Characters.Length];
        _lefties = new bool[level.Characters.Length];
        _javelins = new Dictionary<SimPoint, GameObject>();

        for (int character = 0; character < level.Characters.Length; character++)
        {
            var characterPosition = transform.position + new Vector3(level.Characters[character].Point.X * _tileSize.x, level.Characters[character].Point.Y * _tileSize.y * -1, 0);
            GameObject gameCharacter = GetComponent<SimGameCharacterFactory>().SpawnGameCharacter(level.Characters[character].CharacterType, characterPosition);
            _gameCharacters[character] = gameCharacter;
        }

        if (!Animating)
        {
            _level = level.Clone();
        }
    }

    public void Build3DMap(SimLevel level)
    {
        List<Vector3> open_pos = new List<Vector3>();

        //place the floor and ceiling
        for (int h = 0; h < level.BaseMap.GetLength(0); h++)
        {
            for (int w = 0; w < level.BaseMap.GetLength(1); w++)
            {
                Debug.Log("Here: " + h + ", " + w);
                GameObject newFloor = Instantiate(_floorPrefab3D, new Vector3(w, -0.55f, h), Quaternion.identity);
                newFloor.transform.SetParent(_levelTransform);
                GameObject newCeiling = Instantiate(_ceilingPrefab3D, new Vector3(w, 0.55f, h), Quaternion.identity * Quaternion.Euler(180, 0, 0));
                newCeiling.transform.SetParent(_levelTransform);

                if (_map[h, w].GetComponent<Tile>().TileType == TileTypes.wall || _map[h, w].GetComponent<Tile>().TileType == TileTypes.fillerWall)
                {
                    GameObject newObj = Instantiate(_wallPrefab3D, new Vector3(w, 0.0f, h), Quaternion.identity);
                    newObj.transform.SetParent(_levelTransform);
                }
            }
        }
        _gameCharacters3D = new GameObject[level.Characters.Length];
        _lefties3D = new bool[level.Characters.Length];
        _javelins3D = new Dictionary<SimPoint, GameObject>();

        for (int character = 0; character < level.Characters.Length; character++)
        {
            var characterPosition = _levelTransform.position + new Vector3(level.Characters[character].Point.Y, -0.55f, level.Characters[character].Point.X);
            GameObject gameCharacter = GetComponent<SimGameCharacterFactory>().SpawnGameCharacter3D(level.Characters[character].CharacterType, characterPosition);
            _gameCharacters3D[character] = gameCharacter;
            gameCharacter.transform.SetParent(_levelTransform);

            if (level.Characters[character].CharacterType == GameCharacterTypes.Hero)
            {
                // this is the player, setup the camera
                Camera playerCam = gameCharacter.GetComponentInChildren<Camera>();
                playerCam.targetTexture = targetTexture3D;
                gameCharacter.transform.rotation = Quaternion.identity;
                Player = gameCharacter.transform;
            }
        }
    }
    public void RefreshView(SimLevel level)
    {
        if (Animating && !Replaying)
        {

            int newHealth = level.SimHero.Health;

            StartCoroutine(Animate(level));

            //take damage
            if(newHealth < lastHealth){
                StartCoroutine(FlashDamage());
                StartCoroutine(ShakeShack(fullPlayer3DView,lastHealth-newHealth));
            }

            //update UI
            sbH.UpdateValue(level.SimHero.Health);
            sbT.UpdateValue(level.SimHero.TreasureCollected);
            sbM.UpdateValue(level.GetMonsterKilled());
        }
        else
        {

            //update character
            for (int character = 0; character < _level.Characters.Length; character++)
            {
                SimGameCharacter oldCharacter = _level.Characters[character];
                SimGameCharacter newCharacter = level.Characters[character];
                if (oldCharacter.Point != newCharacter.Point)
                {
                    var characterPosition = transform.position + new Vector3(newCharacter.Point.X * _tileSize.x, newCharacter.Point.Y * _tileSize.y * -1, 0);
                    _gameCharacters[character].transform.position = characterPosition;
                }
                if (_gameCharacters[character].GetComponent<Animator>() != null)
                {
                    _gameCharacters[character].GetComponent<Animator>().SetBool("Alive", newCharacter.Alive);
                }
                if (level.Characters[character].CharacterType == GameCharacterTypes.MinitaurMonster)
                {
                    var minitaur = (SimMinitaurMonster)level.Characters[character];
                    if (minitaur.KnockoutCounter > 0)
                    {
                        _gameCharacters[character].GetComponent<Animator>().SetBool("KnockedOut", true);
                    }
                    else
                    {
                        _gameCharacters[character].GetComponent<Animator>().SetBool("KnockedOut", false);
                    }
                }
            }

            _level = level.Clone();
        }

        lastHealth = level.SimHero.Health;
    }

    IEnumerator TimedAnimationToggle(GameObject target, string parameterName, bool targetValue, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        try
        {
            target.GetComponent<Animator>().SetBool(parameterName, targetValue);
        }
        catch
        {

        }
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.0f));
        try
        {
            target.GetComponent<Animator>().SetBool(parameterName, targetValue);
        }
        catch { }

    }

    IEnumerator TimedDestroy(GameObject target, float time)
    {
        yield return new WaitForSeconds(time);
        try
        {
            Destroy(target);
        }
        catch
        {
            Debug.Log("Target already destroyed. Aborting!");
        }
    }

    IEnumerator DecorateWithBlood(GameObject target, float time)
    {
        Debug.Log("Decorating with blood");
        yield return new WaitForSeconds(0.2f);
        GameObject bloodDecoration = GetComponent<SimGameCharacterFactory>().SpawnGameCharacter(GameCharacterTypes.BloodDecoration, target.transform.position);
        bloodDecoration.transform.parent = target.transform;
        StartCoroutine(TimedDestroy(bloodDecoration, 1f));
    }

    //flash the damage screen if took damage
    IEnumerator FlashDamage(){
        damageFXScreen.SetActive(true);
        yield return new WaitForSeconds(damageTimer);
        damageFXScreen.SetActive(false);
    }

    //shakes an object
    IEnumerator ShakeShack(Transform shook,int intensity=1){

        for ( int i = 0; i < 10; i++){
            Vector3 randVec = new Vector3((5-i/2)*(UnityEngine.Random.value < 0.5 ? -1 : 1),(5-i/2)*(UnityEngine.Random.value < 0.5 ? -1 : 1),0.0f);    //amount to change by

            shook.localPosition += randVec*intensity;
            yield return new WaitForSeconds(damageTimer/20.0f);
            shook.localPosition -= randVec*intensity;
            yield return new WaitForSeconds(damageTimer/20.0f);
        }
    }

    enum MoveDirections
    {
        None, North, East, South, West
    }

    MoveDirections MoveDirectionFromAction(SimCharacterAction action)
    {
        if (action.ToPoint.Equals(action.FromPoint))
        {
            return MoveDirections.None;
        }
        if (action.FromPoint.X == action.ToPoint.X && action.FromPoint.Y > action.ToPoint.Y)
        {
            return MoveDirections.North;
        }
        if (action.FromPoint.X < action.ToPoint.X && action.FromPoint.Y == action.ToPoint.Y)
        {
            return MoveDirections.East;
        }
        if (action.FromPoint.X == action.ToPoint.X && action.FromPoint.Y < action.ToPoint.Y)
        {
            return MoveDirections.South;
        }
        if (action.FromPoint.X > action.ToPoint.X && action.FromPoint.Y == action.ToPoint.Y)
        {
            return MoveDirections.West;
        }
        return MoveDirections.None;
    }

    int AnimDirection(MoveDirections direction)
    {
        int result = -1;
        switch (direction)
        {
            case MoveDirections.None:
                result = -1;
                break;
            case MoveDirections.North:
                result = 1;
                break;
            case MoveDirections.East:
                result = 2;
                break;
            case MoveDirections.South:
                result = 0;
                break;
            case MoveDirections.West:
                result = 2;
                break;
        }
        return result;
    }

    void FaceLeft(GameObject character, int index, bool faceLeft)
    {
        if (!_lefties[index] == faceLeft)
        {
            Vector3 theScale = character.transform.localScale;
            theScale.x *= -1;
            character.transform.localScale = theScale;
            _lefties[index] = faceLeft;
        }
    }

    void AnimateJavelin(SimLevel level)
    {
        foreach (SimCharacterAction action in _level.SimHero.CachedActions)
        {
            var actionStartPosition = transform.position + new Vector3(action.FromPoint.X * _tileSize.x, action.FromPoint.Y * _tileSize.y * -1, 0);
            var actionEndPosition = transform.position + new Vector3(action.ToPoint.X * _tileSize.x, action.ToPoint.Y * _tileSize.y * -1, 0);
            if (action.ActionType == SimCharacterActionTypes.JavelinThrow)
            {
                GameObject newJavelin = GetComponent<SimGameCharacterFactory>().SpawnGameCharacter(GameCharacterTypes.Javelin, actionStartPosition);
                _javelins.Add(action.ToPoint, newJavelin);
                //TODO: Orient Javelin toward target.
                Hashtable tweenArgs = iTween.Hash("position", actionEndPosition, "speed", MoveSpeed * 10f, "easetype", iTween.EaseType.linear, "oncomplete", "SetAnimating", "oncompletetarget", this.gameObject, "oncompleteparams", -1);
                SetAnimating(1);
                iTween.MoveTo(newJavelin, tweenArgs);
            }
            if (action.ActionType == SimCharacterActionTypes.JavelinPickup)
            {
                GameObject javelin = null;
                _javelins.TryGetValue(action.ToPoint, out javelin);
                _javelins.Remove(action.ToPoint);
                if (javelin != null)
                {
                    Destroy(javelin);
                }
            }
        }
    }

    public void AnimateMoves(SimLevel level)
    {
        for (int i = 0; i < _level.Characters.Length; i++)
        {
            // yield return new WaitForSeconds(0.05f);
            var levelChar = _level.Characters[i];
            var viewChar = _gameCharacters[i];
            var viewChar3D = _gameCharacters3D[i];
            List<SimCharacterAction> actions = levelChar.CachedActions;

            foreach (SimCharacterAction action in actions)
            {
                var ownPosition = transform.position + new Vector3(levelChar.Point.X * _tileSize.x, levelChar.Point.Y * _tileSize.y * -1, 0);
                var actionStartPosition = transform.position + new Vector3(action.FromPoint.X * _tileSize.x, action.FromPoint.Y * _tileSize.y * -1, 0);
                var actionEndPosition = transform.position + new Vector3(action.ToPoint.X * _tileSize.x, action.ToPoint.Y * _tileSize.y * -1, 0);

                var ownPosition3D = _levelTransform.position + new Vector3(levelChar.Point.Y, -0.55f, levelChar.Point.X);
                var actionStartPosition3D = _levelTransform.position + new Vector3(action.FromPoint.Y, -0.55f, action.FromPoint.X);
                var actionEndPosition3D = _levelTransform.position + new Vector3(action.ToPoint.Y, -0.55f, action.ToPoint.X);
                
                bool foundFighting = false;
                if (action.ActionType == SimCharacterActionTypes.Fight)
                {
                    // Debug.Log("found fighting: " + viewChar);
                    //Find hero
                    if (levelChar.CharacterType == GameCharacterTypes.Hero)
                    {
                        audio_Manager.PlaySFX(1);
                    }
                    foundFighting = true;
                    StartCoroutine(DecorateWithBlood(viewChar, 1f));
                }

                //Regular move
                if (action.ActionType == SimCharacterActionTypes.Move)
                {
                    // Debug.Log("found moving: " + viewChar + " and " + levelChar);
                    MoveDirections direction = MoveDirectionFromAction(action);
                    if (direction == MoveDirections.West)
                    {
                        FaceLeft(viewChar, i, true);
                    }
                    else
                    {
                        FaceLeft(viewChar, i, false);
                    }
                    viewChar.GetComponent<Animator>().SetInteger("AnimDirection", AnimDirection(direction));
                    viewChar.GetComponent<Animator>().SetBool("Moving", true);
                    StartCoroutine(TimedAnimationToggle(viewChar, "Moving", false, MoveSpeed));
                    Hashtable tweenArgs = iTween.Hash("position", actionEndPosition, "time", MoveSpeed, "easetype", iTween.EaseType.linear, "oncomplete", "SetAnimating", "oncompletetarget", this.gameObject, "oncompleteparams", -1);
                    SetAnimating(1);
                    
                    iTween.MoveTo(viewChar, tweenArgs);
                    viewChar3D.transform.position = actionEndPosition3D;
                }
            }

        }
        

        // yield return new WaitForSeconds(MoveSpeed);
    }
    void AnimateBlockedMoves(SimLevel level)
    {

        for (int i = 0; i < _level.Characters.Length; i++)
        {
            var levelChar = _level.Characters[i];
            var viewChar = _gameCharacters[i];
            List<SimCharacterAction> actions = levelChar.CachedActions;

            foreach (SimCharacterAction action in actions)
            {
                var ownPosition = transform.position + new Vector3(levelChar.Point.X * _tileSize.x, levelChar.Point.Y * _tileSize.y * -1, 0);
                var actionStartPosition = transform.position + new Vector3(action.FromPoint.X * _tileSize.x, action.FromPoint.Y * _tileSize.y * -1, 0);
                var actionEndPosition = transform.position + new Vector3(action.ToPoint.X * _tileSize.x, action.ToPoint.Y * _tileSize.y * -1, 0);

                MoveDirections moveDirection = MoveDirectionFromAction(action);
                if (viewChar.active)
                {
                    switch (moveDirection)
                    {
                        case MoveDirections.North:
                            actionEndPosition += new Vector3(0f, -(_tileSize.y / 2f), 0f);
                            viewChar.GetComponent<Animator>().SetInteger("AnimDirection", 1);
                            break;
                        case MoveDirections.East:
                            actionEndPosition += new Vector3(-(_tileSize.x / 2f), 0f, 0f);
                            viewChar.GetComponent<Animator>().SetInteger("AnimDirection", 1);
                            break;
                        case MoveDirections.South:
                            actionEndPosition += new Vector3(0f, +(_tileSize.y / 2f), 0f);
                            viewChar.GetComponent<Animator>().SetInteger("AnimDirection", 1);
                            break;
                        case MoveDirections.West:
                            actionEndPosition += new Vector3(+(_tileSize.x / 2f), 0f, 0f);
                            viewChar.GetComponent<Animator>().SetInteger("AnimDirection", 1);
                            break;
                    }
                }

                //Blocked move
                if (action.ActionType == SimCharacterActionTypes.BlockedMove)
                {
                    if (viewChar.active)
                    {
                        viewChar.GetComponent<Animator>().SetBool("Moving", true);
                        StartCoroutine(TimedAnimationToggle(viewChar, "Moving", false, MoveSpeed));
                        Vector3[] path = { actionStartPosition, actionEndPosition, actionStartPosition };
                        Hashtable tweenArgs = iTween.Hash("position", actionStartPosition, "path", path, "time", MoveSpeed, "easetype", iTween.EaseType.linear, "oncomplete", "SetAnimating", "oncompletetarget", this.gameObject, "oncompleteparams", -1);
                        SetAnimating(1);
                        iTween.MoveTo(viewChar, tweenArgs);
                    }
                }
            }
        }
    }

    void AnimateMerges(SimLevel level)
    {
        for (int i = 0; i < _level.Characters.Length; i++)
        {
            var levelChar = _level.Characters[i];
            var viewChar = _gameCharacters[i];
            List<SimCharacterAction> actions = levelChar.CachedActions;

            foreach (SimCharacterAction action in actions)
            {
                if (action.ActionType == SimCharacterActionTypes.BlobMerge)
                {
                    //Find the blob that needs to be removed
                    GameObject otherBlob = null;
                    for (int j = 0; j < _level.Characters.Length; j++)
                    {
                        if (_level.Characters[j].Point.Equals(action.ToPoint) && _level.Characters[j].CharacterType == GameCharacterTypes.BlobMonster && _level.Characters[j] != _level.Characters[i])
                        {
                            otherBlob = _gameCharacters[j];
                        }
                    }
                    otherBlob.active = false;
                    //Update the remaining blob to the new development level
                    Debug.Log("Adding to nucleus development");
                    var remainingBlob = (SimBlobMonster)levelChar;
                    int nucleus = viewChar.GetComponent<Animator>().GetInteger("NucleusDevelopment");
                    Debug.Log("Was: " + nucleus);
                    nucleus = nucleus + 1;
                    if (nucleus > 3) { nucleus = 3; }
                    Debug.Log("Is: " + nucleus);
                    viewChar.GetComponent<Animator>().SetInteger("NucleusDevelopment", nucleus);
                }
            }
            foreach (SimCharacterAction action in actions)
            {
                if (action.ActionType == SimCharacterActionTypes.DrankPotion)
                {
                    if (_level.Characters[i].CharacterType == GameCharacterTypes.BlobMonster)
                    {
                        var blob = (SimBlobMonster)_level.Characters[i];
                        Debug.Log("Adding to color development");
                        int potionColor = viewChar.GetComponent<Animator>().GetInteger("ColorDevelopment");
                        Debug.Log("Was: " + potionColor);
                        potionColor = potionColor + 1;
                        if (potionColor > 3) { potionColor = 3; }
                        Debug.Log("Is: " + potionColor);
                        viewChar.GetComponent<Animator>().SetInteger("ColorDevelopment", potionColor);
                    }

                }
            }
        }
    }

    void AnimateKnockedOut(SimLevel level)
    {
        for (int i = 0; i < _level.Characters.Length; i++)
        {
            var levelChar = _level.Characters[i];
            var viewChar = _gameCharacters[i];
            var viewChar3D = _gameCharacters3D[i];
            if (_level.Characters[i].CharacterType == GameCharacterTypes.MinitaurMonster)
            {
                var minitaurMonster = (SimMinitaurMonster)levelChar;
                if (minitaurMonster.KnockoutCounter > 0)
                {
                    viewChar.GetComponentInChildren<Animator>().SetBool("KnockedOut", true);
                    viewChar3D.GetComponentInChildren<Animator>().SetBool("KnockedOut", true);
                }
                else
                {
                    viewChar.GetComponentInChildren<Animator>().SetBool("KnockedOut", false);
                    viewChar3D.GetComponentInChildren<Animator>().SetBool("KnockedOut", false);
                }

            }
        }
    }

    void AnimateClampBlobDevelopment(SimLevel level)
    {
        for (int i = 0; i < _level.Characters.Length; i++)
        {
            var levelChar = _level.Characters[i];
            var viewChar = _gameCharacters[i];

            if (_level.Characters[i].CharacterType == GameCharacterTypes.BlobMonster && _level.Characters[i].Alive)
            {
                var blob = (SimBlobMonster)levelChar;
                int devStage = blob.DevelopmentStage;
                int color = viewChar.GetComponent<Animator>().GetInteger("ColorDevelopment");
                int nucleus = viewChar.GetComponent<Animator>().GetInteger("NucleusDevelopment");
                if (devStage < color)
                {
                    viewChar.GetComponent<Animator>().SetInteger("ColorDevelopment", devStage);
                }
                if (devStage < nucleus)
                {
                    viewChar.GetComponent<Animator>().SetInteger("NucleusDevelopment", devStage);
                }
            }
        }
    }

    void AnimateOgres(SimLevel level)
    {
        for (int i = 0; i < _level.Characters.Length; i++)
        {
            var levelChar = _level.Characters[i];
            var viewChar = _gameCharacters[i];
            var viewChar3D = _gameCharacters3D[i];

            if (levelChar.CharacterType == GameCharacterTypes.OgreMonster)
            {
                var ogre = (SimOgreMonster)levelChar;
                viewChar.GetComponent<Animator>().SetInteger("Development", ogre.TreasureCollected);
                viewChar3D.GetComponent<Animator>().SetInteger("Development", ogre.TreasureCollected);
            }
        }
    }

    void AnimatePortals(SimLevel level)
    {
        foreach (SimCharacterAction action in _level.SimHero.CachedActions)
        {
            if (action.ActionType == SimCharacterActionTypes.PortalActivate)
            {
                var actionStartPosition = transform.position + new Vector3(action.FromPoint.X * _tileSize.x, action.FromPoint.Y * _tileSize.y * -1, 0);
                var actionEndPosition = transform.position + new Vector3(action.ToPoint.X * _tileSize.x, action.ToPoint.Y * _tileSize.y * -1, 0);


                var actionEndPosition3D = _levelTransform.position + new Vector3(action.ToPoint.Y, -0.55f, action.ToPoint.X);

                GameObject viewHero = null;
                GameObject fromPortal = null;
                GameObject toPortal = null;

                GameObject viewHero3D = null;
                GameObject fromPortal3D = null;
                GameObject toPortal3D = null;

                for (int i = 0; i < _level.Characters.Length; i++)
                {


                    //Find hero
                    if (_level.Characters[i].CharacterType == GameCharacterTypes.Hero)
                    {
                        viewHero = _gameCharacters[i];
                        viewHero3D = _gameCharacters3D[i];
                    }
                    //Find first portal
                    if (_level.Characters[i].Point == action.FromPoint && _level.Characters[i].CharacterType == GameCharacterTypes.Portal)
                    {
                        fromPortal = _gameCharacters[i];
                        fromPortal3D = _gameCharacters3D[i];
                    }
                    //Find second portal
                    if (_level.Characters[i].Point == action.ToPoint && _level.Characters[i].CharacterType == GameCharacterTypes.Portal)
                    {
                        toPortal = _gameCharacters[i];
                        toPortal3D = _gameCharacters3D[i];
                    }
                }

                //Start portal animations
                audio_Manager.PlaySFX(4);
                fromPortal.GetComponent<Animator>().SetTrigger("PortalActive");
                toPortal.GetComponent<Animator>().SetTrigger("PortalActive");

                Hashtable tweenArgs = iTween.Hash("position", actionEndPosition, "delay", 0.05f, "time", 0.05f, "easetype", iTween.EaseType.linear, "oncomplete", "SetAnimating", "oncompletetarget", this.gameObject, "oncompleteparams", -1);
                SetAnimating(1);
                iTween.MoveTo(viewHero, tweenArgs);

                viewHero3D.transform.position = actionEndPosition3D;
            }
        }
    }

    void AnimateTraps(SimLevel level)
    {
        for (int i = 0; i < _level.Characters.Length; i++)
        {
            var levelChar = _level.Characters[i];
            var viewChar = _gameCharacters[i];
            //var viewChar3D = _gameCharacters3D[i];
            List<SimCharacterAction> actions = levelChar.CachedActions;

            foreach (SimCharacterAction action in actions)
            {
                if (action.ActionType == SimCharacterActionTypes.TrapSpring)
                {
                    GameObject trap = null;
                    for (int j = 0; j < _level.Characters.Length; j++)
                    {
                        if (_level.Characters[j].Point == action.ToPoint && _level.Characters[j].CharacterType == GameCharacterTypes.Trap)
                        {
                            viewChar.GetComponent<Animator>().SetTrigger("SpringTrap");
                            //viewChar3D.GetComponent<Animator>().SetTrigger("SpringTrap");
                        }
                    }
                }
            }
        }
    }
    void AnimateRangedAttacks(SimLevel level)
    {
        for (int i = 0; i < _level.Characters.Length; i++)
        {
            var levelChar = _level.Characters[i];
            var viewChar = _gameCharacters[i];
            List<SimCharacterAction> actions = levelChar.CachedActions;

            foreach (SimCharacterAction action in actions)
            {
                var ownPosition = transform.position + new Vector3(levelChar.Point.X * _tileSize.x, levelChar.Point.Y * _tileSize.y * -1, 0);
                var actionStartPosition = transform.position + new Vector3(action.FromPoint.X * _tileSize.x, action.FromPoint.Y * _tileSize.y * -1, 0);
                var actionEndPosition = transform.position + new Vector3(action.ToPoint.X * _tileSize.x, action.ToPoint.Y * _tileSize.y * -1, 0);

                //Blocked move
                if (action.ActionType == SimCharacterActionTypes.RangedAttack)
                {
                    //Debug.Log("animate ranged attack");
                    GameObject attack = GetComponent<SimGameCharacterFactory>().SpawnGameCharacter(GameCharacterTypes.RangedMonsterAttack, actionStartPosition);
                    float angle = Vector3.Angle(actionEndPosition, actionStartPosition);
                    Vector3 rotation = Vector3.zero;
                    if (actionStartPosition.x > actionEndPosition.x && actionStartPosition.y > actionEndPosition.y)
                    {
                        //shooting down left
                        rotation = new Vector3(0f, 0f, angle + 125f);
                    }
                    if (actionStartPosition.x < actionEndPosition.x && actionStartPosition.y > actionEndPosition.y)
                    {
                        //shooting down right
                        rotation = new Vector3(0f, 0f, angle - 125f);
                    }
                    if (actionStartPosition.x > actionEndPosition.x && actionStartPosition.y < actionEndPosition.y)
                    {
                        //shooting up left
                        rotation = new Vector3(0f, 0f, angle);
                    }
                    if (actionStartPosition.x < actionEndPosition.x && actionStartPosition.y < actionEndPosition.y)
                    {
                        //shooting up right
                        rotation = new Vector3(0f, 0f, angle - 90f);
                    }
                    if (actionStartPosition.x == actionEndPosition.x && actionStartPosition.y > actionEndPosition.y)
                    {
                        //shooting down
                        rotation = new Vector3(0f, 0f, 180f);
                    }
                    if (actionStartPosition.x == actionEndPosition.x && actionStartPosition.y < actionEndPosition.y)
                    {
                        //shooting up
                    }
                    if (actionStartPosition.x < actionEndPosition.x && actionStartPosition.y == actionEndPosition.y)
                    {
                        //shooting left
                        rotation = new Vector3(0f, 0f, 270f);
                    }
                    if (actionStartPosition.x > actionEndPosition.x && actionStartPosition.y == actionEndPosition.y)
                    {
                        //shooting right
                        rotation = new Vector3(0f, 0f, 90f);
                    }
                    iTween.RotateTo(attack, rotation, 0f);
                    //Hashtable tweenArgs = iTween.Hash("position",actionEndPosition,"orienttopath",true,"speed",MoveSpeed,"easetype",iTween.EaseType.linear,"oncomplete","Tester","oncompletetarget",this.gameObject);
                    Hashtable tweenArgs = iTween.Hash("position", actionEndPosition, "speed", MoveSpeed * 10f, "easetype", iTween.EaseType.linear, "oncomplete", "DestroyGO", "oncompletetarget", this.gameObject, "oncompleteparams", attack);
                    SetAnimating(1);
                    iTween.MoveTo(attack, tweenArgs);
                }
            }
        }
    }


    public void DestroyGO(GameObject target)
    {
        SetAnimating(-1);
        Destroy(target);
    }

    void AnimateDeaths(SimLevel level)
    {
        for (int i = 0; i < _level.Characters.Length; i++)
        {
            Animator anim = _gameCharacters[i].GetComponent<Animator>();
            if (anim != null && anim.gameObject.GetComponent<Renderer>().enabled)
            {
                bool wasAlive = anim.GetBool("Alive");
                bool isAlive = _level.Characters[i].Alive;
                if (wasAlive != isAlive)
                {
                    if (_level.Characters[i].CharacterType == GameCharacterTypes.Treasure)
                    {
                        audio_Manager.PlaySFX(2);
                    }
                    else if (_level.Characters[i].CharacterType == GameCharacterTypes.Potion)
                    {
                        audio_Manager.PlaySFX(3);
                    }
                }

                anim.SetBool("Alive", _level.Characters[i].Alive);
                if (_level.Characters[i].CharacterType != GameCharacterTypes.Hero && _level.Characters[i].CharacterType != GameCharacterTypes.Potion && _level.Characters[i].CharacterType != GameCharacterTypes.Treasure && _level.Characters[i].CharacterType != GameCharacterTypes.Trap)
                {
                    Animator anim3D = _gameCharacters3D[i].GetComponent<Animator>();
                    anim3D.SetBool("Alive", _level.Characters[i].Alive);
                } else if (_level.Characters[i].CharacterType == GameCharacterTypes.Hero)
                {
                    Animator anim3D = _gameCharacters3D[i].GetComponentInChildren<Animator>();
                    anim3D.SetBool("Alive", _level.Characters[i].Alive);
                }
                // Debug.Log("Char: " + _gameCharacters[i] + ", " + level.Characters[i].Alive);
            }
        }
    }
    void FlushCachedActions(SimLevel level)
    {
        List<SimCharacterAction> flushedActions = new List<SimCharacterAction>(); //TODO hook this into the telemetry system, also add a field for which kind of character is submitting the action.
        foreach (SimGameCharacter simGameCharacter in _level.Characters)
        {
            flushedActions.AddRange(simGameCharacter.FlushCachedActions());
        }
    }

    void SetAnimating(int busyAnimating)
    {
        _busyAnimating += busyAnimating;
    }

    public int _busyAnimating;
    public bool _okForInput;

    IEnumerator Animate(SimLevel level)
    {
        _okForInput = false;

        DepthSortGameCharacters(_gameCharacters);
        AnimateJavelin(level);
        while (_busyAnimating > 0) { yield return null; }
        AnimateMoves(level);
        DepthSortGameCharacters(_gameCharacters);
        while (_busyAnimating > 0) { yield return null; }
        AnimateMerges(level);
        AnimateClampBlobDevelopment(level);
        AnimateOgres(level);
        while (_busyAnimating > 0) { yield return null; }
        AnimateBlockedMoves(level);
        DepthSortGameCharacters(_gameCharacters);
        while (_busyAnimating > 0) { yield return null; }
        AnimateKnockedOut(level);
        AnimatePortals(level);
        DepthSortGameCharacters(_gameCharacters);
        while (_busyAnimating > 0) { yield return null; }
        AnimateRangedAttacks(level);
        while (_busyAnimating > 0) { yield return null; }
        AnimateTraps(level);
        while (_busyAnimating > 0) { yield return null; }
        AnimateClampBlobDevelopment(level);
        AnimateDeaths(level);
        
        
        
        
        FlushCachedActions(level);
        yield return null;

        _okForInput = true;
    }

    public void DepthSortGameCharacters()
    {
        DepthSortGameCharacters(_gameCharacters);
    }

    void DepthSortGameCharacters(GameObject[] charactersToSort)
    {
        foreach (GameObject characterToSort in charactersToSort)
        {
            if (characterToSort != null)
            {
                characterToSort.GetComponent<SpriteRenderer>().sortingOrder = (int)Camera.main.WorldToScreenPoint(characterToSort.GetComponent<SpriteRenderer>().bounds.min).y * -1;
            }
        }
    }

    public void ClearView()
    {
        if (Initialized)
        {
            Initialized = false;
            for (int character = 0; character < _gameCharacters.Length; character++)
            {
                try
                {
                    _gameCharacters[character].GetComponent<Animator>().SetBool("Alive", true);
                    _gameCharacters3D[character].GetComponent<Animator>().SetBool("Alive", true);
                }
                catch
                {
                    Debug.Log(_gameCharacters[character] + " has no animator controller to modify");
                }
                Destroy(_gameCharacters[character]);
                Destroy(_gameCharacters3D[character]);
            }

            _gameCharacters = null;
            _gameCharacters3D = null;
            for (int x = 0; x < _map.GetLength(0); x++)
            {
                for (int y = 0; y < _map.GetLength(1); y++)
                {
                    Destroy(_map[x, y]);
                }
            }
            _map = null;
        }
        
    }
}
