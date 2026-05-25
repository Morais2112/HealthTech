const STATUS_LABEL = { 0: "Agendada", 1: "Concluida", 2: "Cancelada" };
const STATUS_CLASS = { 0: "badge-agendada", 1: "badge-concluida", 2: "badge-cancelada" };

const consultasView = {

  async lista() {
    const app = document.getElementById("app");
    app.innerHTML = `<p class="loading">Carregando consultas...</p>`;

    try {
      const consultas = await api.consultas.listar();

      let html = `
        <div class="view-header">
          <h2>Consultas</h2>
          <a href="#/consultas/nova" class="btn btn-primary">+ Nova consulta</a>
        </div>
      `;

      if (consultas.length === 0) {
        html += `<div class="empty">Nenhuma consulta cadastrada ainda.</div>`;
      } else {
        html += `
          <table>
            <thead>
              <tr>
                <th>Data / Hora</th>
                <th>Paciente</th>
                <th>Médico</th>
                <th>Especialidade</th>
                <th>Status</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              ${consultas.map(c => {
                const statusKey = typeof c.status === "number" ? c.status : statusFromString(c.status);
                return `
                  <tr>
                    <td>${formatarDataHora(c.dataHora)}</td>
                    <td>${escapeHtml(c.pacienteNome)}</td>
                    <td>${escapeHtml(c.medicoNome)}</td>
                    <td>${escapeHtml(c.medicoEspecialidade)}</td>
                    <td><span class="badge ${STATUS_CLASS[statusKey]}">${STATUS_LABEL[statusKey]}</span></td>
                    <td class="actions">
                      <a href="#/consultas/${c.id}/editar" class="btn btn-secondary btn-small">Editar</a>
                      ${auth.ehAdmin() ? `<button class="btn btn-danger btn-small" data-remove="${c.id}">Excluir</button>` : ""}
                    </td>
                  </tr>
                `;
              }).join("")}
            </tbody>
          </table>
        `;
      }

      app.innerHTML = html;

      app.querySelectorAll("[data-remove]").forEach(btn => {
        btn.addEventListener("click", async () => {
          const id = btn.dataset.remove;
          if (!confirm("Remover essa consulta?")) return;
          try {
            await api.consultas.remover(id);
            toast("Consulta removida.", "success");
            consultasView.lista();
          } catch (e) {
            toast(e.message, "error");
          }
        });
      });
    } catch (e) {
      app.innerHTML = `<div class="empty">Erro ao carregar: ${escapeHtml(e.message)}</div>`;
    }
  },

  async form(id) {
    const app = document.getElementById("app");
    const editando = !!id;
    app.innerHTML = `<p class="loading">Carregando...</p>`;

    let consulta = null;
    let pacientes = [];
    let medicos = [];

    try {
      [pacientes, medicos] = await Promise.all([
        api.pacientes.listar(),
        api.medicos.listar()
      ]);
      if (editando) consulta = await api.consultas.obter(id);
    } catch (e) {
      app.innerHTML = `<div class="empty">Erro: ${escapeHtml(e.message)}</div>`;
      return;
    }

    if (!editando && (pacientes.length === 0 || medicos.length === 0)) {
      app.innerHTML = `
        <div class="empty">
          Para criar uma consulta, é preciso ter pelo menos um paciente e um médico cadastrados.
        </div>
        <div style="text-align:center; margin-top:10px;">
          <a href="#/pacientes" class="btn btn-secondary">Ir para Pacientes</a>
          <a href="#/medicos" class="btn btn-secondary">Ir para Médicos</a>
        </div>
      `;
      return;
    }

    const dataHoraValor = consulta
      ? new Date(consulta.dataHora).toISOString().substring(0, 16)
      : "";
    const statusAtual = consulta
      ? (typeof consulta.status === "number" ? consulta.status : statusFromString(consulta.status))
      : 0;

    app.innerHTML = `
      <div class="view-header">
        <h2>${editando ? "Editar consulta" : "Nova consulta"}</h2>
      </div>

      <form id="form-consulta">
        ${editando ? "" : `
          <div class="form-row">
            <label for="pacienteId">Paciente</label>
            <select id="pacienteId" name="pacienteId" required>
              <option value="">Selecione...</option>
              ${pacientes.map(p => `<option value="${p.id}">${escapeHtml(p.nome)}</option>`).join("")}
            </select>
          </div>

          <div class="form-row">
            <label for="medicoId">Médico</label>
            <select id="medicoId" name="medicoId" required>
              <option value="">Selecione...</option>
              ${medicos.map(m => `<option value="${m.id}">${escapeHtml(m.nome)} - ${escapeHtml(m.especialidade)}</option>`).join("")}
            </select>
          </div>
        `}

        <div class="form-row">
          <label for="dataHora">Data e hora</label>
          <input id="dataHora" name="dataHora" type="datetime-local" required value="${dataHoraValor}" />
        </div>

        ${editando ? `
          <div class="form-row">
            <label for="status">Status</label>
            <select id="status" name="status" required>
              <option value="0" ${statusAtual === 0 ? "selected" : ""}>Agendada</option>
              <option value="1" ${statusAtual === 1 ? "selected" : ""}>Concluida</option>
              <option value="2" ${statusAtual === 2 ? "selected" : ""}>Cancelada</option>
            </select>
          </div>
        ` : ""}

        <div class="form-row">
          <label for="observacoes">Observações</label>
          <textarea id="observacoes" name="observacoes" maxlength="500">${escapeHtml(consulta?.observacoes ?? "")}</textarea>
        </div>

        <div class="form-actions">
          <button type="submit" class="btn btn-primary">${editando ? "Salvar" : "Agendar"}</button>
          <a href="#/consultas" class="btn btn-secondary">Cancelar</a>
        </div>
      </form>
    `;

    document.getElementById("form-consulta").addEventListener("submit", async (ev) => {
      ev.preventDefault();
      const fd = new FormData(ev.target);
      const dataHoraIso = new Date(fd.get("dataHora")).toISOString();

      try {
        if (editando) {
          await api.consultas.atualizar(id, {
            dataHora: dataHoraIso,
            status: Number(fd.get("status")),
            observacoes: fd.get("observacoes") || ""
          });
          toast("Consulta atualizada.", "success");
        } else {
          await api.consultas.criar({
            pacienteId: fd.get("pacienteId"),
            medicoId: fd.get("medicoId"),
            dataHora: dataHoraIso,
            observacoes: fd.get("observacoes") || ""
          });
          toast("Consulta agendada.", "success");
        }
        window.location.hash = "#/consultas";
      } catch (e) {
        toast(e.message, "error");
      }
    });
  }
};

function formatarDataHora(iso) {
  const d = new Date(iso);
  return d.toLocaleString("pt-BR", {
    day: "2-digit", month: "2-digit", year: "numeric",
    hour: "2-digit", minute: "2-digit"
  });
}

function statusFromString(s) {
  if (s === "Agendada") return 0;
  if (s === "Concluida") return 1;
  if (s === "Cancelada") return 2;
  return 0;
}
