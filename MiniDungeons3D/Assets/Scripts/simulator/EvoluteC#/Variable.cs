using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeLanguageEvolute
{
    /// <summary>
    /// The variable class contains a value which is called a variable.
    /// </summary>
    public class Variable : ValueType
    {
        float m_fValue;
        string m_strName;

        /// <summary>
        /// The value of the variable
        /// </summary>
        public float Value
        {
            get
            {
                return (this.m_fValue);
            }
            set
            {
                this.m_fValue = value;
            }
        }

        /// <summary>
        /// Get the name of the variable
        /// </summary>
        public override string Name
        {
            get
            {
                return (this.m_strName);
            }
        }

        /// <summary>
        /// Constructs a new Variable
        /// </summary>
        public Variable()
        {
            this.m_strName = "not initialised";
        }

        /// <summary>
        /// Returns the value of the variable
        /// </summary>
        /// <returns>The value of the variable</returns>
        public override float ReturnValue()
        {
            return (this.m_fValue);
        }

        /// <summary>
        /// Sets the name of the variable
        /// </summary>
        /// <param name="strName">The variable name</param>
        public void SetName(string strName)
        {
            this.m_strName = strName;
        }

        public Object Clone()
        {
            return (this.MemberwiseClone());
        }
    }
}
