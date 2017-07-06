// Write your Javascript code.
$(document).ready(function() {
    console.log("Site", Site);
    Site.initialize();

});

var Site = (function(){
    
    function initialize() {
        initWYSIWYGEditors();
    }

    function initWYSIWYGEditors() {
        $('.summernote').summernote({
            height: 300,                 // set editor height
            minHeight: null,             // set minimum height of editor
            maxHeight: null,
            callbacks: {
                onInit: onSummerNoteInit,
                onChange: onSummerNoteChange
            }

        });

        function onSummerNoteInit() {
            $(this).summernote('code', $(this).text());
        }

        function onSummerNoteChange(contents, $editable) {
            var $input = $(this).siblings("input[type=hidden]");
            $input.val(contents);
        }
    }

    function uploadImage(inputEl, pageElementID, pageID) {
        var fileList = inputEl.files;
        var imageFile = fileList[0];
        var formData = new FormData();
        formData.append('image', imageFile, imageFile.name);
        formData.append('pageID', pageID);
        formData.append('pageElementID', pageElementID);

        var xhr = new XMLHttpRequest;
        xhr.open('POST', '/api/image', true);
        xhr.send(formData);
        
        xhr.onreadystatechange = function() {
            if (xhr.readyState === 4) {
                var response = JSON.parse(xhr.responseText);
                if (xhr.success !== 0) {
                    imageUploadSuccess(response, inputEl);
                } else {
                    console.log('failed');
                }
            }
        }

        
    }

    function imageUploadSuccess(response, inputEl) {
        console.log("success! of image upload", response);
        inputEl.style.display="none";
        var img = document.createElement('img');
        img.src = response.imagePath;
        img.setAttribute("width", "50");
        img.setAttribute("height", "50");        
        $(img).insertAfter($(inputEl));
    }

    return {
        initialize: initialize,
        uploadImage: uploadImage
    }

})();


