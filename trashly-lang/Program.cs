using TrashlyLang.lexer;
using TrashlyLang.Parser;

class TrashlyLangRepl
{
	private static bool viewLex = false;
	static void Main()
	{
		Repl(Console.In, Console.Out);
	}

	static void Repl(TextReader reader, TextWriter writer)
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
			foreach(var statement in parser.Program)
			{
				writer.WriteLine(statement.ToString());
			}
		}
	}
}