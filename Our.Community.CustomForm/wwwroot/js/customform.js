    window.tinymce.init({
        selector: "textarea.rte-editor",
        browser_spellcheck: true,
        contextmenu: false,
        plugins: "link lists anchor codesample code emoticons image",
        toolbar: "undo redo | styleselect | bullist numlist | indent outdent | image link ",
        statusbar: false,
        menubar: false,
        relative_urls: false,
        remove_script_host: false,
        convert_urls: true,
        init_instance_callback: function(editor) {
            editor.on("OpenWindow",
                function(e) {
                    $('[role=tab]:contains("General")').hide();
                });
        }
    });
