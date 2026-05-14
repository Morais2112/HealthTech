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

function rotear() {
  const hash = window.location.hash.replace(/^#\/?/, "") || "pacientes";
  const partes = hash.split("/");

  const entidade = partes[0];
  const acao = partes[1];
  const id = partes[2] === "editar" ? null : partes[2];
  const editar = partes[1] && (partes[2] === "editar");

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

  document.getElementById("app").innerHTML = `<div class="empty">Página não encontrada.</div>`;
}

window.addEventListener("hashchange", rotear);
window.addEventListener("DOMContentLoaded", () => {
  if (!window.location.hash) window.location.hash = "#/pacientes";
  else rotear();
});
