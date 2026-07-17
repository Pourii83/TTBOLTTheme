(function () {
    'use strict';

    const input = document.querySelector('[data-profile-picture-input]');
    const dropzone = document.querySelector('[data-profile-picture-dropzone]');
    const preview = document.querySelector('[data-profile-picture-preview]');

    if (!input || !dropzone || !preview) {
        return;
    }

    function showPreview(file) {
        if (!file || !file.type.startsWith('image/')) {
            return;
        }

        const reader = new FileReader();
        reader.addEventListener('load', function () {
            preview.replaceChildren();
            const image = document.createElement('img');
            image.src = reader.result;
            image.alt = '';
            preview.appendChild(image);
        });
        reader.readAsDataURL(file);
    }

    input.addEventListener('change', function () {
        showPreview(input.files && input.files[0]);
    });

    ['dragenter', 'dragover'].forEach(function (eventName) {
        dropzone.addEventListener(eventName, function (event) {
            event.preventDefault();
            dropzone.classList.add('is-dragging');
        });
    });

    ['dragleave', 'drop'].forEach(function (eventName) {
        dropzone.addEventListener(eventName, function (event) {
            event.preventDefault();
            dropzone.classList.remove('is-dragging');
        });
    });

    dropzone.addEventListener('drop', function (event) {
        const files = event.dataTransfer && event.dataTransfer.files;
        if (!files || files.length === 0) {
            return;
        }

        input.files = files;
        showPreview(files[0]);
    });
})();
