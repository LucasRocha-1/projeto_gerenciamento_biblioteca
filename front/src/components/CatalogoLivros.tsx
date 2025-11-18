import React, { useEffect, useState } from 'react';
import axios from 'axios';

interface Livro {
  id: number;
  titulo: string;
  autor: { nome: string } | null;
  emprestadoParaUsuarioId: number | null;
}

function CatalogoLivros() {
  const [livros, setLivros] = useState<Livro[]>([]);

  useEffect(() => {
    carregarLivros();
  }, []);

  function carregarLivros() {
    axios.get('http://localhost:5093/api/livros') 
      .then(resposta => setLivros(resposta.data));
  }

  function handleReservar(id: number) {
    //usando usuarioId = 1 fixo para teste. 
    const usuarioIdTeste = 1; 

    axios.post(`http://localhost:5093/api/livros/${id}/emprestar/${usuarioIdTeste}`)
      .then(() => {
        alert('Livro reservado com sucesso!');
        carregarLivros();
      })
      .catch(erro => {
        alert('Erro ao reservar: ' + erro.response?.data || erro.message);
      });
  }

  return (
    <div style={{ padding: '20px' }}>
      <h2>Catálogo de Livros Disponíveis</h2>
      <ul style={{ listStyle: 'none', padding: 0 }}>
        {livros.map(livro => (
          <li key={livro.id} style={{ 
              border: '1px solid #ddd', margin: '10px 0', padding: '15px', 
              borderRadius: '5px', display: 'flex', justifyContent: 'space-between', alignItems: 'center'
            }}>
            
            <div>
              <strong>{livro.titulo}</strong> 
              <br/> 
              <small>Autor: {livro.autor?.nome || 'Desconhecido'}</small>
            </div>

            {/* LÓGICA DE EXIBIÇÃO DO BOTÃO */}
            {livro.emprestadoParaUsuarioId === null ? (
              <button 
                onClick={() => handleReservar(livro.id)}
                style={{ backgroundColor: 'green', color: 'white', padding: '8px 15px', border: 'none', cursor: 'pointer' }}>
                Reservar Agora
              </button>
            ) : (
              <span style={{ color: 'red', fontWeight: 'bold', border: '1px solid red', padding: '5px' }}>
                Indisponível
              </span>
            )}

          </li>
        ))}
      </ul>
    </div>
  );
}

export default CatalogoLivros;