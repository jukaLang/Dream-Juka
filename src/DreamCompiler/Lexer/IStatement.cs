using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DReAMCompiler.Lexer
{
    public enum StatementType
    {
        IfStatement,
        WhileStatment,
        CallStatement,
        DeclStatment,
        ReturnStatement,
    }

    public interface IStatement
    {
        StatementType Kind { get; set; }
    }

    public abstract class Statement : IStatement
    {
        public StatementType Kind { get; set; }
    }

    public class IfStatement : Statement
    {

    }




}
