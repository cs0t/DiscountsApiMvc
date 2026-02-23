//multiselect dropdown for categoriyes
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('select[multiple]').forEach(function (el) {
        new TomSelect(el, {
            plugins: ['remove_button'],
            maxItems: null,
            placeholder: 'Select categories...',
            allowEmptyOption: false
        });
    });
});
