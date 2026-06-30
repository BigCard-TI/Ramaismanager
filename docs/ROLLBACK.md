# Documentação de Rollback (Recriação via IA)

Este documento descreve, de forma completa e autocontida, tudo o que é
necessário para que uma IA (ou um desenvolvedor humano) recrie este sistema
do zero, caso o código-fonte original seja perdido. Não pressupõe acesso a
nenhum outro documento deste repositório.

## 1. Contexto e objetivo do sistema

Uma organização mantém uma rede de estações de trabalho Windows, cada uma
com um softphone (compatível com o formato de contatos do MicroSIP)
instalado. Cada estação consome um arquivo XML compartilhado em um servidor
de arquivos da rede local, que lista os ramais internos (número + nome de
identificação).

Antes deste sistema, a edição desse XML era manual (editor de texto comum),
o que era arriscado e pouco amigável para usuários não técnicos.

**Objetivo**: criar uma aplicação desktop em C# (WinForms) que permita a
qualquer colaborador autorizado (após login) visualizar, adicionar, editar e
remover ramais dessa lista através de uma interface gráfica simples,
substituindo a edição manual do XML.

## 2. Formato do arquivo XML de entrada/saída

O arquivo tem a seguinte estrutura (exemplo ilustrativo, sem dados reais):

```xml
<?xml version="1.0"?>
<contacts>
  <contact name="FULANO SETOR X" number="1001" firstname="" lastname=""
            phone="" mobile="" email="" address="" city="" state="" zip=""
            comment="" id="" info="" presence="0" starred="0" directory="0"
            alteradoPor="" alteradoEm="" />
  <contact name="CICLANA SETOR Y" number="1002" firstname="" lastname=""
            phone="" mobile="" email="" address="" city="" state="" zip=""
            comment="" id="" info="" presence="0" starred="0" directory="0"
            alteradoPor="" alteradoEm="" />
</contacts>
```

Características importantes:

- Elemento raiz `<contacts>`, filhos `<contact>`.
- Apenas `name` (identificação exibida) e `number` (número do ramal) são
  efetivamente usados pelo sistema de edição; os demais atributos
  (`firstname`, `lastname`, `phone`, `mobile`, `email`, `address`, `city`,
  `state`, `zip`, `comment`, `id`, `info`, `presence`, `starred`,
  `directory`) fazem parte do formato original do consumidor (o softphone) e
  **devem ser preservados intactos** ao salvar, mesmo sem edição pela
  interface.
- `alteradoPor` e `alteradoEm` são extensões deste projeto, não fazem parte
  do formato original — usadas para auditoria simples (ver seção 6).

## 3. Requisitos funcionais

1. **Login obrigatório** antes de qualquer acesso à lista de ramais.
   - Login validado contra uma tabela existente em SQL Server chamada
     `USUARIOS`, com colunas `CODIGO` e `SENHA`.
   - A senha em `SENHA` está armazenada usando uma cifra numérica
     posicional específica (detalhada na seção 5), não em texto puro nem em
     hash criptográfico padrão.
2. **Configuração da conexão com o banco** via painel de configuração
   próprio, acessível por atalho de teclado oculto (não visível na
   interface principal), com opção de testar a conexão antes de salvar.
   Connection string protegida localmente (ver seção 5.2).
3. **Configuração do caminho do arquivo XML** via painel de configuração
   oculto (atalho de teclado), persistida localmente (não hardcoded no
   código), de forma que o caminho possa ser alterado sem recompilar.
4. **Grid de edição** com duas colunas visíveis: número do ramal e
   identificação (nome). Suporte a adicionar linha, remover linha
   selecionada, e edição direta nas células.
5. **Busca/filtro em tempo real** por número ou nome.
6. **Validação de duplicidade de número de ramal em tempo real**:
   - Bloqueia a saída da célula se o número digitado já existir em outro
     contato da lista.
   - Destaca visualmente (cor de fundo/texto diferenciada) qualquer célula
     de número duplicado, mesmo fora do fluxo de edição direta.
7. **Validação antes de salvar**: bloqueia o salvamento se houver ramais com
   número vazio, nome vazio, ou números duplicados, listando os problemas
   encontrados.
8. **Controle de concorrência multiusuário**:
   - Ao carregar o arquivo, registrar o timestamp de última modificação.
   - Ao salvar, comparar esse timestamp com o do arquivo em disco; se
     diferente, avisar o usuário e oferecer a opção de sobrescrever mesmo
     assim ou cancelar.
   - Tentativas de leitura/escrita devem reagir a bloqueios temporários de
     arquivo (outro processo gravando no mesmo instante) com algumas
     tentativas automáticas antes de desistir.
9. **Backup automático**: antes de cada gravação, copiar o arquivo atual
   para uma subpasta `Backups` (no mesmo diretório do arquivo original), com
   timestamp no nome do arquivo de backup. Manter um número limitado de
   backups mais recentes (ex.: últimos 50), removendo os mais antigos
   automaticamente.
10. **Auditoria simples**: ao criar ou editar um ramal, gravar
    automaticamente (em atributos extras no próprio XML) o código do
    operador logado e o timestamp da alteração.

## 4. Requisitos não funcionais

- Plataforma: Windows Forms, .NET 8 (`net8.0-windows`).
- Nenhuma credencial, caminho de servidor ou identificador de organização
  deve estar fixo no código-fonte. Tudo deve ser configurável em tempo de
  execução e persistido localmente (fora do controle de versão).
- O sistema é de uso interno; não há requisito de internacionalização
  (interface em português é aceitável).

## 5. Especificações técnicas de segurança

### 5.1 Cifra de senha (algoritmo exato)

A senha do operador (numérica, até 6 dígitos) deve ser cifrada com o
algoritmo abaixo antes de ser comparada com o valor salvo em
`USUARIOS.SENHA`. É uma cifra posicional reversível, não um hash.

Pseudocódigo de referência (Python, para validação):

```python
def encode(numero: str) -> str:
    d = numero.zfill(6)
    return ''.join(chr(50 + 2*i + int(d[5-i])) for i in range(6))

def decode(cifrado: str) -> str:
    digits = [ord(cifrado[i]) - 50 - 2*i for i in range(6)]
    return ''.join(str(d) for d in reversed(digits))
```

Casos de teste para validar a implementação (entrada → saída esperada):

| Entrada | Saída esperada |
|---|---|
| `261288` | `:<89@>` |
| `290874` | `6;>8C>` |
| `240424` | `66:8>>` |
| `996971` | `3;?>CE` |

Qualquer reimplementação (em C# ou outra linguagem) deve reproduzir
exatamente esses 4 pares de entrada/saída antes de ser considerada correta.

A consulta de login deve ser equivalente a:

```sql
SELECT TOP 1 CODIGO FROM USUARIOS WHERE CODIGO = @codigo AND SENHA = @senhaCifrada
```

onde `@senhaCifrada` é o resultado de `encode()` aplicado à senha digitada
pelo usuário.

### 5.2 Armazenamento da connection string

- Local: registro do Windows, em
  `HKEY_CURRENT_USER\Software\<NomeDaOrganização>\<NomeDoApp>`.
- Proteção: DPAPI (Windows Data Protection API), escopo `CurrentUser`
  (em .NET: `System.Security.Cryptography.ProtectedData.Protect` /
  `Unprotect`, com `DataProtectionScope.CurrentUser`).
- O valor protegido deve ser convertido para Base64 antes de ser salvo como
  string no registro.
- Importante: por ser DPAPI com escopo de usuário, o valor só pode ser
  decifrado pela mesma conta do Windows, na mesma máquina, que o gravou.
  Isso é uma propriedade intencional, não uma limitação a ser contornada.

### 5.3 Caminho do arquivo XML

- Local: arquivo de configuração JSON simples em
  `%AppData%\<NomeDoApp>\config.json`, contendo ao menos o campo do caminho
  do XML.
- Não requer criptografia (não é dado sensível da mesma forma que a
  connection string).

## 6. Modelo de dados de auditoria

Ao criar ou editar uma linha na grid, registrar:

- `AlteradoPor`: identificador do operador logado no momento da ação.
- `AlteradoEm`: data/hora da ação, em formato ordenável (ex.:
  `yyyy-MM-dd HH:mm:ss`).

Esses dois campos devem ser persistidos como atributos adicionais em cada
`<contact>` do XML (ex.: `alteradoPor`, `alteradoEm`), preservados em
leituras subsequentes, e não devem quebrar a compatibilidade com o software
que originalmente consome esse arquivo (atributos desconhecidos devem ser
ignorados pelo consumidor original — confirmar esse comportamento ao
recriar o sistema em um ambiente real).

## 7. Fluxo de telas

1. **Tela de Login** (modal, primeira a abrir): campos de código e senha,
   botão de entrar, botão de cancelar (fecha a aplicação). Atalho de teclado
   oculto abre o painel de configuração de banco de dados.
2. **Painel de Configuração de Banco** (modal, acessível por atalho):
   campos para servidor, banco de dados, usuário/senha SQL ou opção de
   autenticação integrada do Windows, modo avançado para connection string
   manual, botão de testar conexão, botão de salvar.
3. **Tela Principal** (após login bem-sucedido): grid de ramais, campo de
   busca, botões de adicionar/remover/recarregar/salvar, rótulo indicando o
   caminho do arquivo atualmente em uso, barra de status. Atalhos de teclado
   ocultos abrem o painel de configuração do caminho do XML e o painel de
   configuração de banco.
4. **Painel de Configuração do Caminho do XML** (modal, acessível por
   atalho): campo de caminho, botão de busca de arquivo, botão de testar
   caminho, botão de salvar.

## 8. Critérios de aceite para considerar a recriação bem-sucedida

- O algoritmo de cifra de senha reproduz exatamente os 4 casos de teste da
  seção 5.1.
- A connection string sobrevive a um fechamento e reabertura do programa na
  mesma máquina/usuário (persistência funcionando).
- Dois contatos com o mesmo número de ramal são visualmente sinalizados na
  grid antes do salvamento ser tentado.
- Salvar com um número de ramal duplicado, vazio, ou nome vazio é
  bloqueado, com mensagem explicando o motivo.
- Abrir o programa em duas instâncias (ou duas máquinas) apontando para o
  mesmo arquivo, editar em ambas e salvar em sequência, resulta em aviso de
  conflito na segunda gravação (não em sobrescrita silenciosa).
- Após qualquer gravação bem-sucedida, existe um novo arquivo na subpasta
  `Backups` com timestamp correspondente ao momento do salvamento.
- Nenhuma credencial, caminho de rede real ou nome de organização aparece
  fixo em nenhum arquivo de código-fonte recriado.
