// wwwroot/js/chartInterop.js

// Almacenar instancias de gráficos para poder actualizarlos/destruirlos
let salesChartInstance = null;
let ordersChartInstance = null;
let categoryChartInstance = null;

// Función principal llamada desde Blazor
window.chartInterop = {
    renderAllCharts: (chartJsData) => {
        console.log("Rendering charts via JS Interop with data:", chartJsData); // Para depuración

        // Destruir gráficos anteriores si existen para evitar duplicados
        if (salesChartInstance) salesChartInstance.destroy();
        if (ordersChartInstance) ordersChartInstance.destroy();
        if (categoryChartInstance) categoryChartInstance.destroy();

        // Obtener los contextos 2D de los elementos canvas
        const salesCtx = document.getElementById('salesChartCanvas')?.getContext('2d');
        const ordersCtx = document.getElementById('ordersChartCanvas')?.getContext('2d');
        const categoryCtx = document.getElementById('categoryChartCanvas')?.getContext('2d');

        // Verificar si los canvas existen antes de intentar dibujar
        if (!salesCtx) console.error("Canvas 'salesChartCanvas' not found.");
        if (!ordersCtx) console.error("Canvas 'ordersChartCanvas' not found.");
        if (!categoryCtx) console.error("Canvas 'categoryChartCanvas' not found.");

        // --- Renderizar Gráfico de Ventas (Barra) ---
        if (salesCtx && chartJsData?.salesLabels && chartJsData?.salesData) {
            salesChartInstance = new Chart(salesCtx, {
                type: 'bar',
                data: {
                    labels: chartJsData.salesLabels,
                    datasets: [{
                        label: 'Ventas (₡)',
                        data: chartJsData.salesData,
                        backgroundColor: '#189182',
                        borderColor: '#189182',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: { y: { beginAtZero: true } },
                    // plugins: { legend: { display: false } } // Ocultar leyenda si no se necesita
                }
            });
        }

        // --- Renderizar Gráfico de Órdenes (Línea) ---
        if (ordersCtx && chartJsData?.salesLabels && chartJsData?.ordersData) {
            ordersChartInstance = new Chart(ordersCtx, {
                type: 'line',
                data: {
                    labels: chartJsData.salesLabels, // Usar las mismas etiquetas de meses
                    datasets: [{
                        label: '# Órdenes',
                        data: chartJsData.ordersData,
                        fill: true,
                        borderColor: '#0f54a1',
                        backgroundColor: 'rgba(15, 84, 161, 0.2)',
                        tension: 0.4, // Curvas suaves
                        pointRadius: 3,
                        borderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: { y: { beginAtZero: true } }
                }
            });
        }

        // --- Renderizar Gráfico de Categorías (Pie) ---
        if (categoryCtx && chartJsData?.categoryLabels && chartJsData?.categoryData && chartJsData?.categoryColors) {
            categoryChartInstance = new Chart(categoryCtx, {
                type: 'pie',
                data: {
                    labels: chartJsData.categoryLabels,
                    datasets: [{
                        label: 'Ventas por Categoría',
                        data: chartJsData.categoryData,
                        backgroundColor: chartJsData.categoryColors,
                        borderColor: '#ffffff',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'right', // Posición de la leyenda
                        }
                    }
                }
            });
        }
    },

    // Función opcional para limpiar gráficos si se navega fuera de la página
    destroyCharts: () => {
        if (salesChartInstance) salesChartInstance.destroy();
        if (ordersChartInstance) ordersChartInstance.destroy();
        if (categoryChartInstance) categoryChartInstance.destroy();
        salesChartInstance = null;
        ordersChartInstance = null;
        categoryChartInstance = null;
        console.log("Charts destroyed.");
    }
};