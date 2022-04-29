using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEngine;
#endif
namespace TreeLanguageEvolute
{
    public class GeneticProgrammingEngine : BaseEngine
    {
        #region Data Members

        private int m_nMinInitialTreeDepth; // Initial tree depth when randomizing random trees
        private int m_nMaxInitialTreeDepth; // Maximal tree depth when randomizing random trees
        private int m_nMaxOverallTreeDepth; // Maximal tree depth, to limit the amount of 
        // How many trees are developing.
        public delegate void EvalFitnessHandler(TreeProgram progProgram, GeneticProgrammingEngine sender);
        public event EvalFitnessHandler EvalFitnessForProgramEvent; // An event to be 
        // Invoked when a program needs to be run and to be evaluated for it's fitness
        // The user must take this event, otherwise no fitness can be computed.

        #endregion

        #region Properties

        /// <summary>
        /// Set the minimum initial tree depth that is used by the 
        /// Initial random of the trees. For an example, If the minimal tree depth
        /// Is 3, then the trees will grow no less than 3 sons depth.
        /// </summary>
        public int MinInitialTreeDepth
        {
            set
            {
                this.m_nMinInitialTreeDepth = value;
            }
        }

        /// <summary>
        /// Set the maximal initial tree depth that is used by the 
        /// Initial random of the trees. For an example, If the maximal tree depth
        /// Is 10, then the trees will grow no more than 10 sons depth.
        /// </summary>
        public int MaxInitialTreeDepth
        {
            set
            {
                this.m_nMaxInitialTreeDepth = value;
            }
        }

        /// <summary>
        /// Set the maximal overall tree depth. This means that the trees will never
        /// Grow more than the overall tree depth.
        /// </summary>
        public int MaxOverallTreeDepth
        {
            set
            {
                this.m_nMaxOverallTreeDepth = value;
            }
        }

        #endregion

        #region Constructor

        public GeneticProgrammingEngine() : base()
        {
            this.m_nMinInitialTreeDepth = 3;
            this.m_nMaxInitialTreeDepth = 5;
            this.m_nMaxOverallTreeDepth = 10;
        }

        #endregion

        #region Other methods

        /// <summary>
        /// Creates a random node in the tree. The function is recursive, and 
        /// Creates a random node. Which means, a random function or variable will
        /// Be selected . If a function is selected, then another nodes for the
        /// Function will be created. If a variable will be selected, then 
        /// The tree will stop evolving at this branch.
        /// </summary>
        /// <param name="nCurrentDepth">The current tree depth</param>
        /// <returns>A random tree node</returns>
        private Node CreateRandomNode(int nCurrentDepth, BaseProgram baseProgOwner, ArrayList arrRandomableTypes)
        {
            TreeProgram progOwner = (TreeProgram)baseProgOwner;
            // The chosen index in all the types array, to choose some random thing,
            // Either variable of function, to insert as a node.
            int nChosenTypeIndex = GlobalRandom.m_rndRandom.Next(arrRandomableTypes.Count);
            object objChosenType = arrRandomableTypes[nChosenTypeIndex];

            // If we are above the tree limit, choose randomly until we select a variable.
            bool fFoundLeaf = false;
            while ((nCurrentDepth > this.m_nMaxInitialTreeDepth) &&
                   (fFoundLeaf == false))
            {
                nChosenTypeIndex = GlobalRandom.m_rndRandom.Next(arrRandomableTypes.Count);
                objChosenType = arrRandomableTypes[nChosenTypeIndex];

                if (objChosenType is FunctionType)
                {
                    if (((FunctionType)objChosenType).NumberOfArguments == 0)
                    {
                        fFoundLeaf = true;
                    }
                }
                else // The type is variable
                {
                    fFoundLeaf = true;
                }
            }

            // If we are below the initial tree limit, choose randomly until we select
            // A function.
            bool fFoundBranch = false;
            while ((nCurrentDepth < this.m_nMinInitialTreeDepth) &&
                   (fFoundBranch == false))
            {
                nChosenTypeIndex = GlobalRandom.m_rndRandom.Next(arrRandomableTypes.Count);
                objChosenType = arrRandomableTypes[nChosenTypeIndex];

                if (objChosenType is FunctionType)
                {
                    if (((FunctionType)objChosenType).NumberOfArguments > 0)
                    {
                        fFoundBranch = true;
                    }
                }
            }

            // If the object chosen is a variable, create a new VariableNode
            if (objChosenType is Variable)
            {
                Variable vlVariable = (Variable)objChosenType;
                return (new VariableNode(vlVariable, progOwner));
            }
            // If the object chosen is a value, create a new ValueNode
            else if (objChosenType is Value)
            {
                Value vlValue = (Value)objChosenType;
                return (new ValueNode(vlValue));
            }
            // If the object chosen is a RandomMacro, create a new ValueNode which
            // Will be randomized.
            else if (objChosenType is RandomMacro)
            {
                return (new ValueNode(((RandomMacro)objChosenType).ReturnValue()));
            }
            // If the object chosen is a function, then create a function and also
            // Create trees for that function.
            else if (objChosenType is Function)
            {
                FunctionNode fncNewNode = new FunctionNode();
                Function funcChosenFunction = (Function)objChosenType;
                fncNewNode.m_funcFunction = funcChosenFunction;
                fncNewNode.m_arrSons = new Node[funcChosenFunction.NumberOfArguments];

                for (int nSonIndex = 0;
                     nSonIndex < fncNewNode.m_arrSons.Length;
                     nSonIndex++)
                {
                    fncNewNode.m_arrSons[nSonIndex] =
                        CreateRandomNode(nCurrentDepth + 1, progOwner, arrRandomableTypes);
                }

                return (fncNewNode);
            }

            // We will never normally reach here.
            return (null);
        }

        /// <summary>
        /// The crossover function, takes two parent programs, and generates two sons
        /// From the two parents.
        /// </summary>
        /// <param name="progParentOne">Parent program one</param>
        /// <param name="progParentTwo">Parent program two</param>
        /// <param name="progSonOne">Son program one</param>
        /// <param name="progSonTwo">Son program two</param>
        protected override void Crossover(BaseProgram baseProgParentOne, BaseProgram baseProgParentTwo,
                       out BaseProgram baseProgSonOne, out BaseProgram baseProgSonTwo)
        {
            TreeProgram progParentOne = (TreeProgram)baseProgParentOne;
            TreeProgram progParentTwo = (TreeProgram)baseProgParentTwo;
            TreeProgram progSonOne;
            TreeProgram progSonTwo;

            // Get the global random ...
            ImprovedRandom rndRandom = GlobalRandom.m_rndRandom;

            progSonOne = (TreeProgram)progParentOne.Clone();
            progSonTwo = (TreeProgram)progParentTwo.Clone();
            // Generate new sons more and more, until the new sons tree limit's are
            // Good enough and stand the tree limits. Or just do this for the first time
            // That sons are being generated.
            for (int nRootNum = 0; nRootNum < progSonOne.m_arrRoots.Length; nRootNum++)
            {
                bool bGeneratedNewSons = false;
                while (((progSonOne.m_arrRoots[nRootNum].Depth > this.m_nMaxOverallTreeDepth) ||
                       (progSonTwo.m_arrRoots[nRootNum].Depth > this.m_nMaxOverallTreeDepth)) ||
                       !bGeneratedNewSons)
                {
                    // Clone both parent roots
                    progSonOne.m_arrRoots[nRootNum] = (Node)progParentOne.m_arrRoots[nRootNum].Clone();
                    progSonTwo.m_arrRoots[nRootNum] = (Node)progParentTwo.m_arrRoots[nRootNum].Clone();

                    // Random two nodes as a crossover points in the two sons (which are
                    // Now supposed to be a cloned version of the parents)
                    int nSelectNodeInParentOne = rndRandom.Next(
                            NodeHelper.CountNodes(progSonOne.m_arrRoots[nRootNum]));
                    int nSelectNodeInParentTwo = rndRandom.Next(
                        NodeHelper.CountNodes(progSonTwo.m_arrRoots[nRootNum]));
                    NodeHelper.NodeResult ndNodeToExchangeOne = NodeHelper.FindNode(
                        progSonOne.m_arrRoots[nRootNum], nSelectNodeInParentOne);
                    NodeHelper.NodeResult ndNodeToExchangeTwo = NodeHelper.FindNode(
                        progSonTwo.m_arrRoots[nRootNum], nSelectNodeInParentTwo);

                    // Exchange the crossover branches between the two sons, when it is done,
                    // The new sons are ready to be called "new sons" and not just a clone.
                    NodeHelper.ExchangeNodes(
                        ndNodeToExchangeOne.ndNode, ndNodeToExchangeOne.ndNodeParent,
                        ref progSonOne.m_arrRoots[nRootNum],
                        ndNodeToExchangeTwo.ndNode, ndNodeToExchangeTwo.ndNodeParent,
                        ref progSonTwo.m_arrRoots[nRootNum]);

                    bGeneratedNewSons = true;
                }
            }

            baseProgSonOne = progSonOne;
            baseProgSonTwo = progSonTwo;
        }

        /// <summary>
        /// The function will create a random program and will use the
        /// Function CreateRandomNode in order to do that.
        /// </summary>
        /// <returns>A random program</returns>
        protected override BaseProgram CreateRandomProgram()
        {
            // Create a new program with the wanted number of roots
            TreeProgram progProgram = new TreeProgram(this.m_nNumOfResults);

            // Create the variables array, which consists of new variables
            progProgram.m_arrVariables = new Variable[this.m_arrstrVariables.Count];
            for (int nVariableIndex = 0;
                 nVariableIndex < this.m_arrstrVariables.Count;
                 nVariableIndex++)
            {
                Variable varNew = new Variable();
                varNew.SetName(this.m_arrstrVariables[nVariableIndex]);
                progProgram.m_arrVariables[nVariableIndex] = varNew;
            }

            // Copy the all the main randomable types, concat it the variables of
            // The current program.
            ArrayList arrRandomableTypes = new ArrayList();
            arrRandomableTypes.AddRange(this.m_arrAllTypes);
            foreach (Variable varOfProgram in progProgram.m_arrVariables)
            {
                arrRandomableTypes.Add(varOfProgram);
            }

            // Create a random node for the root
            for (int nRootNum = 0; nRootNum < progProgram.m_arrRoots.Length; nRootNum++)
            {
                progProgram.m_arrRoots[nRootNum] = CreateRandomNode(0, progProgram, arrRandomableTypes);
            }

            // Returns the program
            return (progProgram);
        }

        protected override void RunProgram(BaseProgram progProgram)
        {
            this.EvalFitnessForProgramEvent((TreeProgram)progProgram, this);
        }

        public void SaveProgram(string strFilename, TreeProgram progProgram)
        {
            StreamWriter swWriter = new StreamWriter(strFilename);
            progProgram.Save(swWriter);
            swWriter.Close();
        }

        public TreeProgram LoadProgram(string strFilename)
        {
            Hashtable hsAllTypes = new Hashtable();

            foreach (FunctionType funcFunction in this.m_arrFunctions)
            {
                hsAllTypes[funcFunction.Name] = funcFunction;
            }

            foreach (string strVariableName in this.m_arrstrVariables)
            {
                Variable varNew = new Variable();
                varNew.SetName(strVariableName);
                hsAllTypes[strVariableName] = varNew;
            }

            //...this commented out line does not work in android...
            StreamReader srReader = new StreamReader(strFilename);

            TreeProgram progProgram = new TreeProgram(srReader, hsAllTypes);
            srReader.Close();

            //code for android

   //         string file = Resources.Load<TextAsset>(strFilename).text;
			//String[] fileLines = file.Split ('\n');
			//ArrayList lines = new ArrayList ();
			//foreach (string i in fileLines)
			//{
			//	lines.Add(i);
			//}
			//TreeProgram progProgram = new TreeProgram(lines, hsAllTypes);

            return progProgram;
        }

        #endregion
    }
}
