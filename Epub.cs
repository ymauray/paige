using System.IO.Compression;

namespace Paige
{
    public class Epub
    {
        public void Write(string filename)
        {
            using var outputStream = new FileStream(filename, FileMode.Create);
            using var output = new ZipArchive(outputStream, ZipArchiveMode.Create);

            // 1. Le fichier 'mimetype' - DOIT être le premier et NON COMPRESSÉ
            // Note: ZipArchive ne permet pas facilement de forcer le "Store" (0% compression)
            // via l'API simple, mais pour un test, créons-le normalement :
            var mimetypeEntry = output.CreateEntry("mimetype", CompressionLevel.NoCompression);
            using (var writer = new StreamWriter(mimetypeEntry.Open()))
            {
                writer.Write("application/epub+zip");
            }

            // 2. Le dossier META-INF et le fichier container.xml
            // Il indique à la liseuse où se trouve le fichier de description (OPF)
            var containerEntry = output.CreateEntry("META-INF/container.xml");
            using (var writer = new StreamWriter(containerEntry.Open()))
            {
                writer.Write("""
            <?xml version="1.0" encoding="UTF-8"?>
            <container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
                <rootfiles>
                    <rootfile full-path="OEBPS/content.opf" media-type="application/oebps-package+xml"/>
                </rootfiles>
            </container>
            """);
            }

            // 3. Le fichier de contenu OPF (le cerveau de l'EPUB)
            var opfEntry = output.CreateEntry("OEBPS/content.opf");
            using (var writer = new StreamWriter(opfEntry.Open()))
            {
                writer.Write("""
            <?xml version="1.0" encoding="UTF-8"?>
            <package xmlns="http://www.idpf.org/2007/opf" unique-identifier="pub-id" version="3.0">
                <metadata xmlns:dc="http://purl.org/dc/elements/1.1/">
                    <dc:identifier id="pub-id">12345</dc:identifier>
                    <dc:title>Mon Livre Paige</dc:title>
                    <dc:language>fr</dc:language>
                    <meta property="dcterms:modified">2024-05-22T12:00:00Z</meta>
                </metadata>
                <manifest>
                    <item id="nav" href="nav.xhtml" media-type="application/xhtml+xml" properties="nav"/>
                    <item id="chap1" href="chapitre1.xhtml" media-type="application/xhtml+xml"/>
                </manifest>
                <spine>
                    <itemref idref="chap1"/>
                </spine>
            </package>
            """);
            }

            // 4. Un chapitre minimal (XHTML obligatoire, pas de HTML simple)
            var chapterEntry = output.CreateEntry("OEBPS/chapitre1.xhtml");
            using (var writer = new StreamWriter(chapterEntry.Open()))
            {
                writer.Write("""
            <?xml version="1.0" encoding="UTF-8"?>
            <!DOCTYPE html>
            <html xmlns="http://www.w3.org/1999/xhtml">
            <head><title>Chapitre 1</title></head>
            <body>
                <h1>Hello .NET 10</h1>
                <p>Ceci est un EPUB généré manuellement.</p>
            </body>
            </html>
            """);
            }

            // 5. Le fichier de navigation (nav.xhtml) pour EPUB 3
            var navEntry = output.CreateEntry("OEBPS/nav.xhtml");
            using (var writer = new StreamWriter(navEntry.Open()))
            {
                writer.Write("""
        <?xml version="1.0" encoding="UTF-8"?>
        <!DOCTYPE html>
        <html xmlns="http://www.w3.org/1999/xhtml" xmlns:epub="http://www.idpf.org/2007/ops">
        <head>
            <title>Table des matières</title>
        </head>
        <body>
            <nav epub:type="toc" id="toc">
                <h1>Sommaire</h1>
                <ol>
                    <li><a href="chapitre1.xhtml">Premier Chapitre</a></li>
                </ol>
            </nav>
        </body>
        </html>
        """);
            }
        }
    }
}
