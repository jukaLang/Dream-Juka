using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamCompiler.Expressions
{
    internal class BinaryExpression : IExpression
    {
        internal IExpression left;
        internal IExpression op;
        internal IExpression right;
    }
}
