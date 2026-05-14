const medicosView = {

  async lista() {
    const app = document.getElementById("app");
    app.innerHTML = `<p class="loading">Carregando médicos...</p>`;

    try {
      const medicos = await api.medicos.listar();

      let html = `
        <div class="view-header">
          <h2>Médicos</h2>
          <a href="#/medicos/novo" class="btn btn-primary">+ Novo médico</a>
        </div>
      `;

      if (medicos.length === 0) {
        html += `<div class="empty">Nenhum médico cadastrado ainda.</div>`;
      } else {
        html += `
          <table>
            <thead>
              <tr>
                <th>Nome</th>
                <th>CRM</th>
                <th>Especialidade</th>
                <th>Telefone</th>
                <th>Email</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              ${medicos.map(m => `
                <tr>
                  <td>${escapeHtml(m.nome)}</td>
                  <td>${escapeHtml(m.crm)}</td>
                  <td>${escapeHtml(m.especialidade)}</td>
                  <td>${escapeHtml(m.telefone)}</td>
                  <td>${escapeHtml(m.email)}</td>
                  <td class="actions">
                    <a href="#/medicos/${m.id}/editar" class="btn btn-secondary btn-small">Editar</a>
                    <button class="btn btn-danger btn-small" data-remove="${m.id}" data-nome="${escapeAttr(m.nome)}">Excluir</button>
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
          if (!confirm(`Remover o médico "${nome}"?`)) return;
          try {
            await api.medicos.remover(id);
            toast("Médico removido.", "success");
            medicosView.lista();
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
    let medico = null;

    if (editando) {
      app.innerHTML = `<p class="loading">Carregando...</p>`;
      try {
        medico = await api.medicos.obter(id);
      } catch (e) {
        app.innerHTML = `<div class="empty">Erro: ${escapeHtml(e.message)}</div>`;
        return;
      }
    }

    app.innerHTML = `
      <div class="view-header">
        <h2>${editando ? "Editar médico" : "Novo médico"}</h2>
      </div>

      <form id="form-medico">
        <div class="form-row">
          <label for="nome">Nome</label>
          <input id="nome" name="nome" required minlength="2" maxlength="120" value="${escapeAttr(medico?.nome ?? "")}" />
        </div>

        <div class="form-row">
          <label for="crm">CRM</label>
          <input id="crm" name="crm" required minlength="4" maxlength="20" value="${escapeAttr(medico?.crm ?? "")}" ${editando ? "readonly" : ""} placeholder="MG-123456" />
        </div>

        <div class="form-row">
          <label for="especialidade">Especialidade</label>
          <input id="especialidade" name="especialidade" required minlength="3" maxlength="80" value="${escapeAttr(medico?.especialidade ?? "")}" placeholder="Cardiologia" />
        </div>

        <div class="form-row">
          <label for="telefone">Telefone</label>
          <input id="telefone" name="telefone" required value="${escapeAttr(medico?.telefone ?? "")}" placeholder="+5531988887777" />
        </div>

        <div class="form-row">
          <label for="email">Email</label>
          <input id="email" name="email" type="email" required value="${escapeAttr(medico?.email ?? "")}" />
        </div>

        <div class="form-actions">
          <button type="submit" class="btn btn-primary">${editando ? "Salvar" : "Cadastrar"}</button>
          <a href="#/medicos" class="btn btn-secondary">Cancelar</a>
        </div>
      </form>
    `;

    document.getElementById("form-medico").addEventListener("submit", async (ev) => {
      ev.preventDefault();
      const fd = new FormData(ev.target);
      const dados = {
        nome: fd.get("nome").trim(),
        especialidade: fd.get("especialidade").trim(),
        telefone: fd.get("telefone").trim(),
        email: fd.get("email").trim()
      };
      if (!editando) dados.crm = fd.get("crm").trim();

      try {
        if (editando) {
          await api.medicos.atualizar(id, dados);
          toast("Médico atualizado.", "success");
        } else {
          await api.medicos.criar(dados);
          toast("Médico criado.", "success");
        }
        window.location.hash = "#/medicos";
      } catch (e) {
        toast(e.message, "error");
      }
    });
  }
};
