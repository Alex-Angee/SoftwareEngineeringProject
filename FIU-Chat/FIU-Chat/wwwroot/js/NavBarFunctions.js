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
                var Objul = $('<ul id="#classul"></ul>');
                var classes = response.value.classes;
                for (var key in classes) {
                    // check if the property/key is defined in the object itself, not in parent
                    if (classes.hasOwnProperty(key)) {           
                        var newDict = classes[key];
                        for(var i = 0; i < newDict.length; i++)
                        {
                            var innerDict = newDict[i];
                            for (var innerKey in innerDict)
                            {
                                var Objli = $('<li class="classli"></li>');
                                var Obja = $('<a></a>');

                                Obja.text(key + " " + innerKey + " " + innerDict[innerKey]);
                                Objli.append(Obja);
                                Objul.append(Objli);
                            }
                        }
                    }
                }
                $('#classesDiv').append(Objul);
                        $("#classesDiv").animate({left:'0'},1000);
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