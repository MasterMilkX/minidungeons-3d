using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Drawing;
//using System.Windows.Forms;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEngine;
#endif
namespace TreeLanguageEvolute
{
    public abstract class BaseProgram : ICloneable
    {
        #region Data Members

        internal float m_fFitness; // The fitness of the program
        internal Variable[] m_arrVariables; // The variables array - contains all the
        // Variables inside the program.
        internal float[] m_fReturnedResult; // The results at the end of the program.
        internal System.Object m_objAdditionalInformation;

        #endregion

        #region Properties

        /// <summary>
        /// Get the result of the program, after it has been ran.
        /// </summary>
        public float Result
        {
            get
            {
                return (this.m_fReturnedResult[0]);
            }
        }

        public float[] Results
        {
            get
            {
                return (this.m_fReturnedResult);
            }
        }

        /// <summary>
        /// The fitness of the program
        /// </summary>
        public float Fitness
        {
            get
            {
                 return (this.m_fFitness);
            }
            set
            {
                this.m_fFitness = value;
            }
        }

        public System.Object AdditionalInformation
        {
            get
            {
                return (this.m_objAdditionalInformation);
            }
        }

        public Variable[] Variables
        {
            get
            {
                return (this.m_arrVariables);
            }
        }

        // Returns the program size.
        public abstract int Size
        {
            get;
        }

        #endregion

        public BaseProgram()
        {
            ;
        }


	// ...this code does not work for android....
		public BaseProgram(StreamReader srReader, Hashtable hsAllTypes)
        {
			m_fReturnedResult = new float[int.Parse(srReader.ReadLine ())];
			m_arrVariables = new Variable[int.Parse(srReader.ReadLine () )];

            for (int nVariableNum = 0; nVariableNum < m_arrVariables.Length; nVariableNum++)
            {
				m_arrVariables[nVariableNum] = (Variable)hsAllTypes[ srReader.ReadLine () ];
            }
        }

		//code for android
		public BaseProgram(ArrayList lines, Hashtable hsAllTypes)
		{
			string line = lines[0].ToString().Trim();
			lines.RemoveAt (0);
			m_fReturnedResult = new float[int.Parse(line)];

			line = lines[0].ToString().Trim();
			lines.RemoveAt (0);
			m_arrVariables = new Variable[int.Parse(line)];

			for (int nVariableNum = 0; nVariableNum < m_arrVariables.Length; nVariableNum++)
			{
				line = lines[0].ToString().Trim();
				lines.RemoveAt (0);
				m_arrVariables[nVariableNum] = (Variable)hsAllTypes[ line];
			}
		}

        #region Other methods

        public abstract void Run();

        public void Run(System.Object objAdditionalInformation)
        {
            this.m_objAdditionalInformation = objAdditionalInformation;
            this.Run();
            this.m_objAdditionalInformation = null;
        }

        /// <summary>
        /// Get all the variables in the program, so you can access them 
        /// And change their values
        /// </summary>
        /// <returns>The hashtable of the variables</returns>
        public Hashtable GetVariables()
		{
            Hashtable hsVariables = new Hashtable();

            // Add all the variables to the hash table and return it
            foreach (Variable varVariable in this.m_arrVariables)
            {
				hsVariables.Add(varVariable.Name, varVariable);
            }


            return (hsVariables);
        }

        #endregion

        public virtual void Save(StreamWriter swWriter)
        {
            swWriter.WriteLine(m_fReturnedResult.Length);
            swWriter.WriteLine(m_arrVariables.Length);
            for (int nVariableNum = 0; nVariableNum < m_arrVariables.Length; nVariableNum++)
            {
                swWriter.WriteLine(m_arrVariables[nVariableNum].Name);
            }
        }

        #region ICloneable Members

//         public object Migrate()
//         {
//             this.nMigrated = 2000;
//             return (this.Clone());
//         }

        public abstract object Clone();

        #endregion
    }

    /// <summary>
    /// A program is a class which contains roots that can be ran.
    /// Each root is a node. The node can return a value and it is a recursive
    /// Tree that is made of functions, values and variables.
    /// Once a program is ran, the user can take the results and give fitness to the
    /// Program.
    /// </summary>
    public class TreeProgram : BaseProgram
    {
        #region Data members

        internal Node[] m_arrRoots; // Each root is a node that can be ran.

        #endregion

        #region Properties

        /// <summary>
        /// Get the depth of root of the program
        /// </summary>
        public int Depth
        {
            get
            {
                return (this.m_arrRoots[0].Depth);
            }
        }

        /// <summary>
        /// Get the count of nodes of the program
        /// </summary>
        public int Count
        {
            get
            {
                return (this.m_arrRoots[0].Count);
            }
        }

        public override int Size
        {
            get
            {
                return this.Count;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new program with a number of roots
        /// </summary>
        /// <param name="nNumberOfRoots">The number of roots</param>
        public TreeProgram(int nNumberOfRoots)
        {
            this.m_arrRoots = new Node[nNumberOfRoots];
            this.m_fReturnedResult = new float[nNumberOfRoots];
        }

		// ...this code does not work for android....
		 public TreeProgram(StreamReader srReader, Hashtable hsAllTypes)
		  : base(srReader, hsAllTypes)
		{

			m_arrRoots = new Node[int.Parse( srReader.ReadLine ())];

            for (int nRootNum = 0; nRootNum < m_arrRoots.Length; nRootNum++)
            {
				m_arrRoots[nRootNum] = new FunctionNode(srReader, hsAllTypes);
	        }
        }

		//code for android
		public TreeProgram(ArrayList lines, Hashtable hsAllTypes)
			: base(lines, hsAllTypes)
		{
			string line = lines[0].ToString().Trim();
			lines.RemoveAt (0);
			m_arrRoots = new Node[int.Parse(line)];

			for (int nRootNum = 0; nRootNum < m_arrRoots.Length; nRootNum++)
			{
				m_arrRoots[nRootNum] = new FunctionNode(lines, hsAllTypes);
			}
		}

        #endregion

        #region Other methods

        /// <summary>
        /// Run the program, and return the results in the array of the results.
        /// </summary>
        override public void Run()
        {
            int nIndexOfNode = 0;

            // Pass on each node of the roots, and run it.
            foreach (Node ndNode in this.m_arrRoots)
            {
                float fResult = ndNode.ReturnValue(this);

                // If the results is nan or infinity, then just make it 0.
                if ((double.IsNaN(fResult)) || (double.IsInfinity(fResult)))
                {
                    fResult = 0;
                }

                // Copy the results to the array of the results.
                this.m_fReturnedResult[nIndexOfNode] = fResult;

                nIndexOfNode++;
            }
        }

		public void PrintOutTree() {
			Node root = this.m_arrRoots [0];
			PrintTree (root);
		}

		public void PrintTree(Node n) {
			if (n is FunctionNode) {
				FunctionNode m = (FunctionNode)n;
				Console.Write ("(");
				PrintTree (m.m_arrSons [0]);
				Console.Write (m.GetFunctionValue ());
				PrintTree (m.m_arrSons [1]);
				Console.Write (")");
			} else if (n is VariableNode) {
				VariableNode m = (VariableNode)n;
				Console.Write(this.Variables [m.GetIndexValue ()].Name);
			} else if (n is ValueNode) {
				ValueNode m = (ValueNode)n;
				Console.Write (m.GetValue ());
			}
		}
        /// <summary>
        /// The function draws a string on the entire image.
        /// </summary>
        /// <param name="image">The image to draw the string on</param>
        /// <param name="text">The string to draw</param>
//        static private void DrawStringInWholeImage(Bitmap image, String text)
//        {
//            Graphics graphicsFromImage = Graphics.FromImage(image);
//            Font someFont = new Font("Stone Serif ITC TT", 18, FontStyle.Bold);
//            SizeF sz = graphicsFromImage.MeasureString(text, someFont);
//            float sf = (image.Width / sz.Width < image.Height / sz.Height) ? image.Width / sz.Width : image.Height / sz.Height;
//            graphicsFromImage.ScaleTransform(sf, sf);
//            graphicsFromImage.DrawString(text, someFont, Brushes.Black, new PointF(0, 0), new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip));
//        }

        /// <summary>
        /// The function draws a program nodes using a Graphics object.
        /// </summary>
        /// <param name="grpGraphics">
        /// The graphics object used to draw the program<
        /// /param>
        /// <param name="nWidth">The width of the wanted drawing</param>
        /// <param name="nHeight">The height of the wanted drawing</param>
        /// <param name="frmParentForm">The parent Form</param>
        /*public void Draw(Graphics grpGraphics, int nWidth, int nHeight, Form frmParentForm)
        {
            const float NODE_PERCENT_SIZE = 0.1F;
            const int BITMAP_OF_NODE_SIZE = 400;
            float NODE_PIXEL_SIZE = NODE_PERCENT_SIZE * nWidth;
            Color NODE_COLOR = Color.Green;
            Color BACKGROUND_COLOR = Color.White;
            Color LINE_COLOR = Color.Black;

            Bitmap bmpPicture = new Bitmap(nWidth, nHeight);
            Graphics cGraphicsOnPicture = Graphics.FromImage(bmpPicture);
            Brush brBrush = new SolidBrush(NODE_COLOR);
            List<List<Node>> lstNodeLayers = new List<List<Node>>();
            List<List<Rectangle>> lstNodeBoxLayers = new List<List<Rectangle>>();
            List<Node> lstFirstLayer = new List<Node>();
            List<Node> lstSecondLayer = new List<Node>();
            lstFirstLayer.Add(this.m_arrRoots[0]);

            while ((lstFirstLayer.Count != 0) || (lstSecondLayer.Count != 0))
            {
                foreach (Node ndNodeInLayer in lstFirstLayer)
                {
                    if (ndNodeInLayer is FunctionNode)
                    {
                        FunctionNode fncNode = (FunctionNode)ndNodeInLayer;
                        lstSecondLayer.AddRange(fncNode.m_arrSons);
                    }
                }

                if (lstFirstLayer.Count > 0)
                {
                    lstNodeLayers.Add(lstFirstLayer.GetRange(0, lstFirstLayer.Count));
                }

                lstFirstLayer.Clear();

                foreach (Node ndNodeInLayer in lstSecondLayer)
	            {
                    if (ndNodeInLayer is FunctionNode)
                    {
                        FunctionNode fncNode = (FunctionNode)ndNodeInLayer;
                        lstFirstLayer.AddRange(fncNode.m_arrSons);
                    }
	            }

                if (lstSecondLayer.Count > 0)
                {
                    lstNodeLayers.Add(lstSecondLayer.GetRange(0, lstSecondLayer.Count));
                }
                lstSecondLayer.Clear();
            }

            cGraphicsOnPicture.Clear(Color.White);
            int nNodeHeightSize = nHeight / (lstNodeLayers.Count * 2);
            int nStartingY = 0;
            Bitmap bmpOfNode = new Bitmap(BITMAP_OF_NODE_SIZE, BITMAP_OF_NODE_SIZE);
            Graphics grpGraphicsOfNode = Graphics.FromImage(bmpOfNode);
            foreach (List<Node> lstLayer in lstNodeLayers)
            {
                int nNodeWidthSize = nWidth / (lstLayer.Count * 3);
                int nNodeSize = Math.Min(nNodeWidthSize, nNodeHeightSize);
                int nStartingX = (nWidth / 2) - (nNodeSize * lstLayer.Count);
                List<Rectangle> lstRectLayer = new List<Rectangle>();
                foreach (Node ndNode in lstLayer)
                {
                    Rectangle rctDrawingRectangle =
                        new Rectangle(nStartingX, nStartingY, nNodeSize, nNodeSize);
                    grpGraphicsOfNode.Clear(Color.White);
                    grpGraphicsOfNode.FillEllipse(brBrush,
                        new Rectangle(0, 0, bmpOfNode.Width, bmpOfNode.Height));
                    TreeProgram.DrawStringInWholeImage(bmpOfNode, ndNode.GetName(this));
                    lstRectLayer.Add(rctDrawingRectangle);
                    cGraphicsOnPicture.DrawImage(bmpOfNode, rctDrawingRectangle);
                    nStartingX += nNodeWidthSize * 2;
                }

                lstNodeBoxLayers.Add(lstRectLayer.GetRange(0, lstRectLayer.Count));
                lstRectLayer.Clear();

                nStartingY += nNodeHeightSize * 2;
            }

            int nLayerIndex = 0;
            foreach (List <Rectangle> lstRectsLayer in lstNodeBoxLayers)
            {
                int nNodeIndex = 0;

                foreach (Rectangle rctRectOfNode in lstRectsLayer)
                {
                    int nFatherCenterX = (rctRectOfNode.Left + rctRectOfNode.Right) / 2;
                    int nFatherCenterY = (rctRectOfNode.Top + rctRectOfNode.Bottom) / 2;
                    int nFatherIndex = nNodeIndex;
                    Node ndFatherNode = lstNodeLayers[nLayerIndex][nFatherIndex];
                    
                    // Go through all the sons and draw the lines to them
                    if (ndFatherNode is FunctionNode)
                    {
                        FunctionNode fncFatherNode = (FunctionNode)ndFatherNode;

                        foreach (Node ndSonNode in fncFatherNode.m_arrSons)
                        {
                            int nSonIndex = lstNodeLayers[nLayerIndex + 1].FindIndex(
                                    delegate (Node ndNode)
                                    {
                                        if (ndNode == ndSonNode)
                                        {
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                );

                            Rectangle rctRectSonNode =
                                lstNodeBoxLayers[nLayerIndex + 1][nSonIndex];
                            int nSonCenterX = (rctRectSonNode.Left + rctRectSonNode.Right) / 2;
                            int nSonCenterY = (rctRectSonNode.Top + rctRectSonNode.Bottom) / 2;
                            cGraphicsOnPicture.DrawLine(new Pen(LINE_COLOR), nFatherCenterX, nFatherCenterY,
                                nSonCenterX, nSonCenterY);
                        }
                    }
                    
                    nNodeIndex++;
                }

                nLayerIndex++;
            }

            grpGraphics.DrawImageUnscaled(bmpPicture, 0, 0);
            bmpPicture.Dispose();
            cGraphicsOnPicture.Dispose();
        }*/

        #endregion

        public virtual void Save(StreamWriter swWriter)
        {
            base.Save(swWriter);
            swWriter.WriteLine(m_arrRoots.Length);
            for (int nRootNum = 0; nRootNum < m_arrRoots.Length; nRootNum++)
            {
                m_arrRoots[nRootNum].Save(swWriter);
            }
        }

        #region ICloneable Members

        /// <summary>
        /// The function clones the program. It also passes on all the roots
        /// And clone them recursively, so that all the roots sit on a different
        /// Memory.
        /// </summary>
        /// <returns>A cloned program</returns>
        public override object Clone()
		{ 
            // Construct a new program with the same number of roots
            TreeProgram progNewProgram = new TreeProgram(this.m_arrRoots.Length);

            // Copy basic data members
            progNewProgram.m_fFitness = this.m_fFitness;
            this.m_fReturnedResult.CopyTo(progNewProgram.m_fReturnedResult, 0);

            // Copy the variables
            progNewProgram.m_arrVariables = new Variable[this.m_arrVariables.Length];
            for (int nVariableIndex = 0;
                 nVariableIndex < this.m_arrVariables.Length;
                 nVariableIndex++)
            {
                progNewProgram.m_arrVariables[nVariableIndex] =
                    (Variable)this.m_arrVariables[nVariableIndex].Clone();
            }

            // Copy the nodes recursively
            for (int nRootIndex = 0;
                 nRootIndex < this.m_arrRoots.Length;
                 nRootIndex++)
            {
                progNewProgram.m_arrRoots[nRootIndex] = 
                    (Node)this.m_arrRoots[nRootIndex].Clone();
            }

            return (progNewProgram);
        }

        #endregion
    }
    

    //public class GeneExpressionProgram : BaseProgram
    //{
    //    #region Data members

    //    GeneralType[] m_arrGenome;
    //    private int m_nNumberOfGenes;
    //    private int m_nGeneLength;
    //    internal List<Value> m_arrRandomableValues;
    //    private TreeProgram m_treProgram;

    //    #endregion

    //    #region Properties

    //    public GeneralType[] Genome
    //    {
    //        set
    //        {
    //            this.m_arrGenome = value;
    //        }
    //        get
    //        {
    //            return (this.m_arrGenome);
    //        }
    //    }

    //    #endregion

    //    #region Constructor

    //    public GeneExpressionProgram(int nNumberOfGenes, int nGeneLength,
    //        Variable[] arrVariables)
    //    {
    //        this.m_arrVariables = arrVariables;
    //        this.m_fReturnedResult = new float[nNumberOfGenes];
    //        this.m_nNumberOfGenes = nNumberOfGenes;
    //        this.m_nGeneLength = nGeneLength;
    //        this.m_arrRandomableValues = new List<Value>();
    //        this.m_treProgram = null;
    //    }

    //    #endregion

    //    #region Other methods

    //    /// <summary>
    //    /// Moves a general type from one program's genome to other programs genome,
    //    /// While checking and modifying everything to make this work.
    //    /// </summary>
    //    /// <param name="genTypeToMove">
    //    /// The general type to move to the other program (in the current program genome)
    //    /// </param>
    //    /// <param name="progOther">The other program, to move the general type to</param>
    //    /// <returns>
    //    /// The modified general type, ready for placement at the other's program genome</returns>
    //    internal GeneralType MoveTypeToOtherProgram(GeneralType genTypeToMove,
    //        GeneExpressionProgram progOther)
    //    {
    //        if (genTypeToMove is Variable)
    //        {
    //            int nIndexOfVariable = Array.IndexOf(this.m_arrVariables, genTypeToMove);
    //            return (progOther.m_arrVariables[nIndexOfVariable]);
    //        }

    //        return (genTypeToMove);
    //    }

    //    private void DecodeIntoTree(GeneralType genType, Queue<Node> queLeftToRightNodes,
    //        out Node ndResultNode)
    //    {
    //        // Handle each one of the cases, build the node according to the right case
    //        // Of the general type.
    //        if (genType is FunctionType)
    //            {
    //                ndResultNode = new FunctionNode((FunctionType)genType);
    //                queLeftToRightNodes.Enqueue(ndResultNode);
    //                return;
    //            }
    //        else if (genType is Variable)
    //            {
    //                 ndResultNode = new VariableNode((Variable)genType, this.m_treProgram);
    //                 return;
    //            }
    //        else if (genType is ValueType)
    //            {
    //                ndResultNode = new ValueNode(((ValueType)genType).ReturnValue());
    //                return;
    //            }

    //        // Just to satisfy c# errors, the out parameter is assigned (should've been
    //        // Assigned before), if not then throw exception.
    //        ndResultNode = null;
    //        if (ndResultNode == null)
    //        {
    //            throw new Exception();
    //        }
    //    }

    //    // Basically this function recursively builds the tree using a queue.
    //    // It gets a father node, reads from the genome, and builds it's sons.
    //    // Then it calls itself, builds the sons, call itself , and so on.
    //    private void RecursivelyBuildTree(Queue<Node> queLeftToRightNodes, int nGenomeIndex)
    //    {
    //        // If no elements left in queue, return.
    //        if (queLeftToRightNodes.Count == 0)
    //        {
    //            return;
    //        }

    //        // Get the node at the front of the queue and remove it.
    //        Node nodCurrent = queLeftToRightNodes.Dequeue();

    //        // Check if the current node is a function node (no other option)
    //        if (nodCurrent is FunctionNode)
    //        {
    //            FunctionNode fncNode = (FunctionNode)nodCurrent;
    //            for (int i = 0; i < fncNode.Sons.Length; i++, nGenomeIndex++)
    //            {
    //                this.DecodeIntoTree(this.m_arrGenome[nGenomeIndex],
    //                    queLeftToRightNodes, out fncNode.m_arrSons[i]);
    //            }
    //        }
    //        else throw new Exception();

    //        // Call this function again in order to build the new sons trees.
    //        this.RecursivelyBuildTree(queLeftToRightNodes, nGenomeIndex);
    //    }

    //    public void BuildTreeProgram()
    //    {
    //        // Construct a new tree language program.
    //        this.m_treProgram = new TreeProgram(this.m_nNumberOfGenes);

    //        // Copy the variables of this program to the tree program, so it will be able to run.
    //        this.m_treProgram.m_arrVariables = new Variable[this.m_arrVariables.Length];
    //        this.m_arrVariables.CopyTo(this.m_treProgram.m_arrVariables, 0);

    //        // Pass on all the genes (which would be translated to roots in the program).
    //        for (int nGene = 0; nGene < this.m_nNumberOfGenes; nGene++)
    //        {
    //            // Construct a queue for remembering the nodes in the next level, from left
    //            // To right.
    //            // The construction will be like this : 
    //            // At the start, the first node would be the root ofcourse,
    //            // If the type to put in that node is a function type,
    //            // Then a function node will be created,
    //            // And the queue will be inserted the sons of the function,
    //            // In order to remember them,
    //            // And go to them later in a left to right , per tree level, basis.

    //            // Get the start index of the gene.
    //            int nStartIndex = nGene * this.m_nGeneLength;
    //            int nAfterFatherIndex = nStartIndex + 1;

    //            // Prepare a queue for the decoding and building the tree.
    //            Queue<Node> queLeftToRightNodes = new Queue<Node>();

    //            // First, decode only the father. If the father is a function,
    //            // Then DecodeIntoTree will automatically insert it to the queue.
    //            this.DecodeIntoTree(this.m_arrGenome[nStartIndex],
    //                queLeftToRightNodes, out this.m_treProgram.m_arrRoots[nGene]);

    //            // This function takes the queue (with the father, that should be a function)
    //            // and starts decoding from index 1. The function decodes the children
    //            // Of the father, and recursively decodes also their children.
    //            this.RecursivelyBuildTree(queLeftToRightNodes, nAfterFatherIndex);
    //        }
    //    }

    //    public void DestroyTreeProgram()
    //    {
    //        this.m_treProgram = null;
    //    }

    //    public override void Run()
    //    {
    //        // If the user tried to run the program, but it was not yet prepared
    //        // (For some reasons), then build the tree progarm temporarly, just so that
    //        // The user will be able to run the program once.
    //        if (this.m_treProgram == null)
    //        {
    //            this.BuildTreeProgram();
    //            this.m_treProgram.Run();
    //            this.m_treProgram.m_fReturnedResult.CopyTo(this.m_fReturnedResult, 0);
    //            this.DestroyTreeProgram();
    //        }
    //        else
    //        {   // In a normal operation the program should be set.
    //            // Run the program.
    //            this.m_treProgram.Run();

    //            // Copy the results obtained from the tree program.
    //            this.m_treProgram.m_fReturnedResult.CopyTo(this.m_fReturnedResult, 0);
    //        }
    //    }

    //    #endregion

    //    #region ICloneable Members

    //    public override object Clone()
    //    {
    //        // Construct a new program with the same number of roots
    //        GeneExpressionProgram progNewProgram =
    //            new GeneExpressionProgram(this.m_nNumberOfGenes, this.m_nGeneLength,
    //                this.m_arrVariables);

    //        // Copy basic data members
    //        progNewProgram.m_fFitness = this.m_fFitness;
    //        this.m_fReturnedResult.CopyTo(progNewProgram.m_fReturnedResult, 0);

    //        // Prepare the genome for copying. Since it contains variables, Which are,
    //        // BTW, also appear in the variables array, they both should reference the same
    //        // Location in memory (and not other, since they are being cloned).

    //        // Pass on the genome. clone all the variables you see. all the cloned variables
    //        // Will be inserted to the variables list, so they will both reference the same
    //        // Memory.
    //        progNewProgram.m_arrVariables = new Variable[this.m_arrVariables.Length];
    //        for (int i = 0; i < this.m_arrVariables.Length; i++)
    //        {
    //            progNewProgram.m_arrVariables[i] = (Variable)this.m_arrVariables[i].Clone();
    //        }

    //         // First copy the genome to the other program.
    //        progNewProgram.m_arrGenome = new GeneralType[this.m_arrGenome.Length];

    //        // Now move on the both genomes, find the variables that relate to this program,
    //        // And relate them to variables in the other program.
    //        // This is done, by finding the index of the variable in this program,
    //        // And setting the variable of the genome to be in the same index, but for the other
    //        // Program.
    //        for (int i = 0; i < this.m_arrGenome.Length; i++)
    //        {
    //            progNewProgram.m_arrGenome[i] = 
    //                this.MoveTypeToOtherProgram(this.m_arrGenome[i], progNewProgram);
    //        }

    //        // Copy the randomable types
    //        progNewProgram.m_arrRandomableValues.AddRange(this.m_arrRandomableValues);

    //        return (progNewProgram);
    //    }

    //    #endregion
    //}
}
