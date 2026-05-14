const API_BASE = (() => {
  const port = window.location.port;
  if (port === "5000" || port === "") return "";
  return "http://localhost:5000";
})();

async function request(path, options = {}) {
  const config = { method: options.method || "GET", headers: { ...(options.headers || {}) } };

  if (options.body !== undefined) {
    config.headers["Content-Type"] = "application/json";
    config.body = JSON.stringify(options.body);
  }

  const res = await fetch(API_BASE + path, config);

  if (res.status === 204) return null;

  let data = null;
  const text = await res.text();
  if (text) {
    try { data = JSON.parse(text); } catch { data = text; }
  }

  if (!res.ok) {
    const msg = (data && (data.mensagem || data.title)) || `Erro ${res.status}`;
    throw new Error(msg);
  }

  return data;
}

const api = {
  pacientes: {
    listar: () => request("/pacientes"),
    obter: (id) => request(`/pacientes/${id}`),
    criar: (body) => request("/pacientes", { method: "POST", body }),
    atualizar: (id, body) => request(`/pacientes/${id}`, { method: "PUT", body }),
    remover: (id) => request(`/pacientes/${id}`, { method: "DELETE" })
  },
  medicos: {
    listar: () => request("/medicos"),
    obter: (id) => request(`/medicos/${id}`),
    criar: (body) => request("/medicos", { method: "POST", body }),
    atualizar: (id, body) => request(`/medicos/${id}`, { method: "PUT", body }),
    remover: (id) => request(`/medicos/${id}`, { method: "DELETE" })
  },
  consultas: {
    listar: () => request("/consultas"),
    obter: (id) => request(`/consultas/${id}`),
    criar: (body) => request("/consultas", { method: "POST", body }),
    atualizar: (id, body) => request(`/consultas/${id}`, { method: "PUT", body }),
    remover: (id) => request(`/consultas/${id}`, { method: "DELETE" })
  }
};
