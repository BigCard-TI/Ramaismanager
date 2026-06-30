using System.Xml.Linq;
using RamaisManager.Models;

namespace RamaisManager.Services;

public class RamalConflictException : Exception
{
    public RamalConflictException(string message) : base(message) { }
}

public class RamalFileLockedException : Exception
{
    public RamalFileLockedException(string message) : base(message) { }
}

/// <summary>
/// Responsável por ler e gravar o arquivo Contacts.xml usado pelo MicroSIP.
///
/// Estratégia de concorrência (vários usuários podem editar ao mesmo tempo):
///  - Ao salvar, abrimos o arquivo com exclusividade (FileShare.None) e fazemos
///    algumas tentativas com pequeno delay, caso outra máquina esteja gravando
///    naquele exato instante.
///  - Antes de gravar, comparamos o "carimbo" (LastWriteTimeUtc) do arquivo no disco
///    com o carimbo que tínhamos quando carregamos os dados na tela. Se o arquivo
///    mudou nesse meio tempo (alguém mais salvou primeiro), avisamos o usuário em vez
///    de sobrescrever silenciosamente o trabalho de outra pessoa.
///  - Sempre criamos um backup (.bak com timestamp) antes de sobrescrever o arquivo,
///    numa subpasta "Backups" ao lado do XML original.
/// </summary>
public class RamalXmlService
{
    private const int MaxRetries = 5;
    private const int RetryDelayMs = 400;

    public DateTime? LastLoadedWriteTimeUtc { get; private set; }

    public List<Ramal> Load(string xmlPath)
    {
        if (!File.Exists(xmlPath))
        {
            throw new FileNotFoundException(
                $"Arquivo de ramais não encontrado em:\n{xmlPath}\n\n" +
                "Verifique se o caminho está correto nas configurações (Ctrl+Shift+Alt+C).");
        }

        List<Ramal> result = new();

        Exception? lastError = null;
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                using var stream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var doc = XDocument.Load(stream);

                foreach (var c in doc.Root?.Elements("contact") ?? Enumerable.Empty<XElement>())
                {
                    result.Add(new Ramal
                    {
                        Name = Attr(c, "name"),
                        Number = Attr(c, "number"),
                        FirstName = Attr(c, "firstname"),
                        LastName = Attr(c, "lastname"),
                        Phone = Attr(c, "phone"),
                        Mobile = Attr(c, "mobile"),
                        Email = Attr(c, "email"),
                        Address = Attr(c, "address"),
                        City = Attr(c, "city"),
                        State = Attr(c, "state"),
                        Zip = Attr(c, "zip"),
                        Comment = Attr(c, "comment"),
                        Id = Attr(c, "id"),
                        Info = Attr(c, "info"),
                        Presence = Attr(c, "presence", "0"),
                        Starred = Attr(c, "starred", "0"),
                        Directory = Attr(c, "directory", "0"),
                    });
                }

                LastLoadedWriteTimeUtc = File.GetLastWriteTimeUtc(xmlPath);
                return result;
            }
            catch (IOException ex)
            {
                lastError = ex;
                Thread.Sleep(RetryDelayMs);
            }
        }

        throw new RamalFileLockedException(
            $"Não foi possível abrir o arquivo de ramais para leitura após {MaxRetries} tentativas.\n" +
            "Outro processo pode estar utilizando o arquivo no momento.\n\n" +
            $"Detalhe técnico: {lastError?.Message}");
    }

    public void Save(string xmlPath, List<Ramal> ramais, bool forceOverwriteIgnoringConflict = false)
    {
        // Checagem de conflito: o arquivo foi alterado por outra pessoa desde que carregamos?
        if (!forceOverwriteIgnoringConflict && LastLoadedWriteTimeUtc.HasValue && File.Exists(xmlPath))
        {
            var currentWriteTime = File.GetLastWriteTimeUtc(xmlPath);
            if (currentWriteTime > LastLoadedWriteTimeUtc.Value)
            {
                throw new RamalConflictException(
                    "Este arquivo foi alterado por outra pessoa depois que você carregou a lista.\n\n" +
                    "Para evitar sobrescrever o trabalho de alguém, recarregue a lista antes de salvar.");
            }
        }

        var doc = new XDocument(
            new XDeclaration("1.0", null, null),
            new XElement("contacts",
                ramais.Select(r => new XElement("contact",
                    new XAttribute("name", r.Name ?? ""),
                    new XAttribute("number", r.Number ?? ""),
                    new XAttribute("firstname", r.FirstName ?? ""),
                    new XAttribute("lastname", r.LastName ?? ""),
                    new XAttribute("phone", r.Phone ?? ""),
                    new XAttribute("mobile", r.Mobile ?? ""),
                    new XAttribute("email", r.Email ?? ""),
                    new XAttribute("address", r.Address ?? ""),
                    new XAttribute("city", r.City ?? ""),
                    new XAttribute("state", r.State ?? ""),
                    new XAttribute("zip", r.Zip ?? ""),
                    new XAttribute("comment", r.Comment ?? ""),
                    new XAttribute("id", r.Id ?? ""),
                    new XAttribute("info", r.Info ?? ""),
                    new XAttribute("presence", r.Presence ?? "0"),
                    new XAttribute("starred", r.Starred ?? "0"),
                    new XAttribute("directory", r.Directory ?? "0")
                ))
            )
        );

        if (File.Exists(xmlPath))
        {
            BackupFile(xmlPath);
        }

        Exception? lastError = null;
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                using var stream = new FileStream(xmlPath, FileMode.Create, FileAccess.Write, FileShare.None);
                doc.Save(stream);
                LastLoadedWriteTimeUtc = File.GetLastWriteTimeUtc(xmlPath);
                return;
            }
            catch (IOException ex)
            {
                lastError = ex;
                Thread.Sleep(RetryDelayMs);
            }
        }

        throw new RamalFileLockedException(
            $"Não foi possível salvar o arquivo de ramais após {MaxRetries} tentativas.\n" +
            "O arquivo pode estar aberto ou sendo gravado por outra pessoa neste momento. " +
            "Tente novamente em alguns segundos.\n\n" +
            $"Detalhe técnico: {lastError?.Message}");
    }

    private static void BackupFile(string xmlPath)
    {
        try
        {
            var dir = Path.GetDirectoryName(xmlPath) ?? ".";
            var backupDir = Path.Combine(dir, "Backups");
            Directory.CreateDirectory(backupDir);

            var fileName = Path.GetFileNameWithoutExtension(xmlPath);
            var ext = Path.GetExtension(xmlPath);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupPath = Path.Combine(backupDir, $"{fileName}_{timestamp}{ext}");

            File.Copy(xmlPath, backupPath, overwrite: false);

            CleanupOldBackups(backupDir, fileName, ext, keep: 50);
        }
        catch
        {
            // Backup é "boa prática", não pode travar o salvamento principal.
            // Se falhar (ex: permissão), seguimos em frente com o save.
        }
    }

    private static void CleanupOldBackups(string backupDir, string fileName, string ext, int keep)
    {
        var files = Directory.GetFiles(backupDir, $"{fileName}_*{ext}")
            .OrderByDescending(f => f)
            .Skip(keep);

        foreach (var f in files)
        {
            try { File.Delete(f); } catch { /* ignora */ }
        }
    }

    private static string Attr(XElement el, string name, string fallback = "")
        => el.Attribute(name)?.Value ?? fallback;
}
