import React, { useState } from "react";
import axios from "axios";

function CadastroLivro() {
  const [titulo, setTitulo] = useState("");
  const [isbn, setIsbn] = useState("");
  const [anoPublicacao, setAnoPublicacao] = useState("");
  const [autorId, setAutorId] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const livro = { titulo, isbn, anoPublicacao, autorId };

    try {
      const response = await axios.post("http://localhost:5093/api/livros", livro);
      if (response.status === 201) {
        alert("Livro cadastrado com sucesso!");
        setTitulo("");
        setIsbn("");
        setAnoPublicacao("");
        setAutorId("");
      }
    } catch (error) {
      alert("Erro ao cadastrar livro.");
    }
  };

  return (
    <div>
      <h2>Cadastrar Livro</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Título"
          value={titulo}
          onChange={(e) => setTitulo(e.target.value)}
          required
        />
        <input
          type="text"
          placeholder="ISBN"
          value={isbn}
          onChange={(e) => setIsbn(e.target.value)}
          required
        />
        <input
          type="number"
          placeholder="Ano de Publicação"
          value={anoPublicacao}
          onChange={(e) => setAnoPublicacao(e.target.value)}
          required
        />
        <input
          type="number"
          placeholder="ID do Autor"
          value={autorId}
          onChange={(e) => setAutorId(e.target.value)}
          required
        />
        <button type="submit">Cadastrar</button>
      </form>
    </div>
  );
}

export default CadastroLivro;
