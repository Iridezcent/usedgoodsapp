//    $("input[class*='editable']").click(function () {
//        var $input = $(this);
//        var $dissabled = $input.attr("disabled");
//        if (typeof $dissabled !== typeof undefined && $dissabled !== false) {
//            $input.removeAttr("disabled");
//        }
//        else {
//            $input.attr("disabled", "disabled");
//        }
//});

//function Disable() {
//        var $input = $(this);
//        var $dissabled = $input.attr("disabled");
//        if (typeof $dissabled !== typeof undefined && $dissabled !== false) {
//            $input.removeAttr("disabled");
//        }
//        else {
//            $input.attr("disabled", "disabled");
//        }
//}

//$("input[class*='editable']").dblclick(function () {
//    var array = $(this).attr("class").split('-');
//    var id = array[2];
//    $.ajax(
//        {
//            url: '@Url.Action("Edit")',
//            type: "POST",
//            data: "id=" + id,
//            //id: "table"
//        });
//});

    $("input.editable").click(function () {
        if ($("input.editable").is("[readonly]"))
            $("input.editable").prop("readonly", false);
        else
            $("input.editable").prop("readonly", true);
    });
