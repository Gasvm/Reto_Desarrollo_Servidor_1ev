const API_URL = 'http://localhost:5044/api'; 

// --- Estado ---
let cart = [];
let productosGlobal = [];
let tiposIvaGlobal = [];

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
        cargarProductos()
    ]).catch(err => {
        console.error("Error inicializando datos", err);
        showToast('Error al cargar datos iniciales', 'error');
    });

    const fechaEl = document.getElementById('fecha-hoy');
    if(fechaEl) fechaEl.innerText = new Date().toLocaleDateString('es-ES', { 
        weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' 
    });

    setupForm('form-cliente', 'Cliente', cargarClientes);
    setupForm('form-producto', 'Producto', cargarProductos);
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

// --- CRUD Mejorado ---
async function apiCall(endpoint, method = 'GET', body = null) {
    const options = {
        method,
        headers: { 'Content-Type': 'application/json' }
    };
    if (body) options.body = JSON.stringify(body);
    
    const response = await fetch(`${API_URL}/${endpoint}`, options);
    
    // Parsear respuesta JSON si existe
    let data = null;
    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
        data = await response.json();
    }
    
    return { ok: response.ok, status: response.status, data };
}

function setupForm(formId, entity, reloadCallback) {
    const form = document.getElementById(formId);
    if(!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const data = {};
        let id = null;
        let endpoint = entity;
        
        if (entity === 'Cliente') {
            id = document.getElementById('cli-id').value;
            data.idCliente = id ? parseInt(id) : 0;
            data.nombre = document.getElementById('cli-nombre').value.trim();
            data.apellidos = document.getElementById('cli-apellidos').value.trim();
            data.email = document.getElementById('cli-email').value.trim();
            data.telefono = document.getElementById('cli-telefono').value.trim();
            data.password = "123456"; 
            data.fechaCreacion = new Date().toISOString();
            data.activo = true;
            
        } else if (entity === 'Producto') {
            id = document.getElementById('prod-id').value;
            data.idProducto = id ? parseInt(id) : 0;
            data.descripcion = document.getElementById('prod-desc').value.trim();
            data.precio = parseFloat(document.getElementById('prod-precio').value);
            data.idTipoIVA = parseInt(document.getElementById('prod-iva').value);
            data.fechaCreacion = new Date().toISOString();
            data.activo = true;
        }

        // Validaciones básicas
        if (entity === 'Producto' && (!data.descripcion || data.precio <= 0)) {
            showToast('Por favor completa todos los campos correctamente', 'warning');
            return;
        }

        const isUpdate = id && id !== '' && id !== '0';
        const method = isUpdate ? 'PUT' : 'POST';
        
        // Para PUT, agregar ID a la URL
        if (isUpdate) {
            endpoint = `${entity}/${id}`;
        }

        try {
            const result = await apiCall(endpoint, method, data);
            
            if (result.ok) {
                const action = isUpdate ? 'actualizado' : 'creado';
                showToast(`${entity} ${action} correctamente`, 'success');
                window.limpiarForm(formId);
                reloadCallback();
            } else {
                const mensaje = result.data?.mensaje || `Error al guardar ${entity}`;
                showToast(mensaje, 'error');
            }
        } catch(err) { 
            console.error(err);
            showToast(`Error de conexión: ${err.message}`, 'error');
        }
    });
}

window.limpiarForm = function(formId) {
    const form = document.getElementById(formId);
    form.reset();
    const hiddenInput = form.querySelector('input[type=hidden]');
    if (hiddenInput) hiddenInput.value = '';
    
    // Actualizar título del formulario si existe
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

// --- Carga de Datos ---

async function cargarClientes() {
    try {
        const result = await apiCall('Cliente');
        const data = result.data || [];
        const tbody = document.getElementById('tabla-clientes');
        const selectTPV = document.getElementById('modal-cliente');
        
        if(tbody) tbody.innerHTML = '';
        if(selectTPV) selectTPV.innerHTML = '';

        data.forEach(d => {
            if(!d.activo) return;
            
            if(tbody) {
                tbody.innerHTML += `
                    <tr>
                        <td class="data-table__cell">${d.nombre} ${d.apellidos}</td>
                        <td class="data-table__cell">${d.email}</td>
                        <td class="data-table__cell">
                            <button class="btn btn--sm btn--primary" onclick="editarCliente(${d.idCliente})">Editar</button>
                            <button class="btn btn--sm btn--danger" onclick="eliminar('Cliente', ${d.idCliente}, cargarClientes)">Borrar</button>
                        </td>
                    </tr>`;
            }
            if(selectTPV) selectTPV.innerHTML += `<option value="${d.idCliente}">${d.nombre} ${d.apellidos}</option>`;
        });
    } catch(err) {
        console.error(err);
        showToast('Error al cargar clientes', 'error');
    }
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
        
        // Cambiar título
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

async function cargarTiposIVA() {
    try {
        const result = await apiCall('TipoIVA');
        tiposIvaGlobal = result.data || [];
        const select = document.getElementById('prod-iva');
        if(select) {
            select.innerHTML = '';
            tiposIvaGlobal.forEach(t => {
                if(t.activo) select.innerHTML += `<option value="${t.idTipoIVA}">${t.descripcion} (${t.tasa}%)</option>`;
            });
        }
    } catch(err) {
        console.error(err);
        showToast('Error al cargar tipos de IVA', 'error');
    }
}

async function cargarProductos() {
    try {
        const result = await apiCall('Producto');
        productosGlobal = result.data || [];
        
        const tbody = document.getElementById('tabla-productos');
        const gridTPV = document.getElementById('tpv-productos');
        
        if(tbody) tbody.innerHTML = ''; 
        if(gridTPV) gridTPV.innerHTML = '';

        productosGlobal.forEach(d => {
            if(!d.activo) return;
            
            if(tbody) {
                tbody.innerHTML += `
                    <tr>
                        <td class="data-table__cell">${d.descripcion}</td>
                        <td class="data-table__cell">${d.precio.toFixed(2)} €</td>
                        <td class="data-table__cell">
                            <button class="btn btn--sm btn--primary" onclick="editarProducto(${d.idProducto})">Editar</button>
                            <button class="btn btn--sm btn--danger" onclick="eliminar('Producto', ${d.idProducto}, cargarProductos)">Borrar</button>
                        </td>
                    </tr>`;
            }

            if(gridTPV) {
                gridTPV.innerHTML += `
                    <button class="product-card" onclick="addToCart(${d.idProducto})" aria-label="Añadir ${d.descripcion} al carrito">
                        <span class="product-card__title">${d.descripcion}</span>
                        <span class="product-card__price">${d.precio.toFixed(2)} €</span>
                    </button>`;
            }
        });
    } catch(err) {
        console.error(err);
        showToast('Error al cargar productos', 'error');
    }
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
        
        // Cambiar título
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

async function cargarMediosPago() {
    try {
        const result = await apiCall('MedioDePago');
        const data = result.data || [];
        const sel = document.getElementById('modal-medio');
        if(sel) {
            sel.innerHTML = '';
            data.forEach(d => {
                if(d.activo) sel.innerHTML += `<option value="${d.idMedioDePago}">${d.descripcion}</option>`;
            });
        }
    } catch(err) {
        console.error(err);
        showToast('Error al cargar medios de pago', 'error');
    }
}

// --- TPV Lógica ---

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
            const result = await apiCall('TarjetaCredito'); 
            const all = result.data || [];
            const filtered = all.filter(t => t.idCliente == cliId && t.activo);
            
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

    } catch(err) {
        console.error(err);
        showToast('Error procesando la venta', 'error');
    }
}