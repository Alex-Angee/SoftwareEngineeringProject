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
                $("body").html(response.value.page);
                var Objul = document.createElement('ul');
                Objul.id = "classul";
                var classes = response.value.classes;
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