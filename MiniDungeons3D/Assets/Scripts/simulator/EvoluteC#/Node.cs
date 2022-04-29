using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows;
using System.IO;
using System.Collections;
#if UNITY_EDITOR
using UnityEngine;
#endif
namespace TreeLanguageEvolute
{

    #region Node class
    /// <summary>
    /// The node class, is an abstract general class for a node.
    /// </summary>
    public abstract class Node : ICloneable
    {
        /// <summary>
        /// Returns the count of how many nodes there are in this tree
        /// And in the subtrees of this node
        /// </summary>
        public int Count
        {
            get
            {
                return (NodeHelper.CountNodes(this));
            }
        }

        /// <summary>
        /// Returns what is the depth, of the tree that this node contains.
        /// </summary>
        public int Depth
        {
            get
            {
                return (NodeHelper.FindDepth(this));
            }
        }

        /// <summary>
        /// An abstract function that returns the value that is contained in this node.
        /// </summary>
        /// <returns>The value that is contained in this node</returns>
        public abstract float ReturnValue(TreeProgram progOwner);

        /// <summary>
        /// Returns the name of the node
        /// </summary>
        public abstract string GetName(TreeProgram progOwner);

        public abstract void Save(StreamWriter swWriter);

        #region ICloneable Members

        /// <summary>
        /// Clones the node recursively
        /// </summary>
        /// <returns>A recursive clone of the node</returns>
        public abstract object Clone();

        #endregion
    }

    #endregion

    #region NodeHelper class

    /// <summary>
    /// A static class which contains various helper functions for handling
    /// Recursive node structures.
    /// </summary>
    public static class NodeHelper
    {
        //public class nodetreelistelement
        //{
        //    public node m_ndpointertonode;
        //    public int[] m_arrsons;
        //    public int m_nparent;

        //    public nodetreelistelement(node ndpointertonode, int[] arrsons, int nparent)
        //    {
        //        this.m_ndpointertonode = ndpointertonode;
        //        this.m_arrsons = arrsons;
        //        this.m_nparent = nparent;
        //    }
        //}

        //public static List<NodeTreeListElement> GetNodeList(Node ndRoot)
        //{
        //    List<NodeTreeListElement> lstNodes;
        //    this.GetNodeListHelper(ndRoot, -1, lstNodes);
        //    return (lstNodes);
        //}

        //static void GetNodeListHelper(Node ndRoot, int nLocationInList,
        //    int nParentLocation, List<NodeTreeListElement> lstNodes)
        //{
        //    NodeTreeListElement lstElement = new NodeTreeListElement(ndRoot, null, nParentLocation);
        //    lstNodes.Add(lstElement);
        //    int nThisNodeLocation = lstNodes.Count-1;

        //    if (ndRoot is FunctionNode)
        //    {
        //        FunctionNode fncNode = (FunctionNode)ndRoot;
        //        lstElement.m_arrSons = new int[fncNode.m_arrSons.Length];
        //        foreach (Node ndSonNode in fncNode.m_arrSons)
        //        {
        //            NodeTreeListElement lstSonElement = new NodeTreeListElement(ndSonNode, null, nThisNodeLocation);
        //            lstNodes.Add(lstSonElement);
        //        }

        //        foreach (Node ndSonNode in fncNode.m_arrSons)
        //        {
        //            GetNodeListHelper(ndSonNode, )
        //        }
        //    }
        //}

        public class NodeResult
        {
            public Node ndNode;
            public Node ndNodeParent;
        }

        static int nCurrentNodeNumber; // A helper variable

        /// <summary>
        /// Returns what is the depth, of the tree that a node contains.
        /// </summary>
        /// <param name="ndRoot">The root node</param>
        /// <returns>Returns what is the depth, of the tree that a node contains.</returns>
        public static int FindDepth(Node ndRoot)
        {
	        int nDepth =  0;
	        if (ndRoot is FunctionNode)
	        {
                FunctionNode fncRoot = (FunctionNode) ndRoot;
		        for (int nSonNum = 0;
                     nSonNum != fncRoot.m_arrSons.Length;
                     nSonNum++)
		        {
			        nDepth = Math.Max(nDepth, FindDepth(fncRoot.m_arrSons[nSonNum]));
		        }
	        }

	        return (nDepth+1);
        }

        /// <summary>
        /// Returns the count of how many nodes there are in this tree
        /// And in the subtrees of this node
        /// </summary>
        /// <param name="ndNode">The root node</param>
        /// <returns>
        /// Returns the count of how many nodes there are in this tree
        /// And in the subtrees of this node
        /// </returns>
        public static int CountNodes(Node ndNode)
        {
            int nSum = 0;
            if (ndNode is FunctionNode)
            {
                FunctionNode fndNode = (FunctionNode)ndNode;
                foreach (Node ndSonNode in fndNode.m_arrSons)
                {
                    nSum += CountNodes(ndSonNode);
                }
            }

            return (nSum + 1);
        }

        /// <summary>
        /// Finds a node in the tree by a node number,
        /// when the node numbers are marked from left - downwards, then right
        /// in the tree.
        /// </summary>
        /// <param name="ndRoot">The root of the tree</param>
        /// <param name="nNodeNumber">The node number to look for</param>
        /// <returns>a node in the tree by a node number,
        /// when the node numbers are marked from left - downwards, then right
        /// in the tree.
        /// </returns>
        public static NodeResult FindNode(Node ndRoot, int nNodeNumber)
        {
            nCurrentNodeNumber = -1;
            return (FindNodeRecursive(ndRoot, nNodeNumber, null));
        }

        /// <summary>
        /// A helper function
        /// </summary>
        /// <param name="ndRoot"></param>
        /// <param name="nNodeNumber"></param>
        /// <param name="ndNodeParent"></param>
        /// <returns></returns>
        static NodeResult FindNodeRecursive(Node ndRoot, int nNodeNumber, Node ndNodeParent)
        {
	        nCurrentNodeNumber++;
	        if (nCurrentNodeNumber == nNodeNumber)
	        {
                NodeResult rslFoundResult = new NodeResult();
                rslFoundResult.ndNode = ndRoot;
                rslFoundResult.ndNodeParent = ndNodeParent;
                return (rslFoundResult);
	        }
	        if (ndRoot is FunctionNode)	
	        {
                FunctionNode fndNode = (FunctionNode)ndRoot;
		        for (int nSonNum = 0;
                     nSonNum != fndNode.m_arrSons.Length;
                     nSonNum++)
		        {
                    NodeResult rslFoundResult = FindNodeRecursive(fndNode.m_arrSons[nSonNum], nNodeNumber, fndNode);
                    if (rslFoundResult != null)
			        {
                        return (rslFoundResult);
			        }
		        }
	        }

            return null;
        }

        /// <summary>
        /// Exchanges two nodes, such that any other nodes that hold the same memory
        /// Will not be affected.
        /// </summary>
        /// <param name="ndNodeOne">Node one</param>
        /// <param name="ndNodeOneParent">Node one parent</param>
        /// <param name="ndProgramOneRootNode">The main root of node one</param>
        /// <param name="ndNodeTwo">Node two</param>
        /// <param name="ndNodeTwoParent">Node two parent</param>
        /// <param name="ndProgramTwoRootNode">The main root of node two</param>
        public static void ExchangeNodes(Node ndNodeOne, Node ndNodeOneParent, ref Node ndProgramOneRootNode,
            Node ndNodeTwo, Node ndNodeTwoParent, ref Node ndProgramTwoRootNode)
        {
            //node ndnodeoneclone = (node)ndnodeone.clone();
            //node ndnodetwoclone = (node)ndnodetwo.clone();
            //ndnodeone = ndnodetwoclone;
            //ndnodetwo = ndnodeoneclone;
            if (ndNodeOneParent == null)
            {
                ndProgramOneRootNode = ndNodeTwo;
            }
            else
            {
                FunctionNode fncNodeOneParent = (FunctionNode)ndNodeOneParent;
                // Find node one in node one parent sons and exchange it with node two
                for (int nSonIndexForParentOne = 0;
                     nSonIndexForParentOne < fncNodeOneParent.m_arrSons.Length;
                     nSonIndexForParentOne++)
                {
                    if (fncNodeOneParent.m_arrSons[nSonIndexForParentOne] == ndNodeOne)
                    {
                        fncNodeOneParent.m_arrSons[nSonIndexForParentOne] = ndNodeTwo;
                    }
                }
            }

            if (ndNodeTwoParent == null)
            {
                ndProgramTwoRootNode = ndNodeOne;
            }
            else
            {
                FunctionNode fncNodeTwoParent = (FunctionNode)ndNodeTwoParent;
                // Find node two in node two parent sons and exchange it with node one
                for (int nSonIndexForParentTwo = 0;
                     nSonIndexForParentTwo < fncNodeTwoParent.m_arrSons.Length;
                     nSonIndexForParentTwo++)
                {
                    if (fncNodeTwoParent.m_arrSons[nSonIndexForParentTwo] == ndNodeTwo)
                    {
                        fncNodeTwoParent.m_arrSons[nSonIndexForParentTwo] = ndNodeOne;
                    }
                }
            }
        }
    }

    #endregion

    #region FunctionNode class

    /// <summary>
    /// The class is a node of a function, which contains sons. The sons are nodes
    /// And those provide the values to compute in the function itself (of this node).
    /// The parameters of the function are the sons of this node. 
    /// For an example, the function add takes two parameters.
    /// The two parameters will be the son[0] of this node, and son[1] of this node.
    /// </summary>
    public class FunctionNode : Node
    {
        #region Data members

        public Node[] m_arrSons; // The node sons
        public FunctionType m_funcFunction; // The function of this node

        #endregion

        #region Properties

        public Node[] Sons
        {
            get
            {
                return (this.m_arrSons);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// An empty constructor
        /// </summary>
        public FunctionNode()
        {
        }

        /// <summary>
        /// Construct a function node with the wanted function.
        /// </summary>
        /// <param name="fncWantedFunction">Wanted function</param>
        public FunctionNode(FunctionType fncWantedFunction)
        {
            this.m_funcFunction = fncWantedFunction;
            this.m_arrSons = new Node[this.m_funcFunction.NumberOfArguments];
        }

		// ...this code does not work for android....
		public FunctionNode(StreamReader srReader, Hashtable hsAllTypes)
		{
			this.m_funcFunction = (FunctionType)hsAllTypes[srReader.ReadLine()];
            int nNumOfSons = int.Parse(srReader.ReadLine());

            this.m_arrSons = new Node[nNumOfSons];

            for (int nSonNum = 0; nSonNum < this.m_arrSons.Length; nSonNum++)
            {
				string strSonType = srReader.ReadLine();

				if (strSonType.Equals( typeof(FunctionNode).ToString()))
                {
					m_arrSons[nSonNum] = new FunctionNode(srReader, hsAllTypes);
                }
				else if (strSonType.Equals( typeof(VariableNode).ToString()))
                {
					m_arrSons[nSonNum] = new VariableNode(srReader);
                }
				else if (strSonType .Equals( typeof(ValueNode).ToString()))
                {
					m_arrSons[nSonNum] = new ValueNode(srReader);
                }
                else
                {
                    throw new Exception("Error : Could not load unknown node type");
                }
            }
        }


		//code for android
		public FunctionNode(ArrayList lines, Hashtable hsAllTypes)
		{
			string line = lines[0].ToString().Trim();
			lines.RemoveAt (0);
			this.m_funcFunction = (FunctionType)hsAllTypes[line];


			line = lines[0].ToString().Trim();
			lines.RemoveAt (0);
			int nNumOfSons = int.Parse(line);

			this.m_arrSons = new Node[nNumOfSons];

			for (int nSonNum = 0; nSonNum < this.m_arrSons.Length; nSonNum++)
			{
				line = lines[0].ToString().Trim();
				lines.RemoveAt (0);
				string strSonType = line.Trim();

				if (strSonType.Equals( typeof(FunctionNode).ToString()))
				{
					m_arrSons[nSonNum] = new FunctionNode(lines, hsAllTypes);
				}
				else if (strSonType.Equals( typeof(VariableNode).ToString()))
				{
					m_arrSons[nSonNum] = new VariableNode(lines);
				}
				else if (strSonType .Equals( typeof(ValueNode).ToString()))
				{
					m_arrSons[nSonNum] = new ValueNode(lines);
				}
				else
				{
					throw new Exception("Error : Could not load unknown node type");
				}
			}
		}

        #endregion

        #region Other methods

        /// <summary>
        /// Activates the function and returns the value of the function
        /// </summary>
        /// <returns>
        /// Activates the function and returns the value of the function
        /// </returns>
        public override float ReturnValue(TreeProgram progOwner)
        {
            return (this.m_funcFunction.action(this.m_arrSons, progOwner));
        }

		// returns the string value of this function contained inside this node
		public string GetFunctionValue() {
			return m_funcFunction.Name;
		}
        public override string GetName(TreeProgram progOwner)
        {
            return (this.m_funcFunction.Name);
        }

        // Saves everything to a stream writer.
        public override void Save(StreamWriter swWriter)
        {
            swWriter.WriteLine(m_funcFunction.Name);
            swWriter.WriteLine(m_arrSons.Length);
            foreach (Node ndSon in m_arrSons)
            {
                swWriter.WriteLine(ndSon.GetType().ToString());
                ndSon.Save(swWriter);
            }
        }

        /// <summary>
        /// Clones the function node recursively, The cloned version sits on a
        /// Different memory
        /// </summary>
        /// <returns>The clone of the function node</returns>
        public override object Clone()
        {
            FunctionNode fncNewNode = new FunctionNode();

            // Copies the regular data members
            fncNewNode.m_funcFunction = this.m_funcFunction;
            fncNewNode.m_arrSons = new Node[this.m_arrSons.Length];

            // Passes on all the sons and clones them too (This works recursively)
            for (int nSonNodeIndex = 0;
                 nSonNodeIndex < this.m_arrSons.Length;
                 nSonNodeIndex++)
            {
                fncNewNode.m_arrSons[nSonNodeIndex] =
                    (Node)this.m_arrSons[nSonNodeIndex].Clone();
            }

            return (fncNewNode);
        }

        #endregion
    }

    #endregion

    #region VariableNode class

    /// <summary>
    /// A variable node is a node that contains a variable.
    /// </summary>
    public class VariableNode : Node
    {
        int m_nVariableIndex; // The index of the variable, 
        // In the variable array inside the program

        /// <summary>
        /// Construct the node with a variable
        /// </summary>
        /// <param name="varVariable">The variable</param>
        public VariableNode(Variable varVariable, TreeProgram progOwner)
        {
            // Find the variable index in the program, when constructing the variable
            // Node. So that when we run the variable node, we can access the 
            // Variable index of that program and get the variable value
            for (int nVariableIndex = 0;
                 nVariableIndex < progOwner.m_arrVariables.Length;
                 nVariableIndex++)
            {
                Variable varInProgram = progOwner.m_arrVariables[nVariableIndex];
                if (varInProgram == varVariable)
                {
                    this.m_nVariableIndex = nVariableIndex;
                }
            }
        }

        public VariableNode(StreamReader srReader)
        {
            m_nVariableIndex = int.Parse(srReader.ReadLine());
        }

		public VariableNode(ArrayList lines)
		{
			string line = lines[0].ToString();
			lines.RemoveAt (0);
			m_nVariableIndex = int.Parse(line);
		}

        /// <summary>
        /// The function returns the value of the node 
        /// (Which is the value of the variable)
        /// </summary>
        /// <returns>the value of the node 
        /// (Which is the value of the variable)</returns>
        public override float ReturnValue(TreeProgram progOwner)
        {
            // Return the value of the variable that this node points to
            return (progOwner.m_arrVariables[this.m_nVariableIndex].ReturnValue());
        }

        public override string GetName(TreeProgram progOwner)
        {
            return (progOwner.m_arrVariables[this.m_nVariableIndex].Name);
        }

        public override void Save(StreamWriter swWriter)
        {
            swWriter.WriteLine(m_nVariableIndex);
        }

        /// <summary>
        /// Clones the node
        /// </summary>
        /// <returns>A cloned variable node</returns>
        public override object Clone()
        {
            VariableNode vrbNewNode = (VariableNode)this.MemberwiseClone();
            return vrbNewNode;
        }
		// returns the index of the variable index inside this variable node
		public int GetIndexValue() {
			return m_nVariableIndex;
		}
    }

    #endregion

    #region ValueNode class

    /// <summary>
    /// A value node is a node that contains a value
    /// </summary>
    public class ValueNode : Node
    {
        float m_fValue; // The value inside the node

        /// <summary>
        /// Consturcts a new value node with a given value
        /// </summary>
        /// <param name="fValue">The value of the node</param>
        public ValueNode(float fValue)
        {
            this.m_fValue = fValue;
        }

        /// <summary>
        /// Consturcts a new value node with a given value
        /// </summary>
        /// <param name="vlValue">The value of the node</param>
        public ValueNode(Value vlValue)
        {
            this.m_fValue = vlValue.ReturnValue();
        }

        public ValueNode(StreamReader srReader)
        {
            m_fValue = float.Parse(srReader.ReadLine());
        }

		public ValueNode(ArrayList lines)
		{
			string line = lines[0].ToString();
			lines.RemoveAt (0);
			m_fValue = float.Parse(line);
		}

        /// <summary>
        /// Returns the value
        /// </summary>
        /// <returns>Returns the value of the node</returns>
        public override float ReturnValue(TreeProgram progOwner)
        {
            return (this.m_fValue);
        }

		public float GetValue() {
			return (this.m_fValue);
		}

        public override string GetName(TreeProgram progOwner)
        {
            return (this.m_fValue.ToString());
        }

        public override void Save(StreamWriter swWriter)
        {
            swWriter.WriteLine(m_fValue);
        }

        /// <summary>
        /// Clones the value node
        /// </summary>
        /// <returns>A cloned value node</returns>
        public override object Clone()
        {
            ValueNode valNewNode = (ValueNode)this.MemberwiseClone();
            return valNewNode;
        }
    }

    #endregion
}