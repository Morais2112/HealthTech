const usuariosView = {

  async lista() {
    const app = document.getElementById("app");
    app.innerHTML = `<p class="loading">Carregando usuários...</p>`;

    try {
      const usuarios = await api.auth.listarUsuarios();
      const meuId = auth.usuario()?.id;

      let html = `
        <div class="view-header">
          <h2>Usuários</h2>
        </div>
      `;

      if (usuarios.length === 0) {
        html += `<div class="empty">Nenhum usuário cadastrado.</div>`;
      } else {
        html += `
          <table>
            <thead>
              <tr>
                <th>Nome</th>
                <th>Email</th>
                <th>Perfil</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              ${usuarios.map(u => {
                const badgeClasse = u.perfil === "Admin" ? "badge-concluida" : "badge-agendada";
                const ehEu = u.id === meuId;
                let acoes = "";
                if (ehEu) {
                  acoes = `<span style="color:#888; font-size:0.85rem;">(você)</span>`;
                } else if (u.perfil === "Admin") {
                  acoes = `<button class="btn btn-secondary btn-small" data-rebaixar="${u.id}" data-nome="${escapeAttr(u.nome)}">Rebaixar</button>`;
                } else {
                  acoes = `<button class="btn btn-primary btn-small" data-promover="${u.id}" data-nome="${escapeAttr(u.nome)}">Promover a Admin</button>`;
                }
                return `
                  <tr>
                    <td>${escapeHtml(u.nome)}</td>
                    <td>${escapeHtml(u.email)}</td>
                    <td><span class="badge ${badgeClasse}">${escapeHtml(u.perfil)}</span></td>
                    <td class="actions">${acoes}</td>
                  </tr>
                `;
              }).join("")}
            </tbody>
          </table>
        `;
      }

      app.innerHTML = html;

      app.querySelectorAll("[data-promover]").forEach(btn => {
        btn.addEventListener("click", async () => {
          const id = btn.dataset.promover;
          const nome = btn.dataset.nome;
          if (!confirm(`Promover "${nome}" a Admin?`)) return;
          try {
            await api.auth.promover(id);
            toast("Usuário promovido.", "success");
            usuariosView.lista();
          } catch (e) {
            toast(e.message, "error");
          }
        });
      });

      app.querySelectorAll("[data-rebaixar]").forEach(btn => {
        btn.addEventListener("click", async () => {
          const id = btn.dataset.rebaixar;
          const nome = btn.dataset.nome;
          if (!confirm(`Rebaixar "${nome}" para usuário comum?`)) return;
          try {
            await api.auth.rebaixar(id);
            toast("Usuário rebaixado.", "success");
            usuariosView.lista();
          } catch (e) {
            toast(e.message, "error");
          }
        });
      });
    } catch (e) {
      app.innerHTML = `<div class="empty">Erro ao carregar: ${escapeHtml(e.message)}</div>`;
    }
  }
};
