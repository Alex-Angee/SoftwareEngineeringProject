"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var lastSent = "";

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + ": " + msg;

    if(lastSent != encodedMsg)
    {
        var li = document.createElement("li");
        li.textContent = encodedMsg;
        lastSent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
    }
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

//document.getElementById("sendButton").addEventListener("click", function (event) {
// Replacing line below with line above could resolve problems 
$(document).on('click', '#sendButton', function (event) {
    
    var className = document.getElementById("classTitle").innerHTML;
    var user = localStorage.getItem("token");
    var message = document.getElementById("messageInput").value;
    if (message != lastSent)
    {
        connection.invoke("SendMessage", user, className, message).catch(function (err) {
            return console.error(err.toString());
        });
        console.log(message);
        $("#messageInput").val("");
        event.preventDefault();
    }
});