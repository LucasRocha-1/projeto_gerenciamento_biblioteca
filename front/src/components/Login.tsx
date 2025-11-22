import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

function Login({ onLogin }: { onLogin: (usuario: any) => void }) {
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("admin@biblioteca.com"); // Iniciar preenchidos
  const [senha, setSenha] = useState("senhateste@123"); // Iniciar preenchidos
  const [mensagem, setMensagem] = useState("");
  const [mostrarCadastro, setMostrarCadastro] = useState(false);
  const [erros, setErros] = useState<{ nome?: string; email?: string; senha?: string }>({});
  const [loadingLogin, setLoadingLogin] = useState(false);
  const [loadingCadastro, setLoadingCadastro] = useState(false);
  const [mostrarSenha, setMostrarSenha] = useState(false);
  const navigate = useNavigate();

  function validarCamposLogin() {
    const novoErros: { email?: string; senha?: string } = {};
    if (!email) novoErros.email = "Informe o email";
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) novoErros.email = "Email inválido";
    if (!senha) novoErros.senha = "Informe a senha";
    else if (senha.length < 5) novoErros.senha = "Mínimo 5 caracteres";
    setErros(novoErros);
    return Object.keys(novoErros).length === 0;
  }

  function validarCamposCadastro() {
    const novoErros: { nome?: string; email?: string; senha?: string } = {};
    if (!nome) novoErros.nome = "Informe o nome";
    if (!email) novoErros.email = "Informe o email";
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) novoErros.email = "Email inválido";
    if (!senha) novoErros.senha = "Informe a senha";
    else if (senha.length < 5) novoErros.senha = "Mínimo 5 caracteres";
    setErros(novoErros);
    return Object.keys(novoErros).length === 0;
  }

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validarCamposLogin()) return;
    try {
      setLoadingLogin(true);
      const payload = { email, senha };
      console.log("[LOGIN] Enviando dados:", payload);
      console.log("[LOGIN] URL: http://localhost:5093/api/auth/login");

      const response = await axios.post("http://localhost:5093/api/auth/login", payload);
      
      console.log("[LOGIN] Resposta recebida:", response.data);
      setMensagem(response.data.message);
      onLogin(response.data);
      localStorage.setItem("usuario", JSON.stringify(response.data));
      setTimeout(() => navigate("/"), 1500);
    } catch (error: any) {
      console.error("[LOGIN] Erro:", {
        status: error.response?.status,
        statusText: error.response?.statusText,
        data: error.response?.data,
        message: error.message,
        url: error.config?.url,
        method: error.config?.method,
      });
      setMensagem(
        error.response?.data || 
        `Erro ao fazer login: ${error.message} (Status: ${error.response?.status})`
      );
    } finally {
      setLoadingLogin(false);
    }
  };

  const handleCadastro = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validarCamposCadastro()) return;
    try {
      setLoadingCadastro(true);
      const payload = { nome, email, senha };
      console.log("[CADASTRO] Enviando dados:", payload);
      console.log("[CADASTRO] URL: http://localhost:5093/api/auth/cadastro");

      const response = await axios.post("http://localhost:5093/api/auth/cadastro", payload);
      
      console.log("[CADASTRO] Resposta recebida:", response.data);
      setMensagem(response.data.message);
      setTimeout(() => setMostrarCadastro(false), 1500);
      setNome("");
      setEmail("");
      setSenha("");
    } catch (error: any) {
      console.error("[CADASTRO] Erro:", {
        status: error.response?.status,
        statusText: error.response?.statusText,
        data: error.response?.data,
        message: error.message,
        url: error.config?.url,
        method: error.config?.method,
      });
      setMensagem(
        error.response?.data || 
        `Erro ao cadastrar: ${error.message} (Status: ${error.response?.status})`
      );
    } finally {
      setLoadingCadastro(false);
    }
  };

  return (
    <div className="login-container">
      <div className={`login-card ${mostrarCadastro ? 'small' : ''}`}>
        <h1 className="login-title">Biblioteca</h1>
        <p className="login-subtitle">{mostrarCadastro ? "Crie sua conta" : "Acesse sua conta"}</p>

        {!mostrarCadastro ? (
          <>
            <h2 className="login-section-title compact">Login</h2>
            <form onSubmit={handleLogin} noValidate className="login-form">
              <div className="login-field">
                <label htmlFor="emailLogin" className="login-label">Email</label>
                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="seu@email.com"
                  required
                  id="emailLogin"
                  className={`login-input ${erros.email ? 'error' : ''}`}
                />
                {erros.email && <small className="login-error-text">{erros.email}</small>}
              </div>
              <div className="login-field">
                <label htmlFor="senhaLogin" className="login-label">Senha</label>
                <input
                  type={mostrarSenha ? "text" : "password"}
                  value={senha}
                  onChange={(e) => setSenha(e.target.value)}
                  placeholder="Sua senha"
                  required
                  id="senhaLogin"
                  className={`login-input ${erros.senha ? 'error' : ''}`}
                />
                <div className="login-inline-actions">
                  {erros.senha && <small className="login-error-text">{erros.senha}</small>}
                  <button type="button" onClick={() => setMostrarSenha(s => !s)} className="login-link-btn small">
                    {mostrarSenha ? 'Ocultar senha' : 'Mostrar senha'}
                  </button>
                </div>
              </div>
              <button
                type="submit"
                className="login-primary-btn"
                disabled={loadingLogin}
              >
                {loadingLogin ? 'Entrando...' : 'Entrar'}
              </button>
            </form>
            <hr className="login-divider" />
            <div className="login-switch">
              <button type="button" onClick={() => { setMostrarCadastro(true); setErros({}); setMensagem(''); }} className="login-link-btn">
                Não tem conta? Cadastre-se
              </button>
            </div>
          </>
        ) : (
          <>
            <h2 className="login-section-title compact">Cadastro</h2>
            <form onSubmit={handleCadastro} noValidate className="login-form">
              <div className="login-field">
                <label htmlFor="nomeCadastro" className="login-label">Nome</label>
                <input
                  type="text"
                  value={nome}
                  onChange={(e) => setNome(e.target.value)}
                  placeholder="Seu nome"
                  required
                  id="nomeCadastro"
                  className={`login-input ${erros.nome ? 'error' : ''}`}
                />
                {erros.nome && <small className="login-error-text">{erros.nome}</small>}
              </div>
              <div className="login-field">
                <label htmlFor="emailCadastro" className="login-label">Email</label>
                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="seu@email.com"
                  required
                  id="emailCadastro"
                  className={`login-input ${erros.email ? 'error' : ''}`}
                />
                {erros.email && <small className="login-error-text">{erros.email}</small>}
              </div>
              <div className="login-field">
                <label htmlFor="senhaCadastro" className="login-label">Senha</label>
                <input
                  type={mostrarSenha ? "text" : "password"}
                  value={senha}
                  onChange={(e) => setSenha(e.target.value)}
                  placeholder="Sua senha"
                  required
                  id="senhaCadastro"
                  className={`login-input ${erros.senha ? 'error' : ''}`}
                />
                <div className="login-inline-actions">
                  {erros.senha && <small className="login-error-text">{erros.senha}</small>}
                  <button type="button" onClick={() => setMostrarSenha(s => !s)} className="login-link-btn small">
                    {mostrarSenha ? 'Ocultar senha' : 'Mostrar senha'}
                  </button>
                </div>
              </div>
              <button
                type="submit"
                className="login-primary-btn green"
                disabled={loadingCadastro}
              >
                {loadingCadastro ? 'Cadastrando...' : 'Cadastrar'}
              </button>
            </form>
            <hr className="login-divider" />
            <div className="login-switch">
              <button type="button" onClick={() => { setMostrarCadastro(false); setErros({}); setMensagem(''); }} className="login-link-btn">
                Voltar ao Login
              </button>
            </div>
          </>
        )}

        {mensagem && (
          <div className={`login-message ${mensagem.toLowerCase().includes('sucesso') && !mensagem.toLowerCase().includes('erro') ? 'success' : 'error'}`}>{mensagem}</div>
        )}
      </div>
    </div>
  );
}

export default Login;
