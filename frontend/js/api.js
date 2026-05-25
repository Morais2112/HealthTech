const API_BASE = (() => {
  const port = window.location.port;
  if (port === "5000" || port === "") return "";
  return "http://localhost:5000";
})();

const TOKEN_KEY = "healthtech_token";
const USUARIO_KEY = "healthtech_usuario";

const auth = {
  salvar(token, usuario) {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USUARIO_KEY, JSON.stringify(usuario));
  },
  limpar() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USUARIO_KEY);
  },
  token() {
    return localStorage.getItem(TOKEN_KEY);
  },
  usuario() {
    const raw = localStorage.getItem(USUARIO_KEY);
    return raw ? JSON.parse(raw) : null;
  },
  logado() {
    return !!auth.token();
  },
  ehAdmin() {
    const u = auth.usuario();
    return !!u && u.perfil === "Admin";
  }
};

async function request(path, options = {}) {
  const config = { method: options.method || "GET", headers: { ...(options.headers || {}) } };

  const token = auth.token();
  if (token) {
    config.headers["Authorization"] = `Bearer ${token}`;
  }

  if (options.body !== undefined) {
    config.headers["Content-Type"] = "application/json";
    config.body = JSON.stringify(options.body);
  }

  const res = await fetch(API_BASE + path, config);

  if (res.status === 401) {
    auth.limpar();
    window.location.hash = "#/login";
    throw new Error("Sessao expirada. Faca login novamente.");
  }

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
  auth: {
    registrar: (body) => request("/auth/registrar", { method: "POST", body }),
    login: (body) => request("/auth/login", { method: "POST", body }),
    listarUsuarios: () => request("/auth/usuarios"),
    promover: (id) => request(`/auth/usuarios/${id}/promover`, { method: "POST" }),
    rebaixar: (id) => request(`/auth/usuarios/${id}/rebaixar`, { method: "POST" })
  },
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
