using Microsoft.AspNetCore.Mvc;

namespace StockApi.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        // Criamos uma classe Produto cá dentro, como tinhas
        public class Produto
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string CodigoBarras { get; set; } = string.Empty;
            public int Quantidade { get; set; }
            public decimal Preco { get; set; }
            public DateTime DataValidade { get; set; }
        }

        // Criamos uma lista estática para guardar os dados enquanto a API corre
        private static readonly List<Produto> _produtosDB = new List<Produto>
        {
            new Produto { Id = 1, Nome = "Iogurte Natural", CodigoBarras = "5601234567891", Quantidade = 20, Preco = 1.99m, DataValidade = DateTime.Now.AddDays(-2) },
            new Produto { Id = 2, Nome = "Arroz Agulha 1kg", CodigoBarras = "5609876543210", Quantidade = 50, Preco = 1.25m, DataValidade = DateTime.Now.AddMonths(6) },
            new Produto { Id = 3, Nome = "Leite Condensado", CodigoBarras = "5604567890123", Quantidade = 15, Preco = 2.49m, DataValidade = DateTime.Now.AddDays(5) }
        };

        // 1. ROTA PARA LER TODOS
        [HttpGet]
        public IActionResult GetProdutos()
        {
            return Ok(_produtosDB);
        }

        // 2. NOVA ROTA: RECEBER E GRAVAR UM NOVO PRODUTO
        [HttpPost]
        public IActionResult CriarProduto([FromBody] Produto novo)
        {
            if (novo == null) return BadRequest();

            // Gera um ID novo automaticamente incrementando o último
            novo.Id = _produtosDB.Count > 0 ? _produtosDB.Max(p => p.Id) + 1 : 1;
            
            _produtosDB.Add(novo);
            return Ok(novo);
        }

        // 3. NOVA ROTA: ATUALIZAR/EDITAR UM PRODUTO EXISTENTE
        [HttpPut("{id}")]
        public IActionResult EditarProduto(int id, [FromBody] Produto atualizado)
        {
            var produtoExistente = _produtosDB.FirstOrDefault(p => p.Id == id);
            if (produtoExistente == null) return NotFound();

            produtoExistente.Nome = atualizado.Nome;
            produtoExistente.Quantidade = atualizado.Quantidade;
            produtoExistente.Preco = updatedPreco(atualizado.Preco);
            produtoExistente.DataValidade = atualizado.DataValidade;

            return Ok(produtoExistente);
        }

        // 4. NOVA ROTA: APAGAR UM PRODUTO
        [HttpDelete("{id}")]
        public IActionResult EliminarProduto(int id)
        {
            var produto = _produtosDB.FirstOrDefault(p => p.Id == id);
            if (produto == null) return NotFound();

            _produtosDB.Remove(produto);
            return Ok();
        }

        private decimal updatedPreco(decimal preco) => preco;
    }
}