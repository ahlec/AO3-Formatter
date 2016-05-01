using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AO3_Formatter
{
    public static class Extensions
    {
        public static void Splice(this StringBuilder builder, String replacement, int startIndex, int count)
        {
            for (int index = 0; index < replacement.Length; ++index)
            {
                if (index >= count)
                {
                    builder.Insert(index + startIndex, replacement[index]);
                }
                else if (builder.Length <= index + startIndex)
                {
                    builder.Append(replacement[index]);
                }
                else
                {
                    builder[index + startIndex] = replacement[index];
                }
            }
        }
    }
}
