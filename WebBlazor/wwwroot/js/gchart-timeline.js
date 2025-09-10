// wwwroot/js/gchart-timeline.js
window.gchartTimeline = (function () {
    const NS = "http://www.w3.org/2000/svg";

    function toDate(ms) { return new Date(ms); }

    function applyOptions(opts) {
        const o = opts || {};
        if (o.hAxis) {
            if (o.hAxis.minMs) o.hAxis.minValue = toDate(o.hAxis.minMs);
            if (o.hAxis.maxMs) o.hAxis.maxValue = toDate(o.hAxis.maxMs);
            if (Array.isArray(o.hAxis.ticksMs)) o.hAxis.ticks = o.hAxis.ticksMs.map(toDate);
            delete o.hAxis.minMs; delete o.hAxis.maxMs; delete o.hAxis.ticksMs;
        }
        o.tooltip = o.tooltip || {};
        // usamos tooltips HTML para poder contrastarlos en dark
        o.tooltip.isHtml = true;
        return o;
    }

    function zebraBackground(el, lanes) {
        const h = el.clientHeight || 1;
        const rowH = h / Math.max(1, lanes);
        el.style.background = `repeating-linear-gradient(
      180deg,
      rgba(255,255,255,0.06) 0,
      rgba(255,255,255,0.06) ${rowH}px,
      rgba(0,0,0,0) ${rowH}px,
      rgba(0,0,0,0) ${rowH * 2}px)`;
    }

    function polishSvg(el, lanes) {
        const svg = el.querySelector("svg");
        if (!svg) return;

        // 1) Quita “paneles” blancos y líneas muy marcadas
        svg.querySelectorAll("rect").forEach(r => {
            const fill = r.getAttribute("fill");
            const h = +r.getAttribute("height") || 0;
            const stroke = r.getAttribute("stroke");
            // Grandes rects de fila / fondo
            if ((fill === "#ffffff" || fill === "#f5f5f5" || h > 20) && stroke) {
                r.setAttribute("fill", "transparent");
                r.setAttribute("stroke", "#3A3530");       // tu palette dark (TableLines)
                r.setAttribute("stroke-opacity", "0.35");
            }
            // Barras: sin borde negro
            if (fill && fill !== "none" && !stroke && h >= 6) {
                r.setAttribute("stroke", "none");
                r.setAttribute("rx", "6"); // esquinas redondeadas
            }
        });

        // 2) Color de etiquetas del eje (minutos)
        svg.querySelectorAll("text").forEach(t => {
            const s = (t.textContent || "").trim();
            if (/^\d{2}$/.test(s) || s === "00") {
                t.setAttribute("fill", "#B9B3AA"); // TextSecondary dark
                t.setAttribute("font-size", "10");
            }
        });

        // 3) Zebra de fondo (detrás del SVG)
        zebraBackground(el, lanes);
    }

    // Overlay con etiquetas 00..59 para forzar cada minuto (opcional)
    function drawMinuteOverlay(el, minX, maxX, baseY, lanes) {
        let overlay = el.querySelector(".minute-overlay");
        if (!overlay) {
            overlay = document.createElement("div");
            overlay.className = "minute-overlay";
            overlay.style.position = "absolute";
            overlay.style.left = `${minX}px`;
            overlay.style.top = `${baseY - 16}px`;
            overlay.style.width = `${maxX - minX}px`;
            overlay.style.height = "16px";
            overlay.style.display = "grid";
            overlay.style.gridTemplateColumns = "repeat(60, 1fr)";
            overlay.style.pointerEvents = "none";
            overlay.style.color = "#B9B3AA";
            overlay.style.font = "10px Inter, system-ui, -apple-system, Helvetica, Arial";
            el.style.position = "relative";
            el.appendChild(overlay);
        } else {
            overlay.innerHTML = "";
            overlay.style.left = `${minX}px`;
            overlay.style.top = `${baseY - 16}px`;
            overlay.style.width = `${maxX - minX}px`;
        }

        for (let m = 0; m < 60; m++) {
            const span = document.createElement("span");
            span.style.textAlign = "center";
            span.textContent = String(m).padStart(2, "0");
            overlay.appendChild(span);
        }
    }

    // Intenta deducir el área de plot (x izquierdo/derecho) para el overlay
    function getPlotBounds(svg) {
        let minX = Infinity, maxX = -Infinity, axisY = 0;
        svg.querySelectorAll("path").forEach(p => {
            const d = p.getAttribute("d") || "";
            // buscamos líneas verticales del grid: "M x,y L x,y2"
            const m = d.match(/M\s*([\d.]+),\s*([\d.]+)\s*L\s*([\d.]+),\s*([\d.]+)/);
            if (m) {
                const x1 = parseFloat(m[1]), y1 = parseFloat(m[2]);
                const x2 = parseFloat(m[3]), y2 = parseFloat(m[4]);
                if (Math.abs(x1 - x2) < 0.5) {
                    minX = Math.min(minX, x1);
                    maxX = Math.max(maxX, x1);
                } else {
                    axisY = Math.max(axisY, y1, y2);
                }
            }
        });
        if (!isFinite(minX) || !isFinite(maxX)) {
            // fallback: usa el viewBox
            const vb = svg.viewBox.baseVal;
            minX = vb.x + 140; // deja margen aproximado para labels
            maxX = vb.x + vb.width - 20;
            axisY = vb.y + vb.height - 8;
        }
        return { minX, maxX, axisY };
    }

    function draw(elemId, rows, opts) {
        google.charts.load("current", { packages: ["timeline"] });
        google.charts.setOnLoadCallback(function () {
            const el = document.getElementById(elemId);
            if (!el) return;

            const lanes = [...new Set(rows.map(r => r.lane))].length;

            const chart = new google.visualization.Timeline(el);
            const dataTable = new google.visualization.DataTable();
            dataTable.addColumn({ type: "string", id: "Lane" });
            dataTable.addColumn({ type: "string", id: "Label" });      // oculto
            dataTable.addColumn({ type: "string", role: "tooltip" });  // tooltip (HTML)
            dataTable.addColumn({ type: "string", role: "style" });    // color
            dataTable.addColumn({ type: "date", id: "Start" });
            dataTable.addColumn({ type: "date", id: "End" });

            const rowsArr = rows.map(x => [
                x.lane, "",
                x.tooltip || "",              // HTML inline (dark)
                x.style || null,              // color
                toDate(x.startMs), toDate(x.endMs)
            ]);
            dataTable.addRows(rowsArr);

            const options = applyOptions(opts);
            chart.draw(dataTable, options);

            // Tunear look tras render
            requestAnimationFrame(() => {
                const svg = el.querySelector("svg");
                if (!svg) return;
                const { minX, maxX, axisY } = getPlotBounds(svg);
                polishSvg(el, lanes);

                // Overlay de minutos 00..59 si así se pide
                if (opts?.minuteOverlay === true) {
                    drawMinuteOverlay(el, minX, maxX, axisY, lanes);
                }
            });

            // Redibuja en resize para ocupar todo el ancho
            const ro = new ResizeObserver(() => chart.draw(dataTable, options));
            ro.observe(el);
        });
    }

    return { draw };
})();
