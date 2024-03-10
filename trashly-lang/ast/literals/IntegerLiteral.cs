using DotNetGraph.Core;
using DotNetGraph.Extensions;
using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class IntegerLiteral : Expression
{
	private string literal;
	public IntegerLiteral(Token token) : base(token)
	{
		literal = token.Literal;
	}

	public override string ToString()
	{
		return literal;
	}

	public override DotNode GetGraphNode()
	{
		return new DotNode().WithIdentifier(GUID.ToString()).WithLabel(Token.Literal.ToString());	
	}
}