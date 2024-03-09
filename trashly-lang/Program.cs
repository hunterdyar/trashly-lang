using TrashlyLang.lexer;

class TrashlyLangRepl
{
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

			Lexer lex = new Lexer(line);
			var toks = lex.GetAllLex();

			foreach(var token in toks)
			{
				writer.WriteLine(token.Type.ToString());
			}
		}
	}
}