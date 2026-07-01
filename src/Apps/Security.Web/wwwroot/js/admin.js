window.AdminComponent = {
  endpoints: {
    users: '/Api/Security/SSOUser',
    tenants: '/Api/Security/Tenant',
    roles: '/Api/Security/SSORole',
    privileges: '/Api/Security/SSOPrivilege',
    userRoles: '/Api/Security/SSOUserRole',
    tenantAnalysis: '/Api/Security/TenantAnalysis',
    userEvents: '/Api/Security/UserEvent'
  },

  currentList: null,

  init() {
    $('[data-admin-refresh]').on('click', event => {
      const key = $(event.currentTarget).data('admin-refresh');
      this.refresh(key);
    });

    $('#user-form').on('submit', event => this.updateUser(event));
    $('#user-get-button').on('click', () => this.getUser());
    $('#user-delete-button').on('click', () => this.deleteUser());

    $('#tenant-form').on('submit', event => this.addTenant(event));
    $('#tenant-get-button').on('click', () => this.getTenant());
    $('#tenant-update-button').on('click', () => this.updateTenant());
    $('#tenant-delete-button').on('click', () => this.deleteTenant());

    $('#role-form').on('submit', event => this.addRole(event));
    $('#role-get-button').on('click', () => this.getRole());
    $('#role-update-button').on('click', () => this.updateRole());
    $('#role-delete-button').on('click', () => this.deleteRole());

    $('#user-role-form').on('submit', event => this.attachUserRole(event));
    $('#user-role-delete-button').on('click', () => this.removeUserRole());
  },

  async refresh(key) {
    await this.run(async () => {
      this.currentList = key;
      const rows = await SecurityApi.list(this.endpoints[key]);

      $('#admin-list-context').text(`Showing ${key}. Click a row to copy values into the matching form.`);
      this.renderTable(rows || []);
      $('#admin-result').text(SecurityApi.format(rows || []));
    });
  },

  async getUser() {
    await this.run(async () => {
      const user = await SecurityApi.get(`${this.endpoints.users}${SecurityApi.stringKey($('#user-id').val())}`);
      this.populateUser(user);
      $('#admin-result').text(SecurityApi.format(user));
    });
  },

  async updateUser(event) {
    event.preventDefault();

    await this.run(async () => {
      const user = this.readUser();
      const result = await SecurityApi.put(`${this.endpoints.users}${SecurityApi.stringKey(user.id)}`, user);

      $('#admin-result').text(SecurityApi.format(result));
      await this.refresh('users');
    });
  },

  async deleteUser() {
    await this.run(async () => {
      const id = $('#user-id').val();
      const result = await SecurityApi.delete(`${this.endpoints.users}${SecurityApi.stringKey(id)}`);

      $('#admin-result').text(SecurityApi.format(result || 'User deleted'));
      await this.refresh('users');
    });
  },

  async addTenant(event) {
    event.preventDefault();

    await this.run(async () => {
      const tenant = this.readTenant();
      const result = await SecurityApi.post(this.endpoints.tenants, tenant);

      $('#admin-result').text(SecurityApi.format(result));
      await this.refresh('tenants');
    });
  },

  async getTenant() {
    await this.run(async () => {
      const tenant = await SecurityApi.get(`${this.endpoints.tenants}${SecurityApi.stringKey($('#tenant-id').val())}`);
      this.populateTenant(tenant);
      $('#admin-result').text(SecurityApi.format(tenant));
    });
  },

  async updateTenant() {
    await this.run(async () => {
      const tenant = this.readTenant();
      const result = await SecurityApi.put(`${this.endpoints.tenants}${SecurityApi.stringKey(tenant.id)}`, tenant);

      $('#admin-result').text(SecurityApi.format(result));
      await this.refresh('tenants');
    });
  },

  async deleteTenant() {
    await this.run(async () => {
      const id = $('#tenant-id').val();
      const result = await SecurityApi.delete(`${this.endpoints.tenants}${SecurityApi.stringKey(id)}`);

      $('#admin-result').text(SecurityApi.format(result || 'Tenant deleted'));
      await this.refresh('tenants');
    });
  },

  async addRole(event) {
    event.preventDefault();

    await this.run(async () => {
      const role = this.readRole();
      delete role.id;

      const result = await SecurityApi.post(this.endpoints.roles, role);

      $('#admin-result').text(SecurityApi.format(result));
      await this.refresh('roles');
    });
  },

  async getRole() {
    await this.run(async () => {
      const role = await SecurityApi.get(`${this.endpoints.roles}${SecurityApi.guidKey($('#role-id').val())}`);
      this.populateRole(role);
      $('#admin-result').text(SecurityApi.format(role));
    });
  },

  async updateRole() {
    await this.run(async () => {
      const role = this.readRole();
      const result = await SecurityApi.put(`${this.endpoints.roles}${SecurityApi.guidKey(role.id)}`, role);

      $('#admin-result').text(SecurityApi.format(result));
      await this.refresh('roles');
    });
  },

  async deleteRole() {
    await this.run(async () => {
      const id = $('#role-id').val();
      const result = await SecurityApi.delete(`${this.endpoints.roles}${SecurityApi.guidKey(id)}`);

      $('#admin-result').text(SecurityApi.format(result || 'Role deleted'));
      await this.refresh('roles');
    });
  },

  async attachUserRole(event) {
    event.preventDefault();

    await this.run(async () => {
      const result = await SecurityApi.post(this.endpoints.userRoles, {
        userId: $('#user-role-user-id').val(),
        roleId: $('#user-role-role-id').val()
      });

      $('#admin-result').text(SecurityApi.format(result));
      await this.refresh('userRoles');
    });
  },

  async removeUserRole() {
    await this.run(async () => {
      const userId = encodeURIComponent($('#user-role-user-id').val());
      const roleId = encodeURIComponent($('#user-role-role-id').val());
      const result = await SecurityApi.delete(`${this.endpoints.userRoles}?userId=${userId}&roleId=${roleId}`);

      $('#admin-result').text(SecurityApi.format(result || 'User role removed'));
      await this.refresh('userRoles');
    });
  },

  readUser() {
    return {
      id: $('#user-id').val(),
      displayName: $('#user-display-name').val(),
      email: $('#user-email').val(),
      phoneNumber: $('#user-phone').val(),
      emailConfirmed: $('#user-email-confirmed').is(':checked'),
      phoneNumberConfirmed: $('#user-phone-confirmed').is(':checked')
    };
  },

  readTenant() {
    return {
      id: $('#tenant-id').val(),
      name: $('#tenant-name').val(),
      description: $('#tenant-description').val(),
      createdBy: $('#tenant-created-by').val() || undefined
    };
  },

  readRole() {
    return {
      id: $('#role-id').val(),
      name: $('#role-name').val(),
      tenantId: $('#role-tenant-id').val(),
      privs: $('#role-privs').val(),
      description: $('#role-description').val(),
      usersArePortalAdmins: $('#role-portal-admins').is(':checked')
    };
  },

  populateUser(user) {
    $('#user-id').val(user.id || '');
    $('#user-display-name').val(user.displayName || '');
    $('#user-email').val(user.email || '');
    $('#user-phone').val(user.phoneNumber || '');
    $('#user-email-confirmed').prop('checked', !!user.emailConfirmed);
    $('#user-phone-confirmed').prop('checked', !!user.phoneNumberConfirmed);
  },

  populateTenant(tenant) {
    $('#tenant-id').val(tenant.id || '');
    $('#tenant-name').val(tenant.name || '');
    $('#tenant-description').val(tenant.description || '');
    $('#tenant-created-by').val(tenant.createdBy || '');
  },

  populateRole(role) {
    $('#role-id').val(role.id || '');
    $('#role-name').val(role.name || '');
    $('#role-tenant-id').val(role.tenantId || '');
    $('#role-privs').val(role.privs || '');
    $('#role-description').val(role.description || '');
    $('#role-portal-admins').prop('checked', !!role.usersArePortalAdmins);
  },

  populateUserRole(userRole) {
    $('#user-role-user-id').val(userRole.userId || '');
    $('#user-role-role-id').val(userRole.roleId || '');
  },

  populateFromRow(row) {
    if (this.currentList === 'users') {
      this.populateUser(row);
      return;
    }

    if (this.currentList === 'tenants') {
      this.populateTenant(row);
      return;
    }

    if (this.currentList === 'roles') {
      this.populateRole(row);
      return;
    }

    if (this.currentList === 'userRoles') {
      this.populateUserRole(row);
    }
  },

  renderTable(rows) {
    const table = $('#admin-table');
    table.empty();

    if (!rows.length) {
      table.append('<tbody><tr><td>No rows</td></tr></tbody>');
      return;
    }

    const columns = Object.keys(rows[0])
      .filter(key => !key.startsWith('@'))
      .filter(key => typeof rows[0][key] !== 'object');

    const head = $('<thead><tr></tr></thead>');

    columns.forEach(column => head.find('tr').append($('<th></th>').text(column)));
    table.append(head);

    const body = $('<tbody></tbody>');

    rows.forEach(row => {
      const tr = $('<tr class="selectable-row"></tr>');
      columns.forEach(column => tr.append($('<td></td>').text(row[column] ?? '')));
      tr.on('click', () => this.populateFromRow(row));
      body.append(tr);
    });

    table.append(body);
  },

  async run(action) {
    try {
      await action();
    } catch (error) {
      $('#admin-result').text(error.message);
    }
  }
};
