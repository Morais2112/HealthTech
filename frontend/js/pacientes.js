const pacientesView = {

  async lista() {
    const app = document.getElementById("app");
    app.innerHTML = `<p class="loading">Carregando pacientes...</p>`;

    try {
      const pacientes = await api.pacientes.listar();

      let html = `
        <div class="view-header">
          <h2>Pacientes</h2>
          <a href="#/pacientes/novo" class="btn btn-primary">+ Novo paciente</a>
        </div>
      `;

      if (pacientes.length === 0) {
        html += `<div class="empty">Nenhum paciente cadastrado ainda.</div>`;
      } else {
        html += `
          <table>
            <thead>
              <tr>
                <th>Nome</th>
                <th>CPF</th>
                <th>Telefone</th>
                <th>Email</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              ${pacientes.map(p => `
                <tr>
                  <td>${escapeHtml(p.nome)}</td>
                  <td>${formatarCpf(p.cpf)}</td>
                  <td>${escapeHtml(p.telefone)}</td>
                  <td>${escapeHtml(p.email)}</td>
                  <td class="actions">
                    <a href="#/pacientes/${p.id}/editar" class="btn btn-secondary btn-small">Editar</a>
                    <button class="btn btn-danger btn-small" data-remove="${p.id}" data-nome="${escapeAttr(p.nome)}">Excluir</button>
                  </td>
                </tr>
              `).join("")}
            </tbody>
          </table>
        `;
      }

      app.innerHTML = html;

      app.querySelectorAll("[data-remove]").forEach(btn => {
        btn.addEventListener("click", async () => {
          const id = btn.dataset.remove;
          const nome = btn.dataset.nome;
          if (!confirm(`Remover o paciente "${nome}"?`)) return;
          try {
            await api.pacientes.remover(id);
            toast("Paciente removido.", "success");
            pacientesView.lista();
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
    let paciente = null;

    if (editando) {
      app.innerHTML = `<p class="loading">Carregando...</p>`;
      try {
        paciente = await api.pacientes.obter(id);
      } catch (e) {
        app.innerHTML = `<div class="empty">Erro: ${escapeHtml(e.message)}</div>`;
        return;
      }
    }

    const dataNascimento = paciente
      ? new Date(paciente.dataNascimento).toISOString().substring(0, 10)
      : "";

    app.innerHTML = `
      <div class="view-header">
        <h2>${editando ? "Editar paciente" : "Novo paciente"}</h2>
      </div>

      <form id="form-paciente">
        <div class="form-row">
          <label for="nome">Nome</label>
          <input id="nome" name="nome" required minlength="2" maxlength="120" value="${escapeAttr(paciente?.nome ?? "")}" />
        </div>

        <div class="form-row">
          <label for="cpf">CPF</label>
          <input id="cpf" name="cpf" required value="${escapeAttr(paciente?.cpf ?? "")}" placeholder="000.000.000-00" ${editando ? "readonly" : ""} />
        </div>

        <div class="form-row">
          <label for="dataNascimento">Data de nascimento</label>
          <input id="dataNascimento" name="dataNascimento" type="date" required value="${dataNascimento}" />
        </div>

        <div class="form-row">
          <label for="telefone">Telefone</label>
          <input id="telefone" name="telefone" required value="${escapeAttr(paciente?.telefone ?? "")}" placeholder="+5531999998888" />
        </div>

        <div class="form-row">
          <label for="email">Email</label>
          <input id="email" name="email" type="email" required value="${escapeAttr(paciente?.email ?? "")}" />
        </div>

        <div class="form-actions">
          <button type="submit" class="btn btn-primary">${editando ? "Salvar" : "Cadastrar"}</button>
          <a href="#/pacientes" class="btn btn-secondary">Cancelar</a>
        </div>
      </form>
    `;

    document.getElementById("form-paciente").addEventListener("submit", async (ev) => {
      ev.preventDefault();
      const fd = new FormData(ev.target);
      const dados = {
        nome: fd.get("nome").trim(),
        dataNascimento: new Date(fd.get("dataNascimento")).toISOString(),
        telefone: fd.get("telefone").trim(),
        email: fd.get("email").trim()
      };
      if (!editando) dados.cpf = fd.get("cpf").trim();

      try {
        if (editando) {
          await api.pacientes.atualizar(id, dados);
          toast("Paciente atualizado.", "success");
        } else {
          await api.pacientes.criar(dados);
          toast("Paciente criado.", "success");
        }
        window.location.hash = "#/pacientes";
      } catch (e) {
        toast(e.message, "error");
      }
    });
  }
};

function formatarCpf(cpf) {
  const d = (cpf || "").replace(/\D/g, "");
  if (d.length !== 11) return cpf;
  return `${d.substring(0,3)}.${d.substring(3,6)}.${d.substring(6,9)}-${d.substring(9)}`;
}
