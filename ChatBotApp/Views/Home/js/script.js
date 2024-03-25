// Function to send a message to the backend
function sendMessage() {
    var sender = document.getElementById("sender").value;
    var message = document.getElementById("message").value;

    if (!sender || !message) {
        alert("Please enter both your name and message.");
        return;
    }

    // Create a new XMLHttpRequest object
    var xhr = new XMLHttpRequest();

    // Configure the request
    xhr.open("POST", "/Chat/SendMessage", true);
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    // Define the function to handle the response
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                // If the message was sent successfully, clear the input field
                document.getElementById("message").value = "";
                document.getElementById("message").focus();
                // Retrieve and display the updated chat messages
                retrieveMessages();
            } else {
                alert("Error sending message.");
            }
        }
    };

    // Send the request with the form data
    xhr.send("sender=" + encodeURIComponent(sender) + "&message=" + encodeURIComponent(message));
}

// Function to retrieve messages from the backend
function retrieveMessages() {
    // Create a new XMLHttpRequest object
    var xhr = new XMLHttpRequest();

    // Configure the request
    xhr.open("GET", "/Chat/GetMessages", true);

    // Define the function to handle the response
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                // Parse the JSON response
                var messages = JSON.parse(xhr.responseText);
                // Display the messages in the chat box
                displayMessages(messages);
            } else {
                alert("Error retrieving messages.");
            }
        }
    };

    // Send the request
    xhr.send();
}

// Function to display messages in the chat box
function displayMessages(messages) {
    var chatBox = document.getElementById("chat-box");
    chatBox.innerHTML = ""; // Clear previous messages

    // Iterate through the messages and display them in the chat box
    messages.forEach(function (message) {
        var messageElement = document.createElement("div");
        messageElement.textContent = message.sender + ": " + message.messageContent;
        chatBox.appendChild(messageElement);
    });

    // Scroll to the bottom of the chat box
    chatBox.scrollTop = chatBox.scrollHeight;
}

// Automatically retrieve messages when the page loads
window.onload = retrieveMessages;
