import React, { useState } from "react";
import axios from "axios";

function CadastroUsuario() {
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const usuario = { nome, email };

    try {
      const response = await axios.post("http://localhost:5093/api/usuarios", usuario);
      if (response.status === 201) {
        alert("Usuário cadastrado com sucesso!");
        setNome("");
        setEmail("");
      }
    } catch (error) {
      alert("Erro ao cadastrar usuário.");
    }
  };

  return (
    <div>
      <h2>Cadastrar Usuário</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Nome"
          value={nome}
          onChange={(e) => setNome(e.target.value)}
          required
        />
        <input
          type="email"
          placeholder="E-mail"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <button type="submit">Cadastrar</button>
      </form>
    </div>
  );
}

export default CadastroUsuario;
