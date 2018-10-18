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

                    // MAKE SURE TO DO JSON.stringify(classes) decode the object stored.

                    swal("Login successfully!", {
                        icon: "success",
                        timer: 1500,
                        buttons: false
                    })
                    .then((value) => {
                        setHeader();
                        redirect(response.value.redirectUrl, response.value.classes);
                    });

                }
            }
        });
    });

    function loadClasses(classes)
    {
        var Objul = $('<ul id="#classul"></ul>');
        for (var key in classes) {
            // check if the property/key is defined in the object itself, not in parent
            if (classes.hasOwnProperty(key)) {           
                console.log("Professor: " + key);
                var newDict = classes[key];
                for(var i = 0; i < newDict.length; i++)
                {
                    var innerDict = newDict[i];
                    for (var innerKey in innerDict)
                    {
                        var Objli = $('<li class="classli"></li>');
                        var Obja = $("<a></a>");

                        Obja.text("Class: " + innerKey + "\nSection: " + innerDict[innerKey]);
                        Objli.append(Obja);
                        Objul.append(Objli);
                    }
                }
            }
        }

        $('#classesDiv').append(Objul);
    }

    function setHeader() {
        $.ajaxSetup({
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', localStorage.getItem("token"));
            }
        });
    }

    function redirect(redirectUrl, classes) {
        $.ajax({
            type: 'GET',
            contentType: 'application/json; charset=utf-8;',
            url: redirectUrl,
            success: function (response) {
                $("body").html(response);
                loadClasses(classes);
                $("#classesDiv").animate({left:'0'},1000);
            }
        });      
    }
});