# MyApi (Produtos e Pedidos)

Projeto .NET Web API para gerenciar produtos e emitir pedidos .

**Collections Postman**: há uma pasta `exportPostman` com as collections contendo exemplos de uso de todas as requisições e payloads.

## Pré-requisitos
- .NET SDK. Verifique com:

```powershell
dotnet --info
```

- VS Code com extensão C#.

## Rodando a API
1. Abra um terminal e navegue até a seguinte pasta do projeto:

```powershell
testeTech4People/api/MyApi'
```

2. Restaurar dependências e compilar:

```powershell
dotnet restore
dotnet build
```

3. Rodar localmente:

```powershell
dotnet run
```

A API vai iniciar em `http://localhost:5237` e `https://localhost:7237`.



## Endpoints principais
- Produtos:
  - `GET /api/produtos` — lista produtos
  - `GET /api/produtos/{id}` — obtém produto
  - `POST /api/produtos` — cria (body JSON: `nome`, `descricao`, `preco`, `estoque`)
  - `PUT /api/produtos/{id}` — atualiza
  - `DELETE /api/produtos/{id}` — remove
  - `PATCH /api/produtos/{id}/adicionar-estoque/{quantidade}` — adiciona quantidade
  - `PATCH /api/produtos/{id}/remover-estoque/{quantidade}` — remove quantidade 

- Pedidos:
  - `GET /api/pedido` — lista pedidos com itens
  - `GET /api/pedido/{id}` — obtém pedido
  - `POST /api/pedido` — cria pedido. Exemplo de body:

```json
{
  "cliente": "João",
  "items": [
    { "produtoId": 1, "quantidade": 2 },
    { "produtoId": 2, "quantidade": 1 }
  ]
}
```


## Testes com Postman
- Importe a collection em `exportPostman` no Postman.
- Ajuste a variável `base_url` se a API estiver em outra porta.


