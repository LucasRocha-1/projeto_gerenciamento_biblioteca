import React, { useEffect, useState } from "react";
import axios from "axios";
import { Link } from "react-router-dom";

interface Livro {
  id: number;
  isbn: string;
  titulo: string;
  autor: { nome: string };
  emprestadoParaUsuarioId: number | null;
  emprestadoParaUsuario: { nome: string } | null;
}

interface Usuario {
  id: number;
  nome: string;
}

function LivrosList() {
  const [livros, setLivros] = useState<Livro[]>([]);
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [carregando, setCarregando] = useState(false);

  // --- ESTADOS PARA O MODAL DE EMPRÉSTIMO ---
  const [livroParaEmprestar, setLivroParaEmprestar] = useState<number | null>(null);
  const [termoBusca, setTermoBusca] = useState(""); // O que o admin digita
  const [usuarioSelecionadoId, setUsuarioSelecionadoId] = useState<number | null>(null);

  useEffect(() => {
    carregarTudo();
  }, []);

  function carregarTudo() {
    carregarLivros();
    // Já buscamos os usuários para deixar pronto na memória
    axios.get("http://localhost:5093/api/usuarios").then(res => setUsuarios(res.data));
  }

  function carregarLivros() {
    axios.get("http://localhost:5093/api/livros").then((resposta) => {
      setLivros(resposta.data);
    });
  }

  // --- LÓGICA DE DEVOLUÇÃO ---
  function handleDevolver(id: number) {
    if (!window.confirm("Confirmar a devolução deste livro?")) return;
    axios.post(`http://localhost:5093/api/emprestimos/devolver-por-livro/${id}`)
      .then(() => { alert("Devolvido!"); carregarLivros(); })
      .catch((erro) => alert("Erro: " + (erro.response?.data || erro.message)));
  }

  function handleExcluir(id: number) {
    if (!window.confirm("Excluir livro?")) return;
    axios.delete(`http://localhost:5093/api/livros/${id}`).then(() => carregarLivros());
  }

  // --- LÓGICA DE EMPRÉSTIMO (ADMIN) ---
  function abrirModalEmprestimo(idLivro: number) {
    setLivroParaEmprestar(idLivro);
    setTermoBusca(""); // Limpa a busca
    setUsuarioSelecionadoId(null); // Limpa seleção anterior
  }

  function confirmarEmprestimo() {
    if (!usuarioSelecionadoId) {
        alert("Selecione um usuário na lista!");
        return;
    }
    
    axios.post(`http://localhost:5093/api/emprestimos/registrar/${livroParaEmprestar}/${usuarioSelecionadoId}`)
      .then(() => {
          alert("Empréstimo realizado com sucesso!");
          setLivroParaEmprestar(null); // Fecha modal
          carregarLivros(); // Atualiza tabela
      })
      .catch(err => alert("Erro: " + err.message));
  }

  // Filtra a lista de usuários baseado no que o admin digita
  const usuariosFiltrados = usuarios.filter(u => 
    u.nome.toLowerCase().includes(termoBusca.toLowerCase())
  );

  // --- IMPORTAÇÃO DO GOOGLE ---
  function handleImportarDoGoogle() {
    setCarregando(true);
    axios.post("http://localhost:5093/api/seed").then(() => carregarLivros()).finally(() => setCarregando(false));
  }

  return (
    <div className="container">
      <h2>Gestão de Acervo (Admin)</h2>
      
      {/* Barra de Ações */}
      <div style={{ display: 'flex', gap: '15px', marginBottom: '20px', alignItems: 'center' }}>
        <Link to="/cadastro-livro" className="btn-primary" style={{textDecoration: 'none'}}>+ Novo Livro</Link>
        <Link to="/cadastro-usuario" className="btn-primary" style={{textDecoration: 'none'}}>+ Novo Cliente</Link> {}
        {/* Autor agora é criado durante o cadastro do livro; link removido */}
        
        <button onClick={handleImportarDoGoogle} className="btn-success" disabled={carregando} style={{ marginLeft: 'auto' }}>
            {carregando ? "⏳ Importando..." : "☁️ Importar Google"}
        </button>
      </div>

      {/* Tabela */}
      <table>
        <thead>
          <tr>
            <th>Título</th>
            <th>Autor</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {livros.map((livro) => (
            <tr key={livro.id}>
              <td>{livro.titulo}</td>
              <td>{livro.autor?.nome}</td>
              
              <td style={{ fontWeight: 'bold', color: livro.emprestadoParaUsuarioId ? "#e74c3c" : "#27ae60" }}>
                {livro.emprestadoParaUsuarioId ? `Emprestado para: ${livro.emprestadoParaUsuario?.nome || "Cliente (Nome não carregado)"}`: "Disponível"}
              </td>
              
              <td>
                {/* SE ESTIVER EMPRESTADO -> MOSTRA DEVOLVER */}
                {livro.emprestadoParaUsuarioId ? (
                  <button className="btn-primary" onClick={() => handleDevolver(livro.id)} style={{marginRight: '5px'}}>
                    Devolver
                  </button>
                ) : (
                
                  <button className="btn-success" onClick={() => abrirModalEmprestimo(livro.id)} style={{marginRight: '5px'}}>
                    Emprestar
                  </button>
                )}
                
                <button className="btn-danger" onClick={() => handleExcluir(livro.id)}>Excluir</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* --- MODAL DE EMPRÉSTIMO --- */}
      {livroParaEmprestar && (
        <div style={{
            position: 'fixed', top: 0, left: 0, width: '100%', height: '100%',
            backgroundColor: 'rgba(0,0,0,0.6)', display: 'flex', justifyContent: 'center', alignItems: 'center', zIndex: 1000
        }}>
            <div style={{ backgroundColor: 'white', padding: '30px', borderRadius: '10px', width: '400px', maxHeight: '80vh', display: 'flex', flexDirection: 'column' }}>
                <h3>Emprestar Livro</h3>
                <p>Busque o nome do cliente:</p>

                {/* Barra de Pesquisa */}
                <input 
                    type="text" 
                    placeholder="Digite o nome..." 
                    value={termoBusca}
                    onChange={(e) => setTermoBusca(e.target.value)}
                    autoFocus
                />

                {/* Lista de Resultados Filtrados */}
                <div style={{ maxHeight: '150px', overflowY: 'auto', border: '1px solid #eee', marginBottom: '15px' }}>
                    {usuariosFiltrados.map(u => (
                        <div 
                            key={u.id}
                            onClick={() => setUsuarioSelecionadoId(u.id)}
                            style={{ 
                                padding: '10px', cursor: 'pointer', 
                                backgroundColor: usuarioSelecionadoId === u.id ? '#dff9fb' : 'white',
                                borderBottom: '1px solid #eee'
                            }}
                        >
                            {u.nome}
                        </div>
                    ))}
                    {usuariosFiltrados.length === 0 && <div style={{padding:'10px', color:'#999'}}>Nenhum usuário encontrado.</div>}
                </div>

                <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px' }}>
                    <button onClick={() => setLivroParaEmprestar(null)} style={{background: '#ccc'}}>Cancelar</button>
                    <button onClick={confirmarEmprestimo} className="btn-success" disabled={!usuarioSelecionadoId}>Confirmar Empréstimo</button>
                </div>
            </div>
        </div>
      )}

    </div>
  );
}

export default LivrosList;