namespace RamaisManager.Security;

/// <summary>
/// Bloqueio temporário após tentativas de login malsucedidas, para dificultar
/// força bruta na tela de login. Controle em memória (por execução do programa) —
/// como o login é validado contra a tabela USUARIOS do banco corporativo,
/// não duplicamos esse controle lá; é só uma barreira local extra.
/// </summary>
public class LoginThrottle
{
    private const int MaxTentativas = 5;
    private static readonly TimeSpan BloqueioDuracao = TimeSpan.FromMinutes(2);

    private int _tentativasFalhas = 0;
    private DateTime? _bloqueadoAte = null;

    public bool EstaBloqueado(out TimeSpan tempoRestante)
    {
        if (_bloqueadoAte.HasValue && DateTime.Now < _bloqueadoAte.Value)
        {
            tempoRestante = _bloqueadoAte.Value - DateTime.Now;
            return true;
        }

        if (_bloqueadoAte.HasValue && DateTime.Now >= _bloqueadoAte.Value)
        {
            // Bloqueio expirou: reseta o contador.
            _bloqueadoAte = null;
            _tentativasFalhas = 0;
        }

        tempoRestante = TimeSpan.Zero;
        return false;
    }

    public void RegistrarFalha()
    {
        _tentativasFalhas++;
        if (_tentativasFalhas >= MaxTentativas)
        {
            _bloqueadoAte = DateTime.Now.Add(BloqueioDuracao);
        }
    }

    public void RegistrarSucesso()
    {
        _tentativasFalhas = 0;
        _bloqueadoAte = null;
    }

    public int TentativasRestantes => Math.Max(0, MaxTentativas - _tentativasFalhas);
}
