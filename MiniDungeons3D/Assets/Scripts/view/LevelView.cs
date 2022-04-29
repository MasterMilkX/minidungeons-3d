using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelView : MonoBehaviour
{
    [HideInInspector]
    public StatusBar sbH, sbT, sbM;
    public SimLevel _level;
    public GameObject healthBar, treasureBar, monsterBar;
    public GameObject[,] _map;
    public GameObject[] _gameCharacters;
    public Dictionary<SimPoint, GameObject> _javelins;
    int counter = 0;

    bool[] _lefties;

    public GameObject _tilePrefab;
    public Vector3 _tileSize;

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
        LayoutView(level);

        _level.SetActionCaching(true);
        Initialized = true;
        _okForInput = true;
        _busyAnimating = 0;

        sbH = (StatusBar)healthBar.GetComponent(typeof(StatusBar));
        sbH.CreateFullBar(10, false);
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
                viewChar.GetComponent<Animator>().SetFloat("RandomOffset", Random.Range(0.5f, 1.0f));
                viewChar.GetComponent<Animator>().SetBool("Alive", true);
            }

        }
        StartCoroutine(Animate(level));
    }

    void LayoutView(SimLevel level)
    {
        for (int x = 0; x < level.BaseMap.GetLength(0); x++)
        {
            for (int y = 0; y < level.BaseMap.GetLength(1); y++)
            {
                Vector3 newPosition = transform.position + new Vector3(_tilePrefab.GetComponent<Renderer>().bounds.size.x * x, _tilePrefab.GetComponent<Renderer>().bounds.size.y * y * -1, 0);
                GameObject newTile = (GameObject)Instantiate(_tilePrefab, newPosition, Quaternion.identity);


                //Debug.Log(newTile + " " + (++counter).ToString());
                //Debug.Log(level + " " + (counter).ToString());


                try
                {
                    newTile.GetComponent<Tile>().TileType = level.BaseMap[x, y].TileType;

                }
                catch
                {
                    Debug.Log(level.BaseMap[x, y] + " :: " + x + " : " + y + " :: ");
                    Debug.Log("New tile crashed");
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
    public void RefreshView(SimLevel level)
    {
        if (Animating && !Replaying)
        {
            StartCoroutine(Animate(level));
        }
        else
        {
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
        yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
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
            List<SimCharacterAction> actions = levelChar.CachedActions;

            foreach (SimCharacterAction action in actions)
            {
                var ownPosition = transform.position + new Vector3(levelChar.Point.X * _tileSize.x, levelChar.Point.Y * _tileSize.y * -1, 0);
                var actionStartPosition = transform.position + new Vector3(action.FromPoint.X * _tileSize.x, action.FromPoint.Y * _tileSize.y * -1, 0);
                var actionEndPosition = transform.position + new Vector3(action.ToPoint.X * _tileSize.x, action.ToPoint.Y * _tileSize.y * -1, 0);

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
            if (_level.Characters[i].CharacterType == GameCharacterTypes.MinitaurMonster)
            {
                var minitaurMonster = (SimMinitaurMonster)levelChar;
                if (minitaurMonster.KnockoutCounter > 0)
                {
                    viewChar.GetComponentInChildren<Animator>().SetBool("KnockedOut", true);
                }
                else
                {
                    viewChar.GetComponentInChildren<Animator>().SetBool("KnockedOut", false);
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

            if (levelChar.CharacterType == GameCharacterTypes.OgreMonster)
            {
                var ogre = (SimOgreMonster)levelChar;
                viewChar.GetComponent<Animator>().SetInteger("Development", ogre.TreasureCollected);
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

                GameObject viewHero = null;
                GameObject fromPortal = null;
                GameObject toPortal = null;

                for (int i = 0; i < _level.Characters.Length; i++)
                {


                    //Find hero
                    if (_level.Characters[i].CharacterType == GameCharacterTypes.Hero)
                    {
                        viewHero = _gameCharacters[i];
                    }
                    //Find first portal
                    if (_level.Characters[i].Point == action.FromPoint && _level.Characters[i].CharacterType == GameCharacterTypes.Portal)
                    {
                        fromPortal = _gameCharacters[i];
                    }
                    //Find second portal
                    if (_level.Characters[i].Point == action.ToPoint && _level.Characters[i].CharacterType == GameCharacterTypes.Portal)
                    {
                        toPortal = _gameCharacters[i];
                    }
                }

                //Start portal animations
                audio_Manager.PlaySFX(4);
                fromPortal.GetComponent<Animator>().SetTrigger("PortalActive");
                toPortal.GetComponent<Animator>().SetTrigger("PortalActive");

                Hashtable tweenArgs = iTween.Hash("position", actionEndPosition, "delay", 0.05f, "time", 0.05f, "easetype", iTween.EaseType.linear, "oncomplete", "SetAnimating", "oncompletetarget", this.gameObject, "oncompleteparams", -1);
                SetAnimating(1);
                iTween.MoveTo(viewHero, tweenArgs);
            }
        }
    }

    void AnimateTraps(SimLevel level)
    {
        for (int i = 0; i < _level.Characters.Length; i++)
        {
            var levelChar = _level.Characters[i];
            var viewChar = _gameCharacters[i];
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
                            _gameCharacters[j].GetComponent<Animator>().SetTrigger("SpringTrap");
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
            for (int character = 0; character < _gameCharacters.Length; character++)
            {
                try
                {
                    _gameCharacters[character].GetComponent<Animator>().SetBool("Alive", true);
                }
                catch
                {
                    Debug.Log(_gameCharacters[character] + " has no animator controller to modify");
                }
                Destroy(_gameCharacters[character]);
            }
            _gameCharacters = null;
            for (int x = 0; x < _map.GetLength(0); x++)
            {
                for (int y = 0; y < _map.GetLength(1); y++)
                {
                    Destroy(_map[x, y]);
                }
            }
            _map = null;
        }
        Initialized = false;
    }
}
