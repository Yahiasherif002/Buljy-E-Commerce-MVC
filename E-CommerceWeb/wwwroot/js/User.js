// User Management Module
const UserManagement = {
    // Initialize the module
    init() {
        this.initializeDataTable();
        this.initializeToastr();
        this.setupEventListeners();
    },

    // Initialize Toastr notifications
    initializeToastr() {
        toastr.options = {
            closeButton: true,
            progressBar: true,
            positionClass: "toast-bottom-right",
            timeOut: 3000
        };
    },

    // Setup event listeners
    setupEventListeners() {
        document.addEventListener('DOMContentLoaded', () => {
            const refreshBtn = document.getElementById('refreshTable');
            if (refreshBtn) {
                refreshBtn.addEventListener('click', () => this.refreshTable());
            }
        });
    },

    // Initialize DataTable
    initializeDataTable() {
        this.dataTable = $('#usersTable').DataTable({
            responsive: true,
            ajax: {
                url: '/Admin/User/GetAll',
                type: 'GET',
                dataType: 'json',
                dataSrc: this.handleDataSource,
                error: this.handleAjaxError
            },
            processing: true,
            columns: [
                {
                    data: null,
                    render: this.renderUserCard
                },
                {
                    data: 'phoneNumber',
                    render: this.renderPhoneNumber
                },
                {
                    data: 'company.name',
                    render: this.renderCompany
                },
                {
                    data: 'role',
                    render: this.renderRole
                },
                {
                    data: 'lockoutEnd',
                    render: this.renderLockStatus
                },
                {
                    data: null,
                    orderable: false,
                    render: this.renderActions
                }
            ],
            language: {
                emptyTable: "No users found",
                zeroRecords: "No matching users found",
                info: "Showing _START_ to _END_ of _TOTAL_ users",
                infoEmpty: "Showing 0 to 0 of 0 users",
                infoFiltered: "(filtered from _MAX_ total users)",
                search: "",
                searchPlaceholder: "Search users...",
                lengthMenu: "Show _MENU_ users",
                processing: '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>'
            },
            dom: '<"top"lf>rt<"bottom"ip><"clear">',
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
            pageLength: 10,
            order: [[0, 'asc']]
        });
    },

    // Data handling methods
    handleDataSource(json) {
        if (!json) {
            console.error('No data received');
            return [];
        }
        return Array.isArray(json) ? json : (json.data || []);
    },

    handleAjaxError(xhr, error, thrown) {
        console.error('DataTable error:', { xhr, error, thrown });
        toastr.error('Failed to load user data. Please try refreshing the page.');
    },

    // Render methods
    renderUserCard(data, type, row) {
        if (!row || !row.userName) {
            console.warn('Invalid row data:', row);
            return '<div class="user-card"><div class="user-info"><div class="user-name">Unknown User</div></div></div>';
        }
        const initials = UserManagement.getInitials(row.userName);
        return `
            <div class="user-card">
                <div class="user-avatar">${initials}</div>
                <div class="user-info">
                    <div class="user-name">${row.userName}</div>
                    <div class="user-email">${row.email || 'No email'}</div>
                </div>
            </div>`;
    },

    renderPhoneNumber(data) {
        return `<div class="text-nowrap">
            <i class="bi bi-telephone me-2 text-muted"></i>${data || 'Not provided'}
        </div>`;
    },

    renderCompany(data, type, row) {
        return row.company ? `<span class="text-nowrap">${row.company.name}</span>` : '-';
    },

    renderRole(data) {
        const roleColors = {
            'Admin': 'bg-primary',
            'Employee': 'bg-success',
            'Company': 'bg-info',
            'Customer': 'bg-secondary'
        };
        const bgClass = roleColors[data] || 'bg-secondary';
        return `<span class="role-badge ${bgClass} text-white">${data || 'Unknown'}</span>`;
    },

    renderLockStatus(data) {
        const isLocked = data && new Date(data) > new Date();
        return `
            <span class="status-badge ${isLocked ? 'status-locked' : 'status-active'}">
                <i class="bi ${isLocked ? 'bi-lock-fill' : 'bi-unlock-fill'}"></i>
                ${isLocked ? 'Locked' : 'Active'}
            </span>`;
    },

    renderActions(data, type, row) {
        if (!row || !row.id) {
            console.warn('Missing row ID:', row);
            return '';
        }
        const isLocked = row.lockoutEnd && new Date(row.lockoutEnd) > new Date();
        return `
            <div class="d-flex gap-2 justify-content-end">
                <a href="/Admin/User/RoleManagement/${row.id}" 
                   class="action-btn" 
                   title="Manage Role">
                    <i class="bi bi-person-gear"></i>
                </a>
                <button onclick="UserManagement.toggleLock('${row.id}')" 
                        class="action-btn" 
                        title="${isLocked ? 'Unlock' : 'Lock'} Account">
                    <i class="bi ${isLocked ? 'bi-unlock' : 'bi-lock'}"></i>
                </button>
                <button onclick="UserManagement.resetPassword('${row.id}')" 
                        class="action-btn" 
                        title="Reset Password">
                    <i class="bi bi-key"></i>
                </button>
                <button onclick="UserManagement.deleteUser('${row.id}')" 
                        class="action-btn danger" 
                        title="Delete User">
                    <i class="bi bi-trash"></i>
                </button>
            </div>`;
    },

    // Utility methods
    getInitials(name) {
        if (!name) return 'UN';
        return name
            .split(' ')
            .map(word => word[0])
            .join('')
            .toUpperCase()
            .substring(0, 2);
    },

    refreshTable() {
        this.dataTable.ajax.reload();
        toastr.success('Table refreshed successfully');
    },

    // Action methods
    toggleLock(userId) {
        const row = this.dataTable.rows().data().toArray().find(x => x.id === userId);
        const isLocked = row.lockoutEnd && new Date(row.lockoutEnd) > new Date();
        const action = isLocked ? 'unlock' : 'lock';

        Swal.fire({
            title: `${action.charAt(0).toUpperCase() + action.slice(1)} Account?`,
            text: `Are you sure you want to ${action} this user account?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: `Yes, ${action} it!`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                this.performAction('LockUnlock', userId, action);
            }
        });
    },

    resetPassword(userId) {
        Swal.fire({
            title: 'Reset Password?',
            text: 'A password reset link will be sent to the user\'s email',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, send reset link',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                this.performAction('ResetPassword', userId, 'reset');
            }
        });
    },

    deleteUser(userId) {
        Swal.fire({
            title: 'Delete User?',
            text: 'This action cannot be undone',
            icon: 'error',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete user',
            cancelButtonText: 'No, keep user'
        }).then((result) => {
            if (result.isConfirmed) {
                this.performAction('Delete', userId, 'delete');
            }
        });
    },

    performAction(action, userId, actionType) {
        $.ajax({
            url: `/Admin/User/${action}/${userId}`,
            type: action === 'Delete' ? 'DELETE' : 'POST',
            contentType: 'application/json',
            data: action === 'LockUnlock' ? JSON.stringify(userId) : null,
            success: (response) => {
                if (response.success) {
                    this.dataTable.ajax.reload();
                    toastr.success(response.message || `Action completed successfully`);
                } else {
                    toastr.error(response.message || `Failed to ${actionType}`);
                }
            },
            error: (xhr) => {
                console.error(`${action} error:`, xhr);
                toastr.error(`Error performing ${actionType} action`);
            }
        });
    }
};

// Initialize the module when the document is ready
$(document).ready(() => UserManagement.init());