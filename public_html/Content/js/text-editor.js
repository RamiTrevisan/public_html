(function ($) {
    function HomeIndex() {
        var $this = this;

        function initialize() {
            $('#postContenido').summernote({
                lang: 'es-ES',
                disableDragAndDrop: true,
                focus: true,
                height: 250,
                toolbar: [
                    ['style', ['bold', 'italic', 'underline', 'clear', 'fontsize', 'forecolor']],
                    ['para', ['ul', 'ol', 'paragraph', 'height']],
                    ['hr', ['hr']],
                    ['picture', ['picture']],
                    ['misc', ['codeview', 'undo', 'redo']]
                ]
            });
        }

        $this.init = function () {
            initialize();
        }
    }
    $(function () {
        var self = new HomeIndex();
        self.init();
    })
}(jQuery))  
