const API_URL = 'http://localhost:8607/api'; 

// --- Estado Global ---
let cart = [];
let productosGlobal = [];
let tiposIvaGlobal = [];
let clientesGlobal = [];
let mediosPagoGlobal = [];

// --- Sistema de Notificaciones ---
function showToast(message, type = 'success', duration = 3000) {
    const icons = {
        success: '✓',
        error: '✕',
        warning: '⚠'
    };

    const toast = document.createElement('div');
    toast.className = `toast toast--${type}`;
    toast.innerHTML = `
        <span class="toast__icon">${icons[type]}</span>
        <div class="toast__content">
            <div class="toast__title">${type === 'success' ? 'Éxito' : type === 'error' ? 'Error' : 'Aviso'}</div>
            <div>${message}</div>
        </div>
        <button class="toast__close" onclick="this.parentElement.remove()">×</button>
    `;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => toast.remove(), 300);
    }, duration);
}

// --- Inicialización ---
document.addEventListener('DOMContentLoaded', () => {
    checkAuth();
    initApp();
});

function checkAuth() {
    const user = localStorage.getItem('user');
    if (!user) {
        window.location.href = 'login.html';
        return;
    }
    const uObj = JSON.parse(user);
    const display = document.getElementById('userNameDisplay');
    if(display) display.innerText = `${uObj.nombre} ${uObj.apellidos}`;
}

function logout() {
    localStorage.removeItem('user');
    window.location.href = 'login.html';
}

function initApp() {
    Promise.all([
        cargarTiposIVA(),
        cargarClientes(),
        cargarMediosPago(),
        cargarProductos(),
        cargarTarjetas(),
        cargarPedidos()
    ]).catch(err => {
        console.error("Error inicializando datos", err);
        showToast('Error al cargar datos iniciales', 'error');
    });

    const fechaEl = document.getElementById('fecha-hoy');
    if(fechaEl) fechaEl.innerText = new Date().toLocaleDateString('es-ES', { 
        weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' 
    });

    setupForms();
}

// --- Navegación SPA ---
window.navTo = function(secId, linkElement) {
    document.querySelectorAll('.section').forEach(s => {
        s.classList.remove('section--active');
        s.setAttribute('aria-hidden', 'true');
    });
    
    const activeSec = document.getElementById(secId);
    activeSec.classList.add('section--active');
    activeSec.setAttribute('aria-hidden', 'false');
    
    document.querySelectorAll('.sidebar__link').forEach(a => {
        a.classList.remove('sidebar__link--active');
        a.removeAttribute('aria-current');
    });
    if(linkElement) {
        linkElement.classList.add('sidebar__link--active');
        linkElement.setAttribute('aria-current', 'page');
    }
};

// --- Utilidades API ---
async function apiCall(endpoint, method = 'GET', body = null) {
    const options = {
        method,
        headers: { 'Content-Type': 'application/json' }
    };
    if (body) options.body = JSON.stringify(body);
    
    const response = await fetch(`${API_URL}/${endpoint}`, options);
    
    let data = null;
    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
        data = await response.json();
    }
    
    return { ok: response.ok, status: response.status, data };
}

function getOrderParams(selectId) {
    const orderValue = document.getElementById(selectId)?.value || '';
    if (!orderValue) return { campoOrden: '', direccionOrden: 'ASC' };
    
    const [campo, direccion] = orderValue.split('-');
    return { campoOrden: campo, direccionOrden: direccion };
}

// --- Setup de Formularios ---
function setupForms() {
    // Clientes
    setupFormCliente();
    // Productos
    setupFormProducto();
    // Tipos IVA
    setupFormTipoIVA();
    // Medios de Pago
    setupFormMedioPago();
    // Tarjetas
    setupFormTarjeta();
}

function setupFormCliente() {
    const form = document.getElementById('form-cliente');
    if(!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const id = document.getElementById('cli-id').value;
        const data = {
            idCliente: id ? parseInt(id) : 0,
            nombre: document.getElementById('cli-nombre').value.trim(),
            apellidos: document.getElementById('cli-apellidos').value.trim(),
            email: document.getElementById('cli-email').value.trim(),
            telefono: document.getElementById('cli-telefono').value.trim(),
            password: "123456",
            fechaCreacion: new Date().toISOString(),
            activo: true
        };

        const isUpdate = id && id !== '' && id !== '0';
        const method = isUpdate ? 'PUT' : 'POST';
        const endpoint = isUpdate ? `Cliente/${id}` : 'Cliente';

        try {
            const result = await apiCall(endpoint, method, data);
            
            if (result.ok) {
                showToast(`Cliente ${isUpdate ? 'actualizado' : 'creado'} correctamente`, 'success');
                limpiarForm('form-cliente');
                cargarClientes();
            } else {
                const mensaje = result.data?.mensaje || 'Error al guardar cliente';
                showToast(mensaje, 'error');
            }
        } catch(err) { 
            console.error(err);
            showToast('Error de conexión', 'error');
        }
    });
}

function setupFormProducto() {
    const form = document.getElementById('form-producto');
    if(!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const id = document.getElementById('prod-id').value;
        const data = {
            idProducto: id ? parseInt(id) : 0,
            descripcion: document.getElementById('prod-desc').value.trim(),
            precio: parseFloat(document.getElementById('prod-precio').value),
            idTipoIVA: parseInt(document.getElementById('prod-iva').value),
            fechaCreacion: new Date().toISOString(),
            activo: true
        };

        if (!data.descripcion || data.precio <= 0) {
            showToast('Completa todos los campos correctamente', 'warning');
            return;
        }

        const isUpdate = id && id !== '' && id !== '0';
        const method = isUpdate ? 'PUT' : 'POST';
        const endpoint = isUpdate ? `Producto/${id}` : 'Producto';

        try {
            const result = await apiCall(endpoint, method, data);
            
            if (result.ok) {
                showToast(`Producto ${isUpdate ? 'actualizado' : 'creado'} correctamente`, 'success');
                limpiarForm('form-producto');
                cargarProductos();
            } else {
                showToast('Error al guardar producto', 'error');
            }
        } catch(err) { 
            console.error(err);
            showToast('Error de conexión', 'error');
        }
    });
}

function setupFormTipoIVA() {
    const form = document.getElementById('form-tipoiva');
    if(!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const id = document.getElementById('tipoiva-id').value;
        const data = {
            idTipoIVA: id ? parseInt(id) : 0,
            descripcion: document.getElementById('tipoiva-desc').value.trim(),
            tasa: parseFloat(document.getElementById('tipoiva-tasa').value),
            fechaCreacion: new Date().toISOString(),
            activo: true
        };

        const isUpdate = id && id !== '' && id !== '0';
        const method = isUpdate ? 'PUT' : 'POST';
        const endpoint = isUpdate ? `TipoIVA/${id}` : 'TipoIVA';

        try {
            const result = await apiCall(endpoint, method, data);
            
            if (result.ok) {
                showToast(`Tipo IVA ${isUpdate ? 'actualizado' : 'creado'} correctamente`, 'success');
                limpiarForm('form-tipoiva');
                cargarTiposIVA();
            } else {
                showToast('Error al guardar tipo IVA', 'error');
            }
        } catch(err) { 
            console.error(err);
            showToast('Error de conexión', 'error');
        }
    });
}

function setupFormMedioPago() {
    const form = document.getElementById('form-mediopago');
    if(!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const id = document.getElementById('mediopago-id').value;
        const data = {
            idMedioDePago: id ? parseInt(id) : 0,
            descripcion: document.getElementById('mediopago-desc').value.trim(),
            fechaCreacion: new Date().toISOString(),
            activo: true
        };

        const isUpdate = id && id !== '' && id !== '0';
        const method = isUpdate ? 'PUT' : 'POST';
        const endpoint = isUpdate ? `MedioDePago/${id}` : 'MedioDePago';

        try {
            const result = await apiCall(endpoint, method, data);
            
            if (result.ok) {
                showToast(`Medio de pago ${isUpdate ? 'actualizado' : 'creado'} correctamente`, 'success');
                limpiarForm('form-mediopago');
                cargarMediosPago();
            } else {
                showToast('Error al guardar medio de pago', 'error');
            }
        } catch(err) { 
            console.error(err);
            showToast('Error de conexión', 'error');
        }
    });
}

function setupFormTarjeta() {
    const form = document.getElementById('form-tarjeta');
    if(!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const id = document.getElementById('tarjeta-id').value;
        const data = {
            idTarjetaCredito: id ? parseInt(id) : 0,
            idCliente: parseInt(document.getElementById('tarjeta-cliente').value),
            descripcion: document.getElementById('tarjeta-desc').value.trim(),
            numeroTarjeta: document.getElementById('tarjeta-numero').value.trim(),
            fechaCaducidad: document.getElementById('tarjeta-caducidad').value,
            fechaCreacion: new Date().toISOString(),
            activo: true
        };

        const isUpdate = id && id !== '' && id !== '0';
        const method = isUpdate ? 'PUT' : 'POST';
        const endpoint = isUpdate ? `TarjetaCredito/${id}` : 'TarjetaCredito';

        try {
            const result = await apiCall(endpoint, method, data);
            
            if (result.ok) {
                showToast(`Tarjeta ${isUpdate ? 'actualizada' : 'creada'} correctamente`, 'success');
                limpiarForm('form-tarjeta');
                cargarTarjetas();
            } else {
                const mensaje = result.data?.mensaje || 'Error al guardar tarjeta';
                showToast(mensaje, 'error');
            }
        } catch(err) { 
            console.error(err);
            showToast('Error de conexión', 'error');
        }
    });
}

window.limpiarForm = function(formId) {
    const form = document.getElementById(formId);
    form.reset();
    const hiddenInput = form.querySelector('input[type=hidden]');
    if (hiddenInput) hiddenInput.value = '';
    
    const section = form.closest('.section');
    if (section) {
        const title = section.querySelector('.page-header__title');
        if (title && title.dataset.originalText) {
            title.textContent = title.dataset.originalText;
        }
    }
}

window.eliminar = async function(entity, id, callback) {
    if(!confirm('¿Seguro que deseas eliminar este registro?')) return;
    
    try {
        const result = await apiCall(`${entity}/${id}`, 'DELETE');
        if (result.ok) {
            showToast(`${entity} eliminado correctamente`, 'success');
            callback();
        } else {
            showToast('Error al eliminar', 'error');
        }
    } catch(err) {
        console.error(err);
        showToast('Error de conexión', 'error');
    }
}

// ========== CLIENTES ==========
async function cargarClientes() {
    try {
        const orderParams = getOrderParams('order-clientes');
        const url = `Cliente?campoOrden=${orderParams.campoOrden}&direccionOrden=${orderParams.direccionOrden}&filtroEstadoActivo=true`;
        
        const result = await apiCall(url);
        clientesGlobal = result.data || [];
        
        renderClientes(clientesGlobal);
        
        // Llenar select de TPV
        const selectTPV = document.getElementById('modal-cliente');
        if(selectTPV) {
            selectTPV.innerHTML = '';
            clientesGlobal.forEach(c => {
                selectTPV.innerHTML += `<option value="${c.idCliente}">${c.nombre} ${c.apellidos}</option>`;
            });
        }

        // Llenar select de tarjetas
        const selectTarjeta = document.getElementById('tarjeta-cliente');
        if(selectTarjeta) {
            selectTarjeta.innerHTML = '';
            clientesGlobal.forEach(c => {
                selectTarjeta.innerHTML += `<option value="${c.idCliente}">${c.nombre} ${c.apellidos}</option>`;
            });
        }

        // Llenar select de filtro pedidos
        const selectFiltroPedidos = document.getElementById('filter-cliente-pedidos');
        if(selectFiltroPedidos) {
            selectFiltroPedidos.innerHTML = '<option value="">Todos los clientes</option>';
            clientesGlobal.forEach(c => {
                selectFiltroPedidos.innerHTML += `<option value="${c.idCliente}">${c.nombre} ${c.apellidos}</option>`;
            });
        }
    } catch(err) {
        console.error(err);
        showToast('Error al cargar clientes', 'error');
    }
}

function renderClientes(clientes) {
    const tbody = document.getElementById('tabla-clientes');
    if(!tbody) return;
    
    tbody.innerHTML = '';
    clientes.forEach(c => {
        tbody.innerHTML += `
            <tr>
                <td class="data-table__cell">${c.nombre}</td>
                <td class="data-table__cell">${c.apellidos}</td>
                <td class="data-table__cell">${c.email}</td>
                <td class="data-table__cell">${c.telefono || '-'}</td>
                <td class="data-table__cell">
                    <button class="btn btn--sm btn--primary" onclick="editarCliente(${c.idCliente})">Editar</button>
                    <button class="btn btn--sm btn--danger" onclick="eliminar('Cliente', ${c.idCliente}, cargarClientes)">Borrar</button>
                </td>
            </tr>`;
    });
}

window.buscarClientes = function() {
    const searchTerm = document.getElementById('search-clientes').value.toLowerCase();
    const filtered = clientesGlobal.filter(c => 
        c.nombre.toLowerCase().includes(searchTerm) ||
        c.apellidos.toLowerCase().includes(searchTerm) ||
        c.email.toLowerCase().includes(searchTerm)
    );
    renderClientes(filtered);
}

window.editarCliente = async (id) => {
    try {
        const result = await apiCall(`Cliente/${id}`);
        if (!result.ok) {
            showToast('Cliente no encontrado', 'error');
            return;
        }
        
        const d = result.data;
        document.getElementById('cli-id').value = d.idCliente;
        document.getElementById('cli-nombre').value = d.nombre;
        document.getElementById('cli-apellidos').value = d.apellidos;
        document.getElementById('cli-email').value = d.email;
        document.getElementById('cli-telefono').value = d.telefono || '';
        
        const title = document.querySelector('#sec-clientes .page-header__title');
        if (title) {
            if (!title.dataset.originalText) {
                title.dataset.originalText = title.textContent;
            }
            title.textContent = `Editando: ${d.nombre} ${d.apellidos}`;
        }
        
        const tabLink = document.querySelector('a[onclick*="sec-clientes"]');
        if(tabLink) tabLink.click();
        
        showToast('Datos cargados para edición', 'success', 2000);
    } catch(err) {
        console.error(err);
        showToast('Error al cargar cliente', 'error');
    }
};

// ========== PRODUCTOS ==========
async function cargarProductos() {
    try {
        const orderParams = getOrderParams('order-productos');
        const url = `Producto?campoOrden=${orderParams.campoOrden}&direccionOrden=${orderParams.direccionOrden}&filtroEstadoActivo=true`;
        
        const result = await apiCall(url);
        productosGlobal = result.data || [];
        
        renderProductos(productosGlobal);
        renderProductosTPV(productosGlobal);
    } catch(err) {
        console.error(err);
        showToast('Error al cargar productos', 'error');
    }
}

function renderProductos(productos) {
    const tbody = document.getElementById('tabla-productos');
    if(!tbody) return;
    
    tbody.innerHTML = '';
    productos.forEach(p => {
        const tipoIVA = tiposIvaGlobal.find(t => t.idTipoIVA === p.idTipoIVA);
        tbody.innerHTML += `
            <tr>
                <td class="data-table__cell">${p.descripcion}</td>
                <td class="data-table__cell">${p.precio.toFixed(2)} €</td>
                <td class="data-table__cell">${tipoIVA ? tipoIVA.descripcion : '-'}</td>
                <td class="data-table__cell">
                    <button class="btn btn--sm btn--primary" onclick="editarProducto(${p.idProducto})">Editar</button>
                    <button class="btn btn--sm btn--danger" onclick="eliminar('Producto', ${p.idProducto}, cargarProductos)">Borrar</button>
                </td>
            </tr>`;
    });
}

function renderProductosTPV(productos) {
    const gridTPV = document.getElementById('tpv-productos');
    if(!gridTPV) return;
    
    gridTPV.innerHTML = '';
    productos.forEach(p => {
        gridTPV.innerHTML += `
            <button class="product-card" onclick="addToCart(${p.idProducto})" aria-label="Añadir ${p.descripcion} al carrito">
                <span class="product-card__title">${p.descripcion}</span>
                <span class="product-card__price">${p.precio.toFixed(2)} €</span>
            </button>`;
    });
}

window.buscarProductos = function() {
    const searchTerm = document.getElementById('search-productos').value.toLowerCase();
    const filtered = productosGlobal.filter(p => 
        p.descripcion.toLowerCase().includes(searchTerm)
    );
    renderProductos(filtered);
    renderProductosTPV(filtered);
}

window.editarProducto = async (id) => {
    try {
        const result = await apiCall(`Producto/${id}`);
        if (!result.ok) {
            showToast('Producto no encontrado', 'error');
            return;
        }
        
        const d = result.data;
        document.getElementById('prod-id').value = d.idProducto;
        document.getElementById('prod-desc').value = d.descripcion;
        document.getElementById('prod-precio').value = d.precio;
        document.getElementById('prod-iva').value = d.idTipoIVA;
        
        const title = document.querySelector('#sec-productos .page-header__title');
        if (title) {
            if (!title.dataset.originalText) {
                title.dataset.originalText = title.textContent;
            }
            title.textContent = `Editando: ${d.descripcion}`;
        }
        
        const tabLink = document.querySelector('a[onclick*="sec-productos"]');
        if(tabLink) tabLink.click();
        
        showToast('Datos cargados para edición', 'success', 2000);
    } catch(err) {
        console.error(err);
        showToast('Error al cargar producto', 'error');
    }
};

// ========== TIPOS IVA ==========
async function cargarTiposIVA() {
    try {
        const orderParams = getOrderParams('order-tiposiva');
        const url = `TipoIVA?campoOrden=${orderParams.campoOrden}&direccionOrden=${orderParams.direccionOrden}&filtroEstadoActivo=true`;
        
        const result = await apiCall(url);
        tiposIvaGlobal = result.data || [];
        
        renderTiposIVA(tiposIvaGlobal);
        
        // Llenar select de productos
        const select = document.getElementById('prod-iva');
        if(select) {
            select.innerHTML = '';
            tiposIvaGlobal.forEach(t => {
                select.innerHTML += `<option value="${t.idTipoIVA}">${t.descripcion} (${t.tasa}%)</option>`;
            });
        }
    } catch(err) {
        console.error(err);
        showToast('Error al cargar tipos de IVA', 'error');
    }
}

function renderTiposIVA(tipos) {
    const tbody = document.getElementById('tabla-tiposiva');
    if(!tbody) return;
    
    tbody.innerHTML = '';
    tipos.forEach(t => {
        tbody.innerHTML += `
            <tr>
                <td class="data-table__cell">${t.descripcion}</td>
                <td class="data-table__cell">${t.tasa}%</td>
                <td class="data-table__cell">${new Date(t.fechaCreacion).toLocaleDateString('es-ES')}</td>
                <td class="data-table__cell">
                    <button class="btn btn--sm btn--primary" onclick="editarTipoIVA(${t.idTipoIVA})">Editar</button>
                    <button class="btn btn--sm btn--danger" onclick="eliminar('TipoIVA', ${t.idTipoIVA}, cargarTiposIVA)">Borrar</button>
                </td>
            </tr>`;
    });
}

window.buscarTiposIVA = function() {
    const searchTerm = document.getElementById('search-tiposiva').value.toLowerCase();
    const filtered = tiposIvaGlobal.filter(t => 
        t.descripcion.toLowerCase().includes(searchTerm)
    );
    renderTiposIVA(filtered);
}

window.editarTipoIVA = async (id) => {
    try {
        const result = await apiCall(`TipoIVA/${id}`);
        if (!result.ok) {
            showToast('Tipo IVA no encontrado', 'error');
            return;
        }
        
        const d = result.data;
        document.getElementById('tipoiva-id').value = d.idTipoIVA;
        document.getElementById('tipoiva-desc').value = d.descripcion;
        document.getElementById('tipoiva-tasa').value = d.tasa;
        
        const title = document.querySelector('#sec-tiposiva .page-header__title');
        if (title) {
            if (!title.dataset.originalText) {
                title.dataset.originalText = title.textContent;
            }
            title.textContent = `Editando: ${d.descripcion}`;
        }
        
        const tabLink = document.querySelector('a[onclick*="sec-tiposiva"]');
        if(tabLink) tabLink.click();
        
        showToast('Datos cargados para edición', 'success', 2000);
    } catch(err) {
        console.error(err);
        showToast('Error al cargar tipo IVA', 'error');
    }
};

// ========== MEDIOS DE PAGO ==========
async function cargarMediosPago() {
    try {
        const orderParams = getOrderParams('order-mediospago');
        const url = `MedioDePago?campoOrden=${orderParams.campoOrden}&direccionOrden=${orderParams.direccionOrden}&filtroEstadoActivo=true`;
        
        const result = await apiCall(url);
        mediosPagoGlobal = result.data || [];
        
        renderMediosPago(mediosPagoGlobal);
        
        // Llenar select de TPV
        const sel = document.getElementById('modal-medio');
        if(sel) {
            sel.innerHTML = '';
            mediosPagoGlobal.forEach(m => {
                sel.innerHTML += `<option value="${m.idMedioDePago}">${m.descripcion}</option>`;
            });
        }
    } catch(err) {
        console.error(err);
        showToast('Error al cargar medios de pago', 'error');
    }
}

function renderMediosPago(medios) {
    const tbody = document.getElementById('tabla-mediospago');
    if(!tbody) return;
    
    tbody.innerHTML = '';
    medios.forEach(m => {
        tbody.innerHTML += `
            <tr>
                <td class="data-table__cell">${m.descripcion}</td>
                <td class="data-table__cell">${new Date(m.fechaCreacion).toLocaleDateString('es-ES')}</td>
                <td class="data-table__cell">
                    <button class="btn btn--sm btn--primary" onclick="editarMedioPago(${m.idMedioDePago})">Editar</button>
                    <button class="btn btn--sm btn--danger" onclick="eliminar('MedioDePago', ${m.idMedioDePago}, cargarMediosPago)">Borrar</button>
                </td>
            </tr>`;
    });
}

window.buscarMediosPago = function() {
    const searchTerm = document.getElementById('search-mediospago').value.toLowerCase();
    const filtered = mediosPagoGlobal.filter(m => 
        m.descripcion.toLowerCase().includes(searchTerm)
    );
    renderMediosPago(filtered);
}

window.editarMedioPago = async (id) => {
    try {
        const result = await apiCall(`MedioDePago/${id}`);
        if (!result.ok) {
            showToast('Medio de pago no encontrado', 'error');
            return;
        }
        
        const d = result.data;
        document.getElementById('mediopago-id').value = d.idMedioDePago;
        document.getElementById('mediopago-desc').value = d.descripcion;
        
        const title = document.querySelector('#sec-mediospago .page-header__title');
        if (title) {
            if (!title.dataset.originalText) {
                title.dataset.originalText = title.textContent;
            }
            title.textContent = `Editando: ${d.descripcion}`;
        }
        
        const tabLink = document.querySelector('a[onclick*="sec-mediospago"]');
        if(tabLink) tabLink.click();
        
        showToast('Datos cargados para edición', 'success', 2000);
    } catch(err) {
        console.error(err);
        showToast('Error al cargar medio de pago', 'error');
    }
};

// ========== TARJETAS ==========
async function cargarTarjetas() {
    try {
        const orderParams = getOrderParams('order-tarjetas');
        const url = `TarjetaCredito?campoOrden=${orderParams.campoOrden}&direccionOrden=${orderParams.direccionOrden}&filtroEstadoActivo=true`;
        
        const result = await apiCall(url);
        const tarjetas = result.data || [];
        
        renderTarjetas(tarjetas);
    } catch(err) {
        console.error(err);
        showToast('Error al cargar tarjetas', 'error');
    }
}

function renderTarjetas(tarjetas) {
    const tbody = document.getElementById('tabla-tarjetas');
    if(!tbody) return;
    
    tbody.innerHTML = '';
    tarjetas.forEach(t => {
        const cliente = clientesGlobal.find(c => c.idCliente === t.idCliente);
        const nombreCliente = cliente ? `${cliente.nombre} ${cliente.apellidos}` : 'N/A';
        
        tbody.innerHTML += `
            <tr>
                <td class="data-table__cell">${nombreCliente}</td>
                <td class="data-table__cell">${t.descripcion}</td>
                <td class="data-table__cell">****${t.numeroTarjeta.slice(-4)}</td>
                <td class="data-table__cell">${new Date(t.fechaCaducidad).toLocaleDateString('es-ES')}</td>
                <td class="data-table__cell">
                    <button class="btn btn--sm btn--primary" onclick="editarTarjeta(${t.idTarjetaCredito})">Editar</button>
                    <button class="btn btn--sm btn--danger" onclick="eliminar('TarjetaCredito', ${t.idTarjetaCredito}, cargarTarjetas)">Borrar</button>
                </td>
            </tr>`;
    });
}

window.buscarTarjetas = function() {
    cargarTarjetas(); // Recargar con filtros del backend
}

window.editarTarjeta = async (id) => {
    try {
        const result = await apiCall(`TarjetaCredito/${id}`);
        if (!result.ok) {
            showToast('Tarjeta no encontrada', 'error');
            return;
        }
        
        const d = result.data;
        document.getElementById('tarjeta-id').value = d.idTarjetaCredito;
        document.getElementById('tarjeta-cliente').value = d.idCliente;
        document.getElementById('tarjeta-desc').value = d.descripcion;
        document.getElementById('tarjeta-numero').value = d.numeroTarjeta;
        document.getElementById('tarjeta-caducidad').value = d.fechaCaducidad.split('T')[0];
        
        const title = document.querySelector('#sec-tarjetas .page-header__title');
        if (title) {
            if (!title.dataset.originalText) {
                title.dataset.originalText = title.textContent;
            }
            title.textContent = `Editando: ${d.descripcion}`;
        }
        
        const tabLink = document.querySelector('a[onclick*="sec-tarjetas"]');
        if(tabLink) tabLink.click();
        
        showToast('Datos cargados para edición', 'success', 2000);
    } catch(err) {
        console.error(err);
        showToast('Error al cargar tarjeta', 'error');
    }
};

// ========== PEDIDOS ==========
async function cargarPedidos() {
    try {
        const orderParams = getOrderParams('order-pedidos');
        const clienteFilter = document.getElementById('filter-cliente-pedidos')?.value || '';
        
        let url = `PedidoCab?campoOrden=${orderParams.campoOrden}&direccionOrden=${orderParams.direccionOrden}&filtroEstadoActivo=true`;
        if(clienteFilter) url += `&filtroIdCliente=${clienteFilter}`;
        
        const result = await apiCall(url);
        const pedidos = result.data || [];
        
        renderPedidos(pedidos);
    } catch(err) {
        console.error(err);
        showToast('Error al cargar pedidos', 'error');
    }
}

function renderPedidos(pedidos) {
    const tbody = document.getElementById('tabla-pedidos');
    if(!tbody) return;
    
    tbody.innerHTML = '';
    pedidos.forEach(p => {
        const cliente = clientesGlobal.find(c => c.idCliente === p.idCliente);
        const nombreCliente = cliente ? `${cliente.nombre} ${cliente.apellidos}` : 'N/A';
        
        const medio = mediosPagoGlobal.find(m => m.idMedioDePago === p.idMedioPago);
        const nombreMedio = medio ? medio.descripcion : 'N/A';
        
        tbody.innerHTML += `
            <tr>
                <td class="data-table__cell">#${p.idPedido}</td>
                <td class="data-table__cell">${nombreCliente}</td>
                <td class="data-table__cell">${new Date(p.fechaPedido).toLocaleString('es-ES')}</td>
                <td class="data-table__cell">${nombreMedio}</td>
                <td class="data-table__cell">
                    <button class="btn btn--sm" onclick="verDetallePedido(${p.idPedido})">Ver Detalle</button>
                    <button class="btn btn--sm btn--danger" onclick="eliminar('PedidoCab', ${p.idPedido}, cargarPedidos)">Borrar</button>
                </td>
            </tr>`;
    });
}

window.verDetallePedido = async (id) => {
    try {
        const result = await apiCall(`PedidoCab/${id}/completo`);
        
        if (!result.ok) {
            showToast('Error al cargar detalle del pedido', 'error');
            return;
        }
        
        const pedido = result.data;
        
        let detalle = `=== PEDIDO #${pedido.idPedido} ===\n\n`;
        detalle += `Cliente: ${pedido.nombreCliente} ${pedido.apellidosCliente}\n`;
        detalle += `Fecha: ${new Date(pedido.fechaPedido).toLocaleString('es-ES')}\n`;
        detalle += `Medio de Pago: ${pedido.descripcionMedioPago}\n`;
        if (pedido.numeroTarjetaEnmascarado) {
            detalle += `Tarjeta: ${pedido.numeroTarjetaEnmascarado}\n`;
        }
        detalle += `\n--- LÍNEAS ---\n\n`;
        
        pedido.lineas.forEach(l => {
            detalle += `${l.descripcionProducto}\n`;
            detalle += `  ${l.cantidad} x ${l.precioUnitario.toFixed(2)} € = ${l.subtotalLinea.toFixed(2)} €\n`;
            if (l.descuento > 0) {
                detalle += `  Descuento (${l.descuento}%): -${l.descuentoAplicado.toFixed(2)} €\n`;
            }
            detalle += `  IVA (${l.tasaIVA}%): +${l.importeIVA.toFixed(2)} €\n`;
            detalle += `  Total línea: ${l.totalLinea.toFixed(2)} €\n\n`;
        });
        
        detalle += `\n--- TOTALES ---\n`;
        detalle += `Subtotal: ${pedido.subtotalPedido.toFixed(2)} €\n`;
        detalle += `Descuento: -${pedido.descuentoTotal.toFixed(2)} €\n`;
        detalle += `Base Imponible: ${pedido.baseImponibleTotal.toFixed(2)} €\n`;
        detalle += `IVA: +${pedido.ivaTotal.toFixed(2)} €\n`;
        detalle += `TOTAL: ${pedido.totalPedido.toFixed(2)} €\n`;
        
        alert(detalle);
    } catch(err) {
        console.error(err);
        showToast('Error al cargar detalle del pedido', 'error');
    }
}

// ========== TPV ==========
window.addToCart = function(prodId) {
    const prod = productosGlobal.find(p => p.idProducto === prodId);
    if(!prod) return;

    const existing = cart.find(item => item.idProducto === prodId);
    if(existing) {
        existing.cantidad++;
    } else {
        cart.push({ ...prod, cantidad: 1 });
    }
    renderCart();
    showToast(`${prod.descripcion} agregado`, 'success', 1500);
}

function renderCart() {
    const container = document.getElementById('ticket-items');
    if(!container) return;
    
    container.innerHTML = '';
    let subtotal = 0;
    
    if(cart.length === 0) {
        container.innerHTML = '<p style="text-align:center; padding:10px; color: var(--color-text-light);">Carrito vacío</p>';
    }

    cart.forEach((item, index) => {
        const totalLinea = item.precio * item.cantidad;
        subtotal += totalLinea;
        
        container.innerHTML += `
            <div class="ticket__item">
                <div style="flex:1">
                    <strong>${item.descripcion}</strong><br>
                    <small>${item.cantidad} x ${item.precio.toFixed(2)} €</small>
                </div>
                <div>
                    <span style="margin-right:8px;">${totalLinea.toFixed(2)} €</span>
                    <button class="btn btn--sm btn--danger" onclick="removeFromCart(${index})" aria-label="Quitar ${item.descripcion}">×</button>
                </div>
            </div>
        `;
    });

    const ivaEstimado = subtotal * 0.21;
    const total = subtotal;

    document.getElementById('lbl-subtotal').innerText = (total - ivaEstimado).toFixed(2) + ' €';
    document.getElementById('lbl-iva').innerText = ivaEstimado.toFixed(2) + ' €';
    document.getElementById('lbl-total').innerText = total.toFixed(2) + ' €';
    document.getElementById('modal-total').innerText = total.toFixed(2) + ' €';
}

window.removeFromCart = function(index) {
    const item = cart[index];
    cart.splice(index, 1);
    renderCart();
    showToast(`${item.descripcion} eliminado del carrito`, 'success', 1500);
}

window.abrirModalPago = function() {
    if(cart.length === 0) { 
        showToast('El ticket está vacío', 'warning'); 
        return; 
    }
    const modal = document.getElementById('modal-pago');
    modal.classList.add('modal--active');
    modal.setAttribute('aria-hidden', 'false');
    window.checkMedioPago();
}

window.cerrarModal = function() {
    const modal = document.getElementById('modal-pago');
    modal.classList.remove('modal--active');
    modal.setAttribute('aria-hidden', 'true');
}

window.checkMedioPago = async function() {
    const sel = document.getElementById('modal-medio');
    const txt = sel.options[sel.selectedIndex].text.toLowerCase();
    const div = document.getElementById('div-tarjeta');
    const selTarj = document.getElementById('modal-tarjeta');
    
    if(txt.includes('tarjeta')) {
        div.style.display = 'block';
        const cliId = document.getElementById('modal-cliente').value;
        
        try {
            const result = await apiCall(`TarjetaCredito?filtroIdClienteTarjeta=${cliId}&filtroEstadoActivo=true`); 
            const filtered = result.data || [];
            
            selTarj.innerHTML = '';
            if(filtered.length > 0) {
                filtered.forEach(t => selTarj.innerHTML += `<option value="${t.idTarjetaCredito}">${t.descripcion} (***${t.numeroTarjeta.slice(-4)})</option>`);
            } else {
                selTarj.innerHTML = '<option value="">Sin tarjetas registradas</option>';
            }
        } catch(err) {
            console.error(err);
            showToast('Error al cargar tarjetas', 'error');
        }
    } else {
        div.style.display = 'none';
    }
}

window.procesarPedido = async function() {
    const clienteId = parseInt(document.getElementById('modal-cliente').value);
    const medioId = parseInt(document.getElementById('modal-medio').value);
    
    let tarjetaId = null;
    if(document.getElementById('div-tarjeta').style.display !== 'none') {
        const val = document.getElementById('modal-tarjeta').value;
        if(val) tarjetaId = parseInt(val);
    }

    const cabecera = {
        idCliente: clienteId,
        fechaPedido: new Date().toISOString(),
        idMedioPago: medioId,
        idTarjetaCredito: tarjetaId,
        activo: true
    };

    try {
        const resultCab = await apiCall('PedidoCab', 'POST', cabecera);
        
        if (!resultCab.ok) {
            showToast('Error al crear pedido', 'error');
            return;
        }

        const idPedido = resultCab.data.idPedido;

        for(const item of cart) {
            const linea = {
                idPedido: idPedido,
                idProducto: item.idProducto,
                precio: item.precio,
                descuento: 0,
                idTipoIVA: item.idTipoIVA,
                cantidad: item.cantidad,
                totalLinea: item.precio * item.cantidad,
                activo: true
            };
            await apiCall('PedidoLin', 'POST', linea);
        }

        showToast('Venta realizada con éxito', 'success');
        cart = [];
        renderCart();
        cerrarModal();
        cargarPedidos();

    } catch(err) {
        console.error(err);
        showToast('Error procesando la venta', 'error');
    }
}