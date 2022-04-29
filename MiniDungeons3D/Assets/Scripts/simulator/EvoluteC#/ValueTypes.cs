using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeLanguageEvolute
{
    #region ValueType class

    /// <summary>
    /// The class is an abstract class which represeents all the possible types of 
    /// Values, Which are RandomMacro and Value
    /// </summary>
    public abstract class ValueType : GeneralType
    {
        /// <summary>
        /// An abstract function which returns the value of the variableType
        /// </summary>
        /// <returns>The value of the variableType</returns>
        public abstract float ReturnValue();

        /// <summary>
        /// Gets the name of the VariableType
        /// </summary>
        public abstract string Name
        {
            get;
        }
    }

    #endregion

    #region RandomMacro class

    /// <summary>
    /// The random macro class is a class that generates random values
    /// </summary>
    public class RandomMacro : ValueType
    {
        /// <summary>
        /// Returns a random value
        /// </summary>
        /// <returns>Returns a random value</returns>
        public override float ReturnValue()
        {
            return (float)(GlobalRandom.m_rndRandom.NextDouble());
        }
        
        /// <summary>
        /// Get a name for the RandomMacro (just returns "Random Macro")
        /// </summary>
        public override string Name
        {
            get
            {
                return ("Random Macro");
            }
        }
    }

    #endregion

    #region Value class

    /// <summary>
    /// A value class, which represents a value.
    /// </summary>
    public class Value : ValueType
    {
        float m_fValue;

        /// <summary>
        /// Returns the value
        /// </summary>
        /// <returns>The value</returns>
        public override float ReturnValue()
        {
            return (this.m_fValue);
        }

        /// <summary>
        /// Returns the name (which is the value)
        /// </summary>
        public override string Name
        {
            get
            {
                return (this.m_fValue.ToString());
            }
        }

        public Value(float fValue)
        {
            this.m_fValue = fValue;
        }
    }

    #endregion
}