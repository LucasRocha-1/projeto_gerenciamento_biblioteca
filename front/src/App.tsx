import React from "react";
import { BrowserRouter as Router, Route, Routes, Link } from "react-router-dom";
import CadastroLivro from "./components/CadastroLivro";
import CadastroUsuario from "./components/CadastroUsuario";
import LivrosList from "./components/LivrosList";
import UsuariosList from "./components/UsuariosList";
import Home from "./components/Home";
import CatalogoLivros from "./components/CatalogoLivros"; 

function App() {
  return (
    <Router>
      <div className="App">
        <nav style={{ padding: '10px', backgroundColor: '#eee', marginBottom: '20px' }}>
          <Link to="/" style={{ marginRight: '10px' }}>Home</Link> | 
          <Link to="/admin/livros" style={{ margin: '0 10px' }}>Admin Livros</Link>
        </nav>

        <Routes>
          {/* Rota principal agora é a Home com os 3 botões */}
          <Route path="/" element={<Home />} />

          {/* Rotas do Usuário */}
          <Route path="/catalogo" element={<CatalogoLivros />} />
          <Route path="/dashboard" element={<h2>falta fazer</h2>} />

          {/* Rotas de Admin*/}
          <Route path="/admin/livros" element={<LivrosList />} />
          <Route path="/admin/usuarios" element={<UsuariosList />} />
          <Route path="/cadastro-livro" element={<CadastroLivro />} />
          <Route path="/cadastro-usuario" element={<CadastroUsuario />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;