# Gerenciador de Ramais - MicroSIP

Aplicação WinForms (.NET 8) para editar a lista de ramais (`Contacts.xml`) usada
pelo MicroSIP em todas as máquinas da empresa, sem precisar mexer direto no XML.

## Como compilar

Pré-requisitos: Visual Studio 2022 (com workload ".NET desktop development")
ou .NET 8 SDK + `dotnet build`.

1. Abra `RamaisManager.sln` no Visual Studio, **ou**
2. Pelo terminal, dentro da pasta `RamaisManager`:
   ```
   dotnet build -c Release
   dotnet publish -c Release -r win-x64 --self-contained false
   ```
   O executável fica em `bin\Release\net8.0-windows\` (ou na pasta de publish).

## Primeira execução

Na primeira vez que abrir o programa em uma máquina, ele vai avisar que o
caminho do arquivo ainda não está configurado (ou vai sugerir um caminho
padrão que você pode ajustar no código antes de compilar, em
`Config/AppSettings.cs`, na constante `DefaultSuggestedPath`).

Para configurar (ou trocar depois) o caminho do `Contacts.xml`:

**Pressione `Ctrl + Shift + Alt + C`** dentro do programa.

Isso abre um painel discreto onde você escolhe o caminho do arquivo (rede ou
local), pode testar se o caminho é válido, e salvar. Essa configuração fica
gravada em:

```
%AppData%\RamaisManager\config.json
```

Ou seja: fica por máquina/usuário, não precisa recompilar nada para trocar o
caminho — só repetir o atalho em cada máquina (ou copiar o `config.json` pronto
para outras máquinas, se preferir distribuir já configurado).

## Funcionalidades

- **Grid editável**: edita diretamente nome e número do ramal na tela, sem
  abrir o XML.
- **Adicionar / Remover ramal**: botões dedicados, com sugestão automática do
  próximo número livre.
- **Busca**: filtro em tempo real por nome ou número.
- **Validações antes de salvar**: bloqueia números vazios, nomes vazios e
  números de ramal duplicados.
- **Proteção contra conflito de edição simultânea**: se duas pessoas abrirem
  o programa ao mesmo tempo, ao salvar o sistema detecta se o arquivo mudou
  desde que foi carregado e avisa antes de sobrescrever (com opção de
  recarregar ou forçar a sobrescrita).
- **Lock de arquivo com retentativas**: se o arquivo estiver sendo gravado
  por outra máquina naquele milissegundo exato, o programa tenta novamente
  automaticamente (até 5 vezes) antes de avisar o usuário.
- **Backup automático**: antes de cada gravação, o arquivo anterior é copiado
  para uma subpasta `Backups` (ao lado do XML original) com timestamp no
  nome. Mantém os últimos 50 backups e limpa os mais antigos automaticamente.
- **Atributos extras preservados**: o XML do MicroSIP tem vários atributos
  (`email`, `phone`, `presence`, etc.) que não usamos na tela, mas que são
  mantidos intactos ao salvar — nada é perdido.

## Estrutura do projeto

```
RamaisManager/
├── Program.cs                  → ponto de entrada
├── MainForm.cs / .Designer.cs  → tela principal (grid de ramais)
├── SettingsForm.cs / .Designer.cs → painel oculto de configuração do caminho
├── Models/
│   └── Ramal.cs                → modelo de dados de um contato/ramal
├── Config/
│   └── AppSettings.cs          → leitura/gravação do caminho do XML (AppData)
└── Services/
    └── RamalXmlService.cs      → leitura/escrita do XML com lock e backup
```

## Distribuição para os colaboradores

Sugestões (escolha a que fizer mais sentido para vocês):

1. **Mais simples**: publicar como self-contained (`--self-contained true`)
   e colocar o `.exe` numa pasta de rede compartilhada, todo mundo roda de lá.
2. **Mais limpo**: publicar como framework-dependent (exige .NET 8 Desktop
   Runtime instalado nas máquinas) e distribuir via GPO/script de logon,
   como vocês já fazem com outras ferramentas internas.

Em ambos os casos, depois de configurar o caminho do XML uma vez (Ctrl+Shift+Alt+C),
você pode copiar o `config.json` gerado (`%AppData%\RamaisManager\config.json`)
para as outras máquinas, evitando configurar uma por uma.

## Observação sobre segurança

Como combinado, não foi implementado controle de permissão/login (qualquer
colaborador com o programa instalado pode editar). Se no futuro vocês
quiserem restringir por usuário/grupo do AD, dá para evoluir isso depois sem
mudar a estrutura principal do projeto.
