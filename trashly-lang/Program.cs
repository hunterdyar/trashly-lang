
using System.Text;
using LLVMSharp;
using LLVMSharp.Interop;
using TrashlyLang.evaluator;
using TrashlyLang.lexer;
using TrashlyLang.memory;
using TrashlyLang.Parser;
using Environment = TrashlyLang.memory.Environment;

class TrashlyLangRepl
{
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
		
		file = args[0];
	}

	static unsafe async Task Build(string program)
	{
		Lexer lex = new Lexer(program);
		Parser parser = new Parser(lex);
		parser.Parse();
		//now we have an AST
		LLVMModuleRef module = LLVM.ModuleCreateWithName((sbyte*)0);
		LLVMBuilderRef builder = LLVM.CreateBuilder();
		

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