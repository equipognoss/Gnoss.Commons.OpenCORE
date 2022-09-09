using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Prepared class to use in Sparql laguage
    /// </summary>
    public class GnossStringBuilder
    {
        private static int NUM_MAX_LINES = 1400;
        private List<StringBuilder> stringBuilderList = null;
        private int count = 0;
        private int mLength = 0;

        /// <summary>
        /// It returns the length of the current instance
        /// </summary>
        public int Length
        {
            get
            {
                if (mLength == 0)
                {
                    foreach (StringBuilder stringBuilder in stringBuilderList)
                    {
                        mLength += stringBuilder.Length;
                    }
                }
                return mLength;
            }
        }

        /// <summary>
        /// Class prepared to use in Sparql language
        /// </summary>
        public GnossStringBuilder()
        {
            stringBuilderList = new List<StringBuilder>();
        }

        /// <summary>
        /// Apend new line to the current instance
        /// </summary>
        /// <param name="pStringToAppend">String to append</param>
        public void AppendLine(string pStringToAppend)
        {
            if (count % NUM_MAX_LINES == 0)
            {
                stringBuilderList.Add(new StringBuilder());
            }
            int index = count / NUM_MAX_LINES;
            stringBuilderList[index].AppendLine(pStringToAppend);
            count++;
        }

        /// <summary>
        /// Insert the given string in the indicated position 
        /// </summary>
        /// <param name="pIndex"></param>
        /// <param name="pStringToAppend"></param>
        public void Insert(int pIndex, string pStringToAppend)
        {
            stringBuilderList[0].Insert(pIndex, pStringToAppend);
        }

        /// <summary>
        /// Gets a list of StringBuilder formed by grups of 1400 lines
        /// </summary>
        /// <returns>List of StringBuilder</returns>
        public List<StringBuilder> GetStringBuilder()
        {
            return stringBuilderList;
        }

        /// <summary>
        /// Get the num of lines of the final String
        /// </summary>
        /// <returns>Num of lines</returns>
        public int GetCount()
        {
            return count;
        }

        /// <summary>
        /// Set the number of lines
        /// </summary>
        /// <param name="pNumLines">New num of lines</param>
        public void SetCount(int pNumLines)
        {
            count = pNumLines;
        }
    }
}