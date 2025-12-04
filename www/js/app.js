// Configuración
const API_URL = 'http://localhost:5044/api'; // Ajustar puerto según launchSettings.json

// Estado
let productosCache = [];

document.addEventListener('DOMContentLoaded', () => {
    cargarProductos();
    cargarClientes();

    // Listener para formulario cliente
    document.getElementById('form-cliente').addEventListener('submit', async (e) => {
        e.preventDefault();
        await registrarCliente();
    });
});

// --- Lógica de UI ---

function mostrarSeccion(idSeccion, elementoNav) {
    // Ocultar todas las secciones
    document.querySelectorAll('.section').forEach(sec => sec.style.display = 'none');
    
    // Mostrar la seleccionada
    document.getElementById(idSeccion).style.display = 'block';

    // Actualizar nav activa
    document.querySelectorAll('.nav-bar__item').forEach(item => item.classList.remove('nav-bar__item--active'));
    elementoNav.classList.add('nav-bar__item--active');
}

// --- Lógica de Productos ---

async function cargarProductos() {
    const contenedor = document.getElementById('grid-productos');
    contenedor.innerHTML = '<p class="loading-text">Cargando...</p>';

    try {
        const respuesta = await fetch(`${API_URL}/Producto`);
        if (!respuesta.ok) throw new Error('Error al conectar con API');
        
        const productos = await respuesta.json();
        productosCache = productos; // Guardar para filtrado local
        renderizarProductos(productos);

    } catch (error) {
        console.error(error);
        contenedor.innerHTML = '<p style="color:red">Error al cargar productos. Verifica que la API esté corriendo.</p>';
    }
}

function renderizarProductos(lista) {
    const contenedor = document.getElementById('grid-productos');
    contenedor.innerHTML = '';

    if(lista.length === 0) {
        contenedor.innerHTML = '<p>No hay productos disponibles.</p>';
        return;
    }

    lista.forEach(prod => {
        // Solo mostrar activos si el backend no lo filtra
        if(prod.activo === false) return; 

        const card = document.createElement('div');
        card.className = 'product-card';
        card.innerHTML = `
            <h3>${prod.descripcion}</h3>
            <span class="price-tag">${prod.precio.toFixed(2)} €</span>
            <small style="color: var(--gris-oscuro)">ID: ${prod.idProducto}</small>
        `;
        contenedor.appendChild(card);
    });
}

function filtrarProductos() {
    const texto = document.getElementById('searchProductos').value.toLowerCase();
    const filtrados = productosCache.filter(p => 
        p.descripcion.toLowerCase().includes(texto)
    );
    renderizarProductos(filtrados);
}

// --- Lógica de Clientes ---

async function cargarClientes() {
    const contenedor = document.getElementById('lista-clientes');
    
    try {
        const respuesta = await fetch(`${API_URL}/Cliente`);
        const clientes = await respuesta.json();

        contenedor.innerHTML = '';
        clientes.forEach(cliente => {
            if(!cliente.activo) return;

            const div = document.createElement('div');
            div.className = 'cliente-item';
            div.innerHTML = `
                <div>
                    <strong>${cliente.nombre} ${cliente.apellidos}</strong><br>
                    <small>${cliente.email}</small>
                </div>
                <button class="btn btn-secondary" style="font-size: 0.8em; padding: 5px 10px;" onclick="eliminarCliente(${cliente.idCliente})">Eliminar</button>
            `;
            contenedor.appendChild(div);
        });

    } catch (error) {
        console.error('Error cargando clientes', error);
    }
}

async function registrarCliente() {
    const nuevoCliente = {
        nombre: document.getElementById('nombre').value,
        apellidos: document.getElementById('apellidos').value,
        email: document.getElementById('email').value,
        password: document.getElementById('password').value,
        telefono: document.getElementById('telefono').value,
        activo: true
    };

    try {
        const respuesta = await fetch(`${API_URL}/Cliente`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(nuevoCliente)
        });

        if (respuesta.ok) {
            alert('Cliente registrado correctamente');
            document.getElementById('form-cliente').reset();
            cargarClientes(); // Recargar lista
        } else {
            alert('Error al registrar cliente');
        }
    } catch (error) {
        console.error(error);
        alert('Error de conexión');
    }
}

async function eliminarCliente(id) {
    if(!confirm('¿Seguro que deseas dar de baja a este cliente?')) return;

    try {
        await fetch(`${API_URL}/Cliente/${id}`, { method: 'DELETE' });
        cargarClientes();
    } catch (error) {
        console.error(error);
    }
}