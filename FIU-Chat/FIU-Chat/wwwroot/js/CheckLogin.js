var shouldlogin = true;

$(document).one("ready", function () {
    var loggedIn = checkLogin();

    if (loggedIn === true && shouldlogin === true) {
        console.log("Not logged in");
        setHeader();
        redirectToHome();
        console.log("Logged in");
    }
    else if (loggedIn === false && shouldlogin === false) {
        redirectToLogin();
    }
    return true;
});

function setHeader() {
    $.ajaxSetup({
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', localStorage.getItem("token"));
        }
    });
}

function redirectToHome() {
    shouldlogin = false;
    $.ajax({
        type: 'GET',
        contentType: 'application/json; charset=utf-8;',
        url: 'Home/Index',
        success: function (response) {
            $("body").html(response);
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