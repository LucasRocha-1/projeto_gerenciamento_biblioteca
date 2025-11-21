import React, { useEffect, useState } from "react";
import axios from "axios";
import { Link } from "react-router-dom";

interface Usuario {
  id: number;
  nome: string;
  email?: string; // O email pode ser opcional dependendo do seu banco
}

function UsuariosList() {
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);

  useEffect(() => {
    carregarUsuarios();
  }, []);

  function carregarUsuarios() {
    axios.get("http://localhost:5093/api/usuarios")
      .then((res) => setUsuarios(res.data));
  }

  function handleExcluir(id: number) {
    if (!window.confirm("Tem certeza que deseja excluir este cliente?")) return;

    axios.delete(`http://localhost:5093/api/usuarios/${id}`)
      .then(() => {
          carregarUsuarios();
          alert("Usuário excluído!");
      })
      .catch(err => alert("Erro ao excluir (talvez ele tenha livros pendentes?): " + err.message));
  }

  return (
    <div className="container">
      <h2>Lista de Clientes</h2>
      
      <div style={{ marginBottom: '20px' }}>
        <Link to="/cadastro-usuario" className="btn-primary" style={{textDecoration: 'none'}}>
            + Novo Cliente
        </Link>
      </div>

      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Nome</th>
            <th>Email</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {usuarios.map((user) => (
            <tr key={user.id}>
              <td>{user.id}</td>
              <td>{user.nome}</td>
              <td>{user.email || "-"}</td>
              <td>
                <button className="btn-danger" onClick={() => handleExcluir(user.id)}>
                  Excluir
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default UsuariosList;