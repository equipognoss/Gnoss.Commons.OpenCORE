using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    public class GnossStringBuilder
    {
        private static int NUM_MAX_LINES = 1400;
        private List<StringBuilder> stringBuilderList = null;
        private int count = 0;

        public GnossStringBuilder()
        {
            stringBuilderList = new List<StringBuilder>();
        }

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

        public List<StringBuilder> GetStringBuilder()
        {
            return stringBuilderList;
        }

        public int GetCount()
        {
            return count;
        }

        public void SetCount(int pNumLines)
        {
            count = pNumLines;
        }
    }
}