using DotNetGraph.Core;
using DotNetGraph.Extensions;
using TrashlyLang.lexer;

namespace TrashlyLang.ast;

//node is a statement
public class Node
{
	public Token Token;
	public Guid GUID = Guid.NewGuid();
	public Node(Token token)
	{
		Token = token;
	}


	public override string ToString()
	{
		return this.Token.Type.ToString();
	}

	public virtual void ProcessGraph(DotGraph graph)
	{
		var node = GetGraphNode(); 
		graph.Add(node);
	}

	public virtual DotNode GetGraphNode()
	{
		return new DotNode().WithIdentifier(GUID.ToString()).WithLabel(Token.Type.ToString());	
	}
}