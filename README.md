# Mercadinho Du Zéh

Aplicação desktop desenvolvida em C# para cadastrar clientes e controlar as dívidas feitas na vendinha. O sistema permite consultar, pesquisar, editar e excluir clientes, além de registrar e acompanhar os pagamentos das dívidas.

## Funcionalidades

- Cadastro, consulta, edição e exclusão de clientes.
- Validação de nome, CPF, data de nascimento e e-mail.
- Cálculo da idade a partir da data de nascimento.
- Pesquisa de clientes pelo nome.
- Listagem ordenada pelo total de dívidas em aberto, com paginação de 10 clientes.
- Cadastro e consulta de dívidas por cliente.
- Registro de pagamento de dívidas.
- Limite de uma dívida em aberto por cliente.

## Tecnologias

- C# e .NET 10
- Windows Forms
- Entity Framework Core
- PostgreSQL
- Npgsql

## Estrutura da solução

- `Solution1/MercadinhoDuZeh.Classes`: modelos, contexto do Entity Framework e serviços.
- `Solution1/MercadinhoDuZeh.Desktop`: aplicação Windows Forms.
- `database/schema.sql`: script de criação das tabelas `clientes` e `dividas`.

## Requisitos

- Windows.
- Visual Studio com suporte a desenvolvimento para desktop com .NET.
- .NET 10 SDK.
- PostgreSQL.

## Configuração do banco de dados

1. Inicie o serviço do PostgreSQL.
2. No pgAdmin, crie um banco chamado `Mercadinho_du_zeh`.
3. Selecione esse banco, abra o **Query Tool** e execute o conteúdo de `database/schema.sql`.

O script cria as tabelas, mas não cria o banco. Ele também não insere dados de exemplo.

## Configuração da conexão

Crie o arquivo `appsettings.Development.json` dentro da pasta `Solution1/MercadinhoDuZeh.Desktop`, ao lado do arquivo `.csproj`, com o conteúdo abaixo. Troque `SUA_SENHA` pela senha local do usuário PostgreSQL:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=Mercadinho_du_zeh;Username=postgres;Password=SUA_SENHA"
  }
}