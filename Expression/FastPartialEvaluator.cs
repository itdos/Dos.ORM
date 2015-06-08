using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Dos.ORM
{
    public class FastPartialEvaluator : PartialEvaluatorBase
    {
        public FastPartialEvaluator()
            : base(new FastEvaluator())
        { }
    }
}
