import React, { useState, useEffect } from 'react';
import axios from 'axios';

interface DashboardData {
  totalLivros: number;
  totalUsuarios: number;
  emprestimosAtivos: number;
  emprestimosDevolvidos: number;
}

function Dashboard() {
  const [dados, setDados] = useState<DashboardData>({
    totalLivros: 0,
    totalUsuarios: 0,
    emprestimosAtivos: 0,
    emprestimosDevolvidos: 0,
  });
  const [carregando, setCarregando] = useState(true);

  useEffect(() => {
    carregarDados();
  }, []);

  async function carregarDados() {
    try {
      const [livros, usuarios, emprestimos] = await Promise.all([
        axios.get('http://localhost:5093/api/livros'),
        axios.get('http://localhost:5093/api/usuarios'),
        axios.get('http://localhost:5093/api/emprestimos'),
      ]);

      const emprestimosData = emprestimos.data;
      const ativos = emprestimosData.filter((e: any) => e.status === 'ativo').length;
      const devolvidos = emprestimosData.filter((e: any) => e.status === 'devolvido').length;

      setDados({
        totalLivros: livros.data.length,
        totalUsuarios: usuarios.data.length,
        emprestimosAtivos: ativos,
        emprestimosDevolvidos: devolvidos,
      });
    } catch (erro) {
      console.error('Erro ao carregar dados do dashboard:', erro);
    } finally {
      setCarregando(false);
    }
  }

  if (carregando) {
    return <div style={{ textAlign: 'center', padding: '50px' }}>Carregando dados...</div>;
  }

  const cardStyle: React.CSSProperties = {
    flex: 1,
    padding: '20px',
    borderRadius: '8px',
    backgroundColor: '#f3f4f6',
    textAlign: 'center',
    boxShadow: '0 1px 3px rgba(0,0,0,0.1)',
  };

  const numeroStyle: React.CSSProperties = {
    fontSize: '32px',
    fontWeight: 'bold',
    color: '#2563eb',
    margin: '10px 0',
  };

  return (
    <div style={{ padding: '30px' }}>
      <h1>Dashboard da Biblioteca</h1>
      <p>Visualize as estatísticas gerais do sistema</p>

      <div style={{ display: 'flex', gap: '20px', marginTop: '30px', flexWrap: 'wrap' }}>
        {/* Total de Livros */}
        <div style={cardStyle}>
          <h3>Total de Livros</h3>
          <div style={numeroStyle}>{dados.totalLivros}</div>
          <p style={{ color: '#666' }}>Livros no acervo</p>
        </div>

        {/* Total de Usuários */}
        <div style={cardStyle}>
          <h3>Total de Usuários</h3>
          <div style={numeroStyle}>{dados.totalUsuarios}</div>
          <p style={{ color: '#666' }}>Usuários cadastrados</p>
        </div>

        {/* Empréstimos Ativos */}
        <div style={{ ...cardStyle, backgroundColor: '#fef3c7' }}>
          <h3>Empréstimos Ativos</h3>
          <div style={{ ...numeroStyle, color: '#d97706' }}>{dados.emprestimosAtivos}</div>
          <p style={{ color: '#666' }}>Livros emprestados</p>
        </div>

        {/* Empréstimos Devolvidos */}
        <div style={{ ...cardStyle, backgroundColor: '#d1fae5' }}>
          <h3>Empréstimos Devolvidos</h3>
          <div style={{ ...numeroStyle, color: '#059669' }}>{dados.emprestimosDevolvidos}</div>
          <p style={{ color: '#666' }}>Livros devolvidos</p>
        </div>
      </div>

      {/* Resumo */}
      <div style={{ marginTop: '40px', padding: '20px', backgroundColor: '#eff6ff', borderRadius: '8px', borderLeft: '4px solid #2563eb' }}>
        <h3>Resumo</h3>
        <p>
          A biblioteca possui <strong>{dados.totalLivros}</strong> livros e <strong>{dados.totalUsuarios}</strong> usuários cadastrados.
          Atualmente, <strong>{dados.emprestimosAtivos}</strong> livro{dados.emprestimosAtivos !== 1 ? 's' : ''} está{dados.emprestimosAtivos !== 1 ? 'ão' : ''} emprestado{dados.emprestimosAtivos !== 1 ? 's' : ''}.
        </p>
      </div>
    </div>
  );
}

export default Dashboard;
