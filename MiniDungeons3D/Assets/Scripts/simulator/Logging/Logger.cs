using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
[JsonConverter(typeof(StringEnumConverter))]
public enum Mechanic
{
    GoblinMeleeHit,
    GoblinRangedHit,
    OgreHit,
    BlobHit,
    MinitaurHit,
    JavelinThrow,
    JavelinPickup,

    CollectTreasure,
    CollectPotion,
    Die,
    ReachStairs,

    MoveLeft,
    MoveRight,
    MoveUp,
    MoveDown,
    None,
    UsePortal,
    TriggerTrap,

    BlobPotion,
    BlobCombine,
    OgreTreasure
}

public class Mech
{
    public int timestep { get; set; }
    public Mechanic mechanic { get; set; }
    public Mech(int timestep, Mechanic mechanic)
    {
        this.timestep = timestep;
        this.mechanic = mechanic;
    }
}


public class Logger
{
    public List<Mech> Mechanics { get; set; }

    public Logger()
    {
        this.Mechanics = new List<Mech>();
    }

    public Logger(Logger logger)
    {
        this.Mechanics = new List<Mech>(logger.Mechanics);
    }

    public void LogMechanic(int turnCount, Mechanic mech)
    {
        Mech newMech = new Mech(turnCount, mech);
        this.Mechanics.Add(newMech);
    }

    public void PrintLog()
    {
        Console.Write("**** Printing Log ****\n");
        foreach (var item in this.Mechanics)
        {
            Console.Write(String.Format("\t{0}: {1}\n", item.timestep, item.mechanic));
        }

    }

    public string GetLog()
    {
        string log = "";
        foreach (var item in this.Mechanics)
        {
            log += (String.Format("\t{0}: {1}\n", item.timestep, item.mechanic));
        }
        return log;
    }

    public Dictionary<Mechanic, int> GetFrequencies()
    {
        Dictionary<Mechanic, int> frequencies = new Dictionary<Mechanic, int>();
        foreach (Mech mechanic in Mechanics)
        {
            if (!frequencies.ContainsKey(mechanic.mechanic))
            {
                frequencies.Add(mechanic.mechanic, 1);
            }
            else
            {
                frequencies[mechanic.mechanic] += 1;
            }
        }
        return frequencies;
    }

    public void flushLog()
    {
        this.Mechanics = new List<Mech>();
    }


}