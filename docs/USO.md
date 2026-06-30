# Documentação de Uso

Guia para quem vai operar o Gerenciador de Ramais no dia a dia. Não é
necessário conhecimento técnico para seguir este documento.

## 1. O que este sistema faz

Ele edita, de forma visual, a lista de ramais que softphones (como o
MicroSIP) usam para mostrar nomes de contatos ao lado dos números internos.
Antes deste sistema, essa lista só podia ser editada abrindo o arquivo XML
manualmente em um editor de texto — algo arriscado, já que um erro de digitação
pode quebrar o arquivo para todos os usuários da rede.

## 2. Login

Ao abrir o programa, a primeira tela pedida é de login.

| Campo | O que informar |
|---|---|
| Código do operador | Seu código de operador cadastrado no sistema interno |
| Senha | Sua senha numérica de até 6 dígitos |

Se o código ou a senha estiverem incorretos, uma mensagem de erro aparece e
os campos podem ser preenchidos novamente.

Se aparecer uma mensagem dizendo que a conexão com o banco de dados não está
configurada, veja a seção **6. Configuração inicial (apenas TI)** abaixo, ou
procure o suporte técnico.

## 3. Tela principal

Após o login, a tela principal mostra uma tabela com duas colunas:

- **Ramal**: o número do ramal.
- **Identificação (Nome)**: o nome que aparecerá no softphone para esse ramal.

### Editando um ramal existente

Clique duas vezes na célula que deseja alterar (ou selecione e pressione
F2), digite o novo valor e pressione Enter ou clique fora da célula.

### Adicionar um novo ramal

Clique no botão **Adicionar Ramal**. Uma nova linha é criada com um número
sugerido automaticamente (o próximo disponível) e o texto "NOVO RAMAL".
Edite os dois campos conforme necessário.

### Remover um ramal

Selecione a linha desejada e clique em **Remover Selecionado**. Uma
confirmação será exibida antes da remoção definitiva.

### Buscar um ramal

Digite qualquer parte do nome ou do número no campo de busca, no topo da
tela. A lista é filtrada automaticamente, em tempo real.

### Recarregar a lista

Clique em **Recarregar** para descartar alterações não salvas e buscar a
versão mais recente do arquivo (útil se outra pessoa salvou alterações
recentemente).

### Salvar alterações

Clique em **Salvar Alterações** para gravar tudo no arquivo compartilhado.
Veja a seção 4 para os avisos que podem aparecer nesse momento.

## 4. Avisos e mensagens comuns

### "O ramal já está em uso por outro contato"

Aparece ao tentar digitar um número de ramal que já existe em outra linha da
lista. A edição não é aceita até que um número diferente seja informado.
Células com números duplicados também ficam destacadas em vermelho na
tabela, mesmo fora da edição.

### "Este arquivo foi alterado por outra pessoa..."

Aparece ao salvar, caso outra pessoa tenha salvo alterações no mesmo arquivo
entre o momento em que você abriu a lista e o momento em que tentou salvar.
O sistema pergunta se você deseja:

- **Sim** — salvar mesmo assim, sobrescrevendo a alteração da outra pessoa
  (use com cuidado).
- **Não** — cancelar o salvamento. Recomenda-se clicar em **Recarregar**
  para ver as mudanças mais recentes antes de refazer sua edição.

### "Não foi possível salvar / abrir o arquivo"

Indica que o arquivo está sendo usado por outro processo no exato momento.
O sistema tenta novamente algumas vezes automaticamente; se ainda assim
falhar, aguarde alguns segundos e tente de novo.

### Corrigir antes de salvar

Antes de gravar, o sistema verifica se há ramais sem número, sem nome, ou
números duplicados. Se houver, uma lista dos problemas é exibida e o
salvamento é bloqueado até que sejam corrigidos.

## 5. Backups automáticos

Toda vez que alguém salva alterações, uma cópia do arquivo anterior é
guardada automaticamente em uma subpasta **Backups**, ao lado do arquivo
original, com data e hora no nome. Isso permite recuperar uma versão
anterior em caso de erro. Os backups mais antigos são removidos
automaticamente, mantendo sempre os últimos 50.

## 6. Configuração inicial (apenas equipe técnica)

Esta seção é destinada apenas à equipe responsável por instalar e configurar
o sistema, não aos operadores do dia a dia.

### Caminho do arquivo de ramais

Dentro da tela principal, pressione **Ctrl + Shift + Alt + C** para abrir o
painel de configuração do caminho do arquivo XML. É possível digitar o
caminho manualmente ou usar o botão de busca de arquivo, além de testar se o
caminho é válido antes de salvar.

### Conexão com o banco de dados

Na tela de **login**, pressione **Ctrl + Shift + Alt + C** para configurar a
conexão com o banco de dados usado para validar os logins. De dentro da tela
principal, o mesmo painel pode ser aberto com **Ctrl + Shift + Alt + B**.

É possível informar servidor, banco de dados e credenciais separadamente, ou
ativar o "modo avançado" para colar uma string de conexão completa. Use o
botão **Testar conexão** antes de salvar para confirmar que os dados estão
corretos.

Essas configurações ficam salvas localmente, por usuário e por máquina —
não é necessário reinstalar o programa para alterá-las, e cada máquina deve
ser configurada uma única vez.
