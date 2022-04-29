using System.Collections;
using System;
using System.Collections.Generic;
[Serializable]

public class HumanResultObject
{
    public List<Mech> mechanics;
    public Dictionary<Mechanic, int> frequencies;
    public LevelReport levelReport;
    public List<SimPoint> positions;
    public List<string> levelStates;
    public int map_number;

    public HumanResultObject(List<Mech> mechanics, Dictionary<Mechanic, int> frequencies, LevelReport levelReport, List<SimPoint> positions, List<string> levelStates, int map_number)
    {
        this.mechanics = mechanics;
        this.frequencies = frequencies;
        this.levelReport = levelReport;
        this.positions = positions;
        this.levelStates = levelStates;
        this.map_number = map_number;
    }
}

[Serializable]
public class TotalResultObject
{
    public String Q1;
    public String Q2;
    public String Q3;
    public String Q4;
    public String Q5;
    public String Q6;
    public String Q7;
    public String Q8;
    public String Q9;
    public String Q10;

    public List<HumanResultObject> results;

    public TotalResultObject(String Q1, String Q2, String Q3, String Q4, String Q5, String Q6, String Q7, String Q8, String Q9, String Q10, List<HumanResultObject> results)
    {
        this.Q1 = Q1;
        this.Q2 = Q2;
        this.Q3 = Q3;
        this.Q4 = Q4;
        this.Q5 = Q5;
        this.Q6 = Q6;
        this.Q7 = Q7;
        this.Q8 = Q8;
        this.Q9 = Q9;
        this.Q10 = Q10;
        this.results = results;
    }

    public TotalResultObject(List<HumanResultObject> results)
    {
        this.results = results;
    }
}