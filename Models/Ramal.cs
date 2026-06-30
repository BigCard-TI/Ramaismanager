namespace RamaisManager.Models;

/// <summary>
/// Representa um contato/ramal do MicroSIP.
/// Mantém todos os atributos originais do XML (mesmo os que não usamos na UI),
/// para que o arquivo salvo continue 100% compatível com o MicroSIP.
/// </summary>
public class Ramal
{
    // Campos usados ativamente na interface
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;

    // Demais atributos do MicroSIP, preservados mas não editados pela UI.
    // Se algum dia precisar editar algum desses, é só promover pra UI.
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Info { get; set; } = string.Empty;
    public string Presence { get; set; } = "0";
    public string Starred { get; set; } = "0";
    public string Directory { get; set; } = "0";

    /// <summary>
    /// Identificador único interno (para rastrear a linha na grid e detectar duplicatas).
    /// Não é salvo no XML.
    /// </summary>
    public Guid InternalId { get; } = Guid.NewGuid();

    public Ramal Clone()
    {
        return (Ramal)MemberwiseClone();
    }
}
