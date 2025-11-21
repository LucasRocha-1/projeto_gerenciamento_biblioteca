import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Route, Routes, Link, useNavigate } from "react-router-dom";
import CadastroLivro from "./components/CadastroLivro";
import CadastroUsuario from "./components/CadastroUsuario";
import LivrosList from "./components/LivrosList";
import UsuariosList from "./components/UsuariosList";
import Home from "./components/Home";
import CatalogoLivros from "./components/CatalogoLivros";
// CadastroAutor removido: autor agora criado junto com o livro
import HistoricoEmprestimos from "./components/HistoricoEmprestimos";
import Dashboard from "./components/Dashboard";
import AdminPanel from "./components/AdminPanel";
import Login from "./components/Login";

function AppContent() {
  const [usuarioLogado, setUsuarioLogado] = useState<any>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const usuarioSalvo = localStorage.getItem("usuario");
    if (usuarioSalvo) {
      setUsuarioLogado(JSON.parse(usuarioSalvo));
    }
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("usuario");
    setUsuarioLogado(null);
    navigate("/login");
  };

  if (!usuarioLogado) {
    return <Login onLogin={setUsuarioLogado} />;
  }

  return (
    <>
      <nav style={{ padding: "10px", backgroundColor: "#2563eb", marginBottom: "20px", display: "flex", justifyContent: "space-between", alignItems: "center", color: "white" }}>
        <div style={{ display: "flex", gap: "20px" }}>
          <Link to="/" style={{ color: "white", textDecoration: "none", fontWeight: "bold" }}>
            Home
          </Link>
          <Link to="/catalogo" style={{ color: "white", textDecoration: "none", fontWeight: "bold" }}>
            Catálogo
          </Link>
          <Link to="/dashboard" style={{ color: "white", textDecoration: "none", fontWeight: "bold" }}>
            Dashboard
          </Link>
          <Link to="/historico-emprestimos" style={{ color: "white", textDecoration: "none", fontWeight: "bold" }}>
            Histórico
          </Link>
          {usuarioLogado?.isAdmin && (
            <Link to="/admin/painel" style={{ color: "white", textDecoration: "none", fontWeight: "bold", backgroundColor: "#dc2626", padding: "5px 10px", borderRadius: "4px" }}>
              Admin
            </Link>
          )}
          <Link to="/admin/livros" style={{ color: "white", textDecoration: "none", fontWeight: "bold" }}>
            Gerenciar
          </Link>
        </div>
        <div style={{ display: "flex", gap: "10px", alignItems: "center" }}>
          <span>Olá, {usuarioLogado.nome}</span>
          {usuarioLogado?.isAdmin && <span style={{ backgroundColor: "#059669", padding: "4px 8px", borderRadius: "4px", fontSize: "12px" }}>Admin</span>}
          <button onClick={handleLogout} style={{ padding: "6px 12px", backgroundColor: "#dc2626", color: "white", border: "none", borderRadius: "4px", cursor: "pointer" }}>
            Sair
          </button>
        </div>
      </nav>

      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/catalogo" element={<CatalogoLivros usuarioId={usuarioLogado.id} usuarioNome={usuarioLogado.nome} />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/historico-emprestimos" element={<HistoricoEmprestimos />} />
        <Route path="/admin/painel" element={<AdminPanel />} />
        <Route path="/admin/livros" element={<LivrosList />} />
        <Route path="/admin/usuarios" element={<UsuariosList />} />
          <Route path="/cadastro-livro" element={<CadastroLivro />} />
          <Route path="/cadastro-usuario" element={<CadastroUsuario />} />
      </Routes>
    </>
  );
}

function App() {
  return (
    <Router>
      <AppContent />
    </Router>
  );
}

export default App;