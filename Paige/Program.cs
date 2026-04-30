using Paige;
using System.CommandLine;
using System.CommandLine.Invocation;

var rootOption = new Option<string>("--project-root")
{
    Description = "Le dossier racine du projet EPUB.",
};

var rootCommand = new RootCommand("Paige : Générateur d'EPUB");
rootCommand.Options.Add(rootOption);

rootCommand.SetAction((ParseResult result) =>
{
    string rootPath = result.GetValue(rootOption) ?? ".";
    string fullPath = Path.GetFullPath(rootPath);

    var paigeFiles = Directory.GetFiles(fullPath, "*.paige");
    if (paigeFiles.Length == 0)
    {
        Console.WriteLine("Aucun fichier .paige trouvé dans le dossier racine.");
        return;
    }

    var source = File.ReadAllText(paigeFiles[0]);
    var doc = Parser.Parse(source);

    Epub.Write(doc, fullPath, "mybook.epub");
    Console.WriteLine($"EPUB généré : {Path.Combine(fullPath, "mybook.epub")}");
});

return rootCommand.Parse(args).Invoke();
