import React, { useState } from "react";
import axios from "axios";

function CadastroAutor() {
  const [nome, setNome] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const autor = { nome };

    try {
      const response = await axios.post("http://localhost:5093/api/autores", autor);
      if (response.status === 201) {
        alert("Autor cadastrado com sucesso!");
        setNome("");
      }
    } catch (error) {
      alert("Erro ao cadastrar Autor.");
    }
  };

  return (
    <div>
      <h2>Cadastrar Autor</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Nome"
          value={nome}
          onChange={(e) => setNome(e.target.value)}
          required
        />
        <button type="submit">Cadastrar</button>
      </form>
    </div>
  );
}

export default CadastroAutor;
