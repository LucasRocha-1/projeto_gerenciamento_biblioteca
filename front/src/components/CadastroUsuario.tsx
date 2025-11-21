import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

function CadastroUsuario() {
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const navigate = useNavigate();

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    
    if(!nome || !email) {
        alert("Preencha todos os campos!");
        return;
    }

    axios
      .post("http://localhost:5093/api/usuarios", { nome, email })
      .then(() => {
        alert("Usuário cadastrado com sucesso!");
        navigate("/admin/usuarios");
      })
      .catch((erro) => alert("Erro ao cadastrar: " + erro.message));
  }

  return (
    <div className="container" style={{ maxWidth: '500px', marginTop: '50px' }}>
      <div className="card-livro" style={{ padding: '30px', alignItems: 'flex-start', textAlign: 'left' }}>
        <h2>Novo Cliente</h2>
        <p style={{marginBottom: '20px'}}>Cadastre um novo aluno ou leitor no sistema.</p>
        
        <form onSubmit={handleSubmit} style={{ width: '100%' }}>
          <label>Nome Completo:</label>
          <input 
            type="text" 
            placeholder="Ex: João da Silva"
            value={nome} 
            onChange={(e) => setNome(e.target.value)} 
            required 
          />

          <label>E-mail (ou Matrícula):</label>
          <input 
            type="text" 
            placeholder="Ex: joao@email.com"
            value={email} 
            onChange={(e) => setEmail(e.target.value)} 
            required 
          />

          <div style={{ display: 'flex', gap: '10px', marginTop: '10px' }}>
              <button type="submit" className="btn-success" style={{ flex: 1 }}>Salvar Cliente</button>
              <button type="button" onClick={() => navigate('/')} style={{ background: '#ccc', padding: '10px', border: 'none', borderRadius: '6px', cursor: 'pointer' }}>Cancelar</button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default CadastroUsuario;