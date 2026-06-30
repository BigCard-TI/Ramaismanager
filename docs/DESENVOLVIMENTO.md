# Documentação de Desenvolvimento

Guia técnico para quem for manter, estender ou revisar este projeto.

## 1. Stack e requisitos

- **.NET 8** (`net8.0-windows`), Windows Forms.
- **Microsoft.Data.SqlClient** (5.x) para acesso ao SQL Server.
- Compilação: Visual Studio 2022+ (workload "Desenvolvimento para Desktop com
  .NET") ou `dotnet build` / `dotnet publish` via linha de comando.
- O projeto **não** é multiplataforma — depende de Windows Forms e da API de
  registro do Windows (`Microsoft.Win32.Registry`), portanto só compila e
  roda em Windows.

## 2. Estrutura do projeto

```
RamaisManager/
├── Program.cs                     → ponto de entrada; orquestra login → tela principal
├── LoginForm.cs / .Designer.cs    → tela de login do operador
├── DbSettingsForm.cs / .Designer.cs → painel de configuração da conexão com o banco
├── MainForm.cs / .Designer.cs     → tela principal (grid de ramais)
├── SettingsForm.cs / .Designer.cs → painel de configuração do caminho do XML
├── Models/
│   └── Ramal.cs                   → modelo de dados de um contato/ramal
├── Config/
│   └── AppSettings.cs             → persistência do caminho do XML (AppData, JSON)
├── Security/
│   ├── SenhaCipher.cs             → cifrador numérico posicional (senha de login)
│   └── ConnectionStringVault.cs   → connection string protegida (registro + DPAPI)
└── Services/
    ├── RamalXmlService.cs         → leitura/escrita do XML, lock e backup
    └── AuthService.cs             → validação de login contra o banco
```

Cada Form segue o padrão `Form.cs` (lógica) + `Form.Designer.cs` (layout dos
controles), como gerado pelo designer visual do Visual Studio. Os arquivos
`.Designer.cs` foram escritos manualmente neste projeto (sem depender do
designer gráfico), mas seguem exatamente a convenção que o Visual Studio
geraria, então abrem normalmente no editor visual.

## 3. Fluxo da aplicação

1. `Program.Main()` carrega a connection string salva (`ConnectionStringVault.Load()`).
2. Se não houver connection string configurada, exibe um aviso (mas ainda
   assim abre a tela de login, permitindo configurar por lá).
3. `LoginForm` é exibida de forma modal (`ShowDialog`). Internamente:
   - O texto digitado na senha é cifrado via `SenhaCipher.Encode`.
   - `AuthService.Login` consulta `SELECT CODIGO FROM USUARIOS WHERE CODIGO = @codigo AND SENHA = @senha`.
   - Em caso de sucesso, retorna um `OperadorAutenticado` com o código do operador.
4. Se o login for bem-sucedido, `MainForm` é aberta recebendo o operador
   autenticado no construtor.
5. `MainForm.MainForm_Load` carrega as configurações do XML (`AppSettings.Load`)
   e, se configurado, chama `LoadRamais()`, que usa `RamalXmlService.Load`.
6. Edições na grid disparam validação de duplicidade em tempo real
   (`CellValidating` / `CellFormatting`) e marcam metadados de auditoria
   (`AlteradoPor`, `AlteradoEm`) em `CellEndEdit`.
7. Ao salvar, `RamalXmlService.Save`:
   - Verifica se o arquivo foi modificado por outro processo desde o último
     carregamento (comparação de `LastWriteTimeUtc`), lançando
     `RamalConflictException` em caso positivo.
   - Cria um backup do arquivo atual antes de sobrescrever.
   - Grava com `FileShare.None` e algumas tentativas com pequeno atraso, para
     lidar com concorrência entre máquinas.

## 4. Modelo de dados (`Ramal`)

Representa um `<contact>` do XML. Mantém **todos** os atributos originais do
formato (mesmo os não utilizados na interface, como `email`, `phone`,
`presence`), para que o arquivo salvo continue 100% compatível com o
consumidor original (o softphone). Os únicos campos expostos na grid são
`Number` e `Name`; os demais são preservados "passando por baixo".

Dois campos adicionais, **não presentes no formato original**, foram
introduzidos para fins de auditoria:

- `AlteradoPor`: código do operador que fez a última alteração.
- `AlteradoEm`: data/hora da última alteração (`yyyy-MM-dd HH:mm:ss`).

São gravados como atributos extras no XML (`alteradoPor`, `alteradoEm`).
Softwares que consomem o XML original ignoram atributos desconhecidos, então
isso não deve quebrar a compatibilidade — mas convém confirmar esse
comportamento no software consumidor real antes de depender disso em
produção.

## 5. Segurança

### 5.1 Cifra de senha (`SenhaCipher`)

Algoritmo posicional simples (não é hash criptográfico, é uma cifra
reversível por deslocamento), usado para compatibilidade com um padrão já
adotado em outros sistemas internos da organização. A senha de entrada deve
ter até 6 dígitos numéricos (preenchida com zeros à esquerda).

```
Encode: para cada posição i (0 a 5), pega o dígito na posição (5 - i) da
        senha original e desloca por (50 + 2*i)
Decode: operação inversa
```

Por não ser um hash, este método **não deve ser tratado como criptografia
forte**. Ele existe para manter compatibilidade com o esquema de senha já
usado na tabela de usuários consultada, não foi desenhado por este projeto
como medida de segurança robusta. Se a tabela de usuários for revisada no
futuro, considerar a migração para hash com salt (ex.: PBKDF2, bcrypt).

### 5.2 Connection string (`ConnectionStringVault`)

Persistida em `HKEY_CURRENT_USER\Software\RamaisManager`,
protegida com **DPAPI** (`System.Security.Cryptography.ProtectedData`,
escopo `CurrentUser`). Isso significa:

- O valor cifrado só pode ser decifrado pelo mesmo usuário do Windows, na
  mesma máquina, que o gravou.
- Não é possível copiar a chave do registro de uma máquina/usuário para
  outro e esperar que funcione — isso é uma característica do DPAPI, não um
  bug.
- Cada máquina/perfil de usuário precisa ser configurado uma vez,
  individualmente.

### 5.3 Dados sensíveis fora do código-fonte

Nenhum caminho de servidor, credencial ou nome de organização está
hardcoded no código-fonte. Tanto o caminho do XML quanto a connection string
são configurados em tempo de execução e persistidos localmente (AppData e
registro, respectivamente), nunca commitados no repositório.

## 6. Concorrência multiusuário

Como múltiplas estações podem rodar o programa simultaneamente, duas
camadas de proteção foram implementadas:

1. **Otimista, em nível de aplicação**: ao carregar o XML, o timestamp de
   última modificação é guardado. Ao salvar, esse timestamp é comparado com
   o do disco; se mudou, o usuário é avisado antes de sobrescrever.
2. **Em nível de sistema operacional**: a escrita do arquivo usa
   `FileShare.None`, e leituras/escritas falhas por bloqueio de outro
   processo são retentadas algumas vezes com pequeno atraso antes de
   desistir e informar o usuário.

Não há lock distribuído de fato (como um arquivo `.lock` ou mutex de rede);
o controle é baseado inteiramente em timestamps e tentativas de I/O.

## 7. Extensões sugeridas (não implementadas)

- Histórico de alterações em arquivo separado (hoje só existe o último
  editor por ramal, não um log cronológico completo).
- Ordenação por coluna na grid.
- Exportação para CSV/Excel.
- Cadastro de operadores pela própria interface (hoje depende de acesso
  direto à tabela `USUARIOS` no banco).
- Indicador em tempo real de "quem está editando agora" (hoje a detecção de
  conflito só ocorre no momento de salvar).

## 8. Build e publicação

```bash
# Build de desenvolvimento
dotnet build -c Release

# Publicação como executável framework-dependent (requer .NET 8 Desktop
# Runtime instalado na máquina de destino)
dotnet publish -c Release -r win-x64 --self-contained false

# Publicação self-contained (não depende de runtime instalado, gera
# executável maior)
dotnet publish -c Release -r win-x64 --self-contained true
```

Não há pipeline de CI/CD configurado neste repositório. A distribuição é
manual (cópia do executável para uma pasta de rede ou instalação via
script/GPO interno).
