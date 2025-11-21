import React from "react";

// Componente mantido apenas para compatibilidade: a criação de autores
// agora é feita automaticamente no fluxo de cadastro de livro.
const CadastroAutor: React.FC = () => {
  return (
    <div style={{ padding: 20 }}>
      <h2>Cadastro de Autor (removido)</h2>
      <p>Criação de autores agora ocorre ao cadastrar um livro.</p>
    </div>
  );
};

export default CadastroAutor;
