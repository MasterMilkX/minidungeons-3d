using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeLanguageEvolute
{
    /// <summary>
    /// The statistics class returns statistics about the algorithm that is being
    /// Ran.
    /// </summary>
    public class Statistics
    {
        #region Data members

        BaseEngine m_engEngine; // The engine 

        #endregion

        #region Constructor

        /// <summary>
        /// Consturct a new statistics from an Engine class
        /// </summary>
        /// <param name="engEngine">The engine</param>
        public Statistics(BaseEngine engEngine)
        {
            this.m_engEngine = engEngine;
        }

        #endregion

        #region Properties

        public BaseProgram GetMinFitnessProgramForIsland(int nIslandIndex)
        {
            // Sample the first program as a starting fitness
            BaseProgram progMinFitness = this.m_engEngine.Population[nIslandIndex][0];
            float fMinFitness = progMinFitness.m_fFitness;

            // Go through all the programs and get the program with the
            // Minimal fitness
            foreach (BaseProgram progProgram in this.m_engEngine.Population[nIslandIndex])
            {
                // If the current program has least fitness than all the rest so far
                // Then it's the most minimal fitness program.
                if (fMinFitness > progProgram.m_fFitness)
                {
                    fMinFitness = progProgram.m_fFitness;
                    progMinFitness = progProgram;
                }
            }

            return (progMinFitness);
        }

        /// <summary>
        /// Get the program with the max fitness
        /// </summary>
        public BaseProgram MaxFitnessProgram
        {
            get
            {
                // Sample the first program as a starting fitness
                BaseProgram progMaxFitness = this.m_engEngine.Population[0][0];
                float fMaxFitness = progMaxFitness.m_fFitness;

                // Go through all the programs and get the program with the
                // Maximal fitness
                for (int nIslandIndex = 0;
                     nIslandIndex < this.m_engEngine.Population.Length;
                     nIslandIndex++)
                {
                    foreach (BaseProgram progProgram in this.m_engEngine.Population[nIslandIndex])
                    {
                        // If the current program has more fitness than all the rest so far
                        // Then it's the most maximal fitness program.
                        if (fMaxFitness < progProgram.m_fFitness)
                        {
                            fMaxFitness = progProgram.m_fFitness;
                            progMaxFitness = progProgram;
                        }
                    }
                }

                return (progMaxFitness);
            }
        }


        /// <summary>
        /// Get the program with the min fitness
        /// </summary>
        public BaseProgram MinFitnessProgram
        {
            get
            {
                // Sample the first program as a starting fitness
                BaseProgram progMinFitness = this.m_engEngine.Population[0][0];
                float fMinFitness = progMinFitness.m_fFitness;

                // Go through all the programs and get the program with the
                // Minimal fitness
                for (int nIslandIndex = 0;
                  nIslandIndex < this.m_engEngine.Population.Length;
                  nIslandIndex++)
                {
                    foreach (BaseProgram progProgram in this.m_engEngine.Population[nIslandIndex])
                    {
                        // If the current program has least fitness than all the rest so far
                        // Then it's the most minimal fitness program.
                        if (fMinFitness > progProgram.m_fFitness)
                        {
                            fMinFitness = progProgram.m_fFitness;
                            progMinFitness = progProgram;
                        }
                    }
                }

                return (progMinFitness);
            }
        }

		/// <summary>
		/// Get the avg fitness of all the programs
		/// </summary>
		public float AvgFitness
		{
			get
			{
				// Sample the first program as a starting fitness
				float fAvgFitness = 0;
				int counter = 0;
				// Go through all the programs and get the program with the
				// Minimal fitness
				for (int nIslandIndex = 0;
					nIslandIndex < this.m_engEngine.Population.Length;
					nIslandIndex++)
				{
					foreach (BaseProgram progProgram in this.m_engEngine.Population[nIslandIndex])
					{
						// Add the current program fitness to avg
						fAvgFitness += progProgram.Fitness;
						counter++;
					}
				}
				fAvgFitness = fAvgFitness / counter;

				return (fAvgFitness);
			}
		}
        /// <summary>
        /// Get the total nodes that is in the population
        /// </summary>
        public int TotalNodes
        {
            get
            {
                int nTotalNodes = 0;

                for (int nIslandIndex = 0;
                  nIslandIndex < this.m_engEngine.Population.Length;
                  nIslandIndex++)
                {
                    // Sum all the nodes of all the programs, and then return it.
                    foreach (TreeProgram progProgram in this.m_engEngine.Population[nIslandIndex])
                    {
                        nTotalNodes += progProgram.m_arrRoots[0].Count;
                    }
                }

                return (nTotalNodes);
            }
        }

        /// <summary>
        /// Get the current generation number so far in the algorithm
        /// </summary>
        public int GenerationNumber
        {
            get
            {
                return (this.m_engEngine.GenerationNum);
            }
        }

        #endregion
    }
}
