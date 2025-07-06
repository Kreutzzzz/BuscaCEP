// =================================================================
//                      DECLARAÇÃO DE CONSTANTES
// =================================================================
const cepInput = document.getElementById("cep");
const cepForm = document.getElementById("cepForm");
const estadoSelect = document.getElementById("estado");
const cidadeInput = document.getElementById("cidade");
const logradouroInput = document.getElementById("logradouro");
const estadoCidadeForm = document.getElementById("estadoCidadeForm");
const resultadoDiv = document.getElementById("resultado");
const resultadoContainer = document.getElementById("resultadoContainer");
const novaBuscaBtn = document.getElementById("novaBuscaBtn");
const mensagemErroDiv = document.getElementById("mensagemErro");

const BASE_API_URL = "https://localhost:7001"; 
const regexCEP = /^\d{5}-?\d{3}$/;
const regexLogradouro = /^[A-Za-zÀ-ÿ0-9\s\.,ºª\-]{3,}$/;

// Variável para "memorizar" a última busca
let ultimosResultadosSumarizados = [];

// =================================================================
//                      FUNÇÕES DE CONTROLE DA UI
// =================================================================
function showResults() {
  resultadoContainer.classList.remove('hidden');
  mensagemErroDiv.classList.add('hidden');
}
function showForm() {
  resultadoContainer.classList.add('hidden');
  mensagemErroDiv.classList.add('hidden');
}
function showError(message) {
  mensagemErroDiv.innerText = message;
  mensagemErroDiv.classList.remove('hidden');
}
function limparErrosDeCampo() {
  [cepInput, estadoSelect, cidadeInput, logradouroInput].forEach(input => {
    if (input) input.classList.remove("input-erro");
  });
}
function limparBuscaAnterior() {
  resultadoDiv.innerHTML = "";
  limparErrosDeCampo();
  mensagemErroDiv.classList.add('hidden');
  ultimosResultadosSumarizados = []; // Limpa a memória também
}

// =================================================================
//           LÓGICA DE EXIBIÇÃO DE RESULTADOS
// =================================================================

// Funções responsáveis por gerar o HTML dinâmico
function exibirResultadosSumarizados(addresses) {
  if (!addresses || addresses.length === 0) {
    resultadoDiv.innerHTML = "<p>Nenhum endereço encontrado para os critérios fornecidos.</p>";
    return;
  }
  
  ultimosResultadosSumarizados = addresses;

  let htmlContent = "<h3>Resultados da Busca:</h3>";
  addresses.forEach(address => {
    const cepLimpo = (address.cep || "").replace(/\D/g, '');
    
    // Inicia o container do item
    htmlContent += `<div class="resultado-item">`;

    // --- Lógica para adicionar campos apenas se eles existirem ---
    if (address.cep) {
      htmlContent += `<p><strong>CEP:</strong> ${address.cep}</p>`;
    }
    if (address.logradouro) {
      htmlContent += `<p><strong>Logradouro:</strong> ${address.logradouro}</p>`;
    }
    if (address.complemento) {
      htmlContent += `<p><strong>Complemento:</strong> ${address.complemento}</p>`;
    }
    if (address.bairro) {
      htmlContent += `<p><strong>Bairro:</strong> ${address.bairro}</p>`;
    }
  
    // Para cidade, verifica se tem localidade ou UF
    if (address.localidade) {
      htmlContent += `<p><strong>Cidade:</strong> ${address.localidade}${address.uf ? ` (${address.uf})` : ''}</p>`;
    }
    
    // Adiciona o botão se o CEP existir
    if (cepLimpo) {
      htmlContent += `<button class="botao-ver-completo" data-cep="${cepLimpo}">Ver Completo</button>`;
    }

    // Fecha o container do item
    htmlContent += `</div>`;
  });
  resultadoDiv.innerHTML = htmlContent;
}

function exibirTodosOsDetalhes(address) {
  let htmlContent = `<h3>Detalhes Completos</h3><div class="resultado-item">`;

  // Itera sobre todas as chaves do objeto JSON
  for (const key in address) {
    const valor = address[key];

    // Lógica principal: Só exibe a linha se possuir valor
    if (valor && valor !== "") {
      const chaveFormatada = key.charAt(0).toUpperCase() + key.slice(1);
      htmlContent += `<p><strong>${chaveFormatada}:</strong> ${valor}</p>`;
    }
  }

  htmlContent += `</div>`;
  
  if (ultimosResultadosSumarizados.length > 0) {
      htmlContent += `<button class="botao-voltar">&lsaquo; Voltar para a Lista</button>`;
  }

  resultadoDiv.innerHTML = htmlContent;
}

async function buscarDetalhesCompletos(cep) {
  resultadoDiv.innerHTML = "<p>Carregando detalhes...</p>";
  try {
    const response = await fetch(`${BASE_API_URL}/api/BuscaCep/${cep}`);
    const data = await response.json();
    if (!response.ok) {
        throw new Error(data.message || `Erro na busca: ${response.status}`);
    }
    exibirTodosOsDetalhes(data);
  } catch (error) {
    console.error("Erro na requisição de detalhes:", error);
    resultadoDiv.innerHTML = `<p style="color: red;">Não foi possível carregar os detalhes completos. Tente novamente.</p>`;
  }
}

// =================================================================
//                   FUNÇÕES DE CARREGAMENTO (APIs)
// =================================================================

// Busca a lista de estados da API pública do IBGE para popular o dropdown.
async function carregarEstados() {
  estadoSelect.disabled = true;
  try {
    const resposta = await fetch("https://servicodados.ibge.gov.br/api/v1/localidades/estados?orderBy=nome");
    if (!resposta.ok) throw new Error('Falha ao carregar estados.');
    const estados = await resposta.json();
    estadoSelect.innerHTML = "<option value=''>Selecione o estado</option>";
    estados.forEach((estado) => {
      const option = document.createElement("option");
      option.value = estado.sigla;
      option.textContent = estado.nome;
      estadoSelect.appendChild(option);
    });
  } catch (erro) {
    estadoSelect.innerHTML = "<option value=''>Erro ao carregar</option>";
    showError("Não foi possível carregar a lista de estados.");
  } finally {
    estadoSelect.disabled = false;
  }
}
async function carregarCidades(uf) {
  const lista = document.getElementById("listaCidades");
  cidadeInput.disabled = true;
  cidadeInput.value = "";
  lista.innerHTML = "";
  cidadeInput.placeholder = "Carregando cidades...";
  try {
    const resposta = await fetch(`https://servicodados.ibge.gov.br/api/v1/localidades/estados/${uf}/municipios`);
    if (!resposta.ok) throw new Error('Falha ao carregar municípios.');
    const cidades = await resposta.json();
    cidades.forEach((cidade) => {
      const option = document.createElement("option");
      option.value = cidade.nome;
      lista.appendChild(option);
    });
    cidadeInput.placeholder = "Digite ou selecione a cidade";
  } catch (erro) {
    cidadeInput.placeholder = "Erro ao carregar cidades";
    showError("Não foi possível carregar as cidades.");
  } finally {
    cidadeInput.disabled = false;
  }
}

// =================================================================
//                          EVENT LISTENERS
// =================================================================
// Ao carregar a página, inicia o carregamento dos estados e exibe o formulário.
document.addEventListener("DOMContentLoaded", () => {
  carregarEstados();
  showForm();
});

// Quando o estado é alterado, dispara a busca pelas cidades correspondentes.
estadoSelect.addEventListener("change", (e) => {
  const uf = e.target.value;
  if (uf) {
    carregarCidades(uf);
  } else {
    cidadeInput.value = "";
    document.getElementById("listaCidades").innerHTML = "";
    cidadeInput.placeholder = "Selecione o estado primeiro";
    cidadeInput.disabled = true;
  }
  limparErrosDeCampo();
});

// Lógica para o formulário de busca por CEP.
cepForm.addEventListener("submit", async (e) => {
  e.preventDefault();
  limparBuscaAnterior();
  const cep = cepInput.value;
  if (!regexCEP.test(cep)) {
    showError("CEP inválido. Formato: 12345-678 ou 12345678.");
    cepInput.classList.add("input-erro");
    return;
  }
  const cepParaAPI = cep.replace(/\D/g, "");
  try {
    const response = await fetch(`${BASE_API_URL}/api/BuscaCep/${cepParaAPI}`);
    const data = await response.json();
    if (!response.ok) throw new Error(data.message || `Erro na busca: ${response.status}`);
    
    // Envolve o resultado único em um array para usar a mesma função de exibição.
    exibirResultadosSumarizados([data]);
    showResults();
  } catch (error) {
    console.error("Erro na requisição de CEP:", error);
    showError(error.message || "Não foi possível conectar ao servidor.");
  }
});

// Lógica para o formulário de busca por Endereço.
estadoCidadeForm.addEventListener("submit", async (e) => {
  e.preventDefault();
  limparBuscaAnterior();
  const uf = estadoSelect.value;
  const cidade = cidadeInput.value.trim();
  const logradouro = logradouroInput.value.trim();

  // Validação dos campos antes de enviar a requisição.
  let hasError = false;
  if (!uf) { estadoSelect.classList.add("input-erro"); hasError = true; }
  if (!cidade) { cidadeInput.classList.add("input-erro"); hasError = true; }
  if (!logradouro || !regexLogradouro.test(logradouro)) { logradouroInput.classList.add("input-erro"); hasError = true; }
  if(hasError) { showError("Verifique os campos obrigatórios (Estado, Cidade, Logradouro)."); return; }

  try {
    // Usa URLSearchParams para construir a query string de forma segura, tratando caracteres especiais.
    const params = new URLSearchParams({ uf, municipio: cidade, logradouro });
    const response = await fetch(`${BASE_API_URL}/api/BuscaEndereco?${params.toString()}`);
    const data = await response.json();
    if (!response.ok) throw new Error(data.message || `Erro na busca: ${response.status}`);
    
    exibirResultadosSumarizados(data);
    showResults();
  } catch (error) {
    console.error("Erro na requisição de Endereço:", error);
    showError(error.message || "Não foi possível conectar ao servidor.");
  }
});

// Limpa e reseta o formulário para uma nova busca.
novaBuscaBtn.addEventListener("click", () => {
  showForm();
  cepForm.reset();
  estadoCidadeForm.reset();
  cidadeInput.disabled = true;
  document.getElementById("listaCidades").innerHTML = "";
  cidadeInput.placeholder = "Selecione o estado primeiro";
  cepInput.focus();
});

// Melhora a experiência do usuário limpando o status de erro de um campo assim que ele começa a digitar.
[cepInput, estadoSelect, cidadeInput, logradouroInput].forEach(input => {
  input.addEventListener("input", () => {
    input.classList.remove("input-erro");
  });
});

// Usa a técnica de "Event Delegation" para lidar com cliques em botões que são criados dinamicamente.
resultadoContainer.addEventListener('click', function(event) {
    const target = event.target;

    // Ação para "Ver Completo"
    if (target && target.matches('.botao-ver-completo')) {
        const cep = target.dataset.cep; // Pega o CEP armazenado no atributo 'data-cep' do botão.
        if (cep) {
            buscarDetalhesCompletos(cep);
        }
    }

    // Ação para "Voltar"
    if (target && target.matches('.botao-voltar')) {
        if (ultimosResultadosSumarizados.length > 0) {
            // Re-renderiza a lista de resultados que foi "memorizada" anteriormente.
            exibirResultadosSumarizados(ultimosResultadosSumarizados);
        }
    }
});