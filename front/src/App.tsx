import React from "react";
import { BrowserRouter as Router, Route, Routes, Link } from "react-router-dom";
import CadastroLivro from "./components/CadastroLivro";
import CadastroUsuario from "./components/CadastroUsuario";
import LivrosList from "./components/LivrosList";
import UsuariosList from "./components/UsuariosList";

function App() {
  return (
    <Router>
      <div className="App">
        <h1>Biblioteca - Gerenciamento de Livros</h1>
        
        {/* Links de navegação */}
        <nav>
          <ul>
            <li>
              <Link to="/">Lista de Livros</Link>
            </li>
            <li>
              <Link to="/cadastro-livro">Cadastrar Livro</Link>
            </li>
            <li>
              <Link to="/usuarios">Lista de Usuários</Link>
            </li>
            <li>
              <Link to="/cadastro-usuario">Cadastrar Usuário</Link>
            </li>
          </ul>
        </nav>

        {/* Definindo as rotas */}
        <Routes>
          <Route path="/" element={<LivrosList />} />
          <Route path="/usuarios" element={<UsuariosList />} />
          <Route path="/cadastro-livro" element={<CadastroLivro />} />
          <Route path="/cadastro-usuario" element={<CadastroUsuario />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
