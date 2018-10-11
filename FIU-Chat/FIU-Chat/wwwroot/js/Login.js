$(document).ready(function () {
    $("#formSubmit").submit(function (event) {
        event.preventDefault();
        var email = $("#inputEmail").val();
        var password = $("#inputPassword").val();
        var remember = $("#rememberMe").val();
        var loginModel = {
            inputEmail: email,
            inputPassword: password,
            rememberMe: remember
        };

        $.ajax({
            type: 'POST',
            url: 'Account/Login',
            data: JSON.stringify(loginModel),
            contentType: 'application/json; charset=utf-8;',
            success: function (response) {
                var success = response.value.success;
                if (success !== true)
                {
                    alert("Incorrect username/password.");
                    $("#inputPassword").val("");
                }
                else
                {
                    var token = response.value.data;
                    localStorage.setItem("token", token);
                    alert("You have successfully logged in.");
                    setHeader();
                    redirect(response.value.redirectUrl);
                }
            }
        });
    });

    function setHeader() {
        $.ajaxSetup({
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', localStorage.getItem("token"));
            }
        });
    }

    function redirect(redirectUrl) {
        $.ajax({
            type: 'GET',
            contentType: 'application/json; charset=utf-8;',
            url: redirectUrl,
            success: function (response) {
                $("body").html(response);
            }
        });      
    }
});