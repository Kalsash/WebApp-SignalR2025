// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chats")
    .configureLogging(signalR.LogLevel.Debug)
    .build();

// Start connection
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
}

connection.onclose(async () => {
    await start();
});

// Start the connection
start();

// Message handling
connection.on("ReceiveMessage", (user, message) => {
    appendMessage(user, message, false);
});

connection.on("UserTyping", (user) => {
    showTypingIndicator(user);
});

// Read cookie helper
function getCookie(name) {
    return document.cookie.split('; ')
        .find(row => row.startsWith(name + '='))
        ?.split('=')[1];
}

document.getElementById("messageInput").addEventListener("keypress", async (e) => {
    if (e.key === "Enter") {
        const message = e.target.value;
        
        const user = getCookie("username") ?? "CurrentUser"; // Replace with actual user

        try {
            await connection.invoke("SendMessage", user, message);
            appendMessage(user, message, true);
            e.target.value = "";
        } catch (err) {
            console.error(err);
        }
    }
});

    let typingTimeout;

    document.getElementById("messageInput").addEventListener("input", async () => {
        // Send typing notification
        await connection.invoke("TypingNotification", "CurrentUser");

        // Clear existing timeout
        clearTimeout(typingTimeout);

        // Set new timeout
        typingTimeout = setTimeout(() => {
            document.getElementById("typingIndicator").classList.add("hidden");
        }, 1000);
    });

    // Message display functions
    function createMessage(username, avatar, message, isSent) {
        const messageDiv = document.createElement("div");
        messageDiv.className = `flex items-start space-x-2 max-w-3/4 ${isSent ? "justify-end" : ""}`;
        const img = document.createElement("img");
        img.className = "w-8 h-8 rounded-full";
        img.src = avatar;
        messageDiv.appendChild(img);

        const messageWrap = document.createElement("div");

        const messageText = document.createElement("div");
        const nameTag = document.createElement("span");

        if (isSent) {
            messageText.className = 'bg-swamp-600 text-white p-3 rounded-xl shadow-sm';
            nameTag.className = "font-semibold text-sm text-swamp-200";
        } else {
            messageText.className = 'bg-white text-swamp-900 p-3 rounded-xl shadow-sm';
            nameTag.className = "font-semibold text-sm text-swamp-800";
        }
        
        nameTag.textContent = username;
        messageText.appendChild(nameTag);

        const p = document.createElement("p");
        p.textContent = message;
        messageText.appendChild(p);

        messageWrap.appendChild(messageText);
        messageDiv.appendChild(messageWrap);
        return messageDiv;
    }

    function appendMessage(user, message, isSent) {
        const messagesContainer = document.querySelector("#messages-container");
        const messageDiv = createMessage(user,"https://i.pravatar.cc/100", message, isSent);
        messagesContainer.appendChild(messageDiv);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }

    function showTypingIndicator(user) {
        const typingIndicator = document.getElementById("typingIndicator");
        typingIndicator.textContent = `${user} is typing...`;
        typingIndicator.classList.remove("hidden");
    }
