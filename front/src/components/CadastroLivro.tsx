import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

interface Autor {
  id: number;
  nome: string;
}

function CadastroLivro() {
  const [titulo, setTitulo] = useState("");
  const [isbn, setIsbn] = useState("");
  const [ano, setAno] = useState(2024);
  const [autorNome, setAutorNome] = useState(""); // Nome do autor para criar junto

  const navigate = useNavigate();

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    try {
      if (!autorNome) {
        alert('Informe o nome do autor');
        return;
      }

      // 1) Criar o autor (retorna o autor criado com id)
      const autorRes = await axios.post("http://localhost:5093/api/autores", { nome: autorNome });
      const criado = autorRes.data;
      const novoLivro = {
        titulo: titulo,
        isbn: isbn,
        anoPublicacao: ano,
        autorId: Number(criado.id ?? criado.id)
      };

      // 2) Criar o livro com o autor criado
      await axios.post("http://localhost:5093/api/livros", novoLivro);
      alert("Livro cadastrado com sucesso!");
      navigate("/admin/livros");
    } catch (erro: any) {
      alert("Erro: " + (erro.response?.data || erro.message));
    }
  }

  return (
    <div style={{ padding: "20px" }}>
      <h2>Cadastrar Novo Livro</h2>
      <form onSubmit={handleSubmit} style={{ display: "flex", flexDirection: "column", maxWidth: "300px", gap: "10px" }}>
        
        <label>Título:</label>
        <input type="text" value={titulo} onChange={(e) => setTitulo(e.target.value)} required />

        <label>ISBN:</label>
        <input type="text" value={isbn} onChange={(e) => setIsbn(e.target.value)} />

        <label>Ano:</label>
        <input type="number" value={ano} onChange={(e) => setAno(Number(e.target.value))} />

        <label>Autor (novo):</label>
        <input type="text" value={autorNome} onChange={(e) => setAutorNome(e.target.value)} placeholder="Nome do autor" required />

        <button type="submit">Cadastrar Livro</button>
      </form>
    </div>
  );
}

export default CadastroLivro;