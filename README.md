# 📦 StockMaster - Sistema de Controlo de Stock

Este é um sistema completo de gestão e controlo de inventário desenvolvido como solução para um desafio técnico. A aplicação permite gerir produtos em tempo real, monitorizar métricas de stock e suporta funcionalidades avançadas de interface.

## 🚀 Funcionalidades Principais
- **CRUD Completo:** Criação, leitura, atualização e remoção de produtos integrados com o servidor.
- 🌙 **Modo Escuro (Dark Mode):** Alternância dinâmica de tema visual para maior conforto de utilização.
- 🖼️ **Suporte a Imagens:** Apresentação de miniaturas dos produtos através de URLs dinâmicas.
- 📊 **Dashboard Dinâmico:** Cálculo em tempo real do valor total do inventário e contadores automáticos de produtos esgotados ou vencidos.
- 🔍 **Filtros e Ordenação:** Pesquisa instantânea por código de barras e ordenação por nome ou proximidade da data de validade.

## 🛠️ Tecnologias Utilizadas
- **Backend:** C# .NET (API RESTful com endpoints CRUD)
- **Frontend:** Nuxt 3 (Vue 3), Vuetify 3 (estrutura de plugins e pacotes) e CSS Customizado Reativo.

---

## 💻 Como Executar o Projeto Localmente

### 1. Clonar o Repositório
```bash
git clone [https://github.com/paulobueno20126-maker/desafio-zdz-stock.git](https://github.com/paulobueno20126-maker/desafio-zdz-stock.git)
cd desafio-zdz-stock

2. Executar o Backend (C#)
Navega até à pasta do servidor:

```Bash
cd Backend
Executa a API:

```Bash
dotnet run

3. Executar o Frontend (Nuxt 3)
Abre uma nova janela do terminal e navega até à pasta do cliente:

```Bash
cd Frontend
Instala as dependências necessárias:

```Bash
npm install
Inicia o servidor de desenvolvimento:

```Bash
npm run dev
