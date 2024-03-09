using DotNetGraph.Core;
using DotNetGraph.Extensions;
using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class InfixExpression : Expression
{
	public Expression left;
	public Expression right;
	//string? optype?
	public string Operator;
	
	public InfixExpression(Token token) : base(token)
	{
	}

	public override string ToString()
	{ return $"({Operator} {left.ToString()} {right.ToString()})";
	}

	public override void ProcessGraph(DotGraph graph)
	{
		var node = GetGraphNode();
		var l = left.GetGraphNode();
		left.ProcessGraph(graph);
		var r = right.GetGraphNode();
		right.ProcessGraph(graph);
		var el = new DotEdge().From(node).To(l).WithLabel("Left");
		var rl = new DotEdge().From(node).To(r).WithLabel("Right");
		graph.Add(node);
		graph.Add(l).Add(r).Add(el).Add(rl);
	}

	public override DotNode GetGraphNode()
	{
		return base.GetGraphNode();
	}
}