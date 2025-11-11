import React, { useEffect, useState } from "react";
import axios from "axios";

interface Usuario {
  id: number;
  nome: string;
  email: string;
}

const UsuariosList: React.FC = () => {
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);

  useEffect(() => {
    carregarUsuarios();
  }, []);

  const carregarUsuarios = async () => {
    try {
      const response = await axios.get<Usuario[]>("http://localhost:5093/api/usuarios");
      setUsuarios(response.data);
    } catch (error) {
      console.error(error);
    }
  };

  const deletarUsuario = async (id: number) => {
    if (!window.confirm("Deseja realmente deletar este usuário?")) return;

    try {
      await axios.delete(`http://localhost:5093/api/usuarios/${id}`);
      setUsuarios(usuarios.filter(u => u.id !== id));
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div>
      <h2>Lista de Usuários</h2>
      <ul>
        {usuarios.map(u => (
          <li key={u.id}>
            {u.nome} - {u.email}{" "}
            <button onClick={() => deletarUsuario(u.id)}>Deletar</button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default UsuariosList;
