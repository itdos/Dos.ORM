using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Dos.ORM
{
    public interface IEvaluator
    {
        object Eval(System.Linq.Expressions.Expression exp);
    }
}
