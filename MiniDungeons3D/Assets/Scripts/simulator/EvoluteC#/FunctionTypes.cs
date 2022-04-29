using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeLanguageEvolute
{

    #region FunctionType class

    /// <summary>
    /// This is an abstract class which groups all the function types
    /// </summary>
    public abstract class FunctionType : GeneralType
    {
        public abstract float action(Node[] arrNodes, TreeProgram progOwner);

        public abstract int NumberOfArguments
        {
            get;
        }

        /// <summary>
        /// Get's the name of the function.
        /// </summary>
        public abstract string Name
        {
            get;
        }
    }

    #endregion

    #region Function class

    /// <summary>
    /// This is an abstract class which groups all the functions
    /// </summary>
    public abstract class Function : FunctionType
    {
    }

    #endregion

    #region Various Functions

    #region Add function

    /// <summary>
    /// The add function, Needs two tree sons, adds them both and returns
    /// A result as the sum of the both sons
    /// </summary>
    public class Add : Function
    {
        public override float action(Node[] arrNodes, TreeProgram progOwner)
        {
            return (arrNodes[0].ReturnValue(progOwner) + arrNodes[1].ReturnValue(progOwner));
        }

        public override int NumberOfArguments
        {
            get
            {
                return (2);
            }
        }

        public override string Name
        {
            get 
            {
                return ("+");
            }
        }
    }

    #endregion

    #region Multiply function

    /// <summary>
    /// The multiply function, Needs two tree sons, adds them both and returns
    /// A result as the multiplication of the both sons
    /// </summary>
    public class Multiply : Function
    {
        public override float action(Node[] arrNodes, TreeProgram progOwner)
        {
            return (arrNodes[0].ReturnValue(progOwner) * arrNodes[1].ReturnValue(progOwner));
        }

        public override int NumberOfArguments
        {
            get
            {
                return (2);
            }
        }

        public override string Name
        {
            get
            {
                return ("*");
            }
        }
    }

    #endregion

    #region Substract function

    /// <summary>
    /// The substract function, Needs two tree sons, substracts the second son from the
    /// First son, and returns a result.
    /// </summary>
    public class Substract : Function
    {
        public override float action(Node[] arrNodes, TreeProgram progOwner)
        {
            return (arrNodes[0].ReturnValue(progOwner) - arrNodes[1].ReturnValue(progOwner));
        }

        public override int NumberOfArguments
        {
            get
            {
                return (2);
            }
        }

        public override string Name
        {
            get
            {
                return ("-");
            }
        }
    }

    #endregion

    #region Divide function

    /// <summary>
    /// The divide function, Needs two tree sons, Divides the first son
    /// The second son and returns the result.
    /// </summary>
    public class Divide : Function
    {
        public override float action(Node[] arrNodes, TreeProgram progOwner)
        {
            float fLeftValue = arrNodes[0].ReturnValue(progOwner);
            float fRightValue = arrNodes[1].ReturnValue(progOwner);
            return (fLeftValue / fRightValue);
        }

        public override int NumberOfArguments
        {
            get
            {
                return (2);
            }
        }

        public override string Name
        {
            get
            {
                return ("/");
            }
        }
    }

    #endregion

    #region Modulo function

    /// <summary>
    /// The modulo function, Needs two tree sons, modulos the first son
    /// The second son and returns the result.
    /// </summary>
    public class Modulo : Function
    {
        public override float action(Node[] arrNodes, TreeProgram progOwner)
        {
            float fLeftValue = arrNodes[0].ReturnValue(progOwner);
            float fRightValue = arrNodes[1].ReturnValue(progOwner);
            return (fLeftValue % fRightValue);

        }

        public override int NumberOfArguments
        {
            get
            {
                return (2);
            }
        }

        public override string Name
        {
            get
            {
                return ("%");
            }
        }
    }

    #endregion

    #region Cosinus function

    /// <summary>
    /// The Cosinus function, Returns the value of the cosinus of the son.
    /// </summary>
    public class Cos : Function
    {
        public override float action(Node[] arrNodes, TreeProgram progOwner)
        {
            return ((float)Math.Cos(arrNodes[0].ReturnValue(progOwner)));

        }

        public override int NumberOfArguments
        {
            get
            {
                return (1);
            }
        }

        public override string Name
        {
            get
            {
                return ("Cos");
            }
        }
    }

    #endregion

    #region IfGreaterThenElse function

    /// <summary>
    /// The IfGreaterThenElse function, Needs two tree sons, if the first node
    /// Is bigger than the second node, then the result returned is the third node.
    /// Otherwise, the result returned is the fourth node.
    /// </summary>
    public class IfGreaterThenElse : Function
    {
        public override float action(Node[] arrNodes, TreeProgram progOwner)
        {
            float fFirstNode = arrNodes[0].ReturnValue(progOwner);
            float fSecondNode = arrNodes[1].ReturnValue(progOwner);
            if (fFirstNode > fSecondNode)
            {
                return (arrNodes[2].ReturnValue(progOwner));
            }
            else
            {
                return (arrNodes[3].ReturnValue(progOwner));
            }
        }

        public override int NumberOfArguments
        {
            get
            {
                return (4);
            }
        }

        public override string Name
        {
            get
            {
                return ("ITGE");
            }
        }
    }

    #endregion

    #endregion
}
