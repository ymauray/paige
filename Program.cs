using Paige;
using System.CommandLine;
using System.CommandLine.Invocation;

// 1. Définition de l'option (Le constructeur utilise souvent l'alias direct)
var rootOption = new Option<string>("--project-root")
{
    Description = "Le dossier racine du projet EPUB.",
};

// 2. Création de la commande racine
var rootCommand = new RootCommand("Paige : Générateur d'EPUB");
rootCommand.Options.Add(rootOption);

// 3. Définition de l'action
rootCommand.SetAction((ParseResult result) =>
{
    // On récupère la valeur via le ParseResult
    string rootPath = result.GetValue(rootOption) ?? "epub";

    Console.WriteLine($"Dossier racine : {rootPath}");
    var epub = new Epub();
    string fullPath = Path.GetFullPath(rootPath);

    if (!Directory.Exists(fullPath) )
    {
        Directory.CreateDirectory(fullPath);
    }
    fullPath = Path.Combine(fullPath, "mybook.epub");
    epub.Write(fullPath);
});

// 4. Exécution (Plus besoin de InvokeAsync, Invoke suffit ou InvokeAsync si besoin)
return rootCommand.Parse(args).Invoke();