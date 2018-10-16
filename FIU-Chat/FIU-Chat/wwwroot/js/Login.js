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
                    swal("Incorrect username/password!", {
                        icon: "error",                        
                        timer: 1500,
                        buttons: false
                    });
                }
                else
                {
                    var token = response.value.data;
                    localStorage.setItem("token", token);

                    var classes = response.value.classes;
                    localStorage.setItem("classes", classes);

                    // MAKE SURE TO DO JSON.stringify(classes) decode the object stored.

                    swal("Login successfully!", {
                        icon: "success",
                        timer: 1500,
                        buttons: false
                    })
                    .then((value) => {
                        setHeader();
                        redirect(response.value.redirectUrl);
                    });

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