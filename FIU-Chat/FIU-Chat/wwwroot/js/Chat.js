"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + ": " + msg;
    
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
document.getElementById("sendButton").addEventListener("click", function (event) {

    debugger;
    var className = document.getElementById("#classTitle");

    var user = localStorage.getItem("token");
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, className, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
    $("#messageInput").val("");
});