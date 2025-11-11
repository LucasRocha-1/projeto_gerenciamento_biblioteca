import React, { useEffect, useState } from "react";
import axios from "axios";

interface Autor {
  nome: string;
}

interface Livro {
  id: number;
  titulo: string;
  autor?: Autor;
}

const LivrosList: React.FC = () => {
  const [livros, setLivros] = useState<Livro[]>([]);

  useEffect(() => {
    carregarLivros();
  }, []);

  const carregarLivros = async () => {
    try {
      const response = await axios.get<Livro[]>("http://localhost:5093/api/livros");
      setLivros(response.data);
    } catch (error) {
      console.error(error);
    }
  };

  const deletarLivro = async (id: number) => {
    if (!window.confirm("Deseja realmente deletar este livro?")) return;

    try {
      await axios.delete(`http://localhost:5093/api/livros/${id}`);
      setLivros(livros.filter(livro => livro.id !== id));
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div>
      <h2>Lista de Livros</h2>
      <ul>
        {livros.map(livro => (
          <li key={livro.id}>
            {livro.titulo} - {livro.autor?.nome || "Autor não definido"}{" "}
            <button onClick={() => deletarLivro(livro.id)}>Deletar</button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default LivrosList;
