import React, { useEffect, useState } from "react";
import axios from "axios";
import { Link } from "react-router-dom";

interface Livro {
  id: number;
  titulo: string;
  autor: { nome: string };
  emprestadoParaUsuarioId: number | null;
  emprestadoParaUsuario: { nome: string } | null;
}

function LivrosList() {
  const [livros, setLivros] = useState<Livro[]>([]);

  useEffect(() => {
    carregarLivros();
  }, []);

  function carregarLivros() {
    axios.get("http://localhost:5093/api/livros").then((resposta) => {
      setLivros(resposta.data);
    });
  }

  function handleDevolver(id: number) {
    if (!window.confirm("Confirmar a devolução deste livro?")) return;

    axios
      .post(`http://localhost:5093/api/livros/${id}/devolver`)
      .then(() => {
        alert("Livro devolvido com sucesso!");
        carregarLivros(); 
      })
      .catch((erro) => {
        alert("Erro ao devolver: " + erro.message);
      });
  }

  function handleExcluir(id: number) {
     axios.delete(`http://localhost:5093/api/livros/${id}`)
     .then(() => carregarLivros());
  }

  return (
    <div style={{ padding: "20px" }}>
      <h2>Gestão de Acervo (Admin)</h2>
      <Link to="/cadastro-livro">Cadastrar Novo Livro</Link> | 
      <Link to="/cadastro-autor"> Cadastrar Autor</Link>

      <table border={1} style={{ width: "100%", marginTop: "20px", borderCollapse: "collapse" }}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Título</th>
            <th>Autor</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {livros.map((livro) => (
            <tr key={livro.id}>
              <td>{livro.id}</td>
              <td>{livro.titulo}</td>
              <td>{livro.autor?.nome}</td>
              <td style={{ color: livro.emprestadoParaUsuarioId ? "red" : "green" }}>
                {livro.emprestadoParaUsuarioId
                  ? `Emprestado para: ${livro.emprestadoParaUsuario?.nome}`
                  : "Disponível"}
              </td>
              <td>
                {/* BOTÃO DE DEVOLUÇÃO SÓ APARECE SE ESTIVER EMPRESTADO */}
                {livro.emprestadoParaUsuarioId && (
                  <button onClick={() => handleDevolver(livro.id)} style={{ marginRight: "10px" }}>
                  Devolver
                  </button>
                )}
                <button onClick={() => handleExcluir(livro.id)}>Excluir</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default LivrosList;