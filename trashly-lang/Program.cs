
using DotNetGraph.Compilation;
using DotNetGraph.Core;
using DotNetGraph.Extensions;
using TrashlyLang.evaluator;
using TrashlyLang.lexer;
using TrashlyLang.memory;
using TrashlyLang.Parser;

class TrashlyLangRepl
{
	private static bool viewLex = false;
	private static bool graph = false;
	static async Task Main()
	{
		await Repl(Console.In, Console.Out);
	}

	static async Task Repl(TextReader reader, TextWriter writer)
	{
		bool repel = true;
		while (repel)
		{
			Console.Write("~~> ");
			string? line = reader.ReadLine();
			if (line == null)
			{
				break;
			}else if (line == "exit" || line == "quit")
			{
				break;
			}

			if (viewLex)
			{
				Lexer dLEx = new Lexer(line);
				var a = dLEx.GetAllLexDebug();
				foreach (var t in a)
				{
					writer.WriteLine(t.Type);
				}

				writer.WriteLine("------");
			}

			Memory m = new Memory(128,128);
			Lexer lex = new Lexer(line);
			Parser parser = new Parser(lex);
			parser.Parse();
			Evaluator evaluator = new Evaluator();
			foreach (var statement in parser.Program)
			{
				var o = evaluator.Eval(statement);
				writer.Write(o.Inspect()+"\n");
			}
			
			//todo: move the dependency to graph to it's own class/area.
			if (graph)
			{
				var graph = new DotGraph().WithIdentifier("Program Root");
				foreach (var statement in parser.Program)
				{
					statement.ProcessGraph(graph);
				}

				await using var gwriter = new StringWriter();
				var context = new CompilationContext(gwriter, new CompilationOptions());
				await graph.CompileAsync(context);

				var result = gwriter.GetStringBuilder().ToString();

				// Save it to a file
				//await File.WriteAllTextAsync("graph.dot", result);
				var url = "https://dreampuf.github.io/GraphvizOnline/#" + Uri.EscapeDataString(result);
				writer.WriteLine("---");
				writer.WriteLine("'" + url + "'");
			}

			await m.Export();
		}
	}
}