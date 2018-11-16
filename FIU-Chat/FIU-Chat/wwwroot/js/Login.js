$(document).ready(function () {
    //var socket = new io('http://localhost:11000');

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
        var Objul = document.createElement('ul');
        Objul.id = "classul";
        for (var key in classes) {
            // check if the property/key is defined in the object itself, not in parent
            if (classes.hasOwnProperty(key)) {
                var newDict = classes[key];
                for (var i = 0; i < newDict.length; i++) {
                    var innerDict = newDict[i];
                    for (var innerKey in innerDict) {
                        var Objli = document.createElement('li');
                        Objli.className = "classli";
                        var Obja = document.createElement('a');

                        Obja.innerText = innerKey;
                        Objli.appendChild(Obja);
                        Objul.appendChild(Objli);
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
                $("body").html(response.value.page);
                loadClasses(classes);

                $("#classesDiv").animate({ left: '0' }, 1000);
                var parent = document.getElementById("classul");
                console.log(parent);
                parent.addEventListener("click", function (event) {
                    var className = event.target.tagName;
                    $("#classTitle").val(className);
                    loadMessages();
                });
            }
        });      
    }
});


function loadMessages() {
    var firstDiv = document.createElement('div');
    firstDiv.className = "col-sm-offset-2 col-sm-10 well";
    firstDiv.style = "height:400px;";

    var ul = document.createElement('ul');
    ul.id = "messagesList";
    ul.className = "text-left";
    ul.style = "list-style: none;";

    firstDiv.appendChild(ul);

    var secondDiv = document.createElement('div');
    secondDiv.className = "col-sm-offset-2 col-sm-10 well";

    var form = document.createElement('form');

    var thirdDiv = document.createElement('div');
    thirdDiv.className = "input-group";

    var input = document.createElement('input');
    input.className = "form-control";
    input.type = "text";
    input.placeholder = "Enter message";
    input.id = "messageInput";

    var fourthDiv = document.createElement('div');
    fourthDiv.className = "input-group-btn";

    var button = document.createElement('button');
    button.className = "btn btn-default";
    button.type = "submit";
    button.id = "sendButton";

    var i = document.createElement('i');
    i.className = "glyphicon glyphicon-send";

    // Add them all back to the div
    button.appendChild(i);
    fourthDiv.appendChild(button);
    thirdDiv.appendChild(input);
    thirdDiv.appendChild(fourthDiv);
    form.appendChild(thirdDiv);
    secondDiv.appendChild(form);

    // No add them to the html
    var classView = document.getElementById('classView');
    classView.appendChild(firstDiv);
    classView.appendChild(secondDiv);
}