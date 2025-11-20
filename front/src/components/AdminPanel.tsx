import React, { useState, useEffect } from 'react';
import axios from 'axios';

interface Usuario {
  id: number;
  nome: string;
  email: string;
  isAdmin: boolean;
}

function AdminPanel() {
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [mensagem, setMensagem] = useState('');

  useEffect(() => {
    carregarUsuarios();
  }, []);

  const carregarUsuarios = async () => {
    try {
      const response = await axios.get('http://localhost:5093/api/usuarios');
      setUsuarios(response.data || []);
      console.log('Usuários carregados:', response.data);
    } catch (erro) {
        console.error('Erro ao carregar usuários:', erro);
    } finally {
      setCarregando(false);
    }
  };

  const tornarAdmin = async (usuarioId: number, usuarioNome: string) => {
    if (!window.confirm(`Deseja tornar ${usuarioNome} um administrador?`)) return;

    try {
      const response = await axios.post(
        `http://localhost:5093/api/auth/tornar-admin/${usuarioId}`
      );
      console.log('Usuário agora é admin:', response.data);
      setMensagem(`${usuarioNome} agora é administrador!`);
      carregarUsuarios();
      setTimeout(() => setMensagem(''), 3000);
    } catch (erro) {
      console.error('Erro:', erro);
      setMensagem(`Erro ao tornar admin: ${erro}`);
    }
  };

  if (carregando) {
    return <div style={{ textAlign: 'center', padding: '50px' }}>Carregando usuários...</div>;
  }

  return (
    <div style={{ padding: '30px' }}>
      <h1>Painel de Administração</h1>
      <p>Gerencie permissões de usuários</p>

      {mensagem && (
        <div
          style={{
            padding: '15px',
            marginBottom: '20px',
            backgroundColor: mensagem.toLowerCase().includes('erro') ? '#fee2e2' : '#d1fae5',
            color: mensagem.toLowerCase().includes('erro') ? '#7f1d1d' : '#065f46',
            borderRadius: '4px',
            borderLeft: `4px solid ${mensagem.toLowerCase().includes('erro') ? '#ef4444' : '#10b981'}`,
          }}
        >
          {mensagem}
        </div>
      )}

      <table
        style={{
          width: '100%',
          borderCollapse: 'collapse',
          marginTop: '20px',
          boxShadow: '0 1px 3px rgba(0,0,0,0.1)',
        }}
      >
        <thead>
          <tr style={{ backgroundColor: '#f3f4f6' }}>
            <th style={{ padding: '12px', textAlign: 'left', borderBottom: '2px solid #ddd' }}>ID</th>
            <th style={{ padding: '12px', textAlign: 'left', borderBottom: '2px solid #ddd' }}>Nome</th>
            <th style={{ padding: '12px', textAlign: 'left', borderBottom: '2px solid #ddd' }}>Email</th>
            <th style={{ padding: '12px', textAlign: 'left', borderBottom: '2px solid #ddd' }}>Status</th>
            <th style={{ padding: '12px', textAlign: 'left', borderBottom: '2px solid #ddd' }}>Ações</th>
          </tr>
        </thead>
        <tbody>
          {usuarios.map((usuario) => (
            <tr key={usuario.id} style={{ borderBottom: '1px solid #eee' }}>
              <td style={{ padding: '12px' }}>{usuario.id}</td>
              <td style={{ padding: '12px' }}>{usuario.nome}</td>
              <td style={{ padding: '12px' }}>{usuario.email}</td>
              <td style={{ padding: '12px' }}>
                {usuario.isAdmin ? (
                  <span style={{ backgroundColor: '#10b981', color: 'white', padding: '4px 8px', borderRadius: '4px', fontSize: '12px' }}>
                    Admin
                  </span>
                ) : (
                  <span style={{ backgroundColor: '#9ca3af', color: 'white', padding: '4px 8px', borderRadius: '4px', fontSize: '12px' }}>
                    Usuário
                  </span>
                )}
              </td>
              <td style={{ padding: '12px' }}>
                {!usuario.isAdmin ? (
                  <button
                    onClick={() => tornarAdmin(usuario.id, usuario.nome)}
                    style={{
                      backgroundColor: '#2563eb',
                      color: 'white',
                      padding: '8px 12px',
                      border: 'none',
                      borderRadius: '4px',
                      cursor: 'pointer',
                      fontSize: '12px',
                    }}
                  >
                    Tornar Admin
                  </button>
                ) : (
                  <span style={{ color: '#10b981', fontWeight: 'bold' }}>É Admin</span>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div style={{ marginTop: '30px', padding: '20px', backgroundColor: '#eff6ff', borderRadius: '8px', borderLeft: '4px solid #2563eb' }}>
        <h3>Informações</h3>
        <p>
          Total de usuários: <strong>{usuarios.length}</strong>
          <br />
          Administradores: <strong>{usuarios.filter(u => u.isAdmin).length}</strong>
          <br />
          Usuários comuns: <strong>{usuarios.filter(u => !u.isAdmin).length}</strong>
        </p>
      </div>
    </div>
  );
}

export default AdminPanel;
