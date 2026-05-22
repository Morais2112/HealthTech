const authView = {

  login() {
    const app = document.getElementById("app");
    app.innerHTML = `
      <div class="auth-card">
        <h2>Entrar</h2>
        <form id="form-login">
          <div class="form-row">
            <label for="email">Email</label>
            <input id="email" name="email" type="email" required />
          </div>
          <div class="form-row">
            <label for="senha">Senha</label>
            <input id="senha" name="senha" type="password" required />
          </div>
          <div class="form-actions">
            <button type="submit" class="btn btn-primary">Entrar</button>
            <a href="#/registrar" class="btn btn-secondary">Criar conta</a>
          </div>
        </form>
      </div>
    `;

    document.getElementById("form-login").addEventListener("submit", async (ev) => {
      ev.preventDefault();
      const fd = new FormData(ev.target);
      try {
        const res = await api.auth.login({
          email: fd.get("email").trim(),
          senha: fd.get("senha")
        });
        auth.salvar(res.token, res.usuario);
        toast(`Bem-vindo, ${res.usuario.nome}.`, "success");
        window.location.hash = "#/pacientes";
      } catch (e) {
        toast(e.message, "error");
      }
    });
  },

  registrar() {
    const app = document.getElementById("app");
    app.innerHTML = `
      <div class="auth-card">
        <h2>Criar conta</h2>
        <form id="form-registro">
          <div class="form-row">
            <label for="nome">Nome</label>
            <input id="nome" name="nome" required minlength="2" maxlength="80" />
          </div>
          <div class="form-row">
            <label for="email">Email</label>
            <input id="email" name="email" type="email" required />
          </div>
          <div class="form-row">
            <label for="senha">Senha</label>
            <input id="senha" name="senha" type="password" required minlength="6" />
          </div>
          <div class="form-actions">
            <button type="submit" class="btn btn-primary">Registrar</button>
            <a href="#/login" class="btn btn-secondary">Voltar</a>
          </div>
        </form>
      </div>
    `;

    document.getElementById("form-registro").addEventListener("submit", async (ev) => {
      ev.preventDefault();
      const fd = new FormData(ev.target);
      try {
        await api.auth.registrar({
          nome: fd.get("nome").trim(),
          email: fd.get("email").trim(),
          senha: fd.get("senha")
        });
        toast("Conta criada. Faca login.", "success");
        window.location.hash = "#/login";
      } catch (e) {
        toast(e.message, "error");
      }
    });
  }
};
