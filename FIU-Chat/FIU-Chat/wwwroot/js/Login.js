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
                var token = response.value.data;
                localStorage.setItem("token", token);
                alert("You have successfully logged in.");
                redirect(response.value.redirectUrl);
            }
        });
    });

    function redirect(redirectUrl) {
        $.ajaxSetup({
            headers: { 'Authorization': localStorage.getItem("token") }
        });

        $.get(redirectUrl, function (data) {
            alert(data);
        }, 'json');
        console.log("Redirected!");
    }
});