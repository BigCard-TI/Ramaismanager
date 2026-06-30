namespace RamaisManager.Security;

/// <summary>
/// Cifrador numérico posicional usado para a senha de operadores (tabela USUARIOS).
/// Mesmo algoritmo já usado em outros sistemas internos (encode/decode reversível,
/// não é hash — cada dígito é deslocado conforme sua posição).
/// </summary>
public static class SenhaCipher
{
    /// <summary>
    /// Cifra uma senha numérica de até 6 dígitos (preenchida com zeros à esquerda).
    /// </summary>
    public static string Encode(string numero)
    {
        var d = numero.PadLeft(6, '0');
        if (d.Length != 6 || !d.All(char.IsDigit))
            throw new ArgumentException("A senha deve conter até 6 dígitos numéricos.", nameof(numero));

        var chars = new char[6];
        for (int i = 0; i < 6; i++)
        {
            int digito = d[5 - i] - '0';
            chars[i] = (char)(50 + 2 * i + digito);
        }
        return new string(chars);
    }

    /// <summary>
    /// Decifra uma senha previamente cifrada com Encode, devolvendo os 6 dígitos originais.
    /// </summary>
    public static string Decode(string cifrado)
    {
        if (cifrado.Length != 6)
            throw new ArgumentException("Texto cifrado inválido (esperado 6 caracteres).", nameof(cifrado));

        var digits = new int[6];
        for (int i = 0; i < 6; i++)
        {
            digits[i] = cifrado[i] - 50 - 2 * i;
        }
        return new string(digits.Reverse().Select(d => (char)('0' + d)).ToArray());
    }
}
