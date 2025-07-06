# Consulta de Endereços 📍

Aplicação full-stack para consulta de endereços no Brasil a partir de um CEP ou de um endereço parcial (UF, Cidade, Logradouro). Construído como um desafio técnico.

![Demonstração da Aplicação]()

---

## ✨ Funcionalidades

- Busca de endereço completo por CEP.
- Busca de uma lista de CEPs por endereço.
- Visualização de detalhes completos do endereço.
- Interface reativa e responsiva.

---

## 🚀 Tecnologias Utilizadas

* **Backend:** C#, .NET 9, ASP.NET Core (API RESTful)
* **Frontend:** HTML, CSS, JavaScript (Vanilla)
* **APIs Externas:** ViaCEP (dados de endereço) e IBGE (dados de estados/municípios).
* **Ferramentas:** Git, GitHub, Visual Studio.

---

## 🛠️ Como Executar Localmente

### **1. Backend (API)**

1.  Clone o repositório.
2.  Navegue até a pasta da API:
    ```bash
    cd BuscaCepApi
    ```
3.  Execute a API usando o perfil HTTPS:
    ```bash
    dotnet run --launch-profile https
    ```
4.  A API estará disponível em `https://localhost:7001`.

### **2. Frontend**

1.  Abra a pasta do projeto no Visual Studio Code.
2.  Instale a extensão **Live Server**.
3.  Clique com o botão direito no arquivo `index.html` e selecione "Open with Live Server".

---

## 📝 Endpoints da API

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| `GET` | `/api/BuscaCep/{cep}` | Retorna os detalhes de um endereço a partir do CEP. |
| `GET` | `/api/BuscaEndereco`| Retorna uma lista sumarizada de endereços a partir dos parâmetros `uf`, `municipio` e `logradouro`. |

---

## 👤 Autor

**Matheus Kreutzz**
