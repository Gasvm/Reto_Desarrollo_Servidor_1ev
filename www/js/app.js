const API_URL = 'http://localhost:5044/api'; // Asegúrate que coincide con tu puerto .NET

// --- Estado Global ---
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
    } else {
        const uObj = JSON.parse(user);
        document.getElementById('userNameDisplay').innerText = `${uObj.nombre} ${uObj.apellidos}`;
    }
}

function logout() {
    localStorage.removeItem('user');
    window.location.href = 'login.html';
}

function initApp() {
    // Cargar datos iniciales
    cargarTiposIVA(); // Necesario para productos
    cargarClientes(); // Necesario para TPV
    cargarMediosPago(); // Necesario para TPV
    cargarProductos(); // Necesario para TPV y CRUD
    cargarTarjetas();

    // Fecha TPV
    document.getElementById('fecha-hoy').innerText = new Date().toLocaleDateString();

    // Listeners Forms
    setupForm('form-cliente', 'Cliente', cargarClientes);
    setupForm('form-producto', 'Producto', cargarProductos);
    setupForm('form-iva', 'TipoIVA', cargarTiposIVA);
    setupForm('form-pago', 'MedioDePago', cargarMediosPago);
    setupForm('form-tarjeta', 'TarjetaCredito', cargarTarjetas);
}

// --- Navegación SPA ---
window.navTo = function(secId) {
    document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
    document.getElementById(secId).classList.add('active');
    
    // Update Sidebar style
    document.querySelectorAll('.nav-links a').forEach(a => a.classList.remove('active'));
    event.currentTarget.classList.add('active');
};

// --- CRUD GENÉRICO Helpers ---
async function apiCall(endpoint, method = 'GET', body = null) {
    const options = {
        method,
        headers: { 'Content-Type': 'application/json' }
    };
    if (body) options.body = JSON.stringify(body);
    const res = await fetch(`${API_URL}/${endpoint}`, options);
    return res; // Devuelve response raw para manejar status
}

function setupForm(formId, entity, reloadCallback) {
    document.getElementById(formId).addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = {};
        
        // Mapeo manual según la entidad para asegurar tipos correctos
        if (entity === 'Cliente') {
            data.idCliente = document.getElementById('cli-id').value || 0;
            data.nombre = document.getElementById('cli-nombre').value;
            data.apellidos = document.getElementById('cli-apellidos').value;
            data.email = document.getElementById('cli-email').value;
            data.telefono = document.getElementById('cli-telefono').value;
            data.password = document.getElementById('cli-pass').value || "123456"; // Default si no se cambia
            data.fechaCreacion = new Date().toISOString();
            data.activo = true;
        } else if (entity === 'Producto') {
            data.idProducto = document.getElementById('prod-id').value || 0;
            data.descripcion = document.getElementById('prod-desc').value;
            data.precio = parseFloat(document.getElementById('prod-precio').value);
            data.idTipoIVA = parseInt(document.getElementById('prod-iva').value);
            data.fechaCreacion = new Date().toISOString();
            data.activo = true;
        } else if (entity === 'TipoIVA') {
            data.idTipoIVA = document.getElementById('iva-id').value || 0;
            data.descripcion = document.getElementById('iva-desc').value;
            data.tasa = parseFloat(document.getElementById('iva-tasa').value);
            data.fechaCreacion = new Date().toISOString();
            data.activo = true;
        } else if (entity === 'MedioDePago') {
            data.idMedioDePago = document.getElementById('pago-id').value || 0;
            data.descripcion = document.getElementById('pago-desc').value;
            data.fechaCreacion = new Date().toISOString();
            data.activo = true;
        } else if (entity === 'TarjetaCredito') {
            data.idTarjetaCredito = document.getElementById('tarj-id').value || 0;
            data.descripcion = document.getElementById('tarj-desc').value;
            data.numeroTarjeta = document.getElementById('tarj-num').value;
            data.fechaCaducidad = document.getElementById('tarj-cad').value;
            data.idCliente = parseInt(document.getElementById('tarj-cli').value);
            data.fechaCreacion = new Date().toISOString();
            data.activo = true;
        }

        // Determinar si es POST o PUT
        const id = data[`id${entity}`]; // ej: data.idCliente
        const method = (id && id != 0) ? 'PUT' : 'POST';
        
        // El backend espera el objeto completo en el body
        try {
            const res = await apiCall(entity, method, data);
            if(res.ok) {
                alert('Guardado correctamente');
                window[`limpiarForm`](formId);
                reloadCallback();
            } else {
                alert('Error al guardar');
            }
        } catch(err) { console.error(err); }
    });
}

window.limpiarForm = function(formId) {
    document.getElementById(formId).reset();
    // Limpiar campos hidden de ID
    document.getElementById(formId).querySelector('input[type=hidden]').value = '';
}

window.eliminar = async function(entity, id, callback) {
    if(!confirm('¿Seguro que deseas eliminar?')) return;
    await apiCall(`${entity}/${id}`, 'DELETE');
    callback();
}

// --- LOGICA ESPECIFICA DE ENTIDADES ---

// 1. Clientes
async function cargarClientes() {
    const res = await apiCall('Cliente');
    const data = await res.json();
    const tbody = document.getElementById('tabla-clientes');
    tbody.innerHTML = '';
    
    // Llenar selects de TPV y Tarjetas
    const selectTPV = document.getElementById('modal-cliente');
    const selectTarj = document.getElementById('tarj-cli');
    selectTPV.innerHTML = ''; selectTarj.innerHTML = '';

    data.forEach(d => {
        if(!d.activo) return;
        
        // Tabla CRUD
        tbody.innerHTML += `
            <tr>
                <td>${d.idCliente}</td>
                <td>${d.nombre} ${d.apellidos}</td>
                <td>${d.email}</td>
                <td>${d.telefono}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editarCliente(${d.idCliente})">Edit</button>
                    <button class="btn btn-sm btn-danger" onclick="eliminar('Cliente', ${d.idCliente}, cargarClientes)">Del</button>
                </td>
            </tr>`;
        
        // Selects
        const opt = `<option value="${d.idCliente}">${d.nombre} ${d.apellidos}</option>`;
        selectTPV.innerHTML += opt;
        selectTarj.innerHTML += opt;
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
    document.getElementById('cli-pass').value = d.password;
    window.navTo('sec-clientes');
};

// 2. Tipos IVA (Cargar primero para Productos)
async function cargarTiposIVA() {
    const res = await apiCall('TipoIVA');
    const data = await res.json();
    tiposIvaGlobal = data; // Cache para cálculos TPV
    
    const tbody = document.getElementById('tabla-ivas');
    const selectProd = document.getElementById('prod-iva');
    tbody.innerHTML = ''; selectProd.innerHTML = '';

    data.forEach(d => {
        if(!d.activo) return;
        tbody.innerHTML += `
            <tr>
                <td>${d.idTipoIVA}</td>
                <td>${d.descripcion}</td>
                <td>${d.tasa}%</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editarIVA(${d.idTipoIVA})">Edit</button>
                    <button class="btn btn-sm btn-danger" onclick="eliminar('TipoIVA', ${d.idTipoIVA}, cargarTiposIVA)">Del</button>
                </td>
            </tr>`;
        selectProd.innerHTML += `<option value="${d.idTipoIVA}">${d.descripcion} (${d.tasa}%)</option>`;
    });
}
window.editarIVA = async (id) => {
    const res = await apiCall(`TipoIVA/${id}`);
    const d = await res.json();
    document.getElementById('iva-id').value = d.idTipoIVA;
    document.getElementById('iva-desc').value = d.descripcion;
    document.getElementById('iva-tasa').value = d.tasa;
};


// 3. Productos
async function cargarProductos() {
    const res = await apiCall('Producto');
    const data = await res.json();
    productosGlobal = data; // Cache TPV
    
    const tbody = document.getElementById('tabla-productos');
    const gridTPV = document.getElementById('tpv-productos');
    tbody.innerHTML = ''; gridTPV.innerHTML = '';

    data.forEach(d => {
        if(!d.activo) return;
        
        // CRUD Row
        const iva = tiposIvaGlobal.find(i => i.idTipoIVA === d.idTipoIVA);
        const ivaDesc = iva ? `${iva.tasa}%` : '?';
        tbody.innerHTML += `
            <tr>
                <td>${d.idProducto}</td>
                <td>${d.descripcion}</td>
                <td>${d.precio.toFixed(2)}</td>
                <td>${ivaDesc}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editarProducto(${d.idProducto})">Edit</button>
                    <button class="btn btn-sm btn-danger" onclick="eliminar('Producto', ${d.idProducto}, cargarProductos)">Del</button>
                </td>
            </tr>`;

        // TPV Card
        gridTPV.innerHTML += `
            <div class="tpv-card" onclick="addToCart(${d.idProducto})">
                <h4>${d.descripcion}</h4>
                <span class="price">${d.precio.toFixed(2)} €</span>
            </div>`;
    });
}
window.editarProducto = async (id) => {
    const res = await apiCall(`Producto/${id}`);
    const d = await res.json();
    document.getElementById('prod-id').value = d.idProducto;
    document.getElementById('prod-desc').value = d.descripcion;
    document.getElementById('prod-precio').value = d.precio;
    document.getElementById('prod-iva').value = d.idTipoIVA;
    window.navTo('sec-productos');
};

// 4. Medios de Pago
async function cargarMediosPago() {
    const res = await apiCall('MedioDePago');
    const data = await res.json();
    
    const tbody = document.getElementById('tabla-pagos');
    const selectModal = document.getElementById('modal-medio');
    tbody.innerHTML = ''; selectModal.innerHTML = '';

    data.forEach(d => {
        if(!d.activo) return;
        tbody.innerHTML += `<tr><td>${d.idMedioDePago}</td><td>${d.descripcion}</td><td>...</td></tr>`;
        selectModal.innerHTML += `<option value="${d.idMedioDePago}">${d.descripcion}</option>`;
    });
}

// 5. Tarjetas
async function cargarTarjetas() {
    const res = await apiCall('TarjetaCredito');
    const data = await res.json();
    const tbody = document.getElementById('tabla-tarjetas');
    tbody.innerHTML = '';
    
    data.forEach(d => {
        if(!d.activo) return;
        tbody.innerHTML += `<tr><td>${d.idTarjetaCredito}</td><td>${d.descripcion}</td><td>${d.numeroTarjeta}</td><td>${d.idCliente}</td><td>...</td></tr>`;
    });
}

// --- LÓGICA TPV ---

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
    container.innerHTML = '';
    
    let subtotal = 0;
    
    cart.forEach((item, index) => {
        const totalLinea = item.precio * item.cantidad;
        subtotal += totalLinea;
        
        container.innerHTML += `
            <div class="ticket-item">
                <div style="flex:1">
                    <strong>${item.descripcion}</strong><br>
                    <small>${item.cantidad} x ${item.precio.toFixed(2)}</small>
                </div>
                <div>
                    <span>${totalLinea.toFixed(2)} €</span>
                    <button class="btn btn-sm btn-danger" style="margin-left:5px; padding:2px 6px;" onclick="removeFromCart(${index})">X</button>
                </div>
            </div>
        `;
    });

    // Cálculos simples (En real, usar tasa IVA de cada producto)
    // Aquí asumimos IVA incluido o cálculo general para visualización
    const total = subtotal; 
    
    document.getElementById('lbl-subtotal').innerText = (total * 0.79).toFixed(2) + ' €'; // Aprox base
    document.getElementById('lbl-iva').innerText = (total * 0.21).toFixed(2) + ' €'; // Aprox 21%
    document.getElementById('lbl-total').innerText = total.toFixed(2) + ' €';
    document.getElementById('modal-total').innerText = total.toFixed(2) + ' €';
}

window.removeFromCart = function(index) {
    cart.splice(index, 1);
    renderCart();
}

window.abrirModalPago = function() {
    if(cart.length === 0) { alert('El ticket está vacío'); return; }
    document.getElementById('modal-pago').classList.add('active');
    checkMedioPago();
}

window.checkMedioPago = function() {
    // Si selecciona tarjeta, mostrar desplegable de tarjetas del cliente
    // Para simplificar, mostramos si la descripción contiene "Tarjeta"
    const sel = document.getElementById('modal-medio');
    const txt = sel.options[sel.selectedIndex].text.toLowerCase();
    const div = document.getElementById('div-tarjeta');
    
    if(txt.includes('tarjeta')) {
        div.style.display = 'block';
        // Cargar tarjetas del cliente seleccionado
        cargarTarjetasClienteModal();
    } else {
        div.style.display = 'none';
    }
}

async function cargarTarjetasClienteModal() {
    const cliId = parseInt(document.getElementById('modal-cliente').value);
    const res = await apiCall('TarjetaCredito'); // Idealmente filtrar por QueryParams
    const all = await res.json();
    const filtered = all.filter(t => t.idCliente === cliId && t.activo);
    
    const sel = document.getElementById('modal-tarjeta');
    sel.innerHTML = '';
    filtered.forEach(t => {
        sel.innerHTML += `<option value="${t.idTarjetaCredito}">${t.descripcion} - ${t.numeroTarjeta}</option>`;
    });
}

window.procesarPedido = async function() {
    const clienteId = parseInt(document.getElementById('modal-cliente').value);
    const medioId = parseInt(document.getElementById('modal-medio').value);
    const tarjetaId = document.getElementById('div-tarjeta').style.display === 'block' 
                      ? parseInt(document.getElementById('modal-tarjeta').value) 
                      : null;

    if(!clienteId || !medioId) { alert('Faltan datos'); return; }

    // 1. Crear Cabecera
    const cabecera = {
        idCliente: clienteId,
        fechaPedido: new Date().toISOString(),
        idMedioPago: medioId,
        idTarjetaCredito: tarjetaId,
        activo: true
    };

    try {
        // En tu backend, PedidoCab no devuelve el ID en el POST (void), 
        // así que no podemos insertar las líneas linkeadas correctamente sin modificar el backend
        // para que devuelva el objeto creado.
        // **Workaround para este ejemplo**: Insertamos la cabecera, y luego buscamos el último pedido de este cliente.
        
        await apiCall('PedidoCab', 'POST', cabecera);
        
        // Obtener ID (chapuza necesaria por limitación API void)
        const resPedidos = await apiCall(`PedidoCab?filtroIdCliente=${clienteId}`);
        const pedidos = await resPedidos.json();
        // Asumimos el último pedido es el nuestro
        const ultimoPedido = pedidos.sort((a,b) => b.idPedido - a.idPedido)[0];
        
        if(!ultimoPedido) throw new Error("No se pudo recuperar el pedido");

        // 2. Crear Líneas
        for (const item of cart) {
            const linea = {
                idPedido: ultimoPedido.idPedido,
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

        alert(`¡Pedido ${ultimoPedido.idPedido} creado con éxito!`);
        cart = [];
        renderCart();
        document.getElementById('modal-pago').classList.remove('active');

    } catch(err) {
        console.error(err);
        alert('Error al procesar el pedido. Revisa la consola.');
    }
};