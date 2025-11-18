import React from 'react';
import { Link } from 'react-router-dom';



function Home() {
  const containerStyle: React.CSSProperties = {
    display: 'flex', flexDirection: 'column', alignItems: 'center', marginTop: '50px'
  };
  
  const buttonContainer: React.CSSProperties = {
    display: 'flex', gap: '20px', marginTop: '30px'
  };

  const cardStyle: React.CSSProperties = {
    padding: '30px', width: '200px', textAlign: 'center', borderRadius: '8px',
    textDecoration: 'none', color: 'white', fontWeight: 'bold', fontSize: '18px'
  };

  return (
    <div style={containerStyle}>
      <h1>Bem-vindo à Biblioteca</h1>
      <p>Selecione seu perfil de acesso:</p>

      <div style={buttonContainer}>
        {/* Botão Admin */}
        <Link to="/admin/livros" style={{...cardStyle, backgroundColor: '#2c3e50'}}>
          Área do Admin
          <br/><span style={{fontSize:'12px', fontWeight:'normal'}}>Gerenciar Acervo</span>
        </Link>

        {/* Botão Catálogo (Usuário) */}
        <Link to="/catalogo" style={{...cardStyle, backgroundColor: '#27ae60'}}>
          Catálogo
          <br/><span style={{fontSize:'12px', fontWeight:'normal'}}>Reservar Livros</span>
        </Link>

        {/* Botão Dashboard */}
        <Link to="/dashboard" style={{...cardStyle, backgroundColor: '#e67e22'}}>
          Dashboard
          <br/><span style={{fontSize:'12px', fontWeight:'normal'}}>Ver Estatísticas</span>
        </Link>
      </div>
    </div>
  );
}

export default Home;