using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AO3_Formatter
{
    static class VERSIONING
    {
        /// <summary>
        /// The current version of the core (the code without any project specifications). This allows
        /// individual builds that are project-oriented to have some way of determining when their last
        /// merge from master was. This should be updated incrementally every time there is a change to
        /// the master branch.
        /// </summary>
        public static string CORE_VERSION = "v1.0";
    }
}
