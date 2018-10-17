$(document).ready(function () {
    $("#btnHome")[0].onclick = function (event) {
        event.preventDefault();
        $.ajax({
            type: 'GET',
            contentType: 'application/json; charset=utf-8;',
            url: 'Home/Index',
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", localStorage.getItem("token"));
            },
            success: function (response) {
                $("body").html(response);
            }
        });
        return false;
    };

    $("#btnLogout")[0].onclick = function (event) {
        event.preventDefault();
        logOut();
        return false;
    };
});

$(function() {
    if (!checkLogin()) {
        $("#NavBar").hide();
    } else {
        if ($("#NavBar").is(":hidden")) {
            $("#NavBar").show();
        }
    }
});