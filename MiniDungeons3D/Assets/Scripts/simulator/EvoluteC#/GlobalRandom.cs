using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace TreeLanguageEvolute
{
 public class ImprovedRandom
    {
        private RNGCryptoServiceProvider m_rndRandom;

        public ImprovedRandom()
        {
            this.m_rndRandom = new RNGCryptoServiceProvider();
        }
        public int Next(int nMin, int nMax)
        {
            byte[] b = new byte[4];
            m_rndRandom.GetBytes(b);
            uint dwRandomNumber = ((uint)b[0] << 24) + ((uint)b[1] << 16) + ((uint)b[2] << 8) + (uint)b[3];
            if ((nMax - nMin) == 0)
            {
                return 0;
            }

            uint dwResult = (uint)nMin + (dwRandomNumber % ((uint)(nMax + 1) - (uint)nMin));
            return ((int)dwResult);
        }

        public int Next(int nMax)
        {
            return (this.Next(0, nMax - 1));
        }

        public float NextDouble()
        {
            byte[] b = new byte[4];
            m_rndRandom.GetBytes(b);
            uint dwRandomNumber = ((uint)b[0] << 24) + ((uint)b[1] << 16) + ((uint)b[2] << 8) + (uint)b[3];
            float fResult = (float)dwRandomNumber / uint.MaxValue;
            return (fResult);
        }
    }

    /// <summary>
    /// The global random class is just a static class which contains a randomized
    /// Random, just in order to not create it again and again through the 
    /// Program. The user can use this random as well.
    /// </summary>
    public static class GlobalRandom
    {
        public static ImprovedRandom m_rndRandom = new ImprovedRandom();
    }
}
