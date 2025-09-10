// wwwroot/js/vis-timeline-interop.js
window.visTl = (function () {
    const timelines = {};

    function applyFormatting(options) {
        // Locale a ES por defecto (puedes cambiarlo)
        options.locale = options.locale || 'es';

        // Formato de etiqueta de minutos "00..59"
        if (options.minorFormat === 'mm') {
            const fmt = new Intl.DateTimeFormat(options.locale, { minute: '2-digit' });
            options.format = {
                minorLabels: (date/*, scale, step*/) => fmt.format(date)
            };
            options.showMajorLabels = false; // por si no lo pasas desde C#
        }
    }

    function toDataSets(items, groups) {
        return {
            items: new vis.DataSet(items || []),
            groups: new vis.DataSet(groups || [])
        };
    }

    return {
        render: function (elemId, items, groups, options) {
            const el = document.getElementById(elemId);
            if (!el) return;

            options = options || {};
            applyFormatting(options);

            const ds = toDataSets(items, groups);
            const tl = new vis.Timeline(el, ds.items, ds.groups, options);
            timelines[elemId] = { tl, dsItems: ds.items, dsGroups: ds.groups };
        },
        update: function (elemId, items, groups) {
            const ref = timelines[elemId];
            if (!ref) return;
            ref.dsItems.clear(); ref.dsGroups.clear();
            ref.dsItems.add(items || []); ref.dsGroups.add(groups || []);
        }
    };
})();
