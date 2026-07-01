window.AccountComponent = {
  init() {
    $('#login-form').on('submit', event => this.login(event));
    $('#logout-button').on('click', () => this.logout());
    $('#me-button').on('click', () => this.me());
    $('#account-refresh').on('click', () => this.refreshCurrentUser());
    this.refreshCurrentUser();
  },

  async login(event) {
    event.preventDefault();

    await this.run(async () => {
      const token = await SecurityApi.post('/Api/Account/Login', {
        user: $('#login-user').val(),
        pass: $('#login-password').val()
      });

      $('#account-result').text(SecurityApi.format(token));
      await this.refreshCurrentUser();
    });
  },

  async logout() {
    await this.run(async () => {
      const result = await SecurityApi.post('/Api/Account/Logout');
      $('#account-result').text(SecurityApi.format(result || 'Logged out'));
      await this.refreshCurrentUser();
    });
  },

  async me() {
    await this.run(async () => {
      const user = await SecurityApi.get('/Api/Account/Me');
      $('#account-result').text(SecurityApi.format(user));
      $('#current-user').text(user.id || 'Guest');
    });
  },

  async refreshCurrentUser() {
    try {
      const user = await SecurityApi.get('/CurrentUser');
      $('#current-user').text(user || 'Guest');
    } catch {
      $('#current-user').text('Guest');
    }
  },

  async run(action) {
    try {
      await action();
    } catch (error) {
      $('#account-result').text(error.message);
    }
  }
};
