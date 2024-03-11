
using System.Text;
using DotNetGraph.Compilation;
using DotNetGraph.Core;
using DotNetGraph.Extensions;
using TrashlyLang.evaluator;
using TrashlyLang.lexer;
using TrashlyLang.memory;
using TrashlyLang.Parser;
using Environment = TrashlyLang.memory.Environment;

class TrashlyLangRepl
{
	private static bool viewLex = false;
	private static bool graph = false;
	private static bool repl = true;
	static async Task<int> Main(string[] args)
	{
		ParseArgs(args, out string fileName);
		repl = fileName == "";
		if (repl)
		{
			await Repl(Console.In, Console.Out);
			return 0;
		}
		else
		{
			string program = await File.ReadAllTextAsync(fileName);
			var output = await Execute(program,null);
			Console.Write(output);
			return(0);
		}
	}

	private static void ParseArgs(string[] args, out string file)
	{
		file = "";
		if (args.Length == 0)
		{
			return;
		}
		
		viewLex = args.Contains("-l") || args.Contains("--lexer");
		graph = args.Contains("-t") || args.Contains("--tree");
		
		file = args[0];
	}

	static async Task<string> Execute(string program, Environment? environment)
	{
		Memory m;
		StringBuilder output = new StringBuilder();
		if (environment == null)
		{
			m = new Memory(32,32);
			environment = new Environment(m);
		}
		else
		{
			m = environment.Memory;
		}
		
		if (viewLex)
        {
        	Lexer dLEx = new Lexer(program);
        	var a = dLEx.GetAllLexDebug();
        	foreach (var t in a)
        	{
        		Console.WriteLine(t.ToString());
        	}

        	Console.WriteLine("------");
        }

        Lexer lex = new Lexer(program);
        Parser parser = new Parser(lex); 
        parser.Parse();
        if (parser.Errors.Count > 0)
        {
        	output.AppendLine("ERROR - Parsing Error... It's likely that only the first error is the problem.");
        	foreach (var error in parser.Errors)
        	{
        		output.AppendLine(error);
        	}
        }
        else
        {
	        //no errors Evaluate.
	        Evaluator evaluator = new Evaluator(environment);
	        var r = evaluator.EvaluateProgram(parser);

	        output.AppendLine(r.Inspect());
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
        	output.AppendLine("---");
        	output.AppendLine("'" + url + "'");
        }

        await m.Export();
        return output.ToString();
	}
	static async Task Repl(TextReader reader, TextWriter writer)
	{
		bool repel = true;
		Memory m = new Memory(32,32);
		await m.Export();
		Environment? env = new Environment(m);
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

			var output = Execute(line, env);
		}
	}
}