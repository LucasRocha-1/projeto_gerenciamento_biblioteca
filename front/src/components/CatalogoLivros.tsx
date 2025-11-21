import React, { useEffect, useState } from 'react';
import axios from 'axios';

// Tipos
interface Livro {
  id: number;
  titulo: string;
  autor: { nome: string } | null;
  emprestadoParaUsuarioId: number | null;
  capaUrl: string | null;
}

interface Usuario {
  id: number;
  nome: string;
}

interface Props {
  usuarioId: number;
  usuarioNome: string;
}

function CatalogoLivros({ usuarioId, usuarioNome }: Props) {
  const [livros, setLivros] = useState<Livro[]>([]);
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [livroSelecionado, setLivroSelecionado] = useState<number | null>(null);
  const [usuarioSelecionadoId, setUsuarioSelecionadoId] = useState<string>("");

  useEffect(() => {
    carregarDados();
  }, []);
  function carregarDados() {
    // Busca Livros
    axios.get('http://localhost:5093/api/livros').then(res => setLivros(res.data));
    // Busca Usuários (para o dropdown)
    axios.get('http://localhost:5093/api/usuarios').then(res => setUsuarios(res.data));
  }

  // 1. Abre o modal ao invés de emprestar direto
  function iniciarEmprestimo(idLivro: number) {
    setLivroSelecionado(idLivro);
    setUsuarioSelecionadoId(""); 
  }
  function confirmarEmprestimo() {
    if (!usuarioSelecionadoId) {
      alert("Por favor, selecione um usuário.");
      return;
    }

    axios.post(`http://localhost:5093/api/emprestimos/registrar/${livroSelecionado}/${usuarioSelecionadoId}`)
      .then(() => {
        alert('Livro reservado com sucesso!');
        setLivroSelecionado(null); // Fecha o modal
        carregarDados(); // Atualiza a lista
      })
      .catch(erro => {
        alert('Erro: ' + (erro.response?.data?.mensagem || erro.message));
      });
  }

  return (
    <div style={{ padding: '20px', position: 'relative' }}>
      <h2>Catálogo de Livros Disponíveis</h2>
      
      {/* LISTA DE LIVROS */}
      <div className="catalogo-grid">
        {livros.map(livro => (
          <div key={livro.id} className="card-livro">
            <img 
            src={livro.capaUrl ? livro.capaUrl : "https://via.placeholder.com/150x200?text=Sem+Capa"} 
            alt={livro.titulo}
            className="capa-livro" 
            style={{ width: '100%', height: '200px', objectFit: 'cover', borderRadius: '4px', marginBottom: '10px' }}/>
            <h3>{livro.titulo}</h3>
            <p>Autor: {livro.autor?.nome || 'Desconhecido'}</p>

            {livro.emprestadoParaUsuarioId === null ? (
              <button 
                onClick={() => iniciarEmprestimo(livro.id)}
                style={{ width: '100%', padding: '10px', backgroundColor: '#2ecc71', color: 'white', border: 'none', cursor: 'pointer' }}>
                Reservar
              </button>
            ) : (
              <button disabled style={{ width: '100%', padding: '10px', backgroundColor: '#ccc', cursor: 'not-allowed' }}>
                ❌ Indisponível
              </button>
            )}
          </div>
        ))}
      </div>

      {/*janela flutuante */}
      {livroSelecionado !== null && (
        <div style={{
          position: 'fixed', top: 0, left: 0, width: '100%', height: '100%',
          backgroundColor: 'rgba(0,0,0,0.5)', display: 'flex', justifyContent: 'center', alignItems: 'center'
        }}>
          <div style={{ backgroundColor: 'white', padding: '30px', borderRadius: '10px', minWidth: '300px' }}>
            <h3>Confirmar Reserva</h3>
            <p>Quem está retirando este livro?</p>
            
            <select 
              style={{ width: '100%', padding: '10px', marginBottom: '15px' }}
              value={usuarioSelecionadoId}
              onChange={(e) => setUsuarioSelecionadoId(e.target.value)}
            >
              <option value="">Selecione seu nome...</option>
              {usuarios.map(u => (
                <option key={u.id} value={u.id}>{u.nome}</option>
              ))}
            </select>

            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px' }}>
              <button onClick={() => setLivroSelecionado(null)} style={{ padding: '10px' }}>Cancelar</button>
              <button 
                onClick={confirmarEmprestimo}
                style={{ padding: '10px', backgroundColor: '#3498db', color: 'white', border: 'none' }}>
                Confirmar
              </button>
            </div>
          </div>
        </div>
      )}

    </div>
  );
}

export default CatalogoLivros;