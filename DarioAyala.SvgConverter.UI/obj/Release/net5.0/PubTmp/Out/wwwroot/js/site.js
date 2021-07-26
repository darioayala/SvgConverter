
$("#svgFile").change(function (evt) {
    var file = $("#svgFile").prop("files")[0];
    var canvas = document.getElementById("svgCanvas");
    var ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    $("#alertNotFile").hide();

    var img = new Image();

    img.onload = function () {
        var height = img.height;
        var width = img.width;

        ctx.height = height;
        ctx.width = width;

        var offsetX = 0.5;
        var offsetY = 0.5;
        scaleToFit(img, ctx, canvas);
    };
    img.src = URL.createObjectURL(file, ctx, canvas);

});

function scaleToFit(img, ctx, canvas) {
    var scale = Math.min(canvas.width / img.width, canvas.height / img.height);
    var x = (canvas.width / 2) - (img.width / 2) * scale;
    var y = (canvas.height / 2) - (img.height / 2) * scale;
    ctx.drawImage(img, x, y, img.width * scale, img.height * scale);
}

$("#submitBtn").click(function (evt) {

    evt.preventDefault();

    var fd = new FormData();
    var files = $("#svgFile").prop("files");

    fd.append("AlphaValue", $("#alphaValue").val());
    fd.append("rotateAngle", $("#rotateAngle").val());
    fd.append("rotateCenterX", $("#rotateCenterX").val());
    fd.append("rotateCenterY", $("#rotateCenterY").val());
    fd.append("scaleX", $("#scaleX").val());
    fd.append("scaleY", $("#scaleY").val());
    fd.append("translateX", $("#translateX").val());
    fd.append("translateY", $("#translateY").val());
    fd.append("bboxDiag", $("#bbox").val());
    fd.append("bgcolor", $("#bgColor").val());

    if (files.length > 0) {
        var file = files[0];

        fd.append('File', file);

        $.ajax({
            type: 'post',
            url: '/home/index',
            data: fd,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                console.log(data);
                $("#bmpRes").attr("src", "data:image/bmp;base64," + data.res);
            },
            error: function (data) {
                debugger;
                console.log(data);
            }
        });
    } else {
        $("#alertNotFile").show();
    }
});
