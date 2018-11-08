var shouldlogin = true;

function runLoginCheck() {
    var loggedIn = checkLogin();

    if (loggedIn === true && shouldlogin === true) {
        setHeader();
        redirectToHome();
    }
    else if (loggedIn === false && shouldlogin === false) {
        redirectToLogin();
    }
    return true;
}

function redirectToHome() {
    shouldlogin = false;
    $.ajax({
        type: 'GET',
        contentType: 'application/json; charset=utf-8;',
        url: 'Home/Index',
        success: function (response) {
            $("body").html(response.value.page);
            loadClasses(response.value.classes);
            $("#classesDiv").animate({left:'0'},1000);
        }
    });
}

function setHeader() {
    $.ajaxSetup({
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', localStorage.getItem("token"));
        }
    });
}

function checkLogin() {
    if (localStorage.getItem("token") === null) {
        return false;
    }
    else {
        return true;
    }
}

function loadClasses(classes)
{
    var Objul = $('<ul id="#classul"></ul>');
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
}

function redirectToLogin() {
    $.ajax({
        type: 'GET',
        contentType: 'application/json; charset=utf-8;',
        url: 'Account/Index',
        success: function (response) {
            $("body").html(response);
        }
    });
}

function logOut() {
    shouldlogin = true;
    localStorage.clear();
    redirectToLogin();
}

window.addEventListener('load',
    function() {
        runLoginCheck();
    });
