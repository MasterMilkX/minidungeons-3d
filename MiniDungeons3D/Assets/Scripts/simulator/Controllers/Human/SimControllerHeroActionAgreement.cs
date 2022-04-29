using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class AgreementObject
{
    public UtilityFunctions utilityFunction { get; set; }
    public SimHeroAction playerAction { get; set; }
    public SimLevel level { get; set; }

    public string title { get; set; }

    public AgreementObject(UtilityFunctions utilityFunction, SimHeroAction playerAction, SimLevel level, string title)
    {
        this.level = level.Clone();
        this.playerAction = playerAction;
        this.utilityFunction = utilityFunction;
        this.title = title;
    }
}
public class SimControllerHeroActionAgreement : SimControllerHero
{
    public static int JavelinIssues = 0;
    public bool _hasInput;

    Func<SimLevel, double> _utilityFunction;

    public List<SimPoint> positions { get; set; }
    public List<string> ascii_maps { get; set; }
    public SimControllerHeroActionAgreement(List<SimPoint> positions, List<string> ascii_maps)
    {
        this.positions = positions;
        this.ascii_maps = ascii_maps;
    }
    public override MapReport PlayLevel(string levelString, UtilityFunctions utilityFunction, int msPerMove, int maxTurns)
    {
        SimUtilityCalculator suc = new SimUtilityCalculator();
        _utilityFunction = suc.GetUtilityFunction(utilityFunction);
        var level = new SimLevel(levelString);
        var states = new List<string>();
        states.Add(level.ToAsciiMap());

        int counter = 0;

        var stopwatch = new System.Diagnostics.Stopwatch();
        var playReport = new MapReport();
        playReport.startLocation = level.SimHero.Point;
        stopwatch.Start();
        List<SimHeroAction> actions = ExtractActions(level);
        int rAgreement = 0;
        int tcAgreement = 0;
        int mkAgreement = 0;


        int current_action_p = 0;
        while (level.SimLevelState == SimLevelState.Playing)
        {
            SimHeroAction nextAction = actions[current_action_p];
            // Get agreement

            int rVal = 0;
            int tcVal = 0;
            int mkVal = 0;
            Thread rThread = new Thread(() => {
                rVal = RunAgreement(new AgreementObject(UtilityFunctions.Runner, nextAction, level, "Runner"));
            });
            rThread.Start();
            Thread tcThread = new Thread(() =>
            {
                tcVal = RunAgreement(new AgreementObject(UtilityFunctions.TreasureCollector, nextAction, level, "Treasure Collector"));
            });
            tcThread.Start();
            Thread mkThread = new Thread(() => {
                mkVal = RunAgreement(new AgreementObject(UtilityFunctions.MonsterKiller, nextAction, level, "Monster Killer"));
            });
            mkThread.Start();


            rThread.Join();
            tcThread.Join();
            mkThread.Join();

            rAgreement += rVal;
            tcAgreement += tcVal;
            mkAgreement += mkVal;

            // Run action
            level.RunTurn(nextAction);
            states.Add(nextAction + "\n" + level.ToAsciiMap());
            
            counter++;
            current_action_p += 1;


        }

        stopwatch.Stop();
        float timeTaken = (float)stopwatch.ElapsedMilliseconds / 1000;
        Console.WriteLine(timeTaken + " seconds");

        playReport.LevelReport = LevelReport(GetType().Name, _utilityFunction, _utilityFunction(level), timeTaken, counter, level);
        playReport.ActionReport = actions;
        playReport.StateReport = states;
        playReport.Positions = positions;
        playReport.Mechanics = level.Logger.Mechanics;
        playReport.Agreement = new Dictionary<string, int>()
        {
            {"R", rAgreement },
            {"TC", tcAgreement },
            {"MK", mkAgreement },
            {"Total", counter }
        };
        return playReport;
    }

    public List<SimHeroAction> ExtractActions(SimLevel level)
    {
        List<SimHeroAction> actions = new List<SimHeroAction>();

        int current_p = 0;
        this.positions.Insert(0, level.SimHero.Point);
        SimPoint current_point = level.SimHero.Point;
        SimPoint next_point = level.SimHero.Point;
        SimHeroAction action = new SimHeroAction();
        SimLevel future = level.Clone();

        while (current_p + 1 < this.positions.Count)
        {
            current_point = this.positions[current_p];
            next_point = this.positions[current_p + 1];
            SimPoint diff_point = next_point - current_point;
            if (!current_point.Equals(this.positions[current_p]))
            {
                Console.WriteLine("here");
            }
            if (diff_point.Equals(SimPoint.North) || diff_point.Equals(SimPoint.South) || diff_point.Equals(SimPoint.East) || diff_point.Equals(SimPoint.West)){
                // We went north, south, east, or west
                action = new SimHeroAction(HeroActionTypes.Move, diff_point);
            } else if (!diff_point.Equals(SimPoint.Zero)){
                // not any of the cardinal directions but not staying still either == we went through a portal
                // to handle this, look north south east or west and wherever the portal is, move the player there. The system will portal the player to the real location in the next update
                bool break_flag = false;
                foreach (SimPoint direction in new SimPoint[] { SimPoint.North, SimPoint.South, SimPoint.East, SimPoint.West })
                {
                    SimPoint potential_loc = current_point + direction;
                    foreach (SimPortal portal in level.Portals)
                    {
                        if (portal.Point.Equals(potential_loc))
                        {
                            action = new SimHeroAction(HeroActionTypes.Move, direction);
                            break_flag = true;
                            break;
                        }
                    }
                    if (break_flag)
                        break;
                }
            } else
            {
                if (future.GetPossibleJavelinTargets().Count == 1)
                {
                    action = new SimHeroAction(HeroActionTypes.JavelinThrow, future.GetPossibleJavelinTargets()[0]);
                }
                else
                {
                    SimControllerHeroActionAgreement.JavelinIssues += 1;
                    int counter = 0;
                    foreach (SimPoint target in future.GetPossibleJavelinTargets())
                    {
                        SimHeroAction potential_action = new SimHeroAction(HeroActionTypes.JavelinThrow, target);
                        SimLevel potential_future = new SimLevel(future);
                        potential_future.RunTurn(potential_action);
                        string potential_ascii = potential_future.ToAsciiMap();
                        List<string> state_strings = new List<string>(potential_ascii.Split('\n'));
                        state_strings.RemoveAt(0);
                        potential_ascii = String.Join("\n", state_strings);
                        if (this.ascii_maps[current_p].Equals(potential_ascii))
                        {
                            action = potential_action;
                            counter += 1;
                        }

                    }

                    if (counter > 1)
                    {
                        // crap, we have multiple states that could be in the future, try again, this time go 2 states forward
                        foreach (SimPoint target in future.GetPossibleJavelinTargets())
                        {
                            SimHeroAction potential_action = new SimHeroAction(HeroActionTypes.JavelinThrow, target);
                            SimLevel potential_future = new SimLevel(future);
                            potential_future.RunTurn(potential_action);
                            SimPoint current_point_f = this.positions[current_p+1];
                            SimPoint next_point_f = this.positions[current_p + 2];
                            SimPoint diff_point_f = next_point_f - current_point_f;
                            // hope and pray there isnt a portal
                            potential_future.RunTurn(new SimHeroAction(HeroActionTypes.Move, diff_point_f));
                            string potential_ascii = potential_future.ToAsciiMap();
                            List<string> state_strings = new List<string>(potential_ascii.Split('\n'));
                            state_strings.RemoveAt(0);
                            potential_ascii = String.Join("\n", state_strings);
                            if (this.ascii_maps[current_p].Equals(potential_ascii))
                            {
                                action = potential_action;
                                counter += 1;
                            }

                        }
                    }
                    
                }
            }

            future.RunTurn(action);
            //Console.WriteLine(action);
            //Console.WriteLine(future.ToAsciiMap());
            actions.Add(action);
            current_p += 1;
        }





        return actions;
    }
    public override SimHeroAction NextAction(SimLevel level, int msPerMove)
    {
        return NextAction(level);
    }


    public int RunAgreement(AgreementObject obj)
    {
        SimLevel level = obj.level;
        SimHeroAction playerAction = obj.playerAction;
        UtilityFunctions function = obj.utilityFunction;
        int agreement = 0;

        SimControllerHeroAStar agent = new SimControllerHeroAStar();
        SimLevel clone = level.Clone();

        SimHeroAction agentAction = agent.NextAction(clone, 1000, function);
        if (agentAction.ActionType == playerAction.ActionType && agentAction.DirectionOrTarget == playerAction.DirectionOrTarget)
        {
            agreement = 1;
        }

        return agreement;
    }
    public SimHeroAction NextAction(SimLevel level)
    {
        var result = new SimHeroAction(HeroActionTypes.Move, SimPoint.Zero);

        SimLevel rootState = level.Clone();

        List<SimHeroAction> possibleActions = rootState.GetPossibleHeroActions();
        var oneStepFutures = new SimLevel[possibleActions.Count];

        var futureUtilities = new double[possibleActions.Count];
        double maxFutureUtility = double.MinValue;
        int bestUtilityIndex = -1;

        for (int future = 0; future < oneStepFutures.Length; future++)
        {
            oneStepFutures[future] = rootState.Clone();
            oneStepFutures[future].RunTurn(possibleActions[future]);
            futureUtilities[future] = _utilityFunction(oneStepFutures[future]);
            if (futureUtilities[future] > maxFutureUtility)
            {
                maxFutureUtility = futureUtilities[future];
                bestUtilityIndex = future;
            }
        }

        result = possibleActions[bestUtilityIndex];

        return result;
    }
}