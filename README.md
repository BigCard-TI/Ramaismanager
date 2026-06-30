# Gerenciador de Ramais

Aplicação desktop (Windows, WinForms/.NET 8) para gerenciar de forma visual a
lista de ramais consumida por softwares de softphone (como o MicroSIP), sem
precisar editar o arquivo XML manualmente.

Cada estação de trabalho da rede aponta para o mesmo arquivo XML compartilhado
(em um servidor de arquivos local). Este sistema permite que operadores
autorizados visualizem, adicionem, editem e removam ramais por meio de uma
interface gráfica simples, com login, controle de concorrência e backups
automáticos.

## Principais recursos

- Edição da lista de ramais em grid, sem tocar no XML diretamente.
- Login de operador validado contra banco de dados (SQL Server).
- Caminho do arquivo XML e conexão com o banco configuráveis em tempo de
  execução (sem precisar recompilar), via painéis ocultos por atalho de
  teclado.
- Detecção de números de ramal duplicados em tempo real, com destaque visual.
- Proteção contra conflito de edição simultânea por múltiplos usuários.
- Backup automático do arquivo antes de cada gravação.
- Registro de auditoria simples (quem alterou cada ramal e quando).

## Stack

- C# / .NET 8 (Windows Forms)
- SQL Server (autenticação de operadores)
- DPAPI (Windows Data Protection API) para proteger dados sensíveis salvos
  localmente

## Documentação

- [Documentação de Uso](docs/USO.md) — como operar o sistema no dia a dia.
- [Documentação de Desenvolvimento](docs/DESENVOLVIMENTO.md) — arquitetura,
  estrutura do código e como contribuir.
- [Documentação de Rollback](docs/ROLLBACK.md) — guia para recriar o sistema
  do zero (inclusive com auxílio de IA), caso o código-fonte seja perdido.

## Status

Projeto interno, de uso restrito a uma organização específica. Este
repositório não contém nenhuma credencial, caminho de servidor, nome de
empresa ou dado de produção — toda configuração sensível é definida em tempo
de execução pelo próprio usuário, fora do código-fonte.

## Licença

Uso interno. Sem licença pública definida.
