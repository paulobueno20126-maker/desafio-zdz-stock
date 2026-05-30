## 🗄️ Modelagem de Dados - Entidade Produto

A tabela de produtos no banco de dados segue a seguinte estrutura:

| Campo | Tipo | Descrição |
| :--- | :--- | :--- |
| **Id** | int (PK) | Identificador único gerado automaticamente pelo banco |
| **Nome** | string | Nome do artigo (Suporta metadados como [kg] para peso e [IMG:url] para fotos) |
| **Quantidade** | double/decimal| Quantidade atual em stock |
| **Preco** | double/decimal| Preço base por unidade ou quilo |
| **DataValidade** | DateTime | Data de vencimento do produto |
| **CodigoBarras** | string | Código identificador gerado automaticamente (Inicia com 560...) |
