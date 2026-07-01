window.RegistrationComponent = {
  init() {
    $('#register-form').on('submit', event => this.register(event));
    $('#confirm-registration-button').on('click', () => this.confirmRegistration());
    $('#invite-form').on('submit', event => this.invite(event));
    $('#accept-invite-button').on('click', () => this.acceptInvite());
    $('#resend-invite-button').on('click', () => this.resendInvite());
    $('#forgot-password-form').on('submit', event => this.forgotPassword(event));
    $('#confirm-forgot-password-button').on('click', () => this.confirmForgotPassword());
    this.renderStoredTokens();
  },

  async register(event) {
    event.preventDefault();

    await this.run(async () => {
      const result = await SecurityApi.post('/Api/Account/Register', {
        email: $('#register-email').val(),
        displayName: $('#register-display-name').val(),
        password: $('#register-password').val(),
        phoneNumber: $('#register-phone').val(),
        tenantId: $('#register-tenant').val(),
        culture: $('#register-culture').val()
      });

      this.store('registrationToken', result.token);
      this.store('registrationUserId', result.user && result.user.id);
      this.renderResult(result);
    });
  },

  async confirmRegistration() {
    await this.run(async () => {
      const token = sessionStorage.getItem('security.registrationToken');
      const result = await SecurityApi.post(`/Api/Account/ConfirmRegistration?confirmationToken=${encodeURIComponent(token)}`);
      this.renderResult(result || 'Registration confirmed');
    });
  },

  async invite(event) {
    event.preventDefault();

    await this.run(async () => {
      const result = await SecurityApi.post('/Api/Account/Invite', {
        email: $('#invite-email').val(),
        displayName: $('#invite-display-name').val(),
        phoneNumber: $('#invite-phone').val(),
        culture: $('#invite-culture').val()
      });

      this.store('inviteToken', result.token);
      this.store('inviteUserId', result.user && result.user.id);
      $('#resend-user-id').val(result.user && result.user.id);
      this.renderResult(result);
    });
  },

  async acceptInvite() {
    await this.run(async () => {
      const userId = sessionStorage.getItem('security.inviteUserId');
      const token = sessionStorage.getItem('security.inviteToken');
      const result = await SecurityApi.post(
        `/Api/Account/AcceptInvite?userId=${encodeURIComponent(userId)}&inviteToken=${encodeURIComponent(token)}`,
        {
          email: $('#invite-email').val(),
          displayName: $('#invite-display-name').val(),
          password: $('#accept-password').val(),
          phoneNumber: $('#invite-phone').val(),
          culture: $('#invite-culture').val()
        });

      this.renderResult(result || 'Invite accepted');
    });
  },

  async resendInvite() {
    await this.run(async () => {
      const userId = $('#resend-user-id').val();
      const result = await SecurityApi.post(`/Api/Account/ResendInvite?userId=${encodeURIComponent(userId)}`);
      this.store('inviteToken', result.token);
      this.store('inviteUserId', userId);
      this.renderResult(result);
    });
  },

  async forgotPassword(event) {
    event.preventDefault();

    await this.run(async () => {
      const result = await SecurityApi.post('/Api/Account/ForgotPassword', {
        email: $('#forgot-email').val()
      });

      this.renderResult(result || 'Password reset requested');
    });
  },

  async confirmForgotPassword() {
    await this.run(async () => {
      const password = $('#forgot-new-password').val();
      const result = await SecurityApi.post('/Api/Account/ConfirmForgotPassword', {
        token: $('#forgot-token').val(),
        userId: $('#forgot-user-id').val(),
        newPassword: password,
        confirmPassword: password
      });

      this.renderResult(result || 'Password reset confirmed');
    });
  },

  store(key, value) {
    if (value) {
      sessionStorage.setItem(`security.${key}`, value);
      this.renderStoredTokens();
    }
  },

  renderStoredTokens() {
    const registrationToken = sessionStorage.getItem('security.registrationToken');
    const inviteToken = sessionStorage.getItem('security.inviteToken');

    $('#stored-token-summary').html(
      `Registration: <span class="token-chip">${registrationToken || 'none'}</span><br>` +
      `Invite: <span class="token-chip">${inviteToken || 'none'}</span>`);
  },

  renderResult(result) {
    $('#registration-result').text(SecurityApi.format(result));
    this.renderStoredTokens();
  },

  async run(action) {
    try {
      await action();
    } catch (error) {
      $('#registration-result').text(error.message);
    }
  }
};
