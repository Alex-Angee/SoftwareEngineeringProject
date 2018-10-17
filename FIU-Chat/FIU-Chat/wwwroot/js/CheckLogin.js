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
            $("body").html(response);
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
