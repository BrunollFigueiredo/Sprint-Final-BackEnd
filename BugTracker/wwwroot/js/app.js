const API = '';
let currentUser = null;

// ---- Theme ----
function toggleTheme() {
    const html = document.documentElement;
    const next = html.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
    html.setAttribute('data-bs-theme', next);
    localStorage.setItem('theme', next);
    updateThemeIcon(next);
}

function updateThemeIcon(theme) {
    const icon = document.getElementById('themeIcon');
    if (icon) icon.className = theme === 'dark' ? 'bi bi-sun' : 'bi bi-moon';
}

// ---- Auth ----
function showLoginForm() {
    document.getElementById('loginForm').classList.remove('d-none');
    document.getElementById('registerForm').classList.add('d-none');
    document.getElementById('loginTab').classList.add('active');
    document.getElementById('registerTab').classList.remove('active');
}

function showRegisterForm() {
    document.getElementById('loginForm').classList.add('d-none');
    document.getElementById('registerForm').classList.remove('d-none');
    document.getElementById('loginTab').classList.remove('active');
    document.getElementById('registerTab').classList.add('active');
}

async function login() {
    const email = document.getElementById('loginEmail').value;
    const senha = document.getElementById('loginSenha').value;
    if (!email || !senha) return showAuthAlert('Preencha todos os campos.', 'danger');
    const res = await apiFetch('/api/auth/login', 'POST', { email, senha }, false);
    if (res.ok) {
        const data = await res.json();
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify({ id: data.id, nome: data.nome, email: data.email, perfil: data.perfil }));
        initApp();
    } else {
        const err = await res.json();
        showAuthAlert(err.mensagem || 'Erro ao fazer login.', 'danger');
    }
}

async function register() {
    const nome = document.getElementById('regNome').value;
    const email = document.getElementById('regEmail').value;
    const senha = document.getElementById('regSenha').value;
    const aceitouTermos = document.getElementById('regTermos').checked;

    if (!nome || !email || !senha) return showAuthAlert('Preencha todos os campos.', 'danger');
    if (!aceitouTermos) return showAuthAlert('Você deve aceitar os Termos de Uso e a Política de Privacidade para continuar.', 'warning');

    const res = await apiFetch('/api/auth/register', 'POST', { nome, email, senha, aceitouTermos }, false);
    if (res.ok) {
        const data = await res.json();
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify({ id: data.id, nome: data.nome, email: data.email, perfil: data.perfil }));
        initApp();
    } else {
        try {
            const err = await res.json();
            showAuthAlert(err.mensagem || 'Erro ao registrar.', 'danger');
        } catch {
            showAuthAlert('Erro ao registrar (status ' + res.status + ').', 'danger');
        }
    }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    document.getElementById('mainApp').classList.add('d-none');
    document.getElementById('loginPage').classList.remove('d-none');
    currentUser = null;
}

function showAuthAlert(msg, type) {
    const el = document.getElementById('authAlert');
    el.className = `alert alert-${type} mt-3`;
    el.textContent = msg;
    el.classList.remove('d-none');
    setTimeout(() => el.classList.add('d-none'), 5000);
}

function showTermosModal() {
    new bootstrap.Modal(document.getElementById('termosModal')).show();
}

function aceitarTermos() {
    document.getElementById('regTermos').checked = true;
    bootstrap.Modal.getInstance(document.getElementById('termosModal')).hide();
}

// ---- App Init ----
function initApp() {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');
    if (!token || !user) return;
    currentUser = JSON.parse(user);
    document.getElementById('loginPage').classList.add('d-none');
    document.getElementById('mainApp').classList.remove('d-none');
    document.getElementById('userNome').textContent = currentUser.nome;
    document.getElementById('userPerfil').textContent = currentUser.perfil;
    updateThemeIcon(document.documentElement.getAttribute('data-bs-theme') || 'dark');

    document.querySelectorAll('.admin-only').forEach(el => el.classList.add('d-none'));
    document.querySelectorAll('.dev-admin-only').forEach(el => el.classList.add('d-none'));

    if (currentUser.perfil === 'Admin') {
        document.querySelectorAll('.admin-only').forEach(el => el.classList.remove('d-none'));
    }
    if (currentUser.perfil === 'Admin' || currentUser.perfil === 'Dev') {
        document.querySelectorAll('.dev-admin-only').forEach(el => el.classList.remove('d-none'));
    }

    loadDashboard();
    loadProjetosSelect();
}

// ---- API Helper ----
async function apiFetch(url, method = 'GET', body = null, auth = true) {
    const headers = { 'Content-Type': 'application/json' };
    if (auth) {
        const token = localStorage.getItem('token');
        if (token) headers['Authorization'] = `Bearer ${token}`;
    }
    const opts = { method, headers };
    if (body) opts.body = JSON.stringify(body);
    const res = await fetch(API + url, opts);
    if (res.status === 401) { logout(); return res; }
    return res;
}

// ---- Navigation ----
function showSection(section, el) {
    ['dashboard', 'bugs', 'projetos', 'usuarios'].forEach(s =>
        document.getElementById(`${s}Section`).classList.add('d-none'));
    document.getElementById(`${section}Section`).classList.remove('d-none');

    document.querySelectorAll('.app-tab').forEach(e => e.classList.remove('active'));
    if (el) el.classList.add('active');
    else { const nav = document.getElementById(`nav-${section}`); if (nav) nav.classList.add('active'); }

    if (section === 'dashboard') loadDashboard();
    if (section === 'projetos') loadProjetos();
    if (section === 'usuarios') loadUsuarios();
    if (section === 'bugs') loadBugs();
}

// ---- Toast ----
function showToast(msg, type = 'success') {
    const toast = document.getElementById('toast');
    const toastMsg = document.getElementById('toastMsg');
    toast.className = `toast align-items-center border-0 text-white bg-${type}`;
    toastMsg.textContent = msg;
    new bootstrap.Toast(toast, { delay: 3000 }).show();
}

// ---- Dashboard ----
let chartStatus = null;
let chartSeveridade = null;
let chartProjetos = null;
let chartPlataforma = null;
let chartTipo = null;

function statusLabel(s) {
    const map = { EmAndamento: 'Em Andamento', NaoReproduzivel: 'Não Reproduzível', NaoCorrigir: 'Não Corrigir' };
    return map[s] || s;
}

function tipoBugLabel(t) {
    const map = { Audio: 'Áudio', Localizacao: 'Localização', UI: 'UI/UX' };
    return map[t] || t;
}

async function loadDashboard() {
    const res = await apiFetch('/api/dashboard');
    if (!res.ok) return;
    const d = await res.json();

    document.getElementById('dash-total').textContent = d.totalBugs;
    document.getElementById('dash-bloqueadores').textContent = d.bugsBloqueiamLancamento;
    document.getElementById('dash-criticos').textContent = d.bugsCriticos;
    document.getElementById('dash-abertos').textContent = d.bugsAbertos;
    document.getElementById('dash-andamento').textContent = d.bugsEmAndamento;
    document.getElementById('dash-resolvidos').textContent = d.bugsResolvidos;
    document.getElementById('dash-projetos').textContent = d.totalProjetos;
    document.getElementById('dash-usuarios').textContent = d.totalUsuarios;

    renderChartStatus(d);
    renderChartSeveridade(d.bugsPorSeveridade);
    renderChartProjetos(d.bugsPorProjeto);
    renderChartPlataforma(d.bugsPorPlataforma);
    renderChartTipo(d.bugsPorTipo);
    renderBloqueadores(d.bugsBloqueiadores, d.bugsBloqueiamLancamento);
}

function destroyChart(chart) { if (chart) { chart.destroy(); } return null; }

function emptyChartMsg(ctx, msg = 'Sem dados para exibir') {
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
    ctx.font = '13px sans-serif';
    ctx.fillStyle = '#6b7280';
    ctx.textAlign = 'center';
    ctx.fillText(msg, ctx.canvas.width / 2, ctx.canvas.height / 2);
}

function renderChartStatus(d) {
    const ctx = document.getElementById('chartStatus').getContext('2d');
    chartStatus = destroyChart(chartStatus);
    if (d.totalBugs === 0) { emptyChartMsg(ctx, 'Nenhum bug registrado'); return; }
    chartStatus = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Aberto', 'Em Andamento', 'Resolvido', 'Fechado'],
            datasets: [{
                data: [d.bugsAbertos, d.bugsEmAndamento, d.bugsResolvidos, d.bugsFechados],
                backgroundColor: ['#fbbf24', '#7c3aed', '#10b981', '#9ca3af'],
                borderWidth: 3,
                borderColor: '#161b22',
                hoverOffset: 8
            }]
        },
        options: {
            layout: { padding: 8 },
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: { padding: 20, font: { size: 12 }, usePointStyle: true, pointStyleWidth: 10 }
                },
                tooltip: { callbacks: { label: ctx => ` ${ctx.label}: ${ctx.raw} bugs` } }
            },
            cutout: '68%'
        }
    });
}

function renderChartSeveridade(severidades) {
    const ctx = document.getElementById('chartSeveridade').getContext('2d');
    chartSeveridade = destroyChart(chartSeveridade);
    if (!severidades || severidades.length === 0) { emptyChartMsg(ctx, 'Nenhum bug registrado'); return; }
    const cores = { Critica: '#dc3545', Alta: '#fd7e14', Media: '#ffc107', Baixa: '#28a745' };
    const coresSoft = { Critica: '#ef4444', Alta: '#f97316', Media: '#eab308', Baixa: '#22c55e' };
    chartSeveridade = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: severidades.map(s => s.severidade === 'Media' ? 'Média' : s.severidade === 'Critica' ? 'Crítica' : s.severidade),
            datasets: [{
                label: 'Quantidade de Bugs',
                data: severidades.map(s => s.total),
                backgroundColor: severidades.map(s => (coresSoft[s.severidade] || '#6c757d') + 'cc'),
                borderColor: severidades.map(s => coresSoft[s.severidade] || '#6c757d'),
                borderWidth: 1,
                borderRadius: 8,
                borderSkipped: false,
                barPercentage: 0.5,
                categoryPercentage: 0.7
            }]
        },
        options: {
            responsive: true,
            layout: { padding: { left: 4, right: 4, top: 12, bottom: 4 } },
            plugins: { legend: { display: false } },
            scales: {
                y: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: 'rgba(255,255,255,0.06)' }, border: { display: false } },
                x: { grid: { display: false }, border: { display: false } }
            }
        }
    });
}

function renderChartProjetos(projetos) {
    const ctx = document.getElementById('chartProjetos').getContext('2d');
    chartProjetos = destroyChart(chartProjetos);
    if (!projetos || projetos.length === 0) { emptyChartMsg(ctx, 'Nenhum projeto com bugs'); return; }
    chartProjetos = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: projetos.map(p => p.nome),
            datasets: [
                { label: 'Total de Bugs', data: projetos.map(p => p.total), backgroundColor: '#3b82f6cc', borderColor: '#3b82f6', borderWidth: 1, borderRadius: 6, barPercentage: 0.65, categoryPercentage: 0.8 },
                { label: 'Em Aberto', data: projetos.map(p => p.abertos), backgroundColor: '#fbbf24cc', borderColor: '#f59e0b', borderWidth: 1, borderRadius: 6, barPercentage: 0.65, categoryPercentage: 0.8 }
            ]
        },
        options: {
            responsive: true,
            layout: { padding: { left: 4, right: 4, top: 12, bottom: 4 } },
            plugins: { legend: { position: 'bottom', labels: { padding: 20, font: { size: 12 }, usePointStyle: true, pointStyleWidth: 10 } } },
            scales: {
                y: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: 'rgba(255,255,255,0.06)' }, border: { display: false } },
                x: { grid: { display: false }, border: { display: false } }
            }
        }
    });
}

function renderChartPlataforma(plataformas) {
    const ctx = document.getElementById('chartPlataforma').getContext('2d');
    chartPlataforma = destroyChart(chartPlataforma);
    if (!plataformas || plataformas.length === 0) { emptyChartMsg(ctx, 'Nenhuma plataforma informada'); return; }
    chartPlataforma = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: plataformas.map(p => p.plataforma),
            datasets: [{
                label: 'Bugs',
                data: plataformas.map(p => p.total),
                backgroundColor: '#06b6d4cc',
                borderColor: '#06b6d4',
                borderWidth: 1,
                borderRadius: 6,
                borderSkipped: false,
                barPercentage: 0.5,
                categoryPercentage: 0.75
            }]
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            layout: { padding: { left: 4, right: 12, top: 4, bottom: 4 } },
            plugins: { legend: { display: false } },
            scales: {
                x: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: 'rgba(255,255,255,0.06)' }, border: { display: false } },
                y: { grid: { display: false }, border: { display: false } }
            }
        }
    });
}

function renderChartTipo(tipos) {
    const ctx = document.getElementById('chartTipo').getContext('2d');
    chartTipo = destroyChart(chartTipo);
    if (!tipos || tipos.length === 0) { emptyChartMsg(ctx, 'Nenhum tipo informado'); return; }
    const coresTipo = {
        Crash: '#ef4444', Visual: '#8b5cf6', Audio: '#06b6d4', Gameplay: '#f97316',
        Performance: '#eab308', UI: '#3b82f6', Localizacao: '#10b981', Outro: '#9ca3af'
    };
    chartTipo = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: tipos.map(t => tipoBugLabel(t.tipo)),
            datasets: [{
                label: 'Bugs',
                data: tipos.map(t => t.total),
                backgroundColor: tipos.map(t => (coresTipo[t.tipo] || '#9ca3af') + 'cc'),
                borderColor: tipos.map(t => coresTipo[t.tipo] || '#9ca3af'),
                borderWidth: 1,
                borderRadius: 8,
                borderSkipped: false,
                barPercentage: 0.5,
                categoryPercentage: 0.7
            }]
        },
        options: {
            responsive: true,
            layout: { padding: { left: 4, right: 4, top: 12, bottom: 4 } },
            plugins: { legend: { display: false } },
            scales: {
                y: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: 'rgba(255,255,255,0.06)' }, border: { display: false } },
                x: { grid: { display: false }, border: { display: false } }
            }
        }
    });
}

function renderBloqueadores(bloqueadores, total) {
    const section = document.getElementById('bloqueadoresSection');
    if (!bloqueadores || total === 0) { section.classList.add('d-none'); return; }
    section.classList.remove('d-none');
    document.getElementById('dash-bloqueadores-count').textContent = total;
    const tbody = document.getElementById('bloqueadoresTableBody');
    tbody.innerHTML = bloqueadores.map(b => `
        <tr>
            <td><small class="text-muted fw-semibold">#${b.id}</small></td>
            <td><strong>${escHtml(b.titulo)}</strong></td>
            <td><small class="text-muted">${escHtml(b.projetoNome)}</small></td>
            <td><span class="badge rounded-pill badge-soft-${b.severidade}">${b.severidade === 'Critica' ? 'Crítica' : b.severidade === 'Media' ? 'Média' : b.severidade}</span></td>
            <td><span class="badge rounded-pill badge-soft-${b.status}">${statusLabel(b.status)}</span></td>
            <td>${b.atribuidoParaNome ? escHtml(b.atribuidoParaNome) : '<span class="text-muted">—</span>'}</td>
            <td><button class="btn btn-sm btn-outline-primary rounded-pill px-3" onclick="irParaBug(${b.id})"><i class="bi bi-arrow-right"></i></button></td>
        </tr>
    `).join('');
}

// ---- Bugs ----
let allBugs = [];

async function loadBugs() {
    const res = await apiFetch('/api/bugs');
    if (!res.ok) return;
    allBugs = await res.json();
    renderBugs();
}

function renderBugs() {
    const filtroBusca = (document.getElementById('filtroBusca').value || '').trim().toLowerCase();
    const filtroStatus = document.getElementById('filtroStatus').value;
    const filtroTipo = document.getElementById('filtroTipo').value;
    const filtroSev = document.getElementById('filtroSeveridade').value;
    const filtroProjId = document.getElementById('filtroProjeto').value;
    const filtroPlat = document.getElementById('filtroPlataforma').value;
    const filtroBloqueio = document.getElementById('filtroBloqueio').value;

    let bugs = allBugs;
    if (filtroBusca) bugs = bugs.filter(b =>
        (b.titulo || '').toLowerCase().includes(filtroBusca) ||
        (b.descricao || '').toLowerCase().includes(filtroBusca) ||
        (b.atribuidoParaNome || '').toLowerCase().includes(filtroBusca)
    );
    if (filtroStatus) bugs = bugs.filter(b => b.status === filtroStatus);
    if (filtroTipo) bugs = bugs.filter(b => b.tipoBug === filtroTipo);
    if (filtroSev) bugs = bugs.filter(b => b.severidade === filtroSev);
    if (filtroProjId) bugs = bugs.filter(b => b.projetoId == filtroProjId);
    if (filtroPlat) bugs = bugs.filter(b => b.plataforma === filtroPlat);
    if (filtroBloqueio === '1') bugs = bugs.filter(b => b.bloqueiaLancamento);
    if (filtroBloqueio === '0') bugs = bugs.filter(b => !b.bloqueiaLancamento);

    const tbody = document.getElementById('bugsTableBody');
    if (bugs.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" class="text-center text-muted py-4">Nenhum bug encontrado.</td></tr>';
        return;
    }

    tbody.innerHTML = bugs.map(b => `
        <tr class="bug-row" onclick="openBugDetail(${b.id})">
            <td>
                <span class="text-muted small fw-semibold">#${b.id}</span>
                ${b.bloqueiaLancamento ? '<span class="ms-1 text-danger" title="Bloqueia Lançamento" style="font-size:0.7rem"><i class="bi bi-exclamation-triangle-fill"></i></span>' : ''}
            </td>
            <td>
                <div class="fw-semibold" style="font-size:0.9rem">${escHtml(b.titulo)}</div>
                ${(b.tipoBug || b.milestone) ? `<div class="d-flex flex-wrap gap-1 mt-1">
                    ${b.tipoBug ? `<span class="badge badge-soft-Fechado" style="font-size:0.68rem">${tipoBugLabel(b.tipoBug)}</span>` : ''}
                    ${b.milestone ? `<span class="badge" style="font-size:0.68rem;background:rgba(6,182,212,0.15);color:#67e8f9"><i class="bi bi-flag me-1"></i>${escHtml(b.milestone)}</span>` : ''}
                </div>` : ''}
            </td>
            <td>
                <span class="text-muted small">${escHtml(b.projetoNome)}</span>
                ${b.versaoJogo ? `<div class="text-muted" style="font-size:0.73rem">v${escHtml(b.versaoJogo)}${b.numeroBuild ? ` · #${escHtml(b.numeroBuild)}` : ''}</div>` : ''}
            </td>
            <td>${b.plataforma ? `<span class="badge" style="background:rgba(139,148,158,0.15);color:#8b949e;font-size:0.72rem">${escHtml(b.plataforma)}</span>` : '<span class="text-muted">—</span>'}</td>
            <td><span class="badge badge-severidade-${b.severidade}" style="font-size:0.75rem">${b.severidade === 'Critica' ? 'Crítica' : b.severidade === 'Media' ? 'Média' : b.severidade}</span></td>
            <td><span class="badge badge-status-${b.status}" style="font-size:0.75rem">${statusLabel(b.status)}</span></td>
            <td class="text-muted small">${b.atribuidoParaNome ? escHtml(b.atribuidoParaNome) : '—'}</td>
            <td onclick="event.stopPropagation()">
                <div class="d-flex gap-1">
                    ${canEditBug(b) ? `<button class="btn-action" onclick="editBug(${b.id})" title="Editar"><i class="bi bi-pencil"></i></button>` : ''}
                    ${canDeleteBug() ? `<button class="btn-action danger" onclick="deleteBug(${b.id})" title="Excluir"><i class="bi bi-trash"></i></button>` : ''}
                </div>
            </td>
        </tr>
    `).join('');
}

function canEditBug(bug) {
    if (!currentUser) return false;
    if (currentUser.perfil === 'Admin') return true;
    return currentUser.perfil === 'Dev' && bug.atribuidoParaId === currentUser.id;
}
function canDeleteBug() { return currentUser && currentUser.perfil === 'Admin'; }

async function loadProjetosSelect() {
    const res = await apiFetch('/api/projetos');
    if (!res.ok) return;
    const projetos = await res.json();
    const filtroSelect = document.getElementById('filtroProjeto');
    const bugProjetoSelect = document.getElementById('bugProjeto');
    projetos.forEach(p => {
        filtroSelect.innerHTML += `<option value="${p.id}">${escHtml(p.nome)}</option>`;
        bugProjetoSelect.innerHTML += `<option value="${p.id}">${escHtml(p.nome)}</option>`;
    });
}

async function loadUsuariosSelect() {
    const res = await apiFetch('/api/usuarios');
    if (!res.ok) return;
    const usuarios = await res.json();
    const select = document.getElementById('bugAtribuido');
    select.innerHTML = '<option value="">Nenhum</option>';
    usuarios.forEach(u => {
        select.innerHTML += `<option value="${u.id}">${escHtml(u.nome)} (${u.perfil})</option>`;
    });
}

function showBugModal(bug = null) {
    document.getElementById('bugModalTitle').innerHTML = bug
        ? '<i class="bi bi-pencil me-2"></i>Editar Bug'
        : '<i class="bi bi-bug me-2"></i>Reportar Bug';
    document.getElementById('bugId').value = bug?.id || '';
    document.getElementById('bugTitulo').value = bug?.titulo || '';
    document.getElementById('bugDescricao').value = bug?.descricao || '';
    document.getElementById('bugSeveridade').value = bug?.severidade || 'Baixa';
    document.getElementById('bugTipo').value = bug?.tipoBug || '';
    document.getElementById('bugStatus').value = bug?.status || 'Aberto';
    document.getElementById('bugMilestone').value = bug?.milestone || '';
    document.getElementById('bugPlataforma').value = bug?.plataforma || '';
    document.getElementById('bugVersaoJogo').value = bug?.versaoJogo || '';
    document.getElementById('bugNumeroBuild').value = bug?.numeroBuild || '';
    document.getElementById('bugCena').value = bug?.cena || '';
    document.getElementById('bugFrequencia').value = bug?.frequenciaReproducao || '';
    document.getElementById('bugResultadoEsperado').value = bug?.resultadoEsperado || '';
    document.getElementById('bugResultadoObtido').value = bug?.resultadoObtido || '';
    document.getElementById('bugDetalhesAmbiente').value = bug?.detalhesAmbiente || '';
    document.getElementById('bugBloqueiaLancamento').checked = bug?.bloqueiaLancamento || false;

    const projetoSelect = document.getElementById('bugProjeto');
    if (bug?.projetoId) {
        for (let opt of projetoSelect.options) {
            if (opt.value == bug.projetoId) { opt.selected = true; break; }
        }
    }

    if (currentUser.perfil === 'Admin') {
        loadUsuariosSelect().then(() => {
            if (bug?.atribuidoParaId) {
                document.getElementById('bugAtribuido').value = bug.atribuidoParaId;
            }
        });
    }

    new bootstrap.Modal(document.getElementById('bugModal')).show();
}

async function editBug(id) {
    const b = allBugs.find(x => x.id === id);
    if (b) showBugModal(b);
}

async function saveBug() {
    const id = document.getElementById('bugId').value;
    const data = {
        titulo: document.getElementById('bugTitulo').value,
        descricao: document.getElementById('bugDescricao').value,
        severidade: document.getElementById('bugSeveridade').value,
        tipoBug: document.getElementById('bugTipo').value || null,
        status: document.getElementById('bugStatus').value,
        milestone: document.getElementById('bugMilestone').value || null,
        projetoId: parseInt(document.getElementById('bugProjeto').value),
        atribuidoParaId: document.getElementById('bugAtribuido').value ? parseInt(document.getElementById('bugAtribuido').value) : null,
        plataforma: document.getElementById('bugPlataforma').value || null,
        versaoJogo: document.getElementById('bugVersaoJogo').value || null,
        numeroBuild: document.getElementById('bugNumeroBuild').value || null,
        cena: document.getElementById('bugCena').value || null,
        frequenciaReproducao: document.getElementById('bugFrequencia').value || null,
        resultadoEsperado: document.getElementById('bugResultadoEsperado').value || null,
        resultadoObtido: document.getElementById('bugResultadoObtido').value || null,
        detalhesAmbiente: document.getElementById('bugDetalhesAmbiente').value || null,
        bloqueiaLancamento: document.getElementById('bugBloqueiaLancamento').checked
    };

    if (!data.titulo) return showToast('Título é obrigatório.', 'danger');

    let res;
    if (id) {
        res = await apiFetch(`/api/bugs/${id}`, 'PUT', data);
    } else {
        res = await apiFetch('/api/bugs', 'POST', data);
    }

    if (res.ok) {
        bootstrap.Modal.getInstance(document.getElementById('bugModal')).hide();
        showToast(id ? 'Bug atualizado!' : 'Bug reportado!');
        loadBugs();
        loadDashboard();
    } else {
        const err = await res.json();
        showToast(err.mensagem || 'Erro ao salvar bug.', 'danger');
    }
}

async function deleteBug(id) {
    if (!confirm('Excluir este bug?')) return;
    const res = await apiFetch(`/api/bugs/${id}`, 'DELETE');
    if (res.ok) {
        showToast('Bug excluído!');
        loadBugs();
        loadDashboard();
    } else {
        showToast('Erro ao excluir bug.', 'danger');
    }
}

// ---- Projetos ----
let allProjetos = [];

async function loadProjetos() {
    const res = await apiFetch('/api/projetos');
    if (!res.ok) return;
    allProjetos = await res.json();
    renderProjetos();
}

function renderProjetos() {
    const container = document.getElementById('projetosCards');
    if (allProjetos.length === 0) {
        container.innerHTML = '<div class="col"><p class="text-muted">Nenhum projeto cadastrado.</p></div>';
        return;
    }
    container.innerHTML = allProjetos.map(p => `
        <div class="col-md-4">
            <div class="card h-100 shadow-sm">
                <div class="card-body">
                    <h5 class="card-title"><i class="bi bi-folder-fill text-primary me-2"></i>${escHtml(p.nome)}</h5>
                    ${p.descricao ? `<p class="card-text text-muted small">${escHtml(p.descricao)}</p>` : ''}
                    <div class="d-flex flex-wrap gap-1 mb-2">
                        ${p.motorJogo ? `<span class="badge bg-primary"><i class="bi bi-gear me-1"></i>${escHtml(p.motorJogo)}</span>` : ''}
                        ${p.versaoAtual ? `<span class="badge bg-secondary"><i class="bi bi-tag me-1"></i>v${escHtml(p.versaoAtual)}</span>` : ''}
                        <span class="badge bg-dark"><i class="bi bi-bug me-1"></i>${p.totalBugs} bug(s)</span>
                    </div>
                    ${p.plataformasAlvo ? `<p class="small text-muted mb-0"><i class="bi bi-display me-1"></i>${escHtml(p.plataformasAlvo)}</p>` : ''}
                </div>
                <div class="card-footer d-flex gap-2 flex-wrap">
                    ${currentUser.perfil === 'Admin' ? `
                        <button class="btn btn-sm btn-outline-primary" onclick="editProjeto(${p.id})"><i class="bi bi-pencil"></i> Editar</button>
                        <button class="btn btn-sm btn-outline-danger" onclick="deleteProjeto(${p.id})"><i class="bi bi-trash"></i> Excluir</button>
                    ` : ''}
                    <button class="btn btn-sm btn-outline-secondary" onclick="filtrarBugsPorProjeto(${p.id})"><i class="bi bi-bug"></i> Ver Bugs</button>
                </div>
            </div>
        </div>
    `).join('');
}

function showProjetoModal(projeto = null) {
    document.getElementById('projetoModalTitle').innerHTML = projeto
        ? '<i class="bi bi-pencil me-2"></i>Editar Projeto'
        : '<i class="bi bi-folder me-2"></i>Novo Projeto';
    document.getElementById('projetoId').value = projeto?.id || '';
    document.getElementById('projetoNome').value = projeto?.nome || '';
    document.getElementById('projetoDescricao').value = projeto?.descricao || '';
    document.getElementById('projetoMotor').value = projeto?.motorJogo || '';
    document.getElementById('projetoPlataformas').value = projeto?.plataformasAlvo || '';
    document.getElementById('projetoVersao').value = projeto?.versaoAtual || '';
    new bootstrap.Modal(document.getElementById('projetoModal')).show();
}

async function editProjeto(id) {
    const p = allProjetos.find(x => x.id === id);
    if (p) showProjetoModal(p);
}

async function saveProjeto() {
    const id = document.getElementById('projetoId').value;
    const data = {
        nome: document.getElementById('projetoNome').value,
        descricao: document.getElementById('projetoDescricao').value || null,
        motorJogo: document.getElementById('projetoMotor').value || null,
        plataformasAlvo: document.getElementById('projetoPlataformas').value || null,
        versaoAtual: document.getElementById('projetoVersao').value || null
    };
    if (!data.nome) return showToast('Nome é obrigatório.', 'danger');

    let res;
    if (id) {
        res = await apiFetch(`/api/projetos/${id}`, 'PUT', data);
    } else {
        res = await apiFetch('/api/projetos', 'POST', data);
    }

    if (res.ok) {
        bootstrap.Modal.getInstance(document.getElementById('projetoModal')).hide();
        showToast(id ? 'Projeto atualizado!' : 'Projeto criado!');
        loadProjetos();
        reloadProjetosSelects();
    } else {
        showToast('Erro ao salvar projeto.', 'danger');
    }
}

async function deleteProjeto(id) {
    if (!confirm('Excluir este projeto e todos os seus bugs?')) return;
    const res = await apiFetch(`/api/projetos/${id}`, 'DELETE');
    if (res.ok) {
        showToast('Projeto excluído!');
        loadProjetos();
        loadDashboard();
    } else {
        showToast('Erro ao excluir projeto.', 'danger');
    }
}

function filtrarBugsPorProjeto(projetoId) {
    showSection('bugs', document.getElementById('nav-bugs'));
    document.getElementById('filtroProjeto').value = projetoId;
    renderBugs();
}

async function reloadProjetosSelects() {
    const filtroSelect = document.getElementById('filtroProjeto');
    const bugProjetoSelect = document.getElementById('bugProjeto');
    const selectedFiltro = filtroSelect.value;
    filtroSelect.innerHTML = '<option value="">Todos os Projetos</option>';
    bugProjetoSelect.innerHTML = '';
    await loadProjetosSelect();
    filtroSelect.value = selectedFiltro;
}

// ---- Usuários ----
async function loadUsuarios() {
    const res = await apiFetch('/api/usuarios');
    if (!res.ok) return;
    const usuarios = await res.json();
    const tbody = document.getElementById('usuariosTableBody');
    tbody.innerHTML = usuarios.map(u => {
        const isMe = u.id === currentUser.id;
        const badgeColor = u.perfil === 'Admin' ? 'danger' : 'primary';
        const promoverLabel = u.perfil === 'Admin' ? 'Rebaixar para Dev' : 'Promover a Admin';
        const promoverIcon = u.perfil === 'Admin' ? 'bi-arrow-down-circle' : 'bi-arrow-up-circle';
        const promoverNovoPerfil = u.perfil === 'Admin' ? 'Dev' : 'Admin';
        return `
        <tr>
            <td>${u.id}</td>
            <td>${escHtml(u.nome)}${isMe ? ' <span class="badge bg-secondary">você</span>' : ''}</td>
            <td>${escHtml(u.email)}</td>
            <td><span class="badge bg-${badgeColor}">${u.perfil}</span></td>
            <td><small>${new Date(u.criadoEm).toLocaleDateString('pt-BR')}</small></td>
            <td>
                <div class="d-flex gap-1">
                    ${!isMe ? `<button class="btn-action" title="${promoverLabel}" onclick="togglePerfil(${u.id}, '${promoverNovoPerfil}')">
                        <i class="bi ${promoverIcon}"></i>
                    </button>` : ''}
                    ${!isMe ? `<button class="btn-action danger" title="Excluir" onclick="deleteUsuario(${u.id})">
                        <i class="bi bi-trash"></i>
                    </button>` : ''}
                </div>
            </td>
        </tr>`;
    }).join('');
}

async function deleteUsuario(id) {
    if (!confirm('Excluir este usuário?')) return;
    const res = await apiFetch(`/api/usuarios/${id}`, 'DELETE');
    if (res.ok) {
        showToast('Usuário excluído!');
        loadUsuarios();
    } else {
        try {
            const err = await res.json();
            showToast(err.mensagem || 'Erro ao excluir usuário.', 'danger');
        } catch {
            showToast('Erro ao excluir usuário.', 'danger');
        }
    }
}

async function togglePerfil(id, novoPerfil) {
    const acao = novoPerfil === 'Admin' ? 'promover a Admin' : 'rebaixar para Dev';
    if (!confirm(`Deseja ${acao} este usuário?`)) return;
    const res = await apiFetch(`/api/usuarios/${id}`, 'PUT', { perfil: novoPerfil });
    if (res.ok) {
        showToast(`Usuário atualizado para ${novoPerfil}!`);
        loadUsuarios();
    } else {
        showToast('Erro ao alterar perfil.', 'danger');
    }
}

// ---- Comentários / Detalhes do Bug ----
let currentDetailBugId = null;

async function openBugDetail(id) {
    const b = allBugs.find(x => x.id === id);
    if (!b) return;
    currentDetailBugId = id;

    document.getElementById('detailTitulo').textContent = b.titulo;
    document.getElementById('detailDescricao').textContent = b.descricao || 'Sem descrição.';
    document.getElementById('detailProjeto').textContent = b.projetoNome;
    document.getElementById('detailReportado').textContent = b.reportadoPorNome;
    document.getElementById('detailAtribuido').textContent = b.atribuidoParaNome || '—';
    document.getElementById('detailData').textContent = new Date(b.criadoEm).toLocaleDateString('pt-BR');

    const statusEl = document.getElementById('detailStatus');
    statusEl.textContent = statusLabel(b.status);
    statusEl.className = `badge badge-status-${b.status} fs-6`;

    const sevEl = document.getElementById('detailSeveridade');
    sevEl.textContent = b.severidade === 'Critica' ? 'Crítica' : b.severidade === 'Media' ? 'Média' : b.severidade;
    sevEl.className = `badge badge-severidade-${b.severidade} fs-6`;

    const tipoEl = document.getElementById('detailTipo');
    if (b.tipoBug) { tipoEl.textContent = tipoBugLabel(b.tipoBug); tipoEl.classList.remove('d-none'); }
    else tipoEl.classList.add('d-none');

    const platEl = document.getElementById('detailPlataforma');
    if (b.plataforma) { platEl.textContent = b.plataforma; platEl.classList.remove('d-none'); }
    else platEl.classList.add('d-none');

    const milestoneRow = document.getElementById('detailMilestoneRow');
    if (b.milestone) { document.getElementById('detailMilestone').textContent = b.milestone; milestoneRow.classList.remove('d-none'); }
    else milestoneRow.classList.add('d-none');

    const versaoEl = document.getElementById('detailVersaoJogo');
    versaoEl.textContent = b.versaoJogo ? `v${b.versaoJogo}` : '';
    versaoEl.parentElement.style.display = b.versaoJogo ? '' : 'none';

    const buildRow = document.getElementById('detailBuildRow');
    if (b.numeroBuild) { document.getElementById('detailNumeroBuild').textContent = b.numeroBuild; buildRow.classList.remove('d-none'); }
    else buildRow.classList.add('d-none');

    const bloqueioEl = document.getElementById('detailBloqueio');
    b.bloqueiaLancamento ? bloqueioEl.classList.remove('d-none') : bloqueioEl.classList.add('d-none');

    const cenaRow = document.getElementById('detailCenaRow');
    document.getElementById('detailCena').textContent = b.cena || '—';
    cenaRow.style.display = b.cena ? '' : 'none';

    const freqRow = document.getElementById('detailFreqRow');
    document.getElementById('detailFrequencia').textContent = b.frequenciaReproducao || '—';
    freqRow.style.display = b.frequenciaReproducao ? '' : 'none';

    const resultadosRow = document.getElementById('detailResultadosRow');
    if (b.resultadoEsperado || b.resultadoObtido) {
        document.getElementById('detailResultadoEsperado').textContent = b.resultadoEsperado || '—';
        document.getElementById('detailResultadoObtido').textContent = b.resultadoObtido || '—';
        resultadosRow.classList.remove('d-none');
    } else {
        resultadosRow.classList.add('d-none');
    }

    const ambienteRow = document.getElementById('detailAmbienteRow');
    if (b.detalhesAmbiente) {
        document.getElementById('detailAmbiente').textContent = b.detalhesAmbiente;
        ambienteRow.classList.remove('d-none');
    } else {
        ambienteRow.classList.add('d-none');
    }

    const editBtn = document.querySelector('#bugDetailModal .dev-edit-btn');
    canEditBug(b) ? editBtn.classList.remove('d-none') : editBtn.classList.add('d-none');

    document.getElementById('novoComentario').value = '';
    await loadComentarios(id);
    new bootstrap.Modal(document.getElementById('bugDetailModal')).show();
}

async function loadComentarios(bugId) {
    const res = await apiFetch(`/api/bugs/${bugId}/comentarios`);
    if (!res.ok) return;
    const comentarios = await res.json();
    const lista = document.getElementById('comentariosLista');

    if (comentarios.length === 0) {
        lista.innerHTML = '<p class="text-muted small text-center py-2">Nenhum comentário ainda. Seja o primeiro!</p>';
        return;
    }

    lista.innerHTML = comentarios.map(c => {
        const isMe = c.usuarioId === currentUser.id;
        const isAdmin = c.usuarioPerfil === 'Admin';
        const avatarBg = isAdmin ? '#dc2626' : '#0891b2';
        const bubbleBg = isMe ? 'rgba(6,182,212,0.12)' : '#1c2128';
        const bubbleBorder = isMe ? 'rgba(6,182,212,0.25)' : '#30363d';
        const roleBadge = isAdmin
            ? 'style="background:rgba(239,68,68,0.2);color:#fca5a5;font-size:0.6rem"'
            : 'style="background:rgba(6,182,212,0.2);color:#67e8f9;font-size:0.6rem"';
        return `
        <div class="d-flex gap-2 mb-3 ${isMe ? 'flex-row-reverse' : ''}">
            <div class="rounded-circle text-white d-flex align-items-center justify-content-center flex-shrink-0"
                style="width:34px;height:34px;font-size:0.72rem;font-weight:700;background:${avatarBg}">
                ${escHtml(c.usuarioNome.charAt(0).toUpperCase())}
            </div>
            <div style="max-width:78%">
                <div class="rounded-3 px-3 py-2" style="background:${bubbleBg};border:1px solid ${bubbleBorder}">
                    <div class="d-flex align-items-center gap-2 mb-1">
                        <small class="fw-semibold" style="color:#e6edf3">${escHtml(c.usuarioNome)}</small>
                        <span class="badge rounded-pill" ${roleBadge}>${c.usuarioPerfil}</span>
                        <small class="text-muted text-nowrap ms-auto">${new Date(c.criadoEm).toLocaleString('pt-BR', {day:'2-digit',month:'2-digit',hour:'2-digit',minute:'2-digit'})}</small>
                    </div>
                    <p class="mb-0 small" style="color:#c9d1d9">${escHtml(c.texto)}</p>
                </div>
            </div>
        </div>`;
    }).join('');

    lista.scrollTop = lista.scrollHeight;
}

async function postComentario() {
    const texto = document.getElementById('novoComentario').value.trim();
    if (!texto) return;
    const res = await apiFetch(`/api/bugs/${currentDetailBugId}/comentarios`, 'POST', { texto });
    if (res.ok) {
        document.getElementById('novoComentario').value = '';
        await loadComentarios(currentDetailBugId);
    } else {
        showToast('Erro ao enviar comentário.', 'danger');
    }
}

function editBugFromDetail() {
    bootstrap.Modal.getInstance(document.getElementById('bugDetailModal')).hide();
    editBug(currentDetailBugId);
}

async function irParaBug(id) {
    showSection('bugs', document.getElementById('nav-bugs'));
    if (allBugs.length === 0) await loadBugs();
    openBugDetail(id);
}

// ---- Utils ----
function escHtml(str) {
    if (!str) return '';
    return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

// ---- Start ----
document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('token');
    if (token) {
        initApp();
    } else {
        document.getElementById('loginPage').classList.remove('d-none');
    }
});
