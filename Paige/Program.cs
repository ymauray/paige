using Paige;
using System.CommandLine;
using System.CommandLine.Invocation;

var rootOption = new Option<string>("--project-root")
{
    Description = "Le dossier racine du projet EPUB.",
};

var rootCommand = new RootCommand("Paige : Générateur d'EPUB");
rootCommand.Options.Add(rootOption);

rootCommand.SetAction(result =>
{
    try
    {
        string rootPath = result.GetValue(rootOption) ?? ".";
        string fullPath = Path.GetFullPath(rootPath);

        if (!Directory.Exists(fullPath))
        {
            Console.Error.WriteLine($"Erreur : Le dossier '{fullPath}' n'existe pas.");
            Environment.ExitCode = 1;
            return;
        }

        var paigeFiles = Directory.GetFiles(fullPath, "*.paige");
        if (paigeFiles.Length == 0)
        {
            Console.Error.WriteLine("Erreur : Aucun fichier .paige trouvé dans le dossier racine.");
            Environment.ExitCode = 1;
            return;
        }

        var source = File.ReadAllText(paigeFiles[0]);
        var doc = Parser.Parse(source, fullPath);

        Epub.Write(doc, fullPath, "mybook.epub");
        Console.WriteLine($"EPUB généré : {Path.Combine(fullPath, "mybook.epub")}");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Erreur fatale : {ex.Message}");
        Environment.ExitCode = 1;
    }
});

return rootCommand.Parse(args).Invoke();
