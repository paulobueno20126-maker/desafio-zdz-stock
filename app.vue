<template>
  <div class="main-layout" :class="{ 'dark-theme': isDark }">
    
    <aside class="sidebar">
      <div class="logo-area">
        <span class="logo-icon">📦</span>
        <h2>StockMaster</h2>
      </div>

      <button @click="toggleTema" type="button" class="theme-toggle-btn">
        {{ isDark ? '☀️ Modo Claro' : '🌙 Modo Escuro' }}
      </button>

      <div class="form-container" style="margin-top: 20px;">
        <h3>➕ Novo Produto</h3>
        <form @submit.prevent="adicionarProduto" class="vertical-form">
          <div class="form-group">
            <label>Nome do Artigo</label>
            <input v-model="novoProduto.nome" type="text" placeholder="Ex: Queijo Fresco" required />
          </div>

          <div class="form-group">
            <label>URL da Imagem (Link)</label>
            <input v-model="novoProduto.imagemUrl" type="text" placeholder="https://linkdaimagem.com/foto.jpg" />
          </div>

          <div class="form-group">
            <label>Quantidade / Tipo</label>
            <div class="qty-type-input-group">
              <input v-model.number="novoProduto.quantidade" type="number" min="0" step="any" placeholder="0" required />
              <select v-model="novoProduto.tipoMedida" class="type-select">
                <option value="u.">Unid.</option>
                <option value="kg">Kg</option>
              </select>
            </div>
          </div>

          <div class="form-group">
            <label>Preço por {{ novoProduto.tipoMedida == 'kg' ? 'Quilo' : 'Unidade' }} (€)</label>
            <input v-model.number="novoProduto.preco" type="number" step="0.01" min="0" placeholder="0.00" required />
          </div>

          <div class="form-group">
            <label>Data de Validade</label>
            <input v-model="novoProduto.dataValidade" type="date" required />
          </div>

          <button type="submit" class="v-btn meu-botao-roxo">
            Gravar no Inventário
          </button>
        </form>
      </div>

      <div class="sidebar-footer">
        <p>Sistema Ativo v3.0</p>
      </div>
    </aside>

    <main class="content-area">
      <header class="header">
        <div>
          <h1>Painel de Controlo</h1>
          <p class="subtitle">Bem-vindo, Paulo. Aqui está o resumo do teu stock.</p>
        </div>
      </header>

      <section class="dashboard-grid">
        <div class="stat-card accent-blue">
          <div class="stat-icon">💰</div>
          <div class="stat-content">
            <p>Valor de Inventário</p>
            <h3>€ {{ totalValorStock.toFixed(2) }}</h3>
          </div>
        </div>
        
        <div class="stat-card accent-red" @click="mudarFiltroRapido('vencidos')" :class="{ active: filtroRapido === 'vencidos' }">
          <div class="stat-icon">⏰</div>
          <div class="stat-content">
            <p>Vencidos</p>
            <h3>{{ totalVencidos }}</h3>
          </div>
        </div>

        <div class="stat-card accent-orange" @click="mudarFiltroRapido('sem-stock')" :class="{ active: filtroRapido === 'sem-stock' }">
          <div class="stat-icon">📉</div>
          <div class="stat-content">
            <p>Esgotados</p>
            <h3>{{ totalSemStock }}</h3>
          </div>
        </div>

        <div class="stat-card accent-green" @click="mudarFiltroRapido('todos')" :class="{ active: filtroRapido === 'todos' }">
          <div class="stat-icon">✔️</div>
          <div class="stat-content">
            <p>Total Itens</p>
            <h3>{{ produtos.length }}</h3>
          </div>
        </div>
      </section>

      <section class="toolbar">
        <div class="search-bar">
          <span class="icon">🔍</span>
          <input v-model="filtroCodigo" type="text" placeholder="Procurar por Código de Barras..." @input="paginaAtual = 1" />
          <button v-if="filtroCodigo" @click="filtroCodigo = ''; paginaAtual = 1" class="clear-btn">Limpar</button>
        </div>

        <div class="sort-box">
          <label>Ordenar:</label>
          <select v-model="ordenacao" @change="paginaAtual = 1">
            <option value="alfabetica">Nome (A-Z)</option>
            <option value="validade-antiga">Validade Próxima</option>
            <option value="validade-recente">Validade Longa</option>
          </select>
        </div>
      </section>

      <div class="table-wrapper">
        <table class="modern-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Foto</th>
              <th>Produto</th>
              <th>Código de Barras</th>
              <th>Qtd. / Medida</th>
              <th>Preço Base</th>
              <th>Vencimento</th>
              <th>Estado</th>
              <th class="actions-head">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="produto in produtosPaginados" :key="produto.id" class="row-hover">
              <td>#{{ produto.id }}</td>
              
              <td>
                <img 
                  :src="extrairImagem(produto.nome)" 
                  alt="Produto" 
                  class="product-thumb"
                  @error="$event.target.src = 'https://cdn-icons-png.flaticon.com/512/679/679821.png'"
                />
              </td>
              
              <td v-if="editId === produto.id">
                <input v-model="editItem.nome" type="text" class="inline-input" />
              </td>
              <td v-else class="product-name">{{ extrairNomeLimpo(produto.nome) }}</td>

              <td><span class="barcode-tag">{{ produto.codigoBarras }}</span></td>
              
              <td v-if="editId === produto.id">
                <input v-model.number="editItem.quantidade" type="number" class="inline-input-sm" min="0" step="any" />
              </td>
              <td v-else>
                <span :class="getQtyClass(produto.quantidade)" class="qty-badge">
                  {{ produto.quantidade }} {{ extrairMedida(produto.nome) }}
                </span>
              </td>

              <td v-if="editId === produto.id">
                <input v-model.number="editItem.preco" type="number" step="0.01" class="inline-input-sm" min="0" />
              </td>
              <td v-else>€ {{ produto.preco?.toFixed(2) }} <small class="text-muted-dynamic">/{{ extrairMedida(produto.nome) }}</small></td>

              <td v-if="editId === produto.id">
                <input v-model="editItem.dataValidade" type="date" class="inline-input" />
              </td>
              <td v-else>{{ formatarData(produto.dataValidade) }}</td>

              <td>
                <span :class="getStatus(produto.dataValidade).classe" class="status-pill">
                  {{ getStatus(produto.dataValidade).texto }}
                </span>
              </td>

              <td class="actions">
                <div v-if="editId === produto.id" class="buttons-group">
                  <button @click="saveEdit" class="btn-circle save" title="Salvar">💾</button>
                  <button @click="editId = null" class="btn-circle cancel" title="Cancelar">❌</button>
                </div>
                <div v-else class="buttons-group">
                  <button @click="startEdit(produto)" class="btn-circle edit" title="Editar">✏️</button>
                  <button @click="deleteItem(produto.id)" class="btn-circle delete" title="Remover">🗑️</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        
        <div v-if="produtosFiltrados.length === 0" class="empty-state">
          <p>Nenhum produto encontrado com os critérios antigos.</p>
          <button @click="resetFiltros" class="btn-reset">Limpar Todos os Filtros</button>
        </div>

        <div v-if="produtosFiltrados.length > 0" class="pagination-container">
          <span class="pagination-info">
            A mostrar {{ deItem }} até {{ ateItem }} de {{ produtosFiltrados.length }} itens
          </span>
          <div class="pagination-buttons">
            <button @click="paginaAtual--" :disabled="paginaAtual === 1" class="btn-pag">Anterior</button>
            <span class="page-num">Página {{ paginaAtual }} de {{ totalPaginas }}</span>
            <button @click="paginaAtual++" :disabled="paginaAtual === totalPaginas" class="btn-pag">Próximo</button>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

// --- ESTADO DA APLICAÇÃO ---
const produtos = ref([])
const filtroCodigo = ref('')
const ordenacao = ref('alfabetica')
const filtroRapido = ref('todos')
const isDark = ref(false) // Controlo do Modo Escuro

const paginaAtual = ref(1)
const itensPorPagina = ref(5)

const editId = ref(null)
const editItem = ref({})

const novoProduto = ref({ nome: '', imagemUrl: '', quantidade: null, preco: null, dataValidade: '', tipoMedida: 'u.' })

// --- FUNÇÃO PARA INTERCALAR O TEMA ---
const toggleTema = () => {
  isDark.value = !isDark.value
}

// --- HOOKS ---
const carregarProdutos = async () => {
  try {
    const res = await fetch('http://localhost:5185/api/produtos')
    if (res.ok) {
      produtos.value = await res.json()
    }
  } catch (e) { 
    console.error("Erro ao ligar ao servidor C#", e) 
  }
}

onMounted(() => {
  carregarProdutos()
})

// --- PROCESSAMENTO DE METADADOS NO STRING DO NOME ---
const extrairMedida = (nomeCompleto) => {
  if (!nomeCompleto) return 'u.'
  if (nomeCompleto.includes('[kg]')) return 'kg'
  return 'u.'
}

// Retorna o link da imagem se existir codificado no nome, caso contrário dá uma imagem padrão de caixa
const extrairImagem = (nomeCompleto) => {
  if (!nomeCompleto || !nomeCompleto.includes('[IMG:')) {
    return 'https://cdn-icons-png.flaticon.com/512/679/679821.png'
  }
  const parts = nomeCompleto.split('[IMG:')
  if (parts.length > 1) {
    return parts[1].split(']')[0]
  }
  return 'https://cdn-icons-png.flaticon.com/512/679/679821.png'
}

const extrairNomeLimpo = (nomeCompleto) => {
  if (!nomeCompleto) return ''
  let limpo = nomeCompleto
  limpo = limpo.replace(' [kg]', '').replace('[kg]', '')
  if (limpo.includes('[IMG:')) {
    limpo = limpo.split('[IMG:')[0].trim()
  }
  return limpo
}

// --- PROPRIEDADES COMPUTADAS (DASHBOARD) ---
const totalValorStock = computed(() => {
  return produtos.value.reduce((acc, p) => acc + ((p.preco || 0) * (p.quantidade || 0)), 0)
})

const totalVencidos = computed(() => {
  const hoje = new Date().setHours(0,0,0,0)
  return produtos.value.filter(p => p.dataValidade && new Date(p.dataValidade) < hoje).length
})

const totalSemStock = computed(() => {
  return produtos.value.filter(p => p.quantidade === 0).length
})

// --- FILTRAGEM E ORDENAÇÃO ---
const produtosFiltrados = computed(() => {
  let list = [...produtos.value]
  const hoje = new Date().setHours(0,0,0,0)

  if (filtroRapido.value === 'vencidos') {
    list = list.filter(p => p.dataValidade && new Date(p.dataValidade) < hoje)
  } else if (filtroRapido.value === 'sem-stock') {
    list = list.filter(p => p.quantidade === 0)
  }

  if (filtroCodigo.value.trim()) {
    list = list.filter(p => p.codigoBarras && p.codigoBarras.includes(filtroCodigo.value.trim()))
  }

  return list.sort((a, b) => {
    if (ordenacao.value === 'alfabetica') return (extrairNomeLimpo(a.nome) || '').localeCompare(extrairNomeLimpo(b.nome) || '')
    const dA = a.dataValidade ? new Date(a.dataValidade).getTime() : 0
    const dB = a.dataValidade ? new Date(b.dataValidade).getTime() : 0
    return ordenacao.value === 'validade-antiga' ? dA - dB : dB - dA
  })
})

// --- PAGINAÇÃO ---
const totalPaginas = computed(() => {
  return Math.ceil(produtosFiltrados.value.length / itensPorPagina.value) || 1
})

const produtosPaginados = computed(() => {
  const inicio = (paginaAtual.value - 1) * itensPorPagina.value
  const fim = inicio + itensPorPagina.value
  return produtosFiltrados.value.slice(inicio, fim)
})

const deItem = computed(() => (paginaAtual.value - 1) * itensPorPagina.value + 1)
const ateItem = computed(() => {
  const num = paginaAtual.value * itensPorPagina.value
  return num > produtosFiltrados.value.length ? produtosFiltrados.value.length : num
})

// --- AÇÕES DA API (C#) ---
const adicionarProduto = async () => {
  try {
    const barCode = "560" + Math.floor(1000000000 + Math.random() * 9000000000)
    let nomeFinal = novoProduto.value.nome
    
    if (novoProduto.value.tipoMedida === 'kg') {
      nomeFinal += ' [kg]'
    }
    if (novoProduto.value.imagemUrl.trim()) {
      nomeFinal += ` [IMG:${novoProduto.value.imagemUrl.trim()}]`
    }
    
    const payload = { 
      Nome: nomeFinal,
      Quantidade: Number(novoProduto.value.quantidade),
      Preco: Number(novoProduto.value.preco),
      DataValidade: novoProduto.value.dataValidade,
      CodigoBarras: barCode
    }
    
    const res = await fetch('http://localhost:5185/api/produtos', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })
    
    if (res.ok) {
      novoProduto.value = { nome: '', imagemUrl: '', quantidade: null, preco: null, dataValidade: '', tipoMedida: 'u.' }
      paginaAtual.value = 1
      await carregarProdutos()
    }
  } catch (e) { console.error("Erro ao adicionar produto", e) }
}

const startEdit = (p) => {
  editId.value = p.id
  let dataFormatada = ""
  if (p.dataValidade) {
    dataFormatada = p.dataValidade.split('T')[0]
  }
  editItem.value = { ...p, dataValidade: dataFormatada }
}

const saveEdit = async () => {
  try {
    const payload = {
      Id: editItem.value.id,
      Nome: editItem.value.nome,
      Quantidade: Number(editItem.value.quantidade),
      Preco: Number(editItem.value.preco),
      DataValidade: editItem.value.dataValidade,
      CodigoBarras: editItem.value.codigoBarras
    }

    const res = await fetch(`http://localhost:5185/api/produtos/${editId.value}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })
    
    if (res.ok) {
      editId.value = null
      await carregarProdutos()
    }
  } catch (e) { console.error("Erro ao atualizar produto", e) }
}

const deleteItem = async (id) => {
  if (confirm("Remover produto permanentemente do servidor?")) {
    try {
      const res = await fetch(`http://localhost:5185/api/produtos/${id}`, {
        method: 'DELETE'
      })
      if (res.ok) {
        if (produtosPaginados.value.length === 1 && paginaAtual.value > 1) {
          paginaAtual.value--
        }
        await carregarProdutos()
      }
    } catch (e) { console.error("Erro ao remover produto", e) }
  }
}

const mudarFiltroRapido = (tipo) => {
  filtroRapido.value = tipo
  paginaAtual.value = 1
}

const resetFiltros = () => {
  filtroRapido.value = 'todos'
  filtroCodigo.value = ''
  paginaAtual.value = 1
}

// --- AUXILIARES VISUAIS ---
const formatarData = (d) => {
  if (!d) return '-'
  const limpa = d.split('T')[0]
  const partes = limpa.split('-')
  return partes.length === 3 ? `${partes[2]}/${partes[1]}/${partes[0]}` : d
}

const getQtyClass = (q) => {
  if (q === 0) return 'qty-critical'
  if (q < 5) return 'qty-warning'
  return 'qty-ok'
}

const getStatus = (d) => {
  if (!d) return { texto: 'Sem Data', classe: 'status-expired' }
  const hoje = new Date().setHours(0,0,0,0)
  const val = new Date(d).setHours(0,0,0,0)
  return val < hoje 
    ? { texto: 'Expirado', classe: 'status-expired' } 
    : { texto: 'Válido', classe: 'status-valid' }
}
</script>

<style>
:root {
  --primary: #4f46e5;
  --bg: #f1f5f9;
  --card-bg: #ffffff;
  --text-main: #1e293b;
  --text-muted: #64748b;
  --red: #ef4444;
  --orange: #f59e0b;
  --green: #10b981;
  --border-color: #e2e8f0;
  --table-row-even: #fcfdfe;
}

/* REGISTO DE CLASSES PARA O TEMA ESCURO */
.dark-theme {
  --bg: #0f172a;
  --card-bg: #1e293b;
  --text-main: #f8fafc;
  --text-muted: #94a3b8;
  --border-color: #334155;
  --table-row-even: #1e293b;
}

body {
  margin: 0;
  background-color: var(--bg);
  font-family: 'Inter', system-ui, sans-serif;
  color: var(--text-main);
  transition: background-color 0.3s, color 0.3s;
}

.main-layout {
  display: grid;
  grid-template-columns: 320px 1fr;
  min-height: 100vh;
  max-width: 100vw;
  overflow-x: hidden;
  background-color: var(--bg);
}

/* BOTÃO TOGGLE TEMA */
.theme-toggle-btn {
  background: #334155;
  color: white;
  border: 1px solid #475569;
  padding: 10px;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  font-size: 14px;
  transition: background 0.2s;
}
.theme-toggle-btn:hover {
  background: #475569;
}

/* MINIATURA IMAGEM */
.product-thumb {
  width: 40px;
  height: 40px;
  border-radius: 6px;
  object-fit: cover;
  background: #f1f5f9;
  border: 1px solid var(--border-color);
}

/* DYNAMIC HELPERS */
.text-muted-dynamic {
  color: var(--text-muted);
}

/* SIDEBAR */
.sidebar {
  background-color: #1e293b;
  color: white;
  padding: 30px;
  display: flex;
  flex-direction: column;
  border-right: 1px solid var(--border-color);
}

.logo-area { display: flex; align-items: center; gap: 12px; margin-bottom: 30px; }
.logo-icon { font-size: 32px; }
.logo-area h2 { margin: 0; font-size: 22px; letter-spacing: -0.5px; color: white; }
.form-container h3 { font-size: 16px; text-transform: uppercase; letter-spacing: 1px; color: #94a3b8; margin-bottom: 20px; }
.vertical-form { display: flex; flex-direction: column; gap: 16px; }
.form-group label { display: block; font-size: 13px; margin-bottom: 8px; color: #cbd5e1; }
.form-group input { width: 100%; padding: 12px; background: #334155; border: 1px solid #475569; border-radius: 8px; color: white; box-sizing: border-box; }

.qty-type-input-group { display: flex; gap: 5px; }
.qty-type-input-group input { flex: 1; }
.type-select { width: 90px; padding: 12px; background: #475569; border: 1px solid #475569; border-radius: 8px; color: white; cursor: pointer; font-weight: 600; outline: none; }

.meu-botao-roxo {
  background-color: var(--primary) !important;
  color: white !important;
  border: none !important;
  padding: 14px !important;
  font-size: 15px !important;
  font-weight: 700 !important;
  border-radius: 8px !important;
  cursor: pointer !important;
  width: 100% !important;
  text-align: center !important;
  box-shadow: 0 4px 6px -1px rgba(79, 70, 229, 0.3) !important;
  transition: background 0.2s, transform 0.1s !important;
  margin-top: 10px;
  display: block !important;
}
.meu-botao-roxo:hover { background-color: #4338ca !important; transform: translateY(-1px); }

/* CONTEÚDO AREA */
.content-area { padding: 40px; overflow-y: auto; background-color: var(--bg); }
.header h1 { margin: 0; font-size: 28px; font-weight: 800; color: var(--text-main); }
.subtitle { color: var(--text-muted); margin-top: 5px; }

/* DASHBOARD CARDS */
.dashboard-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin: 30px 0; }
.stat-card { background: var(--card-bg); color: var(--text-main); padding: 20px; border-radius: 12px; display: flex; align-items: center; gap: 15px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05); border-left: 5px solid transparent; cursor: pointer; transition: all 0.2s; }
.stat-card:hover { transform: translateY(-3px); box-shadow: 0 6px 12px -2px rgba(0,0,0,0.1); }
.stat-card.active { border-bottom: 3px solid var(--primary); }
.accent-blue { border-left-color: var(--primary); }
.accent-red { border-left-color: var(--red); }
.accent-orange { border-left-color: var(--orange); }
.accent-green { border-left-color: var(--green); }
.stat-icon { font-size: 30px; }
.stat-content p { margin: 0; font-size: 13px; color: var(--text-muted); font-weight: 600; }
.stat-content h3 { margin: 5px 0 0; font-size: 22px; color: var(--text-main); }

/* TOOLBAR */
.toolbar { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; background: var(--card-bg); padding: 15px 20px; border-radius: 10px; box-shadow: 0 1px 3px rgba(0,0,0,0.05); color: var(--text-main); }
.search-bar { display: flex; align-items: center; gap: 10px; flex: 1; }
.search-bar input { border: none; outline: none; width: 300px; font-size: 15px; background: transparent; color: var(--text-main); }
.clear-btn { background: #e2e8f0; border: none; padding: 4px 10px; border-radius: 6px; cursor: pointer; font-size: 12px; color: #1e293b; font-weight: 500; }
.sort-box select { padding: 8px 12px; border-radius: 6px; border: 1px solid var(--border-color); outline: none; background: var(--card-bg); color: var(--text-main); cursor: pointer; }

/* TABELA MODERNIZADA */
.table-wrapper { background: var(--card-bg); border-radius: 12px; overflow-x: auto; box-shadow: 0 10px 15px -3px rgba(0,0,0,0.05); border: 1px solid var(--border-color); }
.modern-table { width: 100%; border-collapse: collapse; }
.modern-table th { background: var(--table-row-even); padding: 16px; text-align: left; font-size: 13px; text-transform: uppercase; color: var(--text-muted); border-bottom: 1px solid var(--border-color); font-weight: 600; }
.modern-table td { padding: 16px; border-bottom: 1px solid var(--border-color); font-size: 14px; color: var(--text-main); white-space: nowrap; }
.row-hover:hover { background-color: var(--border-color); }
.modern-table tbody tr:nth-child(even) { background-color: var(--table-row-even); }
.product-name { font-weight: 700; color: var(--text-main); }
.barcode-tag { background: var(--bg); padding: 4px 8px; border-radius: 4px; font-family: monospace; font-size: 12px; color: var(--text-main); border: 1px solid var(--border-color); }

/* BADGES */
.qty-badge { font-weight: 800; }
.qty-critical { color: var(--red); }
.qty-warning { color: var(--orange); }
.qty-ok { color: var(--green); }

.status-pill { padding: 6px 12px; border-radius: 20px; font-size: 11px; font-weight: 700; text-transform: uppercase; }
.status-valid { background: #d1fae5; color: #065f46; }
.status-expired { background: #fee2e2; color: #991b1b; }

.btn-circle { width: 34px; height: 34px; border-radius: 50%; border: none; display: inline-flex; align-items: center; justify-content: center; cursor: pointer; }
.edit { background: #e0f2fe; color: #0369a1; }
.delete { background: #fee2e2; color: #dc2626; }
.save { background: #d1fae5; }
.cancel { background: #f1f5f9; }

.inline-input, .inline-input-sm { padding: 6px; border: 1px solid var(--primary); border-radius: 6px; background: var(--bg); color: var(--text-main); }

/* PAGINAÇÃO */
.pagination-container { display: flex; justify-content: space-between; align-items: center; padding: 16px 20px; background: var(--table-row-even); border-top: 1px solid var(--border-color); }
.pagination-info { font-size: 13px; color: var(--text-muted); }
.btn-pag { padding: 6px 14px; background: var(--card-bg); border: 1px solid var(--border-color); color: var(--text-main); border-radius: 6px; cursor: pointer; }
.btn-pag:disabled { opacity: 0.4; cursor: not-allowed; }

.sidebar-footer { margin-top: auto; padding-top: 20px; color: #64748b; font-size: 12px; text-align: center; }
</style>