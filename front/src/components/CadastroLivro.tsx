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
  const [autorId, setAutorId] = useState(""); // Vai guardar o ID selecionado
  const [autores, setAutores] = useState<Autor[]>([]); // Lista para o Select

  const navigate = useNavigate();

  // Carrega os autores assim que a tela abre
  useEffect(() => {
    axios.get("http://localhost:5093/api/autores/listar").then((res) => {
      setAutores(res.data);
    });
  }, []);

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    const novoLivro = {
      titulo: titulo,
      isbn: isbn,
      anoPublicacao: ano,
      autorId: Number(autorId) // Converte para numero antes de enviar
    };

    axios.post("http://localhost:5093/api/livros", novoLivro)
      .then(() => {
        alert("Livro cadastrado com sucesso!");
        navigate("/admin/livros");
      })
      .catch((erro) => alert("Erro: " + erro.response?.data || erro.message));
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

        <label>Autor:</label>
        <select value={autorId} onChange={(e) => setAutorId(e.target.value)} required>
          <option value="">Selecione um autor...</option>
          {autores.map((autor) => (
            <option key={autor.id} value={autor.id}>
              {autor.nome}
            </option>
          ))}
        </select>

        <button type="submit">Cadastrar Livro</button>
      </form>
    </div>
  );
}

export default CadastroLivro;