$(document).ready(function () {

    $(".chosen").chosen();

    $(".Eliminar").click(function () {
        return confirm('¿Está seguro de que desea eliminar este registro?');
    });

    $(".show_hide").click(function () {
        var div = "#" + $(this).attr('divtarget');
        $(div).slideToggle();
        $(this).html($(this).text() == "+" ? '-' : '+');
        return false;
    });

    $(".show_hide_check").click(function () {
        var div = "#" + $(this).attr('divtarget');
        $(div).slideToggle();
    });

    if ($('#divedicionregistroclasificacion').length > 0) {
        $(".rbencuesta").each(function () {
            if ($(this).is(':checked')) {
                validarRadioButtons($(this));
            }
        });

        $(".cbencuesta").each(function () {
            validarCheckBox($(this));
        });
    }

    $(".rbencuesta").change(function () {
        var tclass = "." + $(this).attr('targetclass');
        $(tclass + ' :input').val('');
        $(tclass + ' :input').empty();
        validarRadioButtons($(this));
    });

    $(".cbencuesta").change(function () {
        var div = "#" + $(this).attr('target');
        $(div + ' :input').val('');
        $(div + ' :input').empty();
        validarCheckBox($(this));
    });

    $(".bencuesta").click(function (event) {
        var bandfocus = "";
        $("div[id^='divpreg_']").each(function () {
            var div = "";
            var band = false;

            if ($(this).attr('obligatoriochecked') == "True" && $(this).is(':visible')) {
                div = "#" + $(this).attr('id');
                band = ($(div + ' input:checked').length > 0);
            }

            else if ($(this).attr('obligatoriotext') == "True" && $(this).is(':visible')) {
                div = "#" + $(this).attr('id');
                $(div + ' input').each(function () {
                    band = ($(this).val().length > 0);
                    if (band)
                        return false;
                });
            }

            if (!band) {
                $(div + "_mensaje").text("Esta pregunta es obligatoria.");
                if (bandfocus == "") {
                    bandfocus = div;
                }
            }
        });

        if (bandfocus != "") {
            $('html, body').animate({ scrollTop: $(bandfocus).offset().top }, 'slow');
            event.preventDefault();
        }
    });

    function validarRadioButtons(radioButton) {
        var tdiv = "#" + radioButton.attr('targetdiv');
        var tclass = "." + radioButton.attr('targetclass');
        var parentdiv = radioButton.attr('parentdiv');
        var idopc = radioButton.attr('idopc');
        var pregocultar = radioButton.attr('pregocultar');
        var tpreg = $(parentdiv).attr('tpreg');
        var idsopc = $(parentdiv).attr('idsopc');

        $(tclass).hide();
        $(tdiv).show();

        $(parentdiv + "_mensaje").text("");

        if (tpreg) {
            var temppreg = tpreg.split("|");
            var tempopc = idsopc.split("|");
            for (var j = 0; j < temppreg.length; j++) {
                var banderaspreg = $("#divpreg_" + temppreg[j]).attr('band');
                if (banderaspreg && tempopc) {
                    for (var k = 0; k < tempopc.length; k++) {
                        banderaspreg = banderaspreg.replace("|xx", "");
                        banderaspreg = banderaspreg.replace("|" + tempopc[k], "");
                    }
                    $("#divpreg_" + temppreg[j]).attr('band', banderaspreg);
                }
            }
        }

        if (pregocultar) {
            var temp = pregocultar.split("|");
            for (var i = 0; i < temp.length; i++) {
                var banderas = $("#divpreg_" + temp[i]).attr('band');
                if (banderas) {
                    banderas += "|" + idopc;
                } else {
                    banderas = "|" + idopc;
                }
                $("#divpreg_" + temp[i]).attr('band', banderas);
            }
        }

        $("div[id^='divpreg_']").each(function () {
            $(this).show();
            if ($(this).attr('band')) {
                $(this).hide();
            }
        });
    }

    function validarCheckBox(checkbox) {
        var div = "#" + checkbox.attr('target');
        var parentdiv = checkbox.attr('parentdiv');
        $(parentdiv + "_mensaje").text("");
        if (checkbox.is(':checked')) {
            $(div).show();
        } else {
            $(div).hide();
        }
    }

    //$(function () {
    //    if ($('#FormEncuesta').length > 0) {
    //        var validationSettings = $.data($('#FormEncuesta').get(0), 'validator').settings;
    //        validationSettings.ignore = ":not(:visible)";
    //    }
    //});


    $("#TextoRespuesta").htmlarea({
        toolbar: [
            ["bold", "italic", "underline", "strikethrough", "|", "subscript", "superscript"],
            ["increasefontsize", "decreasefontsize"], ["orderedlist", "unorderedlist"], ["indent", "outdent"],
            ["justifyleft", "justifycenter", "justifyright"], ["link", "unlink", "image", "horizontalrule"],
            ["p", "h1", "h2", "h3", "h4", "h5", "h6"], ["cut", "copy", "paste"],
        ],

        toolbarText: $.extend({}, jHtmlArea.defaultOptions.toolbarText, {
            "bold": "Negrita",
            "italic": "Cursiva",
            "underline": "Subrayado"
        }),

    });

    $("#Encuesta_Descripcion").htmlarea({
        toolbar: [
            ["bold", "italic", "underline", "strikethrough", "|", "subscript", "superscript"],
            ["increasefontsize", "decreasefontsize"], ["orderedlist", "unorderedlist"], ["indent", "outdent"],
            ["justifyleft", "justifycenter", "justifyright"], ["link", "unlink", "image", "horizontalrule"],
            ["p", "h1", "h2", "h3", "h4", "h5", "h6"], ["cut", "copy", "paste"],
        ],

        toolbarText: $.extend({}, jHtmlArea.defaultOptions.toolbarText, {
            "bold": "Negrita",
            "italic": "Cursiva",
            "underline": "Subrayado"
        }),

    });

    $("#Solicitud_Descripcion").htmlarea({
        toolbar: [
            ["bold", "italic", "underline", "strikethrough", "|", "subscript", "superscript"],
            ["increasefontsize", "decreasefontsize"], ["orderedlist", "unorderedlist"], ["indent", "outdent"],
            ["justifyleft", "justifycenter", "justifyright"], ["link", "unlink", "image", "horizontalrule"],
            ["p", "h1", "h2", "h3", "h4", "h5", "h6"], ["cut", "copy", "paste"],
        ],

        toolbarText: $.extend({}, jHtmlArea.defaultOptions.toolbarText, {
            "bold": "Negrita",
            "italic": "Cursiva",
            "underline": "Subrayado"
        }),

    });

    $("#SolicitudRegistro_Solicitud_Descripcion").htmlarea({
        toolbar: [
            ["bold", "italic", "underline", "strikethrough", "|", "subscript", "superscript"],
            ["increasefontsize", "decreasefontsize"], ["orderedlist", "unorderedlist"], ["indent", "outdent"],
            ["justifyleft", "justifycenter", "justifyright"], ["link", "unlink", "image", "horizontalrule"],
            ["p", "h1", "h2", "h3", "h4", "h5", "h6"], ["cut", "copy", "paste"],
        ],

        toolbarText: $.extend({}, jHtmlArea.defaultOptions.toolbarText, {
            "bold": "Negrita",
            "italic": "Cursiva",
            "underline": "Subrayado"
        }),

    });

});