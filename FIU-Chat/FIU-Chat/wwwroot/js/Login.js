$(function () {
    $("#btnlogin").click(function() {
        var loginUrl = "/Account/Login";
        var redirectUrl = '@Url.Action("Index", "Home")';
        var loginModel =
        {
            inputEmail: $("#inputEmail").val(),
            inputPassword: $("#inputPassword").val(),
            rememberMe: $("#rememberMe").val()
        };
        $.ajax({
            type: "POST",
            url: loginUrl,
            data: JSON.stringify(loginModel),
            dataType: "json",
            async: true,
            contentType: "application/json; charset=utf8;",
            success: function(response) {
                var token = response.access_token;
                localStorage.setItem("token");
                window.location.href = redirectUrl;
            }
        });
    });
});