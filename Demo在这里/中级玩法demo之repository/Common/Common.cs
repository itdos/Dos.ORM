using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dos.Common;

namespace Common
{
    public class Common
    {
        public static List<TResult> Map<TEntity, TResult>(List<TEntity> input)
        {
            return MapperHelper.Map<TEntity, TResult>(input);
        }
    }
}
