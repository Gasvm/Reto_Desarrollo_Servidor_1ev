const API_URL = 'http://localhost:5044/api'; 

// --- Estado ---
let cart = [];
let productosGlobal = [];
let tiposIvaGlobal = [];

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
    // Carga de datos
    Promise.all([
        cargarTiposIVA(),
        cargarClientes(),
        cargarMediosPago(),
        cargarProductos()
    ]).catch(err => console.error("Error inicializando datos", err));

    const fechaEl = document.getElementById('fecha-hoy');
    if(fechaEl) fechaEl.innerText = new Date().toLocaleDateString('es-ES', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });

    // Listeners Formularios
    setupForm('form-cliente', 'Cliente', cargarClientes);
    setupForm('form-producto', 'Producto', cargarProductos);
}

// --- Navegación SPA ---
window.navTo = function(secId, linkElement) {
    // Ocultar todas las secciones
    document.querySelectorAll('.section').forEach(s => {
        s.classList.remove('section--active');
        s.setAttribute('aria-hidden', 'true');
    });
    
    // Mostrar seleccionada
    const activeSec = document.getElementById(secId);
    activeSec.classList.add('section--active');
    activeSec.setAttribute('aria-hidden', 'false');
    
    // Actualizar menú (BEM)
    document.querySelectorAll('.sidebar__link').forEach(a => {
        a.classList.remove('sidebar__link--active');
        a.removeAttribute('aria-current');
    });
    if(linkElement) {
        linkElement.classList.add('sidebar__link--active');
        linkElement.setAttribute('aria-current', 'page');
    }
};

// --- CRUD Helpers ---
async function apiCall(endpoint, method = 'GET', body = null) {
    const options = {
        method,
        headers: { 'Content-Type': 'application/json' }
    };
    if (body) options.body = JSON.stringify(body);
    return await fetch(`${API_URL}/${endpoint}`, options);
}

function setupForm(formId, entity, reloadCallback) {
    const form = document.getElementById(formId);
    if(!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = {};
        
        // Mapeo básico (extender según necesidad)
        if (entity === 'Cliente') {
            data.idCliente = document.getElementById('cli-id').value || 0;
            data.nombre = document.getElementById('cli-nombre').value;
            data.apellidos = document.getElementById('cli-apellidos').value;
            data.email = document.getElementById('cli-email').value;
            data.telefono = document.getElementById('cli-telefono').value;
            data.password = "123456"; 
            data.fechaCreacion = new Date().toISOString();
            data.activo = true;
        } else if (entity === 'Producto') {
            data.idProducto = document.getElementById('prod-id').value || 0;
            data.descripcion = document.getElementById('prod-desc').value;
            data.precio = parseFloat(document.getElementById('prod-precio').value);
            data.idTipoIVA = parseInt(document.getElementById('prod-iva').value);
            data.fechaCreacion = new Date().toISOString();
            data.activo = true;
        }

        const id = data[`id${entity}`];
        const method = (id && id != 0) ? 'PUT' : 'POST';
        
        try {
            const res = await apiCall(entity, method, data);
            if(res.ok) {
                alert('Guardado correctamente');
                window.limpiarForm(formId);
                reloadCallback();
            } else {
                alert('Error al guardar. Verifique los datos.');
            }
        } catch(err) { console.error(err); }
    });
}

window.limpiarForm = function(formId) {
    document.getElementById(formId).reset();
    document.getElementById(formId).querySelector('input[type=hidden]').value = '';
}

window.eliminar = async function(entity, id, callback) {
    if(!confirm('¿Seguro que deseas eliminar este registro?')) return;
    await apiCall(`${entity}/${id}`, 'DELETE');
    callback();
}

// --- Carga de Datos ---

async function cargarClientes() {
    const res = await apiCall('Cliente');
    const data = await res.json();
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
}

window.editarCliente = async (id) => {
    const res = await apiCall(`Cliente/${id}`);
    const d = await res.json();
    document.getElementById('cli-id').value = d.idCliente;
    document.getElementById('cli-nombre').value = d.nombre;
    document.getElementById('cli-apellidos').value = d.apellidos;
    document.getElementById('cli-email').value = d.email;
    document.getElementById('cli-telefono').value = d.telefono;
    // Forzar navegación
    const tabLink = document.querySelector('a[onclick*="sec-clientes"]');
    if(tabLink) tabLink.click();
};

async function cargarTiposIVA() {
    const res = await apiCall('TipoIVA');
    tiposIvaGlobal = await res.json();
    const select = document.getElementById('prod-iva');
    if(select) {
        select.innerHTML = '';
        tiposIvaGlobal.forEach(t => {
            if(t.activo) select.innerHTML += `<option value="${t.idTipoIVA}">${t.descripcion} (${t.tasa}%)</option>`;
        });
    }
}

async function cargarProductos() {
    const res = await apiCall('Producto');
    productosGlobal = await res.json();
    
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

        // Card Accesible (button)
        if(gridTPV) {
            gridTPV.innerHTML += `
                <button class="product-card" onclick="addToCart(${d.idProducto})" aria-label="Añadir ${d.descripcion} al carrito">
                    <span class="product-card__title">${d.descripcion}</span>
                    <span class="product-card__price">${d.precio.toFixed(2)} €</span>
                </button>`;
        }
    });
}

window.editarProducto = async (id) => {
    const res = await apiCall(`Producto/${id}`);
    const d = await res.json();
    document.getElementById('prod-id').value = d.idProducto;
    document.getElementById('prod-desc').value = d.descripcion;
    document.getElementById('prod-precio').value = d.precio;
    document.getElementById('prod-iva').value = d.idTipoIVA;
    const tabLink = document.querySelector('a[onclick*="sec-productos"]');
    if(tabLink) tabLink.click();
};

async function cargarMediosPago() {
    const res = await apiCall('MedioDePago');
    const data = await res.json();
    const sel = document.getElementById('modal-medio');
    if(sel) {
        sel.innerHTML = '';
        data.forEach(d => {
            if(d.activo) sel.innerHTML += `<option value="${d.idMedioDePago}">${d.descripcion}</option>`;
        });
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
}

function renderCart() {
    const container = document.getElementById('ticket-items');
    if(!container) return;
    
    container.innerHTML = '';
    let subtotal = 0;
    
    if(cart.length === 0) {
        container.innerHTML = '<p style="text-align:center; padding:10px;">Carrito vacío</p>';
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
                    <button class="btn btn--sm btn--danger" onclick="removeFromCart(${index})" aria-label="Quitar ${item.descripcion}">X</button>
                </div>
            </div>
        `;
    });

    const ivaEstimado = subtotal * 0.21; // Simplificación
    const total = subtotal;

    document.getElementById('lbl-subtotal').innerText = (total - ivaEstimado).toFixed(2) + ' €';
    document.getElementById('lbl-iva').innerText = ivaEstimado.toFixed(2) + ' €';
    document.getElementById('lbl-total').innerText = total.toFixed(2) + ' €';
    document.getElementById('modal-total').innerText = total.toFixed(2) + ' €';
}

window.removeFromCart = function(index) {
    cart.splice(index, 1);
    renderCart();
}

window.abrirModalPago = function() {
    if(cart.length === 0) { alert('El ticket está vacío'); return; }
    const modal = document.getElementById('modal-pago');
    modal.classList.add('modal--active');
    modal.setAttribute('aria-hidden', 'false');
    window.checkMedioPago(); // Init estado tarjetas
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
        // Cargar tarjetas del cliente
        const res = await apiCall('TarjetaCredito'); 
        const all = await res.json();
        // Filtro manual (mejorar con query params en backend)
        const filtered = all.filter(t => t.idCliente == cliId && t.activo);
        
        selTarj.innerHTML = '';
        if(filtered.length > 0) {
            filtered.forEach(t => selTarj.innerHTML += `<option value="${t.idTarjetaCredito}">${t.descripcion} (***${t.numeroTarjeta.slice(-4)})</option>`);
        } else {
            selTarj.innerHTML = '<option value="">Sin tarjetas registradas</option>';
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
        // 1. Cabecera
        await apiCall('PedidoCab', 'POST', cabecera);
        
        // 2. Recuperar ID (Workaround por void return)
        const resPed = await apiCall(`PedidoCab?filtroIdCliente=${clienteId}`);
        const pedidos = await resPed.json();
        const ultimo = pedidos.sort((a,b) => b.idPedido - a.idPedido)[0];

        // 3. Líneas
        for(const item of cart) {
            const linea = {
                idPedido: ultimo.idPedido,
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

        alert('Venta realizada con éxito');
        cart = [];
        renderCart();
        cerrarModal();

    } catch(err) {
        console.error(err);
        alert('Error procesando la venta');
    }
}