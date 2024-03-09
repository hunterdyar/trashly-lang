using System.Diagnostics;
using System.Web;
using DotNetGraph.Compilation;
using DotNetGraph.Core;
using DotNetGraph.Extensions;
using TrashlyLang.lexer;
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

			Lexer lex = new Lexer(line);
			Parser parser = new Parser(lex);
			parser.Parse();
			foreach (var statement in parser.Program)
			{
				writer.Write(statement.ToString()+"\n");
			}
			
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
		}
	}
}