function escapeHtml(str) {
  if (str === null || str === undefined) return "";
  return String(str)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#39;");
}

function escapeAttr(str) {
  return escapeHtml(str).replace(/`/g, "&#96;");
}

let toastTimer = null;
function toast(msg, tipo = "") {
  const el = document.getElementById("toast");
  el.className = "toast";
  if (tipo) el.classList.add(tipo);
  el.textContent = msg;
  if (toastTimer) clearTimeout(toastTimer);
  toastTimer = setTimeout(() => el.classList.add("hidden"), 3500);
}

function marcarLinkAtivo(rota) {
  document.querySelectorAll(".nav-link").forEach(link => {
    link.classList.toggle("active", link.dataset.route === rota);
  });
}

function atualizarNav() {
  const navArea = document.getElementById("nav-area");
  const userArea = document.getElementById("user-area");
  const logado = auth.logado();
  const usuario = auth.usuario();

  if (logado) {
    navArea.style.display = "flex";
    userArea.innerHTML = `
      <span class="user-info">${escapeHtml(usuario?.nome ?? "")}</span>
      <button id="btn-logout" class="btn btn-secondary btn-small">Sair</button>
    `;
    document.getElementById("btn-logout").addEventListener("click", () => {
      auth.limpar();
      toast("Voce saiu da conta.", "success");
      window.location.hash = "#/login";
    });
  } else {
    navArea.style.display = "none";
    userArea.innerHTML = "";
  }
}

function rotear() {
  const hash = window.location.hash.replace(/^#\/?/, "") || "pacientes";
  const partes = hash.split("/");
  const entidade = partes[0];
  const acao = partes[1];
  const editar = partes[1] && (partes[2] === "editar");

  atualizarNav();

  if (entidade === "login") return authView.login();
  if (entidade === "registrar") return authView.registrar();

  if (!auth.logado()) {
    window.location.hash = "#/login";
    return;
  }

  marcarLinkAtivo(entidade);

  if (entidade === "pacientes") {
    if (!acao) return pacientesView.lista();
    if (acao === "novo") return pacientesView.form(null);
    if (editar) return pacientesView.form(partes[1]);
  }

  if (entidade === "medicos") {
    if (!acao) return medicosView.lista();
    if (acao === "novo") return medicosView.form(null);
    if (editar) return medicosView.form(partes[1]);
  }

  if (entidade === "consultas") {
    if (!acao) return consultasView.lista();
    if (acao === "nova") return consultasView.form(null);
    if (editar) return consultasView.form(partes[1]);
  }

  document.getElementById("app").innerHTML = `<div class="empty">Pagina nao encontrada.</div>`;
}

window.addEventListener("hashchange", rotear);
window.addEventListener("DOMContentLoaded", () => {
  if (!window.location.hash) {
    window.location.hash = auth.logado() ? "#/pacientes" : "#/login";
  } else {
    rotear();
  }
});
