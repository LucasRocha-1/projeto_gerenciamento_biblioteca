import React, { useEffect, useState } from "react";
import axios from "axios";

interface Emprestimo {
  id: number;
  livroTitulo: string;
  usuarioNome: string;
  dataEmprestimo: string;
  dataDevolucao: string | null;
  status: string;
}

function HistoricoEmprestimos() {
  const [emprestimos, setEmprestimos] = useState<Emprestimo[]>([]);
  const [filtro, setFiltro] = useState("todos");

  useEffect(() => {
    carregarEmprestimos();
  }, []);

  const carregarEmprestimos = async () => {
    try {
      const response = await axios.get("http://localhost:5093/api/emprestimos");
      setEmprestimos(response.data || []);
    } catch (error) {
      console.error("Erro ao carregar empréstimos:", error);
    }
  };

  const formatarData = (data: string) => {
    return new Date(data).toLocaleDateString("pt-BR");
  };

  const registrarDevolucao = async (id: number) => {
    if (!window.confirm("Deseja registrar a devolução deste livro?")) return;
    try {
      await axios.put(`http://localhost:5093/api/emprestimos/${id}/devolver`);
      carregarEmprestimos();
      alert("Devolução registrada com sucesso!");
    } catch (error) {
      console.error("Erro ao devolver:", error);
      alert("Erro ao registrar devolução");
    }
  };

  const emprestimosFiltrados = emprestimos.filter((e) => {
    if (filtro === "ativos") return e.status === "ativo";
    if (filtro === "devolvidos") return e.status === "devolvido";
    return true;
  });

  return (
    <div style={{ padding: "20px" }}>
      <h2>Histórico de Empréstimos</h2>

      <div style={{ marginBottom: "20px", display: "flex", gap: "10px" }}>
        <button onClick={() => setFiltro("todos")} style={{ padding: "8px 16px", backgroundColor: filtro === "todos" ? "#2563eb" : "#e5e7eb", color: filtro === "todos" ? "white" : "black", border: "none", borderRadius: "5px", cursor: "pointer" }}>
          Todos ({emprestimos.length})
        </button>
        <button onClick={() => setFiltro("ativos")} style={{ padding: "8px 16px", backgroundColor: filtro === "ativos" ? "#2563eb" : "#e5e7eb", color: filtro === "ativos" ? "white" : "black", border: "none", borderRadius: "5px", cursor: "pointer" }}>
          Ativos ({emprestimos.filter((e) => e.status === "ativo").length})
        </button>
        <button onClick={() => setFiltro("devolvidos")} style={{ padding: "8px 16px", backgroundColor: filtro === "devolvidos" ? "#2563eb" : "#e5e7eb", color: filtro === "devolvidos" ? "white" : "black", border: "none", borderRadius: "5px", cursor: "pointer" }}>
          Devolvidos ({emprestimos.filter((e) => e.status === "devolvido").length})
        </button>
      </div>

      {emprestimosFiltrados.length === 0 ? (
        <p style={{ color: "#6b7280" }}>Nenhum empréstimo encontrado.</p>
      ) : (
        <table style={{ width: "100%", borderCollapse: "collapse", backgroundColor: "white" }}>
          <thead>
            <tr style={{ backgroundColor: "#f3f4f6", borderBottom: "2px solid #e5e7eb" }}>
              <th style={{ padding: "12px", textAlign: "left" }}>Livro</th>
              <th style={{ padding: "12px", textAlign: "left" }}>Usuário</th>
              <th style={{ padding: "12px", textAlign: "left" }}>Data Empréstimo</th>
              <th style={{ padding: "12px", textAlign: "left" }}>Data Devolução</th>
              <th style={{ padding: "12px", textAlign: "left" }}>Status</th>
              <th style={{ padding: "12px", textAlign: "left" }}>Ação</th>
            </tr>
          </thead>
          <tbody>
            {emprestimosFiltrados.map((emp) => (
              <tr key={emp.id} style={{ borderBottom: "1px solid #e5e7eb" }}>
                <td style={{ padding: "12px" }}>{emp.livroTitulo}</td>
                <td style={{ padding: "12px" }}>{emp.usuarioNome}</td>
                <td style={{ padding: "12px", fontSize: "0.9rem" }}>{formatarData(emp.dataEmprestimo)}</td>
                <td style={{ padding: "12px", fontSize: "0.9rem" }}>{emp.dataDevolucao ? formatarData(emp.dataDevolucao) : "—"}</td>
                <td style={{ padding: "12px" }}>
                  <span style={{ padding: "4px 12px", borderRadius: "16px", fontSize: "0.85rem", backgroundColor: emp.status === "ativo" ? "#fef08a" : "#dbeafe", color: emp.status === "ativo" ? "#92400e" : "#0c4a6e" }}>
                    {emp.status === "ativo" ? "Ativo" : "Devolvido"}
                  </span>
                </td>
                <td style={{ padding: "12px" }}>
                  {emp.status === "ativo" && (
                    <button onClick={() => registrarDevolucao(emp.id)} style={{ padding: "6px 12px", backgroundColor: "#059669", color: "white", border: "none", borderRadius: "5px", cursor: "pointer" }}>
                      Devolver
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default HistoricoEmprestimos;
