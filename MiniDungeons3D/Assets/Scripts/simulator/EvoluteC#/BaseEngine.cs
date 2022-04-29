using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
//using System.Windows;
using System.Threading;

namespace TreeLanguageEvolute
{
    /// <summary>
    /// The engine class is the main class of the Genetic Programming library called Evolute.
    /// The library allows the user to get an outputs of programs, and the program themselves,
    /// And calcualte a fitness for each program. Then, the algorithm will take 
    /// The fitness that was given from the user and generate programs that are closer
    /// To the wanted fitness.
    /// </summary>
    public abstract class BaseEngine
    {
        #region Defines

        public enum FitnessGoalEnum { MIN_FITNESS, MAX_FITNESS }; // The fitness goal,
        // Like, where you want the fitness to go to - higher fitness? or lower fitness?

        #endregion

        #region Data Members

        protected BaseProgram[][] m_arPopulation; // The array of the current population of programs
        protected BaseProgram[][] m_arMatingPool; // The array of the mating pool of programs
        protected FunctionType[] m_arrFunctions; // The array of functions that can be used
        // To develop the trees of code
        protected ValueType[] m_arrValues; // The array of variables that can be used
        // To develop the trees of code
        protected List<string> m_arrstrVariables; // The array of possible variables
        protected object[] m_arrAllTypes; // Array that combines both values and functions
        public delegate void GenerationIsCompleteHandler(Statistics stsStatistics, BaseEngine sender);
        public event GenerationIsCompleteHandler GenerationIsCompleteEvent;
        // Invoked when a generation is complete, and provides statistics for the generation,
        // And a way to get the minimal and maximal (measured by fitness) program from the
        // Population.
        protected int m_nNumOfPrograms; // The amount of population
        protected int m_nNumOfProgramsInIsland;
        protected int m_nNumOfGenerations; // The number of generations to run the algorithm
        protected int m_nNumOfIslands; // The number of islands in the program.
        protected int m_nNumOfMigrationsPerMigrationGeneration; // The number of migrations that happen in the migration generation.
        protected int m_nOnceInHowManyGenerationsPerformMigrationGeneration; // Once in how many generations, perform the migration generation.
        protected int m_nNumOfResults; // The number of roots in a program
        protected int m_nGenerationNum; // The current generation number

        protected int m_nOverselection; // The number of individuals to select
        // From when performing selection in the algorithm
        protected FitnessGoalEnum m_fitFitnessGoal; // The fitness goal
        // (maximal fitness or minimal fitness)
        protected float m_fChanceForCrossover; // A value between 0 to 1 which means
        // What chance will there be a crossover.
        protected float m_fSaveTopIndividuals; // A value between 0 to 1 which means
        // How much precentage of individuals whose fitness is the best, will be 
        // Automatically transferred to the next generation.
        protected bool m_bSaveCrossoverOnFitnessChange; // Boolean if to perform 
        // A fitness check before saving a crossover, so that it will be saved
        // Only when the fitness of the son changes considerably.
        protected float m_fFitnessPrecentageBetterAfterCrossover; // A precentage from
        // 0 to 1 which means in what amount the fitness needs to change in order
        // To save children from a crossover.
        protected int m_nNumberOfThreads; // The number of threads that is used to 
        // Execute the programs
        protected Thread m_trdMainThread; // The thread that runs the evolution, in case that the
        // User wants to run the entire algorithm on a seperate thread


        // Change done in 3.6.2011 : the chance that the evolution would always take the better individuals
        // When in a tournament selection. If the chance is not 1.00, then sometimes the worse individuals
        // Would be selected instead.
        protected float m_dAlwaysTakeBetterIndividualsAmount = 1.0f;

        // The chance to perform mutation (to mutate someone that's already in the mating pool).
        protected float m_dMutationChance = 0.1f;
        protected bool m_fOnlyBetterFitnessIslands = false;

        // Enable parsimony pressure using double tournament method to select first the best fitnesses, 
        // And then to select the lightweight programs.
        protected bool m_fParsimonyPressureEnabled;

        // The desired increase in whole population program size.
        // For an example - if the multiplier is 0.1, then if in 
        // Generation 0 there was size "10" in the population, then in 
        // Generation 1 there would be max 11 nodes.
        // If that's not the case, then parsimony pressure will be activated until
        // The linearity is achieved.
        protected float m_dParsimonyPressureSizeIncrementMultiplier;

        protected int m_nPrasimonyPressure_NodesAtGeneration0;

        // An integer, that is responsible for determining once in how many generations, to
        // Re-evaluate top performing individuals, if the SaveTopIndividuals option is set.
        protected int m_nOnceInHowManyGenerationsToReEvaluateTopIndividuals;

        protected float[] m_arFitnessesForOnlyBetterFitnessMechanism;

        #endregion

        #region Properties

        /// <summary>
        /// The set functions property allows the user to set the random functions that
        /// May (or may not) will be appearing in the trees, when they initially randomized.
        /// </summary>
        public FunctionType[] SetFunctions
        {
            set
            {
                this.m_arrFunctions = value;
                this.BuildAllTypesArray();
            }
        }

        /// <summary>
        /// The set values property allows the user to set the random values that
        /// May (or may not) will be appearing in the trees, when they initially randomized.
        /// </summary>
        public ValueType[] SetValues
        {
            set
            {
                this.m_arrValues = value;
                this.BuildAllTypesArray();
            }
        }

        /// <summary>
        /// The fitness goal to run the algorithm for (for an example, minimal fitness
        /// Is used to run the algorithm towards minimal fitness)
        /// Maximal fitness is used to run the algorithm towards maximal fitness.
        /// </summary>
        public FitnessGoalEnum FitnessGoal
        {
            set
            {
                this.m_fitFitnessGoal = value;
            }
        }

        /// <summary>
        // A value between 0 to 1 which means
        // How much precentage of individuals whose fitness is the best, will be 
        // Automatically transferred to the next generation.
        /// </summary>
        public float SaveTopIndividuals
        {
            set
            {
                this.m_fSaveTopIndividuals = value;
            }
        }

        /// <summary>
        /// A property that once it is set, the crossover will only be saved
        /// If the produced children fitness has changed by a considerable amount.
        /// For an example, if this property is 0.3, then the crossover will
        /// Only be saved when the fitness is 130% of the original parent.
        /// For best performance, this value should be between 0.01 to 0.1
        /// </summary>
        public float ApplyCrossoverOnlyWithFitnessChange
        {
            set
            {
                this.m_bSaveCrossoverOnFitnessChange = true;
                this.m_fFitnessPrecentageBetterAfterCrossover = value;
            }
        }


        /// <summary>
        /// Get the most updated population of programs so far
        /// </summary>
        public BaseProgram[][] Population
        {
            get
            {
                return (this.m_arPopulation);
            }
        }

        /// <summary>
        /// Get the generation number since the Engine was created and was ran.
        /// </summary>
        public int GenerationNum
        {
            get
            {
                return (this.m_nGenerationNum);
            }
        }

        /// <summary>
        /// Set the number of programs in the population
        /// </summary>
        public int NumberOfPrograms
        {
            set
            {
                this.m_nNumOfPrograms = value;
                this.m_nNumOfProgramsInIsland = this.m_nNumOfPrograms / this.m_nNumOfIslands;
            }
        }

        public int NumberOfIslands
        {
            set
            {
                this.m_nNumOfIslands = value;
                this.m_nNumOfProgramsInIsland = this.m_nNumOfPrograms / this.m_nNumOfIslands;
            }
            get
            {
                return this.m_nNumOfIslands;
            }
        }

        /// <summary>
        /// Sets the overselection that is used by the tournament selection.
        /// The larger this number is, the more programs have to "compete" between
        /// Themselves in order to be selected for the genetic operations.
        /// By competing, it means the program that has the best fitness wins.
        /// For an example : If overselection is 1, then a total random program is
        /// Selected for the genetic operations.
        /// If overselection is 2, two random programs compete between themselves 
        /// And the winning program goes to the genetic operations.
        /// </summary>
        public int Overselection
        {
            set
            {
                this.m_nOverselection = value;
            }
        }

        /// <summary>
        /// Sets the chance for crossover that will be taken into consideration
        /// When the algorithm chooses the genetic operations.
        /// Since in version 0.1 the genetic operations are either selection or crossover,
        /// Only those 2 operations can be randomized.
        /// For an example : if the chance is 0.9, then 90% of the time a 
        /// Crossover operation will be selected, and 10% of the time a cloning
        /// Operation will be selected.
        /// </summary>
        public float ChanceForCrossover
        {
            set
            {
                this.m_fChanceForCrossover = value;
            }
        }

        /// <summary>
        /// Sets the number of threads that will be run when evaluating programs.
        /// While this option did not yet checked much, 
        /// If you need more speed, it can be worth to set this to the number of cores
        /// You have in your computer.
        /// </summary>
        public int NumberOfThreads
        {
            set
            {
                this.m_nNumberOfThreads = value;
            }
        }

        public int NumberOfResults
        {
            set
            {
                this.m_nNumOfResults = value;
            }
        }

        /// <summary>
        /// When migrating from island to island, only the worst islands will send their individuals
        /// To the better islands, the better islands will always get their migrations from worst islands 
        /// Than their own. This way each island can learn it's own way (and not get told the solution)
        /// And to increase the diversity in the islands.
        /// </summary>
        public bool OnlyBetterFitnessIslands
        {
            set
            {
                this.m_fOnlyBetterFitnessIslands = value;
            }
        }

        /// <summary>
        /// Number of migrations that occur in the migration generation.
        /// </summary>
        public int MigrationsPerMigrationGeneration
        {
            set
            {
                this.m_nNumOfMigrationsPerMigrationGeneration = value;
            }
        }

        /// <summary>
        /// How frequent (once in how many generations) the migration generation will occur.
        /// </summary>
        public int OnceInHowManyGenerationsPerformMigrationGeneration
        {
            set
            {
                this.m_nOnceInHowManyGenerationsPerformMigrationGeneration = value;
            }
        }

        /// <summary>
        /// The chance to perform mutation.
        /// </summary>
        public float MutationChance
        {
            set
            {
                this.m_dMutationChance = value;
            }
        }

        /// <summary>
        /// The chance to take better individuals (between 0 to 1 floating point number)
        /// for an exmaple, if the number is 0.6
        /// Then there is 60% chance that while doing selection, a better individual will be selected 
        /// And 40% chance that the worst individual will be selected.
        /// This control can give more chance for the worst programs to prove themselves.
        /// </summary>
        public float PrecentageToTakeBetterIndividuals
        {
            set
            {
                this.m_dAlwaysTakeBetterIndividualsAmount = value;
            }
        }

        /// <summary>
        /// This control enables Double-tournament selection.
        /// In the first round, the better fitness individual is selected from the population.
        /// In the second round, the lightweight indiidual (who is already relatively good in his fitness)
        /// Is selected from the two good-fitness individuals.
        /// This control is important for saving CPU time and combatting bloat.
        /// </summary>
        public float EnableParsimonyPressure
        {
            set
            {
                this.m_dParsimonyPressureSizeIncrementMultiplier = value;
                this.m_fParsimonyPressureEnabled = true;
            }
        }

        /// <summary>
        /// An integer, that is responsible for determining once in how many generations, to
        /// Re-evaluate top performing individuals, if the SaveTopIndividuals option is set.
        /// A lower value means that those top-performing individuals WILL be saved, but they will
        /// Be re-evaluated more often. A higher value means that those top-performing individuals
        /// Will be re-evaluated less often. 
        /// In my testing results, I found that evaluating the top performing individuals less often
        /// Means that a possible good solution has more time to get better and not get thrown away
        /// The moment it is performing less good, when dealing with random environments,
        /// And can really help in the evolution process.
        /// However, when working with highly dynamic fitnesses, that is dependent on other factors
        /// Rather than a static environment, a lower value is recommended.
        /// The default is 20. This default value will probably not be very good for co-evolution,
        /// Or will require a coevolutionary process design change.
        /// </summary>
        public int OnceInHowManyGenerationsTopIndividualsShouldBeEvaluated
        {
            set
            {
                this.m_nOnceInHowManyGenerationsToReEvaluateTopIndividuals = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// A constructor for the Engine. Set's up a default values for the algorithm itself.
        /// </summary>
        public BaseEngine()
        {
            this.m_nNumOfPrograms = 1000;
            this.m_nNumOfGenerations = 10;
            this.m_nNumOfResults = 1;
            this.m_nOverselection = 2;
            this.m_fChanceForCrossover = 0.9F;
            this.m_fSaveTopIndividuals = 0.01F;
            this.m_arrFunctions = new FunctionType[] { new Add(), new Multiply() };
            this.m_arrValues = new ValueType[] { new RandomMacro() };
            this.m_arrstrVariables = new List<string>();
            this.m_fitFitnessGoal = FitnessGoalEnum.MIN_FITNESS;
            this.m_bSaveCrossoverOnFitnessChange = false;
            this.m_fFitnessPrecentageBetterAfterCrossover = 0;
            this.m_nNumberOfThreads = 1;
            this.m_nNumOfIslands = 10;
            this.m_nNumOfProgramsInIsland = m_nNumOfPrograms / m_nNumOfIslands;
            this.MigrationsPerMigrationGeneration = this.m_nNumOfIslands;
            this.OnceInHowManyGenerationsPerformMigrationGeneration = 1;
            this.OnceInHowManyGenerationsTopIndividualsShouldBeEvaluated = 20;
        }

        #endregion

        #region Other methods

        /// <summary>
        /// Builds the "All Types" array - which combines both the values array
        /// And the functions array. It's just takes both array and combines them 
        /// Together into one array.
        /// </summary>
        protected void BuildAllTypesArray()
        {
            this.m_arrAllTypes =
                new object[this.m_arrFunctions.Length + this.m_arrValues.Length];

            int nTypeIndex = 0;
            foreach (Function typFunction in this.m_arrFunctions)
            {
                this.m_arrAllTypes[nTypeIndex] = typFunction;
                nTypeIndex++;
            }
            foreach (ValueType typVariable in this.m_arrValues)
            {
                this.m_arrAllTypes[nTypeIndex] = typVariable;
                nTypeIndex++;
            }
        }

        /// <summary>
        /// Declares a new variable (adds it to the list of variables)
        /// </summary>
        /// <param name="strName">The variable name</param>
        public void DeclareVariable(string strName)
        {
            this.m_arrstrVariables.Add(strName);
        }

        /// <summary>
        /// The functions initialises the population, creates the population array,
        /// And randomizing it.
        /// </summary>
        protected void InitPopulation()
        {
            // Creating the population array
            this.m_arPopulation = new BaseProgram[this.m_nNumOfIslands][];
            this.m_arMatingPool = new BaseProgram[this.m_nNumOfIslands][];

            // Creating the population for each island
            for (int nIslandIndex = 0;
                 nIslandIndex < this.m_arPopulation.Length;
                 nIslandIndex++)
            {
                this.m_arPopulation[nIslandIndex] = new BaseProgram[this.m_nNumOfPrograms / this.m_nNumOfIslands];

                // Also create the mating pool array for later.
                this.m_arMatingPool[nIslandIndex] = new BaseProgram[this.m_nNumOfPrograms / this.m_nNumOfIslands];

                // For each inidivual in the population, create a random tree.
                for (int nProgramIndex = 0;
                     nProgramIndex < this.m_arPopulation[nIslandIndex].Length;
                     nProgramIndex++)
                {
                    this.m_arPopulation[nIslandIndex][nProgramIndex] = this.CreateRandomProgram();
                }
            }
        }

        protected virtual void PerformAdditionalOperations(BaseProgram progProgram)
        {

        }

        /// <summary>
        /// The function expects that the 
        /// </summary>
        /// <returns></returns>
        protected BaseProgram PerformTournamentSelectionByFitness(BaseProgram[] arIsland)
        {
            ImprovedRandom rndRandom = GlobalRandom.m_rndRandom;

            // Get a random program from the main population, just to be sampled once.
            // And call it "the best program", later, get other random programs,
            // And test which program is better - the one who supposed to be called
            // "The best program"? or the other random program that was pulled
            // Randomly from the population. 
            int nProgramIndex = rndRandom.Next(m_nNumOfProgramsInIsland);
            BaseProgram progBestProgram = arIsland[nProgramIndex];

            // Because we only taken one program and called it the best, we only
            // Examined one single program.
            int nExaminedIndividuals = 1;

            // Examine other programs until we reached the wanted programs to 
            // Be examined for selection
            while (nExaminedIndividuals < this.m_nOverselection)
            {
                // Get a random program to compete against the best program
                int nCompetitorIndex = rndRandom.Next(m_nNumOfProgramsInIsland);
                BaseProgram progCompetitor = arIsland[nCompetitorIndex];

                float dChanceToTakeBetter = (float)rndRandom.NextDouble();

                // If the goal is maximum fitness, then the maximal progrma is better.
                // Otherwise, the minimal program is better.
                if (this.m_fitFitnessGoal == FitnessGoalEnum.MAX_FITNESS)
                {
                    if ((progBestProgram.Fitness < progCompetitor.Fitness) &&
                        (dChanceToTakeBetter <= m_dAlwaysTakeBetterIndividualsAmount))
                    {
                        progBestProgram = progCompetitor;
                    }
                }
                else
                {
                    if ((progBestProgram.m_fFitness > progCompetitor.m_fFitness) &&
                        (dChanceToTakeBetter <= m_dAlwaysTakeBetterIndividualsAmount))
                    {
                        progBestProgram = progCompetitor;
                    }
                }

                nExaminedIndividuals++;
            }

            return progBestProgram;
        }

        /// <summary>
        /// The function selects the mating pool
        /// The mating pool is an array of relatively good individuals from the 
        /// Current generation.
        /// </summary>
        protected void SelectTheMatingPool()
        {
            // Get the global random...
            ImprovedRandom rndRandom = GlobalRandom.m_rndRandom;

            // Check parsimony pressure difference between desired nodes count and actual nodes count.
            Statistics sStatistics = new Statistics(this);
            int nCurrNodesCount = sStatistics.TotalNodes;
            int nExpectedNodesCount = (int)(m_nPrasimonyPressure_NodesAtGeneration0 * (1 + m_dParsimonyPressureSizeIncrementMultiplier * m_nGenerationNum));
            
            // This is the chance to apply parsimony pressure.
            // If this is lower than 1, then we will start to apply parsimony pressure.
            // For an exmaple, if this is 0.1, then the chance not to apply parsimony pressure will be only 10%.
            // And 90% of the programs will get parsimony pressure.
            float dChanceToNotApplyParsimonyPressure = (float)nExpectedNodesCount / nCurrNodesCount;

            for (int nIslandIndex = 0;
                 nIslandIndex < this.m_nNumOfIslands;
                 nIslandIndex++)
            {
                // Do this for every program in the mating pool
                for (int nProgramIndexInMatingPool = 0;
                     nProgramIndexInMatingPool < this.m_arMatingPool[nIslandIndex].Length;
                     nProgramIndexInMatingPool++)
                {
                    BaseProgram progBestProgramFromTournamentSelection1 = PerformTournamentSelectionByFitness(m_arPopulation[nIslandIndex]);
                    BaseProgram progBestProgramFromTournamentSelection2 = PerformTournamentSelectionByFitness(m_arPopulation[nIslandIndex]);
                    
                    BaseProgram progBestOverallProgramFromTournamentSelection = progBestProgramFromTournamentSelection1;

                    if (m_fParsimonyPressureEnabled == true)
                    {
                        float dChance = rndRandom.NextDouble();
                        if (dChance >= dChanceToNotApplyParsimonyPressure)
                        {
                            if (progBestProgramFromTournamentSelection2.Size < progBestProgramFromTournamentSelection1.Size)
                            {
                                progBestOverallProgramFromTournamentSelection = progBestProgramFromTournamentSelection2;
                            }
                        }
                    }

                    // Clone the selected program for reproduction in the mating pool
                    // From the population.
                    this.m_arMatingPool[nIslandIndex][nProgramIndexInMatingPool] =
                        (BaseProgram)progBestOverallProgramFromTournamentSelection.Clone();
                }
            }
        }

        /// <summary>
        /// The function creates the next generation.
        /// It takes the mating pool, which consists of relatively good programs,
        /// And performs genetic operations on them, such as cloning or crossover.
        /// </summary>
        protected void CreateNextGeneration()
        {
            ImprovedRandom rndRandom = GlobalRandom.m_rndRandom;

            // Perform migration on the programs in the mating pool, so that
            // These new programs (from other islands) can be later recombined with other programs.
            this.PerformMigration();

            // Perform mutation on the programs in the mating pool, so that 
            // these mutated programs can be later recombined with other programs.
            this.PerformMutation();

            // Sort the population in all the islands,
            // In order to allow performing elitist selection later and saving the top individuals.
            for (int nIslandIndex = 0;
                nIslandIndex < this.m_arPopulation.Length;
                nIslandIndex++)
            {
                // Sort the population by fitness. The top individuals will have the
                // Best fitness
                if (this.m_fitFitnessGoal == FitnessGoalEnum.MIN_FITNESS)
                {
                    Array.Sort(this.m_arPopulation[nIslandIndex],
                        delegate(BaseProgram progOne, BaseProgram progTwo)
                        {
                            return (progOne.Fitness.CompareTo(progTwo.Fitness));
                        }
                    );
                }
                else
                {
                    Array.Sort(this.m_arPopulation[nIslandIndex],
                        delegate(BaseProgram progOne, BaseProgram progTwo)
                        {
                            int nCompareResult =
                                progOne.Fitness.CompareTo(progTwo.Fitness);
                            nCompareResult = -nCompareResult;
                            return (nCompareResult);
                        }
                    );
                }
            }

            for (int nIslandIndex = 0;
                 nIslandIndex < this.m_arPopulation.Length;
                 nIslandIndex++)
            {
                // Save the top individuals, by just skipping them (and thus leaving them
                // In the next population). Because the array is sorted so that the top
                // Individuals are at the start, when we put programs in the population,
                // They will still be there.
                int nSaveTopIndividuals = (int)(this.m_fSaveTopIndividuals *
                    this.m_nNumOfProgramsInIsland);

                // Do this for every program that supposed to be in the next population.
                // Which means, fill the population array with random programs from the
                // Mating pool that undergo some genetic operations.
                for (int nProgramIndexInPopulation = nSaveTopIndividuals;
                     nProgramIndexInPopulation < this.m_arPopulation[nIslandIndex].Length;
                     nProgramIndexInPopulation++)
                {
                    // Randomize some random number to choose which genetic operation
                    // Will be used.
                    float nLuck = (float)GlobalRandom.m_rndRandom.NextDouble();

                    // If the random number that was generated is lower than the chance
                    // For crossover, then it means that a crossover operation is selected.
                    // But only perform it if we have enough individuals to perform it
                    // Upon in the population array.
                    if ((nLuck < this.m_fChanceForCrossover) &&
                        ((this.m_arPopulation[nIslandIndex].Length - nProgramIndexInPopulation) > 2))
                    {
                        // Save the old programs from the previous generation for later
                        // Processing, if even neccesary.
                        int nRandomProgramOne = rndRandom.Next(this.m_arMatingPool[nIslandIndex].Length);
                        int nRandomProgramTwo = rndRandom.Next(this.m_arMatingPool[nIslandIndex].Length);
                        BaseProgram progOldOne =
                            this.m_arMatingPool[nIslandIndex][nRandomProgramOne];
                        BaseProgram progOldTwo =
                            this.m_arMatingPool[nIslandIndex][nRandomProgramTwo];

                        // Crossover the programs from the mating pool
                        this.Crossover(progOldOne, progOldTwo,
                                   out this.m_arPopulation[nIslandIndex][nProgramIndexInPopulation],
                                   out this.m_arPopulation[nIslandIndex][nProgramIndexInPopulation + 1]);

                        // Check if the fitness was better after the crossover.
                        // Revert to the original individuals if it's fitness is not high 
                        // Enough.
                        // TODO : can be optimised here instead of cloning the programs.
                        if (this.m_bSaveCrossoverOnFitnessChange)
                        {
                            FitnessCheckAfterCrossover(nIslandIndex,
                                nProgramIndexInPopulation, progOldOne, progOldTwo);
                        }

                        // Advance a position in the population array and the mating pool
                        // Array, cause we performed an operation on two programs
                        nProgramIndexInPopulation++;
                    }
                    else
                    {
                        // If the chance wasn't high enough then a cloning operation is
                        // Chosen. Just clone the program towards to the next population
                        int nRandomProgramIndex = rndRandom.Next(this.m_arMatingPool[nIslandIndex].Length);
                        this.m_arPopulation[nIslandIndex][nProgramIndexInPopulation] =
                            (BaseProgram)this.m_arMatingPool[nIslandIndex][nRandomProgramIndex].Clone();
                    }
                }

                // Perform additional operations, such as mutation and etc,
                // On the individuals that were not the best.
                for (int nProgramIndexInPopulation = nSaveTopIndividuals;
                    nProgramIndexInPopulation < this.m_arPopulation[nIslandIndex].Length;
                    nProgramIndexInPopulation++)
                {
                    // Perform additional operations on those that were not "saved" as top
                    // Individuals.
                    this.PerformAdditionalOperations(
                        this.m_arPopulation[nIslandIndex][nProgramIndexInPopulation]);
                }
            }
        }

        protected abstract void Crossover(BaseProgram progParentOne, BaseProgram progParentTwo,
                       out BaseProgram progSonOne, out BaseProgram progSonTwo);

        void DestroyPopulation()
        {
            // Once in 100 generations, save the top program,
            // And generate totally new programs.
            //ImprovedRandom rndRandom = GlobalRandom.m_rndRandom;
            if ((this.m_nGenerationNum % 1000) == 0)
            {
            //    int nPopulationToCopy = (this.m_nNumOfIslands * this.m_nNumOfPrograms) / 10;
            //    BaseProgram[] arrCopiedPrograms = new BaseProgram[nPopulationToCopy];

            //    for (int i = 0; i < nPopulationToCopy; i++)
            //    {
            //        int nRandomizedIsland = rndRandom.Next(this.m_arrPopulation.Length);
            //        int nRandomizedProgram = rndRandom.Next(this.m_arrPopulation[nRandomizedIsland].Length);
            //        arrCopiedPrograms[i] = this.m_arrPopulation[nRandomizedIsland][nRandomizedProgram];
            //    }

                Statistics stsStatistics = new Statistics(this);
                BaseProgram progBestProgram = stsStatistics.MinFitnessProgram;

                // Initialise totally different population.
                this.InitPopulation();

                this.Population[0][0] = progBestProgram;

            //    for (int i = 0; i < nPopulationToCopy; i++)
            //    {
            //        int nRandomizedIsland = rndRandom.Next(this.m_arrPopulation.Length);
            //        int nRandomizedProgram = rndRandom.Next(this.m_arrPopulation[nRandomizedIsland].Length);
            //        this.m_arrPopulation[nRandomizedIsland][nRandomizedProgram] = arrCopiedPrograms[i];
            //    }
            }
        }

        protected void InsertInitialParsimonyPressureNodesCount()
        {
            Statistics sStatistics = new Statistics(this);
            m_nPrasimonyPressure_NodesAtGeneration0 = sStatistics.TotalNodes;
        }

        protected void Evolute(object objNumOfGenerations)
        {
            int nNumOfGenerations = (int)objNumOfGenerations;

            // Initialises the population first with random trees
            this.InitPopulation();

            // Sets the current generation num as zero
            this.m_nGenerationNum = 0;

            // Check the population count, and insert it to the starting parsimony pressure.
            InsertInitialParsimonyPressureNodesCount();

            // Runs the algorithm for the wanted number of generations
            for (this.m_nGenerationNum = 0;
                 this.m_nGenerationNum < nNumOfGenerations;
                 this.m_nGenerationNum++)
            {
                this.RunCurrentGeneration();
                this.SelectTheMatingPool();
                this.CreateNextGeneration();
            }
        }

        protected abstract BaseProgram CreateRandomProgram();

        protected abstract void RunProgram(BaseProgram progProgram);

        /// <summary>
        /// A function which checks a fitness of the produced programs after a crossover,
        /// If the fitness that was produced was better than the original fitness.
        /// If the fitness that was produced wasn't better enough, then it 
        /// Reverts back the individuals from the original population.
        /// </summary>
        /// <param name="nProgramIndexInPopulation">
        /// The starting program index
        /// That we performed the crossover on
        /// </param>
        /// <param name="progOldOne">
        /// The original program in the son one place in the population array
        /// </param>
        /// <param name="progOldTwo">
        /// The original program in the son two place in the population array
        /// </param>
        protected void FitnessCheckAfterCrossover(int nIslandIndex, int nProgramIndexInPopulation,
            BaseProgram progOldOne, BaseProgram progOldTwo)
        {
            float fPreviousFitnessOne =
                this.m_arMatingPool[nIslandIndex][nProgramIndexInPopulation].Fitness;
            float fPreviousFitnessTwo =
                this.m_arMatingPool[nIslandIndex][nProgramIndexInPopulation + 1].Fitness;

            // Check for the produced sons fitness
            this.RunProgram(this.m_arPopulation[nIslandIndex][nProgramIndexInPopulation]);
            this.RunProgram(this.m_arPopulation[nIslandIndex][nProgramIndexInPopulation + 1]);

            float fCurrentFitnessOne =
                this.m_arPopulation[nIslandIndex][nProgramIndexInPopulation].Fitness;
            float fCurrentFitnessTwo =
                this.m_arPopulation[nIslandIndex][nProgramIndexInPopulation + 1].Fitness;

            float fWantedFitnessOneLower = fPreviousFitnessOne *
                (1 - this.m_fFitnessPrecentageBetterAfterCrossover);
            float fWantedFitnessOneHigher = fPreviousFitnessOne *
                this.m_fFitnessPrecentageBetterAfterCrossover;
            float fWantedFitnessTwoLower = fPreviousFitnessTwo *
               (1 - this.m_fFitnessPrecentageBetterAfterCrossover);
            float fWantedFitnessTwoHigher = fPreviousFitnessTwo *
                this.m_fFitnessPrecentageBetterAfterCrossover;

            // If the fitness has changed considerably, allow the crossover.
            if (!((fCurrentFitnessOne < fWantedFitnessOneLower) ||
                (fCurrentFitnessOne > fWantedFitnessOneHigher)))
            {
                //Revert back
                this.m_arPopulation[nIslandIndex][nProgramIndexInPopulation] =
                     progOldOne;
            }

            // If the fitness has changed considerably, allow the crossover.
            if (!((fCurrentFitnessTwo < fWantedFitnessTwoLower) ||
               (fCurrentFitnessTwo > fWantedFitnessTwoHigher)))
            {
                //Revert back
                this.m_arPopulation[nIslandIndex][nProgramIndexInPopulation + 1] =
                     progOldTwo;
            }

            /*
                        // If the current fitness is higher than the previous fitness
                        // By a considerable amount, then allow the fitness. 
                        // Otherwise, reverse the original individuals from the previous
                        // Generation.
                        if (this.m_fitFitnessGoal == FitnessGoalEnum.MIN_FITNESS)
                        {
                            // Parent and son one.
                            // The fitness should be below this value
                            float fWantedFitnessOne =
                                fPreviousFitnessOne *
                                (1 - this.m_fFitnessPrecentageBetterAfterCrossover);

                            // If the fitness is not below this value, then it's not good
                            // Enough. so revert back the old program.
                            if (!(fCurrentFitnessOne < fWantedFitnessOne))
                            {
                                 //Revert back
                                this.m_arrPopulation[nIslandIndex][nProgramIndexInPopulation] =
                                     progOldOne;
                            }

                            // Parent and son two.
                            // The fitness should be below this value
                            float fWantedFitnessTwo =
                                fPreviousFitnessTwo *
                                (1 - this.m_fFitnessPrecentageBetterAfterCrossover);

                            // If the fitness is not below this value, then it's not good
                            // Enough. so revert back the old program.
                            if (!(fCurrentFitnessTwo < fWantedFitnessTwo))
                            {
                                // Revert back
                                this.m_arrPopulation[nIslandIndex][nProgramIndexInPopulation + 1] =
                                    progOldTwo;
                            }
                        }
                        else
                        {
                            // Do the same thing for maximal fitness
                            // Parent and son one.
                            // The fitness should be above this value
                            float fWantedFitnessOne =
                                fPreviousFitnessOne *
                                    this.m_fFitnessPrecentageBetterAfterCrossover;

                            // If the fitness is not above this value, then it's not good
                            // Enough. so revert back the old program.
                            if (!(fCurrentFitnessOne > fWantedFitnessOne))
                            {
                                // Revert back
                                this.m_arrPopulation[nIslandIndex][nProgramIndexInPopulation] =
                                    progOldOne;
                            }

                            // Parent and son two.
                            // The fitness should be above this value
                            float fWantedFitnessTwo =
                                fPreviousFitnessTwo *
                                    this.m_fFitnessPrecentageBetterAfterCrossover;

                            // If the fitness is not above this value, then it's not good
                            // Enough. so revert back the old program.
                            if (!(fCurrentFitnessTwo > fWantedFitnessTwo))
                            {
                                // Revert back
                                this.m_arrPopulation[nIslandIndex][nProgramIndexInPopulation + 1] =
                                    progOldTwo;
                            }
                        }*/

        }

        protected float EvaulateIslandFitness(int nIslandIndex)
        {
            float dInitialFitness = m_arPopulation[nIslandIndex][0].Fitness;

            for (int i = 0; i < m_arPopulation[nIslandIndex].Length; i++)
            {
                if (m_fitFitnessGoal == FitnessGoalEnum.MIN_FITNESS)
                {
                    if (m_arPopulation[nIslandIndex][i].Fitness < dInitialFitness)
                    {
                        dInitialFitness = m_arPopulation[nIslandIndex][i].Fitness;
                    }
                }
                else
                {
                    if (m_arPopulation[nIslandIndex][i].Fitness > dInitialFitness)
                    {
                        dInitialFitness = m_arPopulation[nIslandIndex][i].Fitness;
                    }
                }
            }

            return dInitialFitness;
        }

        protected bool IsOtherIslandBetterFitness(int nMyIslandIndex, int nIslandIndex)
        {
            
            /* Changed in 19.2.2013 for a more balanced algorithm.
            float dMyFitness = EvaulateIslandFitness(nMyIslandIndex);
            float dOtherIslandFitness = EvaulateIslandFitness(nIslandIndex);
             * */

            float dMyFitness = m_arFitnessesForOnlyBetterFitnessMechanism[nMyIslandIndex];
            float dOtherIslandFitness = m_arFitnessesForOnlyBetterFitnessMechanism[nIslandIndex];

            if (m_fitFitnessGoal == FitnessGoalEnum.MIN_FITNESS)
            {
                return (dOtherIslandFitness < dMyFitness);
            }
            else
            {
                return (dOtherIslandFitness > dMyFitness);
            }
        }

        protected void PerformMigration()
        {
            ImprovedRandom rndRandom = GlobalRandom.m_rndRandom;

            // Once in 50 generations, update which islands have better fitness - for less noise
            // And more balanced rankings of islands.
            float dFirstIslandFitness;
            bool fFoundDifferentFitnessThanFirstIsland = false;
            bool fPerformMigration = true;
            if (((GenerationNum % 20) == 0) && (m_fOnlyBetterFitnessIslands == true))
            {
                m_arFitnessesForOnlyBetterFitnessMechanism = new float[m_nNumOfIslands];
                dFirstIslandFitness = EvaulateIslandFitness(0);
                for (int i = 0; i < m_nNumOfIslands; i++)
                {
                    m_arFitnessesForOnlyBetterFitnessMechanism[i] = EvaulateIslandFitness(i);
                    if (dFirstIslandFitness != m_arFitnessesForOnlyBetterFitnessMechanism[i])
                    {
                        fFoundDifferentFitnessThanFirstIsland = true;
                    }
                }

                // Make sure that all the islands DOESNT have the same fitness, otherwise we won't be able
                // To migrate anything with the "only better fitness islands" algorithm.
                // Since the randomized migration and search for different island will possibly get stuck in infinite loop.
                // Make sure that we have at least one island with different fitness, so that it won't happen.
                if (fFoundDifferentFitnessThanFirstIsland == false)
                {
                    if (m_arFitnessesForOnlyBetterFitnessMechanism.Length >= 2)
                    {
                        m_arFitnessesForOnlyBetterFitnessMechanism[0] = m_arFitnessesForOnlyBetterFitnessMechanism[1] + 1;
                    }
                    else
                    {
                        throw new Exception("Error: Trying to perform Only better fitness islands with only one island");
                    }
                }
            }

            if ((m_fOnlyBetterFitnessIslands == true) && (GenerationNum <= 20))
            {
                fPerformMigration = false;
            }

            if ((GenerationNum % m_nOnceInHowManyGenerationsPerformMigrationGeneration) != 0)
            {
                fPerformMigration = false;
            }

            if (fPerformMigration == true)
            {
                for (int nMigrations = 0; nMigrations < m_nNumOfMigrationsPerMigrationGeneration; nMigrations++)
                {
                    int nOriginIsland = 0;
                    int nDestinationIsland = 0;
                    int nSourceProgramIndex = 0;

                    if (m_fOnlyBetterFitnessIslands == false)
                    {
                        nOriginIsland = rndRandom.Next(m_nNumOfIslands);
                        nDestinationIsland = rndRandom.Next(m_nNumOfIslands);
                        nSourceProgramIndex = rndRandom.Next(m_nNumOfProgramsInIsland);
                    }
                    else
                    {
                        bool fFoundBetterFitnessIsland = false;
                        // Once in 1/100 migrations, there is a slim chance that a better island will send 
                        // An individual to a worse island, to avoid complete starvation of the evolution
                        // Process and to give low-life island some new "food".
                        float CHANCE_TO_SELECT_WORSE_ISLAND = 1 / (float)1000;
                        float dSelectWorseIslandAttempt = rndRandom.NextDouble();

                        while (fFoundBetterFitnessIsland == false)
                        {
                            nOriginIsland = rndRandom.Next(m_nNumOfIslands);
                            nDestinationIsland = rndRandom.Next(m_nNumOfIslands);
                            fFoundBetterFitnessIsland =
                                    IsOtherIslandBetterFitness(nOriginIsland, nDestinationIsland);

                            // If we select a worse island this time, then reverse the result of the boolean "found better island".
                            if (dSelectWorseIslandAttempt < CHANCE_TO_SELECT_WORSE_ISLAND)
                            {
                                fFoundBetterFitnessIsland = !fFoundBetterFitnessIsland;
                            }

                            nSourceProgramIndex = rndRandom.Next(m_nNumOfProgramsInIsland);
                        }
                    }

                    // Copy the program to the same location at the other island.
                    this.m_arMatingPool[nDestinationIsland][nSourceProgramIndex] = (BaseProgram)
                        this.m_arMatingPool[nOriginIsland][nSourceProgramIndex].Clone();
                }
            }
        }

        protected void PerformMutation()
        {
            ImprovedRandom rndRandom = GlobalRandom.m_rndRandom;

            for (int nIslandIndex = 0;
                nIslandIndex < m_nNumOfIslands;
                nIslandIndex++)
            {
                for (int nProgramIndexInPopulation = 0;
                     nProgramIndexInPopulation < m_nNumOfProgramsInIsland;
                     nProgramIndexInPopulation++)
                {
                    if (rndRandom.NextDouble() <= m_dMutationChance)
                    {
                        // Just create an entire new program.
                        this.m_arMatingPool[nIslandIndex][nProgramIndexInPopulation] = this.CreateRandomProgram();
                    }
                }
            }
        }

        /// <summary>
        /// Runs a mini-thread that is responsible for running a group of programs,
        /// The thread will run all the programs that of the provided list.
        /// </summary>
        /// <param name="data">
        /// A list containing all the BaseProgram to run.
        /// </param>
        protected void RunCurrentGenerationThread(object objListOfPrograms)
        {
            List<BaseProgram> lstProgramsToRun = (List<BaseProgram>)objListOfPrograms;
            for (int i = 0; i < lstProgramsToRun.Count; i++)
            {
                this.RunProgram(lstProgramsToRun[i]);
            }
        }

        /// <summary>
        /// Runs the current generation, invokes an event for the user to give fitness
        /// For the programs.
        /// </summary>
        protected void RunCurrentGeneration()
        {
            // Operate serveral threads, that each one is responsible for a different
            // Part of the population.

            // Construct the entire list of programs to run.
            List<BaseProgram> lstEntireBaseOfProgramsToRun = new List<BaseProgram>();
            int nSaveTopIndividuals = 0;
            if (m_nGenerationNum > 0) // It may interfere with co-evolution and dynamic fitness.
            {
                nSaveTopIndividuals = (int)(m_fSaveTopIndividuals * m_nNumOfProgramsInIsland);

                // If we need to re-evluate the top performing programs, so set the SaveTopIndividuals to 0
                // So that we will run them in the next code section.
                if ((m_nGenerationNum % m_nOnceInHowManyGenerationsToReEvaluateTopIndividuals) == 0)
                {
                    nSaveTopIndividuals = 0;
                }
            }

            for (int nIslandIndex = 0; nIslandIndex < m_nNumOfIslands; nIslandIndex++)
            {
                for (int nProgramIndex = nSaveTopIndividuals; nProgramIndex < m_nNumOfProgramsInIsland; nProgramIndex++)
                {
                    lstEntireBaseOfProgramsToRun.Add(m_arPopulation[nIslandIndex][nProgramIndex]);
                }
            }

            Thread[] arrThreads = new Thread[this.m_nNumberOfThreads];
            for (int nThreadNum = 0;
                 nThreadNum < this.m_nNumberOfThreads;
                 nThreadNum++)
            {
                // Construct the list of base programs for the specific thread to run.
                // Those base programs are extracted from the large list of base programs to run for the
                // Entire population.
                List<BaseProgram> lstProgramsToRunForThread = new List<BaseProgram>();
                int nThreadStartProgramIndex = (lstEntireBaseOfProgramsToRun.Count / this.m_nNumberOfThreads) * nThreadNum;
                int nThreadEndProgramIndex = nThreadStartProgramIndex + (lstEntireBaseOfProgramsToRun.Count / this.m_nNumberOfThreads);

                // If we are in the last thread, then set the end index as the end of the array, to prevent
                // Division remainder problems when dividng the number of programs in the number of threads.
                if (nThreadNum == (m_nNumberOfThreads - 1))
                {
                    nThreadEndProgramIndex = lstEntireBaseOfProgramsToRun.Count;
                }

                for (int i = nThreadStartProgramIndex; i < nThreadEndProgramIndex; i++)
                {
                    lstProgramsToRunForThread.Add(lstEntireBaseOfProgramsToRun[i]);
                }

                Thread trdNewThread = new Thread(this.RunCurrentGenerationThread);
                arrThreads[nThreadNum] = trdNewThread;
                arrThreads[nThreadNum].Start(lstProgramsToRunForThread);
            }

            // Wait until they are all done
            foreach (Thread trdRunningThread in arrThreads)
            {
                trdRunningThread.Join();
            }

            // At the end, invoke an event for the user to give the statistics
            // For the current generation.
            if (this.GenerationIsCompleteEvent != null)
            {
                Statistics sStatistics = new Statistics(this);
                this.GenerationIsCompleteEvent(sStatistics, this);
            }
        }

        /// <summary>
        /// Run the genetic programming algorithm directly
        /// </summary>
        public void RunEvolute(int nNumOfGenerations)
        {
            this.Evolute(nNumOfGenerations);
        }

        /// <summary>
        /// Run the genetic programming algorithm on a separate thread
        /// </summary>
        public void RunEvoluteOnThread(int nNumOfGenerations)
        {
            this.m_trdMainThread = new Thread(this.Evolute);
            this.m_trdMainThread.Start(nNumOfGenerations);
        }

        /// <summary>
        /// Stops the genetic programming algorithm which was ran on a seperate thread.
        /// </summary>
        public void StopEvoluteThread()
        {
            this.m_trdMainThread.Abort();
        }

        #endregion
    }
}